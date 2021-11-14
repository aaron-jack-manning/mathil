namespace Mathil

open MathematicalPrimitives
open BezierCurves

module Lines =
    
    /// Wrapper around createBezierCurve for a simple line.
    let createLine (start : Point, finish : Point) : Function =
        createBezierCurve [start; finish]

    /// Creates a series of short lines that create a longer dashed line.
    let createDashedLine (start : Point, finish : Point) (dashes : int) : Function list =
        
        let divisions = 2 * dashes - 1
        let divisionWidthInParameter = 1.0 / float divisions

        [
            for i = 0 to divisions - 1 do
                if i % 2 = 0 then
                    let segmentStart = lerpPoint (start, finish) (float i * divisionWidthInParameter)
                    let segmentEnd = lerpPoint (start, finish) ((float i + 1.0) * divisionWidthInParameter)

                    yield createLine (segmentStart, segmentEnd)
        ]

