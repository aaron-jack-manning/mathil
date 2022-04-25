#![allow(unused_imports)]

mod mathil;
use mathil::{colours::*, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    let init =
        Screen::new(
            3840, 2160,
            Point::new(-2.666, -1.5), Point::new(2.666, 1.5),
            css_colours::WHITE
        );

    Scene::new(scene_1, 8.0).animate(init, 60, "/home/aaron-manning/Pictures/mathil")
}

fn scene_1(init : Screen, time : f32, _len : f32) -> Screen {
    let coefficient = time;

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

    let circle =
        Function::new_circle(
            1.1,
            Point::new(0.0, 0.0),
            (0.0, TAU)
        );

    init 
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
