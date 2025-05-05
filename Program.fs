open System
open System.IO

open Giraffe
open Giraffe.ViewEngine

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

open System.Text.Json

open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

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

                use image = Image.Load(file.OpenReadStream())
                let beforeWidth = image.Width
                let beforeHeight = image.Height

                let uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")
                Directory.CreateDirectory(uploadsDir) |> ignore

                let outputDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output")
                Directory.CreateDirectory(outputDir) |> ignore

                let fileName = Path.GetFileName file.FileName
                let savedPath = Path.Combine(uploadsDir, fileName)

                let outputPath = 
                    match widthOpt, heightOpt with
                    | Some w, Some h when w > 0 && h > 0 ->
                        resizeImage file w h
                    | _ ->
                    match targetSizeKB with
                    | Some target when target > 0 ->
                        compressToTargetSize file target
                    | _ ->
                    match method with
                    | "jpeg" -> compressAsJpeg file quality
                    | "webp" -> compressAsWebp file quality
                    | "png"  -> compressAsPng file
                    | _      -> ""

                use fs = new FileStream(savedPath, FileMode.Create)
                file.CopyTo(fs)

                let beforePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName)
                let afterPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output", Path.GetFileName(outputPath))

                use imageAfter = Image.Load(afterPath)
                let afterWidth = imageAfter.Width
                let afterHeight = imageAfter.Height

                let originalSize: int64 = getFileSize(beforePath)
                let compressedSize: int64 = getFileSize(afterPath)
                let beforeExtension = getExtension(beforePath)
                let afterExtension = getExtension(afterPath)

                let beforeMeta = { 
                    width = beforeWidth; 
                    height = beforeHeight; 
                    size = originalSize; 
                    format = beforeExtension;
                }

                let afterMeta = { 
                    width = afterWidth; 
                    height = afterHeight; 
                    size = compressedSize; 
                    format = afterExtension;
                }

                let jsonMetaBefore = JsonSerializer.Serialize(beforeMeta)
                let jsonMetaAfter = JsonSerializer.Serialize(afterMeta)

                return! htmlView (resultView $"/output/{outputPath}" $"/uploads/{fileName}" jsonMetaBefore jsonMetaAfter) next ctx
            }
       // GET >=> routef "/%s" (fun _ -> redirectTo false "/")
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