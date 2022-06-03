use std::io::Write;
use std::fs::File;
use std::io::BufWriter;

use png;

use crate::{
    primitive_conversions::*,
    colours::Colour,
    maths_objects::*,
    output::{
        bitmap::*,
        png::*,
        generate_file_path
    },
    errors,
    rendering::utilities::*
};

/// Represents an image.
#[derive(Clone)]
pub struct Screen {
    pub (in crate) pixels : Vec<Vec<Colour>>,
    pub (in crate) horizontal_resolution : u16,
    pub (in crate) vertical_resolution : u16,
    pub (in crate) bottom_left_bound : Point,
    pub (in crate) top_right_bound : Point
}

impl Screen {

    /// Creates a blank screen based on the resolution, bounds and colour.
    pub fn new(horizontal_resolution : u16, vertical_resolution : u16, bottom_left_bound : Point, top_right_bound : Point, default_colour : Colour) -> Screen {

        let pixels = vec![
            vec![default_colour; vertical_resolution.into()]; horizontal_resolution.into()
        ];

        Screen {
            pixels,
            horizontal_resolution,
            vertical_resolution,
            bottom_left_bound,
            top_right_bound,
        }
    }

    /// Writes the current screen to a 24-bit uncompressed BitMap at the specified location.
    pub fn write_to_bitmap(&self, output_folder : &str, filename : &str) -> Result<(), errors::Error> {
        let file_path =
            generate_file_path(output_folder, filename, "bmp")?;

        let bitmap_bytes =
            create_bitmap_bytes(self);

        match File::create(&file_path) {
            Ok(mut file) => {
                match file.write_all(&bitmap_bytes) {
                    Ok(()) => (), 
                    Err(e) => return Err(errors::Error::FileWrite(file_path, e)),
                }
            }
            Err(e) => {
                return Err(errors::Error::FileCreation(file_path, e))
            }
        }

        //File::create(&file_path)
        //.map_err(|e| errors::Error::FileCreation(file_path, e))?
        //.write_all(&bitmap_bytes)
        //.map_err(|e| errors::Error::FileWrite(file_path, e))?;

        Ok(())
    }

    /// Writes the current screen to a PNG using the PNG crate.
    pub fn write_to_png(&self, output_folder : &str, filename : &str) -> Result<(), errors::Error> {
        let file_path =
            generate_file_path(output_folder, filename, "png")?;

        //let png_file =
        //    File::create(&file_path)
        //    .map_err(|e| errors::Error::FileCreation(file_path, e))?;

        let png_file =
            match File::create(&file_path) {
                Ok(file) => file,
                Err(e) => return Err(errors::Error::FileCreation(file_path, e)),
            };
        
        let buf_writer =
            &mut BufWriter::new(png_file);

        let mut encoder =
            png::Encoder::new(
                buf_writer,
                u32::from(self.horizontal_resolution),
                u32::from(self.vertical_resolution)
            );

        encoder.set_depth(png::BitDepth::Eight);
        encoder.set_color(png::ColorType::Rgb);
        
        encoder.set_compression(png::Compression::Fast);
        
        let mut writer =
            match encoder.write_header() {
                Ok(writer) => writer,
                Err(e) => return Err(errors::Error::PngError(file_path, e)),
            };

        //let mut writer =
        //    encoder
        //    .write_header()
        //    .map_err(|e| errors::Error::PngError(file_path, e))?;

        let data = create_rgb_byte_array(self);

        writer
        .write_image_data(&data)
        .map_err(|e| errors::Error::PngError(file_path, e))?;

        Ok(())
    }


    /// Renders the provided renderable type to the screen.
    pub fn render<R>(mut self, renderable : R, settings : <R as Renderable>::RenderSettings) -> Self
        where R: Renderable {
        renderable.render(&settings, &mut self);

        self
    }

    /// Renders many of the provided  renderable type to the screen.
    pub fn render_many<R>(mut self, renderables : Vec<Box<R>>, settings : <R as Renderable>::RenderSettings) -> Self
        where R: Renderable {

        Renderable::render_many(renderables, &settings, &mut self);

        self
    }

