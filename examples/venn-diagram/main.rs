#![allow(unused_imports)]

mod mathil;
use mathil::{colours::Colour, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    let horizontal_resolution = 3000;

    let vertical_resolution =
            horizontal_resolution / 3 * 2;
    
    let left_circle_underneath =
        Function::new_circle(
            25.0,
            Point::new(60.0, 50.0),
            (0.0, TAU)
        );
    let left_circle_top =
        Function::new_circle(
            25.0,
            Point::new(60.0, 50.0),
            (0.0, TAU)
        );

    let right_circle_underneath =
        Function::new_circle(
            25.0,
            Point::new(90.0, 50.0),
            (0.0, TAU)
        );
    let right_circle_top =
        Function::new_circle(
            25.0,
            Point::new(90.0, 50.0),
            (0.0, TAU)
        );

    Screen::new(
        horizontal_resolution, vertical_resolution,
        Point::new(0.0, 0.0), Point::new(150.0, 100.0),
        css_colours::ALMOND
    )
    .render_many(
        vec![Box::new(left_circle_underneath), Box::new(right_circle_underneath)],
        FunctionRenderSettings::new(
            css_colours::BLACK,
            Thickness::Relative(0.3),
            900,
            RenderingType::RoundAliased
        )
    )
    .fill(
        Point::new(75.0, 50.0),
        Colour::from_hex("#9b59b6")
    )
    .fill(
        Point::new(60.0, 50.0),
        css_colours::BABY_BLUE
    )
    .fill(
        Point::new(90.0, 50.0),
        css_colours::ALIZARIN_CRIMSON
    )
    .render_many(
        vec![Box::new(left_circle_top), Box::new(right_circle_top)],
        FunctionRenderSettings::new(
            css_colours::BLACK,
            Thickness::Relative(0.6),
            900,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .write_to_png("", "venn-diagram");
}