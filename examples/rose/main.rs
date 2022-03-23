#![allow(unused_imports)]

mod mathil;
use mathil::{colours::*, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    animate(rose_animation_generator, 8.0, 60, "")
}

pub fn rose_animation_generator(time_stamp : f32, _frame : u32, _length : f32) -> Screen {

    let coefficient = time_stamp;

    let horizontal_resolution = 3840;
    let vertical_resolution = 2160;

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
            Thickness::Relative(0.022),
            8000,
            RenderingType::RoundAntiAliased(1.0)
        )
    )
    .render(
        circle,
        FunctionRenderSettings::new(
            css_colours::BLACK,
            Thickness::Relative(0.007),
            5000,
            RenderingType::RoundAntiAliased(1.0)
        )
    )
}