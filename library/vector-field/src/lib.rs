use mathil::{
    colours::*,
    maths_objects::*,
    rendering::*
};

pub fn draw(background_colour : Colour, axis_colour : Colour, horizontal_resolution : u16, margin : f32, points : Vec<Point>, field : fn(Point) -> Point, arrow_width : f32, arrow_height : f32, vector_length : f32, vector_thickness : f32, axis_thickness : f32, vector_samples : u16, axis_samples : u16) -> Screen {

    let mut bottom_left = Point::new(std::f32::MAX, std::f32::MAX);
    let mut top_right = Point::new(std::f32::MIN, std::f32::MIN);
    
    for point in points.iter() {
        bottom_left.x = if point.x < bottom_left.x { point.x } else { bottom_left.x };
        bottom_left.y = if point.y < bottom_left.y { point.y } else { bottom_left.y };
        top_right.x = if point.x > top_right.x { point.x } else { top_right.x };
        top_right.y = if point.y > top_right.y { point.y } else { top_right.y };
    }

    let bounding_box = (Point::subtract(bottom_left, Point::new(margin, margin)), Point::add(top_right, Point::new(margin, margin)));

    let vertical_resolution : u16 =
        ((bounding_box.1.y - bounding_box.0.y) * f32::from(horizontal_resolution) / (bounding_box.1.x - bounding_box.0.x)) as u16;

    let mut screen = 
        Screen::new(
            horizontal_resolution, vertical_resolution,
            bounding_box.0, bounding_box.1,
            background_colour
        )
        .render(
            CartesianPlane::new(
                bottom_left,
                top_right,
                Point::origin(),
                arrow_width,
                arrow_height
            ),
            CartesianPlaneRenderSettings::new(
                axis_colour,
                Thickness::Relative(axis_thickness),
                axis_samples
            )
        );
    
    let shrink_geometric_vector = |(tail, head)| {
        let difference = Point::subtract(head, tail);
        let magnitude = difference.distance();

        let new_head =
            Point::add(
                tail, Point::multiply_scalar(difference, vector_length / magnitude)
            );

        (tail, new_head, magnitude)
    };

    let mut vectors : Vec<(Vector, f32)> =
        points
        .clone()
        .into_iter()
        .zip(points.into_iter().map(|p| Point::add(p, field(p))))
        .map(shrink_geometric_vector)
        .map(|(t, h, m)| (Vector::new(h, t, arrow_width, arrow_height), m))
        .collect();

    let max_magnitude =
        *(vectors
        .iter()
        .map(|(_, m)| m)
        .max_by(|m1, m2| m1.partial_cmp(m2).unwrap())
        .unwrap());

    let min_magnitude =
        *(vectors
        .iter()
        .map(|(_, m)| m)
        .min_by(|m1, m2| m1.partial_cmp(m2).unwrap())
        .unwrap());

    vectors =
        vectors
        .into_iter()
        .map(|(v, m)| (v, (m - min_magnitude) / (max_magnitude - min_magnitude)))
        .collect();

    for (vector, length) in vectors {
        screen = screen.render(
            vector,
            VectorRenderSettings::new(
                rainbow((1.0 - length) * 0.8),
                Thickness::Relative(vector_thickness),
                vector_samples,
                RenderingType::RoundAntiAliased(2.0)
            )
        );
    }

    screen
}

pub fn rectangular_point_array(bottom_left : Point, top_right : Point, x_qty : u16, y_qty : u16) -> Vec<Point> {
    let mut points = Vec::with_capacity(usize::try_from(x_qty * y_qty).unwrap());

    let x_unit = (top_right.x - bottom_left.x) / f32::from(x_qty - 1);
    let y_unit = (top_right.y - bottom_left.y) / f32::from(y_qty - 1);

    for i in 0..x_qty {
        for j in 0..y_qty {
            let x = bottom_left.x + x_unit * f32::from(i);
            let y = bottom_left.y + y_unit * f32::from(j);

            points.push(Point::new(x, y));
        }
    }

    points
}
