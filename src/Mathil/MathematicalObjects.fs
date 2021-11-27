namespace Mathil

open System

module MathematicalObjects =

    /// Mathematical constants.
    module Constants =
        /// Ratio of circumference to diameter of circle.
        let pi = 3.14159

        /// Ratio of circumference to radius of circle.
        let tau = 6.28319

        /// Euler's constant.
        let e = 2.71828


    /// Types to represent mathematical objects.
    module Types =
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

        /// Represents a mathematical function as a parametric rule and domain.
        type Function =
            {
                Rule : float -> Point
                Domain : float * float
            }

        /// Represents a Bezier curve as a function.
        type BezierCurve = Function

        /// Represents a line segment as a function.
        type LineSegment = Function

        /// Represents a dashed line as a list of line segments.
        type DashedLine = LineSegment list

        /// Represents a polygon as a series of points.
        type Polygon =
            {
                Vertices : Point list
                Edges : LineSegment list
            }

        /// Represents a vector as a line segment and polygon.
        type Vector =
            {
                Line : LineSegment
                ArrowHead : Polygon
            }

        /// Represents a circle as a function.
        type Circle = Function


    /// Functions for operating on points.
    module Point =
        
        open Types

        /// Negates both coordinates of a point.
        let negate (point : Point) : Point =
            {
                X = - point.X
                Y = - point.Y
            }
        /// Negates the x coordinate of a point.
        let negateX (point : Point) : Point =
            {
                X = - point.X
                Y = point.Y
            }
        /// Negates the y coordinate of a point.
        let negateY (point : Point) : Point =
            {
                X = point.X
                Y = - point.Y
            }

        /// Calculates the gradient of the segment from the origin to the specified point.
        let gradient (point : Point) : float =
            point.Y / point.X

        /// Calculates the gradient of the normal to the segment from the origin to the specified point.
        let normalGradient (point : Point) : float =
            - point.X / point.Y

        /// Calculates the distance from the point to the origin.
        let distance (point : Point) : float =
            Math.Sqrt(point.X * point.X + point.Y * point.Y)

        /// Rotates the point 90 degrees about the origin clockwise.
        let rotateClockwise (point : Point) : Point =
            {
                X = - point.Y
                Y = point.X
            }

        /// Rotates the point 90 degrees about the origin counterclockwise.
        let rotateCounterClockwise (point : Point) : Point =
            {
                X = point.Y
                Y = - point.X
            }

        /// Linearly interpolates the two specified points.
        let lerp (start : Point) (finish : Point) : (float -> Point) =
            (fun t -> (1.0 - t) * start + t * finish)


    /// Standard parametric functions (float -> Point).
    module Parametric =

        open Types

        /// Parametric sine function.
        let inline sin (x : float) : Point = { X = x; Y = Math.Sin(x) }

        /// Parametric cosine function.
        let inline cos (x : float) : Point = { X = x; Y = Math.Cos(x) }

        /// Parametric tangent function.
        let inline tan (x : float) : Point = { X = x; Y = Math.Tan(x) }

        /// Parametric secant function.
        let inline sec (x : float) : Point = { X = x; Y = 1.0 / Math.Cos(x) } 

        /// Parametric cosecant function.
        let inline csc (x : float) : Point = { X = x; Y = 1.0 / Math.Sin(x) }

        /// Parametric cotangent function.
        let inline cot (x : float) : Point = { X = x; Y = Math.Cos(x) / Math.Sin(x) }

        /// Parametric natural logarithm function.
        let inline ln (x : float) : Point = { X = x; Y = Math.Log(x) }

        /// Parametric exponential function.
        let inline exp (x : float) : Point = { X = x; Y = Math.Exp(x) }

        /// Parametric ellipse function.
        let inline ellipse (rx : float) (ry : float) (x1 : float) (y1  : float) (t  : float) : Point = { X = rx * Math.Cos(t) + x1; Y = ry * Math.Sin(t) + y1 }

        /// Parametric rose function.
        let inline rose (a : float) (t : float) : Point = { X = Math.Cos(a * t) * Math.Cos(t); Y = Math.Cos(a * t) * Math.Sin(t) }


    // Creation Functions -----------------------------------------------------------

    open Types

    /// Creates a Point from a float tuple.
    let createPoint (x : float, y : float) : Point =
        {
            X = x
            Y = y
        }

    /// Creates a list of points from a list of float tuples.
    let createPoints (coordinates : (float * float) list) : Point list = 
        [
            for i = 0 to List.length coordinates - 1 do
                createPoint coordinates.[i]
        ]

    /// Creates a Function from a rule and domain.
    let createFunction (rule : float -> Point) (domain : float * float) : Function =
        {
            Rule = rule
            Domain = domain
        }

    /// Creates a Bezier curve from a list of points.
    let createBezierCurve (points : Point list) : BezierCurve =

        let interpolatePairs (list : Point list) =
            [
                for i = 0 to List.length list - 2 do
                    Point.lerp list.[i] list.[i + 1]
            ]

        let rec constructCurve (parameter : float) (points : Point list) : Point =

            if List.length points = 1 then
                points.[0]
            else
                interpolatePairs points
                |> List.map (fun x -> x parameter)
                |> constructCurve parameter

        {
            Rule = (fun t -> constructCurve t points)
            Domain = (0.0, 1.0)
        }

    /// Creates a line from its endpoints.
    let createLineSegment (start : Point) (finish : Point) : LineSegment =
        createBezierCurve [start; finish]

    /// Creates a series of short lines that create a longer dashed line.
    let createDashedLine (start : Point) (finish : Point) (dashes : int) : DashedLine =
    
        let divisions = 2 * dashes - 1
        let divisionWidthInParameter = 1.0 / float divisions

        [
            for i = 0 to divisions - 1 do
                if i % 2 = 0 then
                    let segmentStart = Point.lerp start finish (float i * divisionWidthInParameter)
                    let segmentEnd = Point.lerp start finish ((float i + 1.0) * divisionWidthInParameter)

                    yield createLineSegment segmentStart segmentEnd
        ]

    /// Creates a polygon from a list of points.
    let createPolygon (vertices : Point list) : Polygon =
        {
            Vertices = vertices
            Edges =
                [        
                    for i = 0 to List.length vertices - 1 do
                        if i = List.length vertices - 1 then
                            createLineSegment vertices.[i] vertices.[0]
                        else
                            createLineSegment vertices.[i] vertices.[i + 1]
                ]
        }

    /// Creates a vector from its head, tail and the dimensions of the arrow head.
    let createVector (head : Point) (tail : Point) (arrowWidth : float) (arrowHeight : float) : Vector =
    
        let desiredVector = head - tail
        let vectorLength = Point.distance desiredVector
    
        let extensionFactorWidth = (arrowWidth / 2.0) / vectorLength
        let extensionFactorLength = arrowHeight / vectorLength

        let arrowHead =
            createPolygon
                [
                    head + extensionFactorLength * desiredVector
                    head + extensionFactorWidth * (Point.rotateCounterClockwise desiredVector)
                    head + extensionFactorWidth * (Point.rotateClockwise desiredVector)
                ]
    
        {
            Line = createLineSegment head tail
            ArrowHead = arrowHead
        }

    /// Creates a circle from its centre and radius.
    let createCircle (radius : float) (centre : Point) : Circle =
        {
            Rule =
                (fun t ->
                    {
                        X = radius * Math.Cos(t) + centre.X
                        Y = radius * Math.Sin(t) + centre.Y
                    })
            Domain = (0.0, 2.0 * Constants.pi)
        }