    /// Fills a region of a screen which has a solid colour with another solid colour.
    pub fn fill(mut self, starting_point : Point, desired_colour : Colour) -> Self {
        
        let starting_location =
            point_to_pixel_coordinates(&self, starting_point);

        let initial_colour =
            self.pixels
            [usize::try_from(starting_location.x).unwrap()]
            [usize::try_from(starting_location.y).unwrap()];

        if initial_colour != desired_colour {

            let mut current_checks = vec![starting_location];

            while !current_checks.is_empty() {
                let current = current_checks[current_checks.len() - 1];

                let is_on_screen =
                    within_screen(current, &self);

                let is_original_colour =
                    self.pixels
                    [usize::try_from(current.x).unwrap()]
                    [usize::try_from(current.y).unwrap()] == initial_colour;

                if is_on_screen && is_original_colour {
                    
                    self.pixels
                    [usize::try_from(current.x).unwrap()]
                    [usize::try_from(current.y).unwrap()] = desired_colour;

                    current_checks.pop();

                    current_checks.push(PixelCoordinate::new(current.x, current.y + 1));
                    current_checks.push(PixelCoordinate::new(current.x + 1, current.y));
                    current_checks.push(PixelCoordinate::new(current.x, current.y - 1));
                    current_checks.push(PixelCoordinate::new(current.x - 1, current.y));
                }
                else {
                    current_checks.pop();
                }
            }
        }

        self
    }
}

mod utilities {
    use crate::rendering::*;

    /// Gets the least upper bounding rectangle from a series of points.
    pub (in crate) fn get_bounds(vertices : &Vec<Point>) -> (Point, Point) {

        let mut min_x = vertices[0].x;
        let mut min_y = vertices[0].y;
        let mut max_x = vertices[0].x;
        let mut max_y = vertices[0].y;

        for point in vertices {
            if point.x < min_x {
                min_x = point.x;
            }
            if point.y < min_y {
                min_y = point.y;
            }
            if point.x > max_x {
                max_x = point.x;
            }
            if point.y > max_y {
                max_y = point.y;
            }
        }

        (Point::new(min_x, min_y), Point::new(max_x, max_y))
    }

    /// Determines if the provided point lies within the provided polygon.
    pub (in crate) fn is_inside_polygon(point : Point, vertices : &Vec<Point>) -> bool {

        let mut sides =
            Vec::with_capacity(vertices.len());

        sides.push(
            (vertices[vertices.len() - 1], vertices[0])
        );

        for i in 0..(vertices.len() - 1) {
            sides.push(
                (vertices[i], vertices[i + 1])
            )
        }


        let mut is_inside = false;

        for (endpoint_1, endpoint_2) in sides {
            if
                (endpoint_1.y > point.y) != (endpoint_2.y > point.y)
                &&
                point.x < (endpoint_2.x - endpoint_1.x) * (point.y - endpoint_1.y) / (endpoint_2.y - endpoint_1.y) + endpoint_1.x {

                is_inside = !is_inside;
            }
        }

        is_inside
    }

    /// Linear interpolation of floats.
    pub (in crate) fn lerp_scalar(start : f32, end : f32, parameter : f32) -> f32 {
        (1.0 - parameter) * start + parameter * end
    }

    /// Converts a Point to a PixelCoordinate depending on the screen.
    pub (in crate) fn point_to_pixel_coordinates(screen : &Screen, point : Point) -> PixelCoordinate {
        let horizontal_parameter = (point.x - screen.bottom_left_bound.x) / (screen.top_right_bound.x - screen.bottom_left_bound.x);

        let vertical_parameter = (point.y - screen.bottom_left_bound.y) / (screen.top_right_bound.y - screen.bottom_left_bound.y);

        PixelCoordinate::new(
            f32_to_i32(lerp_scalar(0.0, f32::from(screen.horizontal_resolution), horizontal_parameter)),
            f32_to_i32(lerp_scalar(0.0, f32::from(screen.vertical_resolution), vertical_parameter)),
        )
    }

    /// Converts a PixelCoordinate to a Point depending on the screen.
    pub (in crate) fn pixel_coordinates_to_point(screen : &Screen, coordinates : PixelCoordinate) -> Point {
        let horizontal_parameter =
            i32_to_f32(coordinates.x) / f32::from(screen.horizontal_resolution);
        let vertical_parameter =
            i32_to_f32(coordinates.y) / f32::from(screen.vertical_resolution);

        Point::new(
            lerp_scalar(screen.bottom_left_bound.x, screen.top_right_bound.x, horizontal_parameter),
            lerp_scalar(screen.bottom_left_bound.y, screen.top_right_bound.y, vertical_parameter),
        )
    }

