namespace FileCompressor

open System.IO

module Utility = 
    let getFileSize (filePath: string) =
        let fileInfo = FileInfo(filePath)
        fileInfo.Length