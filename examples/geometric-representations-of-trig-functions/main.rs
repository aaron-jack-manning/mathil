use mathil::{
    colours::*,
    rendering::*,
    maths_objects::*,
    colours::css_colours,
    animation::*
};

use std::f32::consts::{PI, TAU};

fn main() {
    let init = Screen::new(
        3840, 2160,
        Point::new(-3.555, -2.0), Point::new(3.555, 2.0),
        Colour::from_hex("#2f3640")
    );

    Scene::new(scene_1, 4.0)
    .animate(init, 60, "/home/aaron-manning/Pictures/mathil/").unwrap()
}

fn scene_1(init : Screen, time : f32, _len : f32) -> Screen {

    let angle = PI / 2.0 * time;

    // Anti Aliasing Factors
    let line_anti_aliasing_factor = 2.0;
    let point_anti_aliasing_factor = 10.0;

    // Calculating Dimensions
    let line_thickness =
        Thickness::Relative(0.022);
    let point_radius =
        Thickness::Relative(0.055);
        
    // Colours
    let sine_colour = 
        Colour::from_hex("#4cd137");
    let cosine_colour = 
        Colour::from_hex("#9c88ff");
    let tangent_colour =
        css_colours::ORANGE_PEEL;
    let off_white = 
        Colour::from_hex("#f5f6fa");


    let cos = angle.cos();
    let sin = angle.sin();
    let sec = 1.0 / cos;

    init
    .render(
        // Sine Line
        Function::new_line_segment(
            Point::new(cos, 0.0),
            Point::new(cos, sin),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            sine_colour,
            line_thickness,
            100,
            RenderingType::RoundAntiAliased(line_anti_aliasing_factor)
        )
    )
    .render(
        // Cosine Line
        Function::new_line_segment(
            Point::new(0.0, sin),
            Point::new(cos, sin),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            cosine_colour,
            line_thickness,
            100,
            RenderingType::RoundAntiAliased(line_anti_aliasing_factor)
        )
    )
    .render(
        // Tangent Line
        Function::new_line_segment(
            Point::new(cos, sin),
            Point::new(sec, 0.0),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            tangent_colour,
            line_thickness,
            if angle.tan() == f32::NAN {1000} else {(100.0 * angle.tan()).abs() as u16},
            RenderingType::RoundAntiAliased(line_anti_aliasing_factor)
        )
    )
    .render(
        // Unit Circle
        Function::new_ellipse(
            1.0, 1.0,
            Point::new(0.0, 0.0),
            (0.0, TAU)
        ),
        FunctionRenderSettings::new(
            Colour::from_rgb(240, 240, 240),
            line_thickness,
            800,
            RenderingType::RoundAntiAliased(line_anti_aliasing_factor)
        )
    )
    .render(
        // Radius
        Function::new_line_segment(
            Point::new(0.0, 0.0),
            Point::new(cos, sin),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            off_white,
            line_thickness,
            100,
            RenderingType::RoundAntiAliased(line_anti_aliasing_factor)
        )
    )
    .render(
        CartesianPlane::new(
            Point::new(-1.6, -1.6),
            Point::new(1.6, 1.6),
            Point::new(0.0, 0.0),
            0.13,
            0.13
        ),
        CartesianPlaneRenderSettings::new(
            css_colours::WHITE,
            line_thickness,
            100
        )
    )
    .render(
        // Sine Endpoint
        Point::new(cos, 0.0),
        PointRenderSettings::new(
            sine_colour,
            point_radius,
            RenderingType::RoundAntiAliased(point_anti_aliasing_factor),
        )
    )
    .render(
        // Cosine Endpoint
        Point::new(0.0, sin),
        PointRenderSettings::new(
            cosine_colour,
            point_radius,
            RenderingType::RoundAntiAliased(point_anti_aliasing_factor)
        )
    )
    .render(
        // Tangent Endpoint
        Point::new(sec, 0.0),
        PointRenderSettings::new(
            tangent_colour,
            point_radius,
            RenderingType::RoundAntiAliased(point_anti_aliasing_factor)
        )
    )
}
