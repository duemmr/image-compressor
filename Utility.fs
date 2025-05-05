namespace FileCompressor

open System.IO
open System.Drawing

open SixLabors.ImageSharp
open SixLabors.ImageSharp.PixelFormats

module Utility = 
    let getFileSize (filePath: string) =
        let fileInfo = FileInfo(filePath)
        fileInfo.Length

    let getImageDimensions (filePath: string) =
        use fs = new FileStream(filePath, FileMode.Open)
        use image = Image.Load<Rgba32>(fs)
        image.Width, image.Height

    let getExtension (filePath: string) = 
        Path.GetExtension(filePath)