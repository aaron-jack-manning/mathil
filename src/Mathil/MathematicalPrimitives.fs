namespace Mathil

module MathematicalPrimitives =

    /// Represents a point in the 2D coordinate system used in drawing shapes.
    [<StructuredFormatDisplay("({X}, {Y})")>]
    type Point =
        { X : float; Y : float}

        /// Multiplies the coordinates of a point pairwise.
        static member (*) (scalar : float, point : Point) =
            {
                X = scalar * point.X
                Y = scalar * point.Y
            }
        /// Adds the coordinates of a point pairwise.
        static member (+) (point1 : Point, point2 : Point) =
            {
                X = point1.X + point2.X
                Y = point1.Y + point2.Y
            }
        /// Subtracts the coordinates of a point pairwise.
        static member (-) (point1 : Point, point2 : Point) =
            {
                X = point1.X - point2.X
                Y = point1.Y - point2.Y
            }
        /// Negates both coordinates of a point.
        static member neg (point : Point) =
            {
                X = - point.X
                Y = - point.Y
            }
        /// Negates the x coordinate of a point.
        static member negx (point : Point) =
            {
                X = - point.X
                Y = point.Y
            }
        /// Negates the y coordinate of a point.
        static member negy (point : Point) =
            {
                X = point.X
                Y = - point.Y
            }

    /// Creates a Point from a float tuple.
    let createPoint (x : float, y : float) =
        { X = x; Y = y }

    /// Creates a list of points from a list of float tuples.
    let pointsFromTupleList (coordinates : (float * float) list) = 
        [
            for i = 0 to List.length coordinates - 1 do
                createPoint coordinates.[i]
        ]
    
    /// Represents the allowing values a parameter can take in a function.
    type Domain = float * float

    /// Represents a mathematical function as a parametric rule and domain.
    type Function =
        { Rule : float -> Point; Domain : Domain }

    /// Creates a Function from a rule and domain.
    let createFunction (rule : float -> Point) (domain : Domain) : Function =
        { Rule = rule; Domain = domain }

    /// Represents a polygon as a series of points.
    type Polygon = Point list
