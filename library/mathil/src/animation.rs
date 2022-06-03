use std::thread;
use std::thread::JoinHandle;
use std::path;

use crate::rendering::Screen;
use crate::primitive_conversions::*;
use crate::errors;

/// Represents an animatable scene by a function to generate each frame and the length in seconds.
pub struct Scene {
    generator : fn(Screen, f32, f32) -> Screen,
    length : f32,
}

/// Represents a collection of scenes.
pub struct Video {
    scenes : Vec<Scene>
}

impl Video {
    /// Creates a new Video from a vector of scenes.
    pub fn new(scenes : Vec<Scene>) -> Video {
        Video {
            scenes
        }
    }
    /// Animates the entire video as a sequence of scenes, maintaining frame number between scenes
    /// and passing the final frame of each scene as init to the next scene.
    pub fn animate(self, mut init : Screen, fps : u16, path : &'static str) -> Result<(), errors::Error> {
        if !path::Path::new(path).is_dir() {
            Err(errors::Error::InvalidDirectory(path.to_string()))
        }
        else {
            let mut next_frame = 0;
            for scene in self.scenes {
                (init, next_frame) = animate_helper(init, scene, fps, next_frame, path);
            }

            Ok(())
        }
    }
}

impl Scene {
    /// Creates a new Scene.
    pub fn new(generator : fn(Screen, f32, f32) -> Screen, length : f32) -> Scene {
        Scene {
            generator,
            length,
        }
    }
    /// Animates the scene, using init as the initial frame to be passed to the generator function.
    /// Path specifies the output folder for the frames.
    pub fn animate(self, init : Screen, fps : u16, path : &'static str) -> Result<(), errors::Error> {
        if !path::Path::new(path).is_dir() {
            Err(errors::Error::InvalidDirectory(path.to_string()))
        }
        else {
            animate_helper(init, self, fps, 0, path);
            Ok(())
        }
    }
}

/// Internal function for animating a scene which passes back the final frame and frame count.
fn animate_helper(initial_frame : Screen, scene : Scene, frame_rate : u16, initial_frame_number : u32, output_folder : &'static str) -> (Screen, u32) {

    // Total number of frames of video.
    let frame_qty = f32_to_u32(f32::from(frame_rate) * scene.length);

    // Time spent showing each frame.
    let frame_time = 1.0 / f32::from(frame_rate);

    // Pairs of  frame number and timestamp.
    let timestamps : Vec<(u32, f32)> = 
        (0..frame_qty)
        .map(|frame_no : u32| { (frame_no, u32_to_f32(frame_no) * frame_time) })
        .collect();

    let next_frame_number = u32::try_from(timestamps.len()).unwrap() + initial_frame_number;

    let mut handles : Vec<JoinHandle<Screen>> = Vec::new();

    for (frame, timestamp) in timestamps {
        let new_initial_frame = initial_frame.clone();

        let handle = thread::spawn(move || {
            let filename =
                format!("frame_{:0>#8}", frame + initial_frame_number);

            let current_frame = (scene.generator)(new_initial_frame, timestamp, scene.length);
            if let Err(e) = current_frame.write_to_png(output_folder, &filename) {
                panic!("{}", e.to_string())
            }
            current_frame
        });

        handles.push(handle);
    }

    let mut final_frame = None;
    let length = handles.len();

    for (i, handle) in handles.into_iter().enumerate() {
        if i == length - 1 {
            final_frame = Some(handle.join().unwrap());
        }
        else {
            handle.join().unwrap();
        }
    }

    (final_frame.unwrap(), next_frame_number)
}

// Identity function for scene generators with respect to the initial frame.
// To be used in a Video when the final frame of a Scene should be static for some
// time after the Scene has finished.
pub fn placeholder(init : Screen, _time : f32, _len : f32) -> Screen {
    init
}

/// Types of easing functions to be used with easy_ease.
pub enum EaseFn {
    Arctan(f32),
    Tanh(f32),
}

/// Maps the interval from 0 to 1 onto itself with an increasing function which
/// has the steapest gradient in the middle of the interval.
pub fn easy_ease(t : f32, function : EaseFn) -> f32 {
    match function {
        EaseFn::Arctan(a) => {
            (2.0 * a * t - a).atan() / (2.0 * a.atan()) + 0.5
        },
        EaseFn::Tanh(a) => {
            (2.0 * a * t - a).tanh() / (2.0 * a.tanh()) + 0.5
        }
    }
}
