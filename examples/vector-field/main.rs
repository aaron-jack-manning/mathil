use mathil::{
    colours::*,
    maths_objects::*
};

use vector_field;

fn complex_cube(point : Point) -> Point {
    Point::new(
        point.x.powi(3) - 3.0 * point.x * point.y.powi(2),
        3.0 * point.x.powi(2) * point.y - point.y.powi(3)
    )
}

fn main() {
    let points = vector_field::rectangular_point_array(Point::new(-8.0, -4.5), Point::new(8.0, 4.5), 17, 9);

    vector_field::draw(
        Colour::from_hex("#2f3640"), Colour::from_rgb(240, 240, 240),
        3000,
        1.0,
        points,
        complex_cube,
        0.2,
        0.2,
        0.5,
        0.05,
        0.05,
        300,
        400
    ).write_to_png("/home/aaron-manning/Pictures/mathil", "output").unwrap();
}
