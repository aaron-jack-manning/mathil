namespace Mathil

open MathematicalPrimitives

module BezierCurves =

    let private lerpPoint (startPoint : Point, endPoint : Point) : (float -> Point) =
        (fun t -> (1.0 - t) * startPoint + t * endPoint)

    let private interpolatePairs (list : Point list) =
        [
            for i = 0 to List.length list - 2 do
                lerpPoint (list.[i], list.[i + 1])
        ]

    /// Constructs a Bezier curve from a list of points.
    let createBezierCurve (points : Point list) : Function =

        let rec constructCurve (parameter : float) (points : Point list) : Point =
    
            if List.length points = 1 then
                points.[0]
            else
                interpolatePairs points
                |> List.map (fun x -> x parameter)
                |> constructCurve parameter

        { Rule = (fun t -> constructCurve t points); Domain = (0.0, 1.0) }





