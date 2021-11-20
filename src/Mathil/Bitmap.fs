namespace Mathil

open System.IO

open Colours
open Rendering

module Bitmap =

    let private bytes (number : int) =
        let reduceMod modulus number =
            (number % modulus + modulus) % modulus

        let division a b =
            let remainder = reduceMod b a
            let quotient = (a - remainder)/ b
    
            quotient, remainder

        let baseBAlgorithm (number : int) (radix : int) =
            let digits = List.empty

            let rec convert (current : int) =

                let quotient, remainder = division current radix

                match quotient with
                | 0 -> digits @ [remainder]
                | _ -> (convert quotient) @ [remainder]

            convert number

        let digitList = baseBAlgorithm number 256

        let padding = 4 - List.length digitList % 4

        (digitList
        |> List.rev
        |> List.toArray
        |> Array.append) (Array.zeroCreate padding)
        |> Array.map byte

    let private headerBytes filesize =

        let header =
            [|
                byte 'B'; byte 'M'; // File Type
                0uy; 0uy; 0uy; 0uy; // Temporary - Will be File Size
                0uy; 0uy; 0uy; 0uy; // Reserved
                54uy; 0uy; 0uy; 0uy; // Header Size
            |]

        let filesizeBytes = bytes filesize

        // Add file size to temporary space
        for i = 0 to 3 do
            header.[i + 2] <- filesizeBytes.[i]

        header

    let private dibHeaderBytes horizontalResolution verticalResolution dpcm =

        let dibHeader =
            [|
                40uy; 0uy; 0uy; 0uy; // Size of this Header
                0uy; 0uy; 0uy; 0uy; // Temporary - Horizontal Resolution
                0uy; 0uy; 0uy; 0uy; // Temporary - Vertical Resolution
                1uy; 0uy; // Colour Planes
                24uy; 0uy; // Bits Per Pixel
                0uy; 0uy; 0uy; 0uy; // No Compression
                0uy; 0uy; 0uy; 0uy; // Raw Size - Ignored when no compression is used
                0uy; 0uy; 0uy; 0uy; // Temporary - Print Resolution
                0uy; 0uy; 0uy; 0uy; // Temporary - Print Resolution
                0uy; 0uy; 0uy; 0uy; // 2^n Colours in Palette (from 0)
                0uy; 0uy; 0uy; 0uy; // All Colours are Important (from 0)
            |]

        let horizontalResolutionBytes = bytes horizontalResolution
        let verticalResolutionBytes = bytes verticalResolution
        let printResolutionBytes = bytes (dpcm * 100)

        // Add resolution and print resolution to temporary spaces
        for i = 0 to 3 do
            dibHeader.[4 + i] <- horizontalResolutionBytes.[i]
            dibHeader.[8 + i] <- verticalResolutionBytes.[i]
            dibHeader.[24 + i] <- printResolutionBytes.[i]
            dibHeader.[28 + i] <- printResolutionBytes.[i]

        dibHeader

    let private colourToBytes (colour : Colour) =
        [|
            colour.Blue
            colour.Green
            colour.Red
        |]

    let private roundUpTo4 number =
        if number % 4 = 0 then
            number
        else
            number + 4 - (number % 4)

    /// Writes the provided screen to the specified filepath as a bitmap (.bmp) file.
    let saveScreenToBitmap (filepath : string) (filename : string) (screen : Screen) : unit =

        let filesize = 54 + (screen.VerticalResolution * roundUpTo4 screen.HorizontalResolution)

        let stream = new FileStream((if filepath.EndsWith("\\") then filepath else filepath + "\\") + filename + ".bmp", FileMode.Create)
        let writer = new BinaryWriter(stream)

        let header = headerBytes filesize
        let dibHeader = dibHeaderBytes screen.HorizontalResolution screen.VerticalResolution 40

        // Write the headers to a file
        for byte in header do
            writer.Write(byte)
        for byte in dibHeader do
            writer.Write(byte)

        let padding = screen.HorizontalResolution % 4

        // Write pixels and padding to file
        for i = 0 to screen.VerticalResolution - 1 do
            for j = 0 to screen.HorizontalResolution - 1 do
                let colour = colourToBytes screen.Pixels.[j, i]

                // Writing the colour values of a pixel
                for colourValue in colour do
                    writer.Write(colourValue)
        
            // Padding at end of each row of pixels
            for i = 1 to padding do
                writer.Write(0uy)

        writer.Close()
        stream.Close()
        writer.Dispose()
        stream.Dispose()