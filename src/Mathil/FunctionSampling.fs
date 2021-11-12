namespace Mathil

open MathematicalPrimitives

module FunctionSampling =

    /// Samples a function the specified number of times, to return a list of Points.
    let sample (samples : int) (func : Function) : Point list =

        let lerpScalar (startPoint : float, endPoint : float) : (float -> float) =
            (fun t -> (1.0 - t) * startPoint + t * endPoint)

        let start, finish = func.Domain
    
        [0..(samples - 1)]
        |> List.map (fun x -> float x / float (samples - 1))
        |> List.map (lerpScalar (start, finish))
        |> List.map func.Rule