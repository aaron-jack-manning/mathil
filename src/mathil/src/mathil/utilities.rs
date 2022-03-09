use crate::mathil::rendering::Screen;

use std::convert::TryFrom;

/// Represents the aspect ratios within the resolution and the bounds as 1:output.
#[derive(Debug)]
pub struct AspectRatios {
    pub resolution : f32,
    pub bounds : f32,
}

/// Calculates the aspect ratio in the resolution and in the bounds, to check how significantly the mathematical coordinates system is squeezed before rendering the image.
pub fn aspect_ratios(screen : Screen) -> AspectRatios {
    let resolution_ratio =
        f32::from(screen.horizontal_resolution) / f32::from(screen.vertical_resolution);
    
    let bounds_ratio =
        (screen.top_right_bound.x - screen.bottom_left_bound.x) / (screen.top_right_bound.y - screen.bottom_left_bound.y);

    AspectRatios {
        resolution : resolution_ratio,
        bounds : bounds_ratio,
    }
}

/// Calculates the line thickness as a proportion of the average of the horizontal and vertical resolutions. Allows line thickness to be scaled appropriates upon changing an image's resolution.
pub fn calculate_line_thickness(horizontal_resolution : u16, vertical_resolution : u16, proportion : f32) -> u16 {
    (proportion * f32::from((horizontal_resolution + vertical_resolution) / 2)) as u16
}