    /// Determines if the provided PixelCoordinate lies on the screen.
    pub (in crate) fn within_screen (coordinates : PixelCoordinate, screen : &Screen) -> bool {
        coordinates.x >= 0
        && coordinates.y >= 0
        && coordinates.x < i32::from(screen.horizontal_resolution) 
        && coordinates.y < i32::from(screen.vertical_resolution)
    }
}

/// Represents a point on the screen.
#[derive(Copy, Clone, Debug)]
pub (in crate) struct PixelCoordinate {
    pub (in crate) x : i32,
    pub (in crate) y : i32,
}

impl PixelCoordinate {    
    /// Creates a new PixelCoordinate.
    fn new(x : i32, y : i32) -> PixelCoordinate {
        PixelCoordinate {
            x,
            y,
        }
    }
}

/// Possible rendering types.
#[derive(Copy, Clone)]
pub enum RenderingType {
    Square,
    RoundAntiAliased(f32),
    RoundAliased,
}

/// Line thickness and point radius represented either as a relative length based on the coordinate system or as an absolute length as a number of pixels. When specified relatively, the actual number of pixels will be an average of the number of pixels across the horizontal and vertical directions.
#[derive(Copy, Clone)]
pub enum Thickness {
    Absolute(u16),
    Relative(f32),
}

impl Thickness {
    pub fn to_pixels(&self, screen : &Screen) -> u16 { // only temporarily public
        match self {
            Thickness::Absolute(length) => {
                *length
            },
            Thickness::Relative(length) => {
                let horizontal = (length / (screen.top_right_bound.x - screen.bottom_left_bound.x)) * f32::try_from(screen.horizontal_resolution).unwrap();
                let vertical = (length / (screen.top_right_bound.y - screen.bottom_left_bound.y)) * f32::try_from(screen.vertical_resolution).unwrap();

                f32_to_u16((horizontal + vertical) / 2.0)
            }
        }
    }
}

pub trait Renderable {
    type RenderSettings;

    /// Renders the renderable item on the provided screen.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen);

    /// Renders many of the renderable item on the provided screen.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen);
}

/// Represents the rendering settings for a Point.
pub struct PointRenderSettings {
    colour : Colour,
    radius : Thickness,
    rendering_type : RenderingType,
}

impl PointRenderSettings {
    /// Creates a new PointRenderSettings.
    pub fn new(colour : Colour, radius : Thickness, rendering_type : RenderingType) -> PointRenderSettings {
        PointRenderSettings {
            colour,
            radius,
            rendering_type,
        }
    }
}

impl Renderable for Point {
    type RenderSettings =
        PointRenderSettings;

    /// Renders a Point.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {
        let coordinate = point_to_pixel_coordinates(screen, self);

        // this radius should be in terms of the coordinate system, and therefore needs to be converted properly using similar method to point to pixel coordinates
        let radius = i32::from(
                settings.radius.to_pixels(screen)
            );

        for i in (coordinate.x - radius)..(coordinate.x + radius) {
            for j in (coordinate.y - radius)..(coordinate.y + radius) {

                let is_on_screen =
                    within_screen(PixelCoordinate::new(i, j), screen);

                let squared_distance =
                    (i - coordinate.x) * (i - coordinate.x) + (j - coordinate.y) * (j - coordinate.y);

                let within_radius =
                    squared_distance < radius * radius;

                if is_on_screen {
                    let i_usize = usize::try_from(i).unwrap();
                    let j_usize = usize::try_from(j).unwrap();

                    match settings.rendering_type {
                        RenderingType::Square => {
                            screen.pixels[i_usize][j_usize] = settings.colour;
                        }
                        RenderingType::RoundAliased => {
                            if within_radius {
                                screen.pixels[i_usize][j_usize] = settings.colour;
                            }
                        }
                        RenderingType::RoundAntiAliased(anti_aliasing_factor) => {
                            if within_radius {
                                let previous_colour = screen.pixels[i_usize][j_usize];
        
                                let colour_lerp_parameter =
                                    (i32_to_f32(squared_distance) / i32_to_f32(radius * radius)).powf(anti_aliasing_factor);
                                
                                let new_colour = Colour::lerp(
                                    settings.colour,
                                    previous_colour,
                                    colour_lerp_parameter
                                );
        
                                screen.pixels[i_usize][j_usize] = new_colour;
                            }
                        }
                    }
                }
            }
        }
    }

    /// Renders many Points.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for point in selfs {
            Self::render(*point, settings, screen);
        }
    }
}

