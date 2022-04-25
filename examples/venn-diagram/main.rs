#![allow(unused_imports)]

mod mathil;
use mathil::{colours::*, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn left_circle() -> Function {
    Function::new_circle(
        25.0,
        Point::new(60.0, 50.0),
        (0.0, TAU)
    )
}

fn right_circle() -> Function {
    Function::new_circle(
        25.0,
        Point::new(90.0, 50.0),
        (0.0, TAU)
    )
}

fn main() {
    let horizontal_resolution = 3000;

    let vertical_resolution =
            horizontal_resolution / 3 * 2;

    Screen::new(
        horizontal_resolution, vertical_resolution,
        Point::new(0.0, 0.0), Point::new(150.0, 100.0),
        css_colours::ALMOND
    )
    .render_many(
        // Render the two circles thinner than desired in preparation for the colour fill.
        vec![Box::new(left_circle()), Box::new(right_circle())],
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
        // Render the top circles after the fill.
        vec![Box::new(left_circle()), Box::new(right_circle())],
        FunctionRenderSettings::new(
            css_colours::BLACK,
            Thickness::Relative(0.6),
            900,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .write_to_png("/home/aaron-manning/Pictures/mathil/", "venn-diagram");
}
