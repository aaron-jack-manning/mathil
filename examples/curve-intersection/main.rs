use mathil::{
    colours::*,
    rendering::*,
    maths_objects::*,
    animation::*
};

fn main() {

    let init =
        Screen::new(
        3840, 2160,
        Point::new(-7.11, -4.0),
        Point::new(7.22, 4.0),
        Colour::from_hex("#2c3e50")
    );
    
    let video = Video::new(vec![
        Scene::new(scene_1, 1.0),
        Scene::new(placeholder, 1.0),
        Scene::new(scene_2, 1.0),
        Scene::new(placeholder, 1.0)
    ]);

    video.animate(init, 60, "/home/aaron-manning/Pictures/mathil/").unwrap();
}

fn scene_1(init : Screen, time : f32, len : f32) -> Screen {
    let line_thickness =
        Thickness::Relative(0.044);
    let cartesian_plane_thickness =
        Thickness::Relative(0.022);

    let smoothed_time =
        easy_ease(time / len, EaseFn::Arctan(2.0));

    init
    .render(
        CartesianPlane::new(
            Point::new(-6.11, -3.0),
            Point::new(6.11, 3.0),
            Point::new(0.0, 0.0),
            0.3,
            0.3
        ),
        CartesianPlaneRenderSettings::new(
            Colour::from_hex("#ecf0f1"),
            cartesian_plane_thickness,
            500
        )
    )
    .render(
        Function::new_bezier_curve(
            vec![
                Point::new(-5.0, 2.5),
                Point::new(-4.0, -9.0),
                Point::new(2.0, 9.0),
                Point::new(6.0, -2.0)
            ],
            (0.0, smoothed_time)
        ),
        FunctionRenderSettings::new(
            Colour::from_hex("#2ecc71"),
            line_thickness,
            (1000.0 * smoothed_time) as u16,
            RenderingType::RoundAntiAliased(2.0)
        )
    )   
}

fn scene_2(init : Screen, time : f32, len : f32) -> Screen {
    let line_thickness =
        Thickness::Relative(0.044);

    let smoothed_time = 
        easy_ease(time / len, EaseFn::Arctan(2.0));
        
    init.render(
        Function::new_line_segment(
            Point::new(-6.11, 1.0),
            Point::new(6.11, 1.0),
            (0.0, smoothed_time)
        ),
        FunctionRenderSettings::new(
            Colour::from_hex("#e67e22"),
            line_thickness,
            1000,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
}