/// Represents the rendering settings for a Function.
pub struct FunctionRenderSettings {
    colour : Colour,
    thickness : Thickness,
    samples : u16,
    rendering_type : RenderingType,
}

impl FunctionRenderSettings {
    /// Creates a new FunctionRenderSettings.
    pub fn new(colour : Colour, thickness : Thickness, samples : u16, rendering_type : RenderingType) -> FunctionRenderSettings {
        FunctionRenderSettings {
            colour,
            thickness,
            samples,
            rendering_type,
        }
    }
}

impl Renderable for Function {
    type RenderSettings =
        FunctionRenderSettings;

    /// Renders a Function.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {

        let samples = Function::sample(&self, settings.samples);

        for sample in samples {
            sample.render(
                &PointRenderSettings::new(
                    settings.colour,
                    settings.thickness,
                    settings.rendering_type
                ),
                screen
            );
        }
    }

    /// Renders many Functions.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for function in selfs {
            function.render(settings, screen);
        }
    }
}

/// Represents the rendering settings for a Polygon's sides.
pub struct PolygonSidesRenderSettings {
    colour : Colour,
    thickness : Thickness,
    samples_per_side : u16,
    rendering_type : RenderingType,
}

impl PolygonSidesRenderSettings {
    /// Creates a new PolygonSidesRenderSettings.
    pub fn new(colour : Colour, thickness : Thickness, samples_per_side : u16, rendering_type : RenderingType) -> PolygonSidesRenderSettings {
        PolygonSidesRenderSettings {
            colour,
            thickness,
            samples_per_side,
            rendering_type,
        }
    }
}

/// Represents the rendering settings for a Polygon's fill.
pub struct PolygonFillRenderSettings {
    colour : Colour,
}

impl PolygonFillRenderSettings {
    /// Creates a new PolygonFillRenderSettings.
    pub fn new(colour : Colour) -> PolygonFillRenderSettings {
        PolygonFillRenderSettings {
            colour,
        }
    }
}

/// Represents the rendering settings for a Polygon.
pub struct PolygonRenderSettings {
    sides : Option<PolygonSidesRenderSettings>,
    fill : Option<PolygonFillRenderSettings>,
}

impl PolygonRenderSettings {
    /// Creates a new PolygonRenderSettings.
    pub fn new(sides : Option<PolygonSidesRenderSettings>, fill : Option<PolygonFillRenderSettings>) -> PolygonRenderSettings {
        PolygonRenderSettings {
            sides,
            fill,
        }
    }
}

impl Polygon {
    /// Renders a solid polygon of the specified colour, independent of the background. Use this instead of Screen::fill when other elements may already rendered where the polygon should go, that mean the desired result cannot be achieved by just swapping solid colours.
    fn render_fill(vertices : Vec<Point>, settings : &PolygonFillRenderSettings, screen : &mut Screen) {

        let (bottom_left_point, top_right_point) =
            get_bounds(&vertices);

        let bottom_left = point_to_pixel_coordinates(screen, bottom_left_point);
        let top_right = point_to_pixel_coordinates(screen, top_right_point);

        for i in (bottom_left.x)..=(top_right).x {
            for j in (bottom_left.y)..=(top_right.y) {
                let current_coordinates =
                    PixelCoordinate::new(i, j);
                
                let current_point =
                    pixel_coordinates_to_point(screen, current_coordinates);

                let on_screen =
                    within_screen(current_coordinates, screen);

                if is_inside_polygon(current_point, &vertices) && on_screen {
                    screen.pixels[usize::try_from(i).unwrap()][usize::try_from(j).unwrap()] = settings.colour;
                }
            }
        }
    }

    /// Renders the sides of a polygon.
    fn render_sides(edges : Vec<Function>, settings : &PolygonSidesRenderSettings, screen : &mut Screen) {

        let mut boxed_edges =
            Vec::with_capacity(edges.len());

        for edge in edges {
            boxed_edges.push(
                Box::new(
                    edge
                )
            )
        }

        Function::render_many(boxed_edges, &FunctionRenderSettings::new(settings.colour, settings.thickness, settings.samples_per_side, settings.rendering_type), screen);
    }
}

