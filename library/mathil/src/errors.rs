use std::path;
use std::io;

#[derive(Debug)]
pub enum Error {
    InvalidDirectory(String),
    FileCreation(path::PathBuf, io::Error),
    FileWrite(path::PathBuf, io::Error),
    PngError(path::PathBuf, png::EncodingError)
}

impl ToString for Error {
    fn to_string(&self) -> String {
        match self {
            Error::InvalidDirectory(path) => format!("Provided directory \"{}\" is invalid.", path),
            Error::FileCreation(path, io_err) => {
                match &io_err.raw_os_error() {
                    Some(err) => format!("Could not create file at \"{:?}\". OS error {}", path, err),
                    None => format!("Could not create file at \"{:?}\"", path),
                }
            }
            Error::FileWrite(path, io_err) => {
                match &io_err.raw_os_error() {
                    Some(err) => format!("Could not write to file at {:?}. OS error {}", path, err),
                    None => format!("Could not write to file at \"{:?}\"", path),
                }
            }
            Error::PngError(path, png_err) => format!("PNG encoding error for \"{:?}\": {}", path, png_err.to_string())
        }
    }
}
