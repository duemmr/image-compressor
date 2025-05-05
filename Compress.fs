namespace FileCompressor

open System.IO

open Microsoft.AspNetCore.Http

open SixLabors.ImageSharp
open SixLabors.ImageSharp.Processing
open SixLabors.ImageSharp.Formats.Jpeg
open SixLabors.ImageSharp.Formats.Webp
open SixLabors.ImageSharp.Formats.Png
open SixLabors.ImageSharp.Formats

open FileCompressor.Utility

module Compress = 
    let private loadImageFromFormFile (file: IFormFile) =
        use stream = file.OpenReadStream()
        Image.Load(stream)

    let private getOutputPath (original: IFormFile) (ext: string) =
        let fileName = Path.GetFileNameWithoutExtension(original.FileName)
        let abc = Path.Combine("wwwroot", "output", $"output_{fileName}.{ext}")
        printfn "%O" abc
        abc

    let compressAsJpeg (file: IFormFile) (quality: int) =
        let image = loadImageFromFormFile file
        let outputPath = getOutputPath file "jpg"

       // image.Mutate(fun ctx -> ctx.Resize(image.Width / 2, image.Height / 2) |> ignore)

        use outStream = File.OpenWrite(outputPath)
        let encoder = JpegEncoder(Quality = quality)
        image.Save(outStream, encoder)

        Path.GetFileName(outputPath)

    let compressAsWebp (file: IFormFile) (quality: int) =
        let image = loadImageFromFormFile file
        let outputPath = getOutputPath file "webp"

        use outStream = File.OpenWrite(outputPath)
        let encoder = WebpEncoder(Quality = quality)
        image.Save(outStream, encoder)

        Path.GetFileName(outputPath)

    let compressAsPng (file: IFormFile) =
        let image = loadImageFromFormFile file
        let outputPath = getOutputPath file "png"

        image.Mutate(fun ctx -> ctx.AutoOrient() |> ignore)

        use outStream = File.OpenWrite(outputPath)
        let encoder = PngEncoder(CompressionLevel = PngCompressionLevel.BestCompression)
        image.Save(outStream, encoder)

        Path.GetFileName(outputPath)

    let saveImage (img: Image) (encoder: IImageEncoder) (originalFilename: string) : string =
        let dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output")
        Directory.CreateDirectory(dir) |> ignore

        let nameWithoutExt = Path.GetFileNameWithoutExtension originalFilename
        let ext = 
            match encoder with
            | :? JpegEncoder -> ".jpg"
            | _ -> Path.GetExtension originalFilename

        let outputName = sprintf "output_%s%s" nameWithoutExt ext
        let outputPath = Path.Combine(dir, outputName)

        use fs = new FileStream(outputPath, FileMode.Create)
        img.Save(fs, encoder)
        
        Path.GetFileName(outputPath)

    let rec findQuality (img:Image) targetSizeKB lo hi =
        let q = (lo + hi) / 2
        use ms = new MemoryStream()

        img.Save(ms, JpegEncoder(Quality=q))

        let sizeKB = ms.Length / 1024L

        if abs (sizeKB - int64 targetSizeKB) < 5L || hi - lo < 5 then q
        elif sizeKB > int64 targetSizeKB then findQuality img targetSizeKB lo q
        else findQuality img targetSizeKB q hi

    let compressToTargetSize (file: IFormFile) (targetKB: int) =
        let img = loadImageFromFormFile file
        let q = findQuality img targetKB 10 100
        let encoder = JpegEncoder(Quality=q)

        saveImage img encoder file.FileName

    let resizeImage (inputFile: IFormFile) (width: int) (height: int) : string =
        let uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "output")
        let fileName = Path.GetFileName inputFile.FileName
        let resizedPath = Path.Combine(uploadsDir, $"output_{fileName}")

        use inputStream = inputFile.OpenReadStream()
        use image = Image.Load inputStream

        image.Mutate(fun ctx -> ctx.Resize(width, height) |> ignore)

        use outputStream = new FileStream(resizedPath, FileMode.Create)
        image.SaveAsJpeg(outputStream)

        Path.GetFileName(resizedPath)