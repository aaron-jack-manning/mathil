#![allow(unused_imports)]

mod mathil;
use mathil::{colours::Colour, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    animate(curve_intersection_generator, 4.0, 60, "")
}

pub fn curve_intersection_generator(time_stamp : f32, frame : u32) -> Screen {

    let line_thickness =
        Thickness::Relative(0.044);
    let cartesian_plane_thickness =
        Thickness::Relative(0.022);

    let mut screen =
        Screen::new(
            3840, 2160,
            Point::new(-7.11, -4.0), Point::new(7.22, 4.0),
            Colour::from_hex("#2c3e50")
        )
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
                if time_stamp < 1.0 {
                    (0.0, smooth(time_stamp, Smoother::Arctan(2.0)))
                }
                else {
                    (0.0, 1.0)
                }
            ),
            FunctionRenderSettings::new(
                Colour::from_hex("#2ecc71"),
                line_thickness,
                if time_stamp < 1.0 {
                    (1000.0 * smooth(time_stamp, Smoother::Arctan(2.0))) as u16
                }
                else {
                    1000
                },
                RenderingType::RoundAntiAliased(2.0)
            )
        );

    if time_stamp > 2.0 {
        screen =
            screen.render(
                Function::new_line_segment(
                    Point::new(-6.11, 1.0),
                    Point::new(6.11, 1.0),
                    if time_stamp < 3.0 {
                        (0.0, smooth(time_stamp - 2.0, Smoother::Arctan(2.0)))
                    }
                    else {
                        (0.0, 1.0)
                    }
                ),
                FunctionRenderSettings::new(
                    Colour::from_hex("#e67e22"),
                    line_thickness,
                    1000,
                    RenderingType::RoundAntiAliased(2.0)
                )
            );
    }

    screen
}