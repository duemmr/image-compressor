# Image compressor

This is a simple image compressor web application built in **F#**, using **Giraffe**. The application allows users to upload images, and either compress or resize those.

## Try yourself!
- [Live demo](https://imagecompressor-eycnemgnhthqhubu.westeurope-01.azurewebsites.net/)

## Features

- Upload image files for
  - compression (JPEG, PNG, WebP)
  - resizing
- View before and after images
  - Image metadata included
  - Download the output image

### Features preview

## Home
<video src="https://i.gyazo.com/3b568cfed2f52f547e48436e84ed3dc2.mp4" width=180/>

## After adjustment
<video src="https://i.gyazo.com/00e24d9fd0ef111f712b69568470b4b9.mp4" width=180/>

<img src="https://i.gyazo.com/3b568cfed2f52f547e48436e84ed3dc2.mp4" alt="Main feature" />


## Technologies

- **F#**
- **Giraffe**
- **ASP.NET Core**
- **Tailwind CSS** for responsive design
- **System.Drawing.Common** for image processing

## Setup

### Prerequisites

- **.NET SDK** 6.0 or later
- **F#**
- **NuGet** packages:
  - Giraffe
  - System.Drawing.Common

### Installation and usage

1. Clone the repository:
   ```bash
   git clone https://github.com/duemmr/image-compressor.git
   cd image-compressor
   ```
2. Run the app
    ```bash
    dotnet restore
    dotnet run
    ```

## License

This project is licensed under the [MIT License](./LICENSE). You are free to use, modify, and distribute this software for any purpose.