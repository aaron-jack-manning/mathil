use std::ops;

use crate::primitive_conversions::*;

/// Represents a point.
#[derive(Copy, Clone, Debug)]
pub struct Point {
    pub x : f32,
    pub y : f32,
}

impl Point {
    /// Creates a Point from a pair of floats.
    pub fn new(x : f32, y : f32) -> Point {
        Point {
            x,
            y,
        }
    }

    /// Creates a list of points from a list of float tuples, reversing the order.
    pub fn many_new(coordinates : Vec<(f32, f32)>) -> Vec<Point> {
        let mut points = Vec::with_capacity(coordinates.len());
        
        for (x, y) in coordinates {
            points.push(Point::new(x, y))
        }

        points
    }

    /// Negates the x coordinate of a point.
    pub fn negate_x(&self) -> Point {
        Point {
            x : - self.x,
            y : self.y,
        }
    }

    /// Negates the y coordinate of a point.
    pub fn negate_y(&self) -> Point {
        Point {
            x : self.x,
            y : - self.y,
        }
    }

    /// Calculates the gradient of the segment from the origin to the specified point.
    pub fn gradient(&self) -> f32 {
        self.y / self.x
    }

    /// Calculates the gradient of the normal to the segment from the origin to the specified point.
    pub fn normal_gradient(&self) -> f32 {
        - self.x / self.y
    }
        

    /// Calculates the distance from the point to the origin.
    pub fn distance(&self) -> f32 {
        (self.x * self.x + self.y * self.y)
        .sqrt()
    }

    /// Rotates the point 90 degrees about the origin clockwise.
    pub fn rotate_clockwise(&self) -> Point {
        Point {
            x : - self.y,
            y : self.x,
        }
    }

    /// Rotates the point 90 degrees about the origin counterclockwise.
    pub fn rotate_counter_clockwise(&self) -> Point {
        Point {
            x : self.y,
            y : - self.x,
        }
    }
        
    /// Linearly interpolates the two specified points.
    pub fn lerp(start : Point, finish : Point, t : f32) -> Point {
        (1.0 - t) * start + t * finish
    }

    /// The origin.
    pub fn origin() -> Point {
        Point::new(0.0, 0.0)
    }
}

/// Adds the two provided points element wise.
impl ops::Add for Point {
    type Output = Self;

    fn add(self, other : Self) -> Self {
        Point {
            x : self.x + other.x,
            y : self.y + other.y,
        }
    }
}

/// Subtracts the two points element wise.
impl ops::Sub for Point {
    type Output = Self;

    fn sub(self, other : Self) -> Self {
        Point {
            x : self.x - other.x,
            y : self.y - other.y,
        }
    }
}

/// Negates both coordinates of a point.
impl ops::Neg for Point {
    type Output = Self;

    fn neg(self) -> Self {
        Point {
            x : -self.x,
            y : -self.y,
        }
    }
}

/// Multiplies the point by the provided scalar.
impl ops::Mul<Point> for f32 {
    type Output = Point;

    fn mul(self, other : Point) -> Self::Output {
        Point {
            x : other.x * self,
            y : other.y * self,
        }
    }
}

/// Multiplies the two points element wise.
impl ops::Mul for Point {
    type Output = Self;

    fn mul(self, other : Self) -> Self {
        Point {
            x : self.x * other.x,
            y : self.y * other.y,
        }
    }
}

/// Represents a mathematical function as a parametric rule and domain.
pub struct Function {
    pub (in crate) rule : Box<dyn Fn(f32) -> Point>,
    pub (in crate) domain : (f32, f32),
}

impl Function {
    /// Creates a Function from a rule and domain.
    pub fn new(rule : Box<dyn Fn(f32) -> Point>, domain : (f32, f32)) -> Function {
        Function {
            rule,
            domain,
        }
    }

    // Creates line segment from the endpoints.
    pub fn new_line_segment(start : Point, end : Point, domain : (f32, f32)) -> Function {
        Function {
            rule : Box::new(
                move |t| {
                    Point::lerp(start, end, t)
                }
            ),
            domain,
        }
    }

    // Creates an ellipse from its dimensions.
    pub fn new_ellipse(radius_x : f32, radius_y : f32, centre : Point, domain : (f32, f32)) -> Function {
        Function::new(
            Box::new(
                move |t : f32| {
                    Point::new(
                        radius_x * t.cos() + centre.x,
                        radius_y * t.sin() + centre.y
                    )
                }
            ),
            domain
        )
    }

    // Creates an circle from its centre and radius.
    pub fn new_circle(radius : f32, centre : Point, domain : (f32, f32)) -> Function {
        Function::new_ellipse(
            radius, radius,
            centre,
            domain
        )
    }

    /// Creates a Bezier curve from a vector of points.
    pub fn new_bezier_curve(points: Vec<Point>, domain : (f32, f32)) -> Function {

        fn interpolate_pairs(points : &Vec<Point>) -> Vec<Box<dyn Fn(f32) -> Point>> {

            let mut pairs =
                Vec::with_capacity(points.len() - 1);

            for i in 0..(points.len() - 1) {
                pairs.push((points[i], points[i + 1]));
            }

            pairs
            .into_iter()
            .map(
                |(p1, p2)| -> Box<dyn Fn(f32) -> Point> {
                    let p1_new = p1;
                    let p2_new = p2;
                    Box::new(
                        move |t| Point::lerp(p1_new, p2_new, t)
                    )
                }
            )
            .collect()
        }

        fn construct_curve(parameter : f32, points : &Vec<Point>) -> Point {
            if points.len() == 1 {
                points[0]
            }
            else {
                let pairs = interpolate_pairs(points);

                let result =
                    pairs
                    .iter()
                    .map(|x| { x(parameter) })
                    .collect();

                construct_curve(parameter, &result)
            }
        }

        Function::new(
            Box::new(
                move |t| construct_curve(t, &points)
            ),
            domain
        )
    }

    /// Samples a function the specified number of times.
    pub fn sample(&self, number_of_samples : u16) -> Vec<Point> {

        fn lerp_scalar(start : f32, end : f32, parameter : f32) -> f32 {
            (1.0 - parameter) * start + parameter * end
        }
        
        let (start, finish) = self.domain;

        let mut samples = Vec::with_capacity(usize::try_from(number_of_samples).unwrap());

        for sample_number in 0..number_of_samples {
            let parameter =
                f32::from(sample_number) / (f32::from(number_of_samples) - 1.0);

            let sample = lerp_scalar(start, finish, parameter);

            samples.push((self.rule)(sample));
        }

        samples
    }
}

/// Represents a dashed line as a vector of line segments.
pub struct DashedLine {
    pub (in crate)  dashes : Vec<Function>
}

impl DashedLine {
    // Creates a new dashed line.
    pub fn new(start : Point, finish : Point, dashes : u32) -> DashedLine {
        let divisions = 2 * dashes - 1;
        let divisions_width_in_parameter = 1.0 / u32_to_f32(divisions);

        let mut segments =
            Vec::with_capacity(usize::try_from(divisions / 2 + 1).unwrap());

        for i in 0..divisions {
            if i % 2 == 0 {
                let segment_start = Point::lerp(start, finish, u32_to_f32(i) * divisions_width_in_parameter);
                let segment_end = Point::lerp(start, finish, (1.0 + u32_to_f32(i)) * divisions_width_in_parameter);

                segments.push(Function::new_line_segment(segment_start, segment_end, (0.0, 1.0)));
            }
        }

        DashedLine {
            dashes : segments
        }
    }
}

/// Represents a polygon as a series of points.
pub struct Polygon {
    pub (in crate) vertices : Vec<Point>,
    pub (in crate) edges : Vec<Function>,
}

impl Polygon {
    /// Creates a new polygon.
    pub fn new(vertices : Vec<Point>) -> Polygon {

        let mut edges = Vec::with_capacity(vertices.len());
        
        edges.push(
            Function::new_line_segment(vertices[0], vertices[vertices.len() - 1], (0.0, 1.0))
        );

        for i in 0..(vertices.len() - 1) {
            edges.push(
                Function::new_line_segment(vertices[i], vertices[i + 1], (0.0, 1.0))
            )
        }

        Polygon {
            vertices,
            edges,
        }
    }
}

// /// Represents a vector as a line segment and polygon.
pub struct Vector {
    pub (in crate) line : Option<Function>,
    pub (in crate) arrow_head : Polygon,
}

impl Vector {
    /// Creates a new Vector.
    pub fn new(head : Point, tail : Point, arrow_width : f32, arrow_height : f32) -> Vector {
        let desired_vector =
            head - tail;
        let vector_length =
            Point::distance(&desired_vector);

        let extension_factor_width =
            (arrow_width / 2.0) / vector_length;
        let extension_factor_length =
            arrow_height / vector_length;

        let head_adjustment_factor =
            arrow_height / vector_length;

        let adjusted_head =
            Point::lerp(
                head,
                tail,
                head_adjustment_factor
            );

        let arrow_head =
            Polygon::new(
                vec![
                    adjusted_head + extension_factor_length * desired_vector,
                    adjusted_head + extension_factor_width * Point::rotate_counter_clockwise(&desired_vector),
                    adjusted_head + extension_factor_width * Point::rotate_clockwise(&desired_vector),
                ]
            );

        if head_adjustment_factor > 1.0 {
            Vector {
                line : None,
                arrow_head
            }
        }
        else {
            Vector {
                line : Some(Function::new_line_segment(adjusted_head, tail, (0.0, 1.0))),
                arrow_head,
            }
        }
    }
}

/// Represents a coordinate plane parallel to the bounds of the image.
pub struct CartesianPlane {
    pub (in crate) bottom_left_bound : Point,
    pub (in crate) top_right_bound : Point,
    pub (in crate) origin : Point,
    pub (in crate) arrow_width : f32,
    pub (in crate) arrow_height : f32,
    //pub (in crate) axis : Vec<Vector>,
}

impl CartesianPlane {
    /// Creates a CartesianPlane.
    pub fn new(bottom_left_bound : Point, top_right_bound : Point, origin : Point, arrow_width : f32, arrow_height : f32) -> CartesianPlane {
        CartesianPlane {
            bottom_left_bound,
            top_right_bound,
            origin,
            arrow_width,
            arrow_height,
        }
    }
}