impl Renderable for Polygon {
    type RenderSettings =
        PolygonRenderSettings;

    /// Renders a Polygon.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {
        let edges = self.edges;
        let vertices = self.vertices;

        match &settings.fill {
            Some(edges_settings) => {
                Polygon::render_fill(vertices, edges_settings, screen);
            }
            None => {}
        }
        
        match &settings.sides {
            Some(sides_settings) => {
               Polygon::render_sides(edges, sides_settings, screen)
            }
            None => {}
        }
    }

    /// Renders many polygons.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for polygon in selfs {
            polygon.render(settings, screen);
        }
    }
}

/// Represents the rendering settings for a Vector.
pub struct VectorRenderSettings {
    colour : Colour,
    thickness : Thickness,
    samples : u16,
    rendering_type : RenderingType,
}

impl VectorRenderSettings {
    /// Creates a new VectorRenderSettings.
    pub fn new(colour : Colour, thickness : Thickness, samples : u16, rendering_type : RenderingType) -> VectorRenderSettings {
        VectorRenderSettings {
            colour,
            thickness,
            samples,
            rendering_type,
        }
    }
}

impl Renderable for Vector {
    type RenderSettings =
        VectorRenderSettings;

    /// Renders a vector.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {
        self.arrow_head.render(
            &PolygonRenderSettings::new(
                None,
                Some(PolygonFillRenderSettings::new(
                    settings.colour
                ))
            ),
            screen
        );
        self.line.render(
            &FunctionRenderSettings::new(
                settings.colour,
                settings.thickness,
                settings.samples,
                settings.rendering_type
            ),
            screen
        );
    }

    /// Renders many Vectors.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for vector in selfs {
            vector.render(settings, screen);
        }
    }
}

/// Represents the rendering settings for a DashedLine.
pub struct DashedLineRenderSettings {
    colour : Colour,
    thickness : Thickness,
    samples_per_dash : u16,
    rendering_type : RenderingType,
}

impl DashedLineRenderSettings {
    /// Creates a new DashedLineRenderSettings.
    pub fn new(colour : Colour, thickness : Thickness, samples_per_dash : u16, rendering_type : RenderingType) -> DashedLineRenderSettings {
        DashedLineRenderSettings {
            colour,
            thickness,
            samples_per_dash,
            rendering_type,
        }
    }
}

impl Renderable for DashedLine {
    type RenderSettings =
        DashedLineRenderSettings;

    /// Renders a DashedLine.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {
        
        let mut references = Vec::with_capacity(self.dashes.len());

        for dash in self.dashes {
            references.push(Box::new(dash));
        }

        let function_settings =
            FunctionRenderSettings::new(
                settings.colour,
                settings.thickness,
                settings.samples_per_dash,
                settings.rendering_type
            );
        
        Function::render_many(references, &function_settings, screen);
    }

    /// Renders many DashedLines.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for dashed_line in selfs {
            dashed_line.render(settings, screen);
        }
    }
}

/// Represents the rendering settings for a CartesianPlane.
pub struct CartesianPlaneRenderSettings {
    colour : Colour,
    thickness : Thickness,
    samples_per_axis : u16,
}

impl CartesianPlaneRenderSettings {
    /// Creates a new CartesianPlaneRenderSettings.
    pub fn new(colour : Colour, thickness : Thickness, samples_per_axis : u16) -> CartesianPlaneRenderSettings {
        CartesianPlaneRenderSettings {
            colour,
            thickness,
            samples_per_axis,
        }
    }
}

impl Renderable for CartesianPlane {
    type RenderSettings =
        CartesianPlaneRenderSettings;

    /// Renders a CartesianPlane.
    fn render(self, settings : &Self::RenderSettings, screen : &mut Screen) {
        for vector in self.axis {
            vector.render(
                &VectorRenderSettings::new(
                    settings.colour,
                    settings.thickness,
                    settings.samples_per_axis,
                    RenderingType::Square
                ),
                screen
            );
        }
    }

    /// Renders many CartesianPlanes.
    fn render_many(selfs : Vec<Box<Self>>, settings : &Self::RenderSettings, screen : &mut Screen) {
        for cartesian_plane in selfs {
            cartesian_plane.render(
                settings,
                screen
            );
        }
    }
}
