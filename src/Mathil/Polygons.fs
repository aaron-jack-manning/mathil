namespace Mathil

open MathematicalPrimitives
open BezierCurves
open FunctionSampling

module Polygons =

    /// Describes the orientation of three points on a two dimensional plane.
    type Orientation =
        | Clockwise
        | CounterClockwise
        | Collinear

    /// Converts a polygon to a series of points along its sides.
    let polygonToManyPoints (polygon : Polygon) (samplesPerSide : int) : Point list =

        [
            for i = 0 to List.length polygon - 1 do
                if i = List.length polygon - 1 then
                    createBezierCurve [polygon.[i]; polygon.[0]]
                else
                    createBezierCurve [polygon.[i]; polygon.[i + 1]]
        ]
        |> List.map (fun edge -> sample samplesPerSide edge)
        |> List.concat

    /// Calculates the smallest rectangle with sides parrallel to the coordinate system which contains the specified polygon.
    let polygonBounds (polygon : Polygon) : Point * Point =
    
        let minimumX =
            (polygon
            |> List.minBy (fun p -> p.X)).X

        let minimumY =
            (polygon
            |> List.minBy (fun p -> p.Y)).Y

        let maximumX =
            (polygon
            |> List.maxBy (fun p -> p.X)).X

        let maximumY =
            (polygon
            |> List.maxBy (fun p -> p.Y)).Y

        (
            { X = minimumX; Y = minimumY },
            { X = maximumX; Y = maximumY }
        )

    let private getOrientation (a : Point) (b : Point) (c : Point) : Orientation =
    
        if (c.Y - a.Y) * (b.X - a.X) < (b.Y - a.Y) * (c.X - a.X) then
            Clockwise
        elif (c.Y - a.Y) * (b.X - a.X) = (b.Y - a.Y) * (c.X - a.X) then
            Collinear
        else
            CounterClockwise

    let private withinBounds (point : Point) (bounds : Point * Point) =
    
        let bottomLeft, topRight = bounds

        bottomLeft.X <= point.X
        && point.X <= topRight.X
        && bottomLeft.Y <= point.Y
        && point.Y <= topRight.Y

    let private checkLineSegmentsIntersect (segment1 : Point * Point) (segment2 : Point * Point) : bool =
    
        let p1, q1 = segment1
        let p2, q2 = segment2

        let o1 = getOrientation p1 q1 p2
        let o2 = getOrientation p1 q1 q2
        let o3 = getOrientation p2 q2 p1
        let o4 = getOrientation p2 q2 q1

        o1 <> o2 && o3 <> o4
        || (o1 = Collinear && withinBounds p2 (p1, q1))
        || (o2 = Collinear && withinBounds q2 (p1, q1))
        || (o3 = Collinear && withinBounds p1 (p2, q2))
        || (o4 = Collinear && withinBounds q1 (p2, q2))

    let private getSides (polygon : Polygon) : (Point * Point) list =
        let ends = (List.last polygon, polygon.[0])
    
        let middle =
            [
                for i = 0 to List.length polygon - 2 do
                    (polygon.[i], polygon.[i + 1])
            ]

        ends :: middle

    let private checkEndpoint (polygonSide : Point * Point) (line : Point * Point) : bool =
        
        let lineStart, lineEnd = line
        let endpoint1, endpoint2 = polygonSide

        getOrientation endpoint1 lineStart lineEnd = Collinear

    
    /// Determines if a point lies within a polygon.
    let isInsidePolygon (polygon : Polygon) (point : Point) : bool =

        let bottomLeft, topRight = polygonBounds polygon

        let testLine =
            (point,
            {
                X = point.X + (topRight.X - bottomLeft.X)
                Y = point.Y
            })

        let numberOfIntersections =
            polygon
            |> getSides
            |> List.map (fun x -> checkLineSegmentsIntersect x testLine && point.Y <> topRight.Y && point.Y <> bottomLeft.Y && not (checkEndpoint x testLine))
            |> List.sumBy (fun x -> if x then 1 else 0)


        let liesOnAnySide =
            not
                (polygon
                |> getSides
                |> List.map (fun (a, b) -> Collinear = getOrientation a b point)
                |> List.forall (fun x -> x = false))

        (numberOfIntersections) % 2 = 1
        || (liesOnAnySide && withinBounds point (bottomLeft, topRight))