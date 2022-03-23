use std::path::Path;
use std::thread;
use std::thread::JoinHandle;

use crate::mathil::rendering::Screen;
use crate::mathil::primitive_conversions::*;

/// Generates an animation using a function pointer which returns the frame for the given timestamp.
/// The function pointer takes as input, the current timestamp, the frame number, and the total length of the animation.
pub fn animate(frame_generator : fn(f32, u32, f32) -> Screen, length : f32, frame_rate : u8, output_folder : &'static str) {

    // Total number of frames of video.
    let frame_qty = f32_to_u32(f32::from(frame_rate) * length);

    // Time spent showing each frame.
    let frame_time = 1.0 / f32::from(frame_rate);

    // Pairs of  frame number and timestamp.
    let timestamps : Vec<(u32, f32)> = 
        (0..frame_qty)
        .map(|frame_no : u32| { (frame_no, u32_to_f32(frame_no) * frame_time) })
        .collect();

    let mut handles : Vec<JoinHandle<()>> = Vec::new();

    for (frame, timestamp) in timestamps {
        let handle = thread::spawn(move || {
            let filename =
                format!("frame_{:0>#5}", frame);

            frame_generator(timestamp, frame, length)
            .write_to_png(output_folder, &filename);
        });

        handles.push(handle);
    }

    for handle in handles {
        handle.join().unwrap();
    }
}

/// Types of smoothing functions for animation.
pub enum Smoother {
    Arctan(f32),
    Tanh(f32),
}

/// Maps the interval from 0 to 1 onto itself with an increasing function which alters the gradient at different points.
pub fn smooth(t : f32, function : Smoother) -> f32 {
    match function {
        Smoother::Arctan(a) => {
            (2.0 * a * t - a).atan() / (2.0 * a.atan()) + 0.5
        }
        Smoother::Tanh(a) => {
            (2.0 * a * t - a).tanh() / (2.0 * a.tanh()) + 0.5
        }
    }
}