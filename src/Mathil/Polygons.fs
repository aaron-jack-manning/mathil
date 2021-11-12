namespace Mathil

open MathematicalPrimitives
open BezierCurves
open FunctionSampling

module Polygons =

    /// Converts a polygon to a series of points along its sides.
    let polygonToManyPoints (polygon : Polygon) (samplesPerSide : int) : Point list =

        let edges =
            [
                for i = 0 to List.length polygon - 1 do
                    if i = List.length polygon - 1 then
                        createBezierCurve [polygon.[i]; polygon.[0]]
                    else
                        createBezierCurve [polygon.[i]; polygon.[i + 1]]
            ]

        edges
        |> List.map (fun edge -> sample samplesPerSide edge)
        |> List.concat