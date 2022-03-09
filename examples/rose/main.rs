mod mathil;
use mathil::{colours::Colour, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    animate(rose_animation_generator, 8.0, 60, "")
}

pub fn rose_animation_generator(time_stamp : f32, frame : u32) -> Screen {

    let coefficient = time_stamp;

    let horizontal_resolution = 3840;
    let vertical_resolution = 2160;

    let line_thickness_thick =
        calculate_line_thickness(horizontal_resolution, vertical_resolution, 0.005);

    let line_thickness_thin =
        calculate_line_thickness(horizontal_resolution, vertical_resolution, 0.002);

    let screen =
        Screen::new(
            horizontal_resolution, vertical_resolution,
            Point::new(-2.666, -1.5), Point::new(2.666, 1.5),
            css_colours::WHITE
        );

    let rose =
        Function::new(
            Box::new(move |t : f32| {
                Point::new(
                    (coefficient as f32 * t).cos() * t.cos(),
                    (coefficient as f32 * t).cos() * t.sin()
                )
            }),
            (0.0, 2.0 * PI)
        );

    let circle_radius = 1.1;

    let circle =
        Function::new_circle(
            circle_radius,
            Point::new(0.0, 0.0),
            (0.0, TAU)
        );

    screen
    .render(
        rose,
        FunctionRenderSettings::new(
            css_colours::BLACK,
            line_thickness_thick,
            8000,
            RenderingType::RoundAntiAliased(1.0)
        )
    )
    .render(
        circle,
        FunctionRenderSettings::new(
            css_colours::BLACK,
            line_thickness_thin,
            5000,
            RenderingType::RoundAntiAliased(1.0)
        )
    )
}