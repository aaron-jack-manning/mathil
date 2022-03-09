use std::path::Path;

/// Generates and validates a filepath based on the output folder, filename and extension.
pub (in crate::mathil) fn generate_file_path<'a>(output_folder : &'a str, filename : &'a str, extension : &'a str) -> Option<String> {
    if Path::new(output_folder).is_dir() {
        let path_string =
            format!(
                "{}{}{}.{}",
                output_folder,
                if output_folder.chars().last().unwrap() == '\\' {""} else {"\\"},
                filename,
                extension
            );

        Some(
           path_string
        )
    }
    else {
        None
    }
}


pub (in crate::mathil) mod bitmap {
    use crate::mathil::rendering::*;

    /// Rounds the provided number to the next multiple of round_up_to.
    fn round_up_to_value(number : u32, round_up_to : u32) -> u32 {
        if number % round_up_to == 0 {
            number
        } else {
            number + round_up_to - (number % round_up_to)
        }
    }

    /// Adds the padded bytes of a u32 to the provided Bitmap bytes.
    fn add_bytes_of_u32(bitmap_bytes : &mut Vec<u8>, number : u32) -> () {
        let bytes_of_file_size = number.to_ne_bytes();

        for item in bytes_of_file_size {
            bitmap_bytes.push(item);
        }
    }

    /// Adds the bytes for the header to the provided Bitmap bytes.
    fn add_header_bytes (bitmap_bytes : &mut Vec<u8>, file_size : u32) -> () {
        // B M for Bitmap
        bitmap_bytes.push('B' as u8);
        bitmap_bytes.push('M' as u8);
        
        // File size
        add_bytes_of_u32(bitmap_bytes, file_size);
        
        // Reserved; Header size
        for item in vec![0, 0, 0, 0, 54, 0, 0, 0] {
            bitmap_bytes.push(item);
        }
    }

    /// Adds the bytes for the DIB header to the provided Bitmap bytes.
    fn add_dib_header_bytes(bitmap_bytes : &mut Vec<u8>, horizontal_resolution : u16, vertical_resolution : u16, print_dots_per_metre : u32) -> () {

        for item in vec![40, 0, 0, 0] {
            bitmap_bytes.push(item);
        }

        add_bytes_of_u32(bitmap_bytes, u32::from(horizontal_resolution));
        add_bytes_of_u32(bitmap_bytes, u32::from(vertical_resolution));
        
        for item in vec![
                1, 0, // Colour Planes
                24, 0, // Bits Per Pixel
                0, 0, 0, 0, // No Compression
                0, 0, 0, 0, // Raw Size - Ignore when no compression is used
            ] {
            bitmap_bytes.push(item);
        }

        add_bytes_of_u32(bitmap_bytes, print_dots_per_metre);
        add_bytes_of_u32(bitmap_bytes, print_dots_per_metre);

        for item in vec![
                0, 0, 0, 0, // 2^n Colours in Palette (from 0)
                0, 0, 0, 0, // All Colours are Important (from 0)
            ] {
            bitmap_bytes.push(item);
        }
    }

    /// Creates a Bitmap file from the screen and outputs it as a Vec<u8>, to later be written to a file.
    pub (in crate::mathil) fn create_bitmap_bytes(screen : Screen) -> Vec<u8> {
        let file_size =
            54 + (u32::from(screen.vertical_resolution) * round_up_to_value(3 * u32::from(screen.horizontal_resolution), 4));

        let mut bitmap_bytes : Vec<u8> = Vec::with_capacity(file_size as usize);

        add_header_bytes(&mut bitmap_bytes, file_size);
        add_dib_header_bytes(&mut bitmap_bytes, screen.horizontal_resolution, screen.vertical_resolution, 4_000);


        let padding = screen.horizontal_resolution % 4;

        for i in 0..screen.vertical_resolution {
            for j in 0..screen.horizontal_resolution {
                let colour = &(screen.pixels)[j as usize][i as usize];
                bitmap_bytes.push(colour.blue);
                bitmap_bytes.push(colour.green);
                bitmap_bytes.push(colour.red);
            }

            for _k in 0..padding {
                bitmap_bytes.push(0);
            }
        }

        bitmap_bytes
    }
}


pub (in crate::mathil) mod png {
    use crate::mathil::rendering::*;

    /// Creates an array of RGB values from the image.
    pub (in crate::mathil) fn create_rgb_byte_array(screen : &Screen) -> Vec<u8> {

        let mut bytes =
            Vec::with_capacity((u32::from(screen.horizontal_resolution) * u32::from(screen.vertical_resolution)) as usize);
        
        for y in (0..screen.vertical_resolution).rev() {
            for x in 0..screen.horizontal_resolution {
                let current_colour = screen.pixels[x as usize][y as usize];

                bytes.push(current_colour.red);
                bytes.push(current_colour.green);
                bytes.push(current_colour.blue);
            }
        }

        bytes
    }

}

