namespace Mathil

open System

open MathematicalPrimitives

module MathematicalConstants =

    /// Parametric sine function.
    let sine x = { X = x; Y = Math.Sin(x) }

    /// Parametric cosine function.
    let cosine x = { X = x; Y = Math.Cos(x) }

    /// Parametric natural logarithm function.
    let ln x = { X = x; Y = Math.Log(x) }

    /// Parametric ellipse function.
    let ellipse rx ry x1 y1 t = { X = rx * Math.Cos(t) + x1; Y = ry * Math.Sin(t) + y1 }

    /// Parametric rose function.
    let rose a t = { X = Math.Cos(a * t) * Math.Cos(t); Y = Math.Cos(a * t) * Math.Sin(t) }

    /// Ratio of circumference to diameter of circle.
    let pi = 3.14159

    /// Euler's constant.
    let e = 2.71828