open System
open System.IO

open Giraffe
open Giraffe.ViewEngine

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open System.Text.Json

open FileCompressor.Compress
open FileCompressor.Views
open FileCompressor.Utility

type ImageInfo = {
    width: int
    height: int
    size: int64
    format: string
}

let webApp =
    choose [
        GET >=> route "/" >=> htmlView uploadForm
        POST >=> route "/upload" >=> fun next ctx -> 
            task {
                let! form = ctx.Request.ReadFormAsync()
                let method = form.["method"].ToString()
                let file = form.Files.["file"]
                let quality = form.["quality"] |> Seq.head |> int
                let targetSizeKB = form.["targetKb"] |> Seq.tryHead |> Option.map int
                let mode = form.["mode"]

                let parseIntOption (key: string) =
                    match form.TryGetValue(key) with
                    | true, value ->
                        match Int32.TryParse(value.ToString()) with
                        | true, i -> Some i
                        | _ -> None
                    | _ -> None

                let widthOpt = parseIntOption "width"
                let heightOpt = parseIntOption "height"

                printfn "%O %d %d SIZE" mode (widthOpt |> Option.defaultValue 0) (heightOpt |> Option.defaultValue 0)

                let uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")
                Directory.CreateDirectory(uploadsDir) |> ignore

                let outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output")
                Directory.CreateDirectory(outputDir) |> ignore

                let fileName = Path.GetFileName file.FileName
                let savedPath = Path.Combine(uploadsDir, fileName)

                let outputPath = 
                    match widthOpt, heightOpt with
                    | Some w, Some h when w > 0 && h > 0 ->
                        printfn "resize thingy"
                        resizeImage file w h
                    | _ ->
                    match targetSizeKB with
                    | Some target when target > 0 ->
                        printfn "testtttttt %d" target
                        compressToTargetSize file target
                    | _ ->
                    match method with
                    | "jpeg" -> compressAsJpeg file quality
                    | "webp" -> compressAsWebp file quality
                    | "png"  -> compressAsPng file
                    | _      -> ""

                use fs = new FileStream(savedPath, FileMode.Create)
                file.CopyTo(fs)

                let originalPath = Path.Combine(uploadsDir, fileName)
                let beforePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName)
                let afterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output", Path.GetFileName(outputPath))

               // let width, height = getImageDimensions beforePath
               // let width2, height2 = getImageDimensions afterPath
               // printfn "ASD %O %O %d %d %d %d" xx yy width height width2 height2

                let originalSize: int64 = getFileSize(beforePath)
                let compressedSize: int64 = getFileSize(afterPath)
                let beforeExtension = getExtension(beforePath)
                let afterExtension = getExtension(afterPath)

                printfn "Size: %d %d | Extension: %O %O" originalSize compressedSize beforeExtension afterExtension

                let beforeMeta = { 
                    width = 1920; 
                    height = 1080; 
                    size = originalSize; 
                    format = beforeExtension;
                }

                let afterMeta = { 
                    width = 1920; 
                    height = 1080; 
                    size = compressedSize; 
                    format = afterExtension;
                }

                let jsonMetaBefore = JsonSerializer.Serialize(beforeMeta)
                let jsonMetaAfter = JsonSerializer.Serialize(afterMeta)

                return! htmlView (resultView $"/output/{outputPath}" $"/uploads/{fileName}" jsonMetaBefore jsonMetaAfter) next ctx
            }
        GET >=> routef "/%s" (fun _ -> redirectTo false "/")
    ]

let configureApp (app : IApplicationBuilder) =
    app.UseStaticFiles()
       .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    let builder = WebApplication.CreateBuilder()
    builder.Services |> configureServices

    let app = builder.Build()
    configureApp app
    app.Run()

    0