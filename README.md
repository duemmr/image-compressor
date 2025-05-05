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

## Features preview

# Home page
https://github.com/user-attachments/assets/12cee655-82da-48c7-b261-7f4378ae0a4f

# Result page
https://github.com/user-attachments/assets/1619f6b5-d0ea-4303-a4be-4365feba6b91

# Metadata pop-up
![metadata](https://github.com/user-attachments/assets/a7398bc5-8513-4d21-90be-f49fb3edb556)

## Technologies

- **F#**
- **Giraffe**
- **ASP.NET Core**
- **Tailwind CSS**
- **System.Drawing.Common** 

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
