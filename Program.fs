open System
open System.IO

open Giraffe
open Giraffe.ViewEngine

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open FileCompressor.Compress
open FileCompressor.Views
open FileCompressor.Utility

let webApp =
    choose [
        GET >=> route "/" >=> htmlView uploadForm
        POST >=> route "/upload" >=> fun next ctx -> 
            task {
                let! form = ctx.Request.ReadFormAsync()
                let method = form.["method"].ToString()
                let file = form.Files.["file"]
                let quality = form.["quality"] |> Seq.head |> int
                let targetSizeKB = form.["targetKb"] |> Seq.head |> int
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

                printfn "%O %O %O SIZE" mode (widthOpt |> Option.defaultValue 0) (heightOpt |> Option.defaultValue 0)

                let uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")
                Directory.CreateDirectory(uploadsDir) |> ignore

                let fileName = Path.GetFileName file.FileName
                let savedPath = Path.Combine(uploadsDir, fileName)

                let outputPath = 
                    match widthOpt, heightOpt with
                    | Some w, Some h when w > 0 && h > 0 ->
                        resizeImage file w h
                    | _ ->
                    match method with
                    | "jpeg" -> compressAsJpeg file quality
                    | "webp" -> compressAsWebp file quality
                    | "png"  -> compressAsPng file
                    | _      -> ""

                use fs = new FileStream(savedPath, FileMode.Create)
                file.CopyTo(fs)

                compressToTargetSize file targetSizeKB

                let originalPath = Path.Combine(uploadsDir, fileName)
                // let originalSize: int64 = getFileSize(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", $"/uploads/{fileName}"))
                // let compressedSize: int64 = getFileSize(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Path.GetFileName(outputPath)))
                //   printfn "%O" (Directory.GetCurrentDirectory())
                // let gyerunk = Path.Combine(Directory.GetCurrentDirectory(), outputPath)
                // printfn "%O | Original path: %O\nCompressed path: %O" mode gyerunk originalPath
                return! htmlView (resultView outputPath $"/uploads/{fileName}") next ctx
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