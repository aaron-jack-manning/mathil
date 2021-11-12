namespace Mathil

open Colours
open MathematicalPrimitives
open BezierCurves
open FunctionSampling
open Rendering

module Templates =

    /// Adds a cartesian plane to the specified image.
    let cartesianPlane (samples : int * int) (borderWidth : float) (axisColour : Colour) (axisThickness : int) (screen : Screen) : Screen =
    
        let horizontalSamples, verticalSamples = samples

        let width = screen.TopRightBound.X - screen.BottomLeftBound.X
        let height = screen.TopRightBound.Y - screen.BottomLeftBound.Y

        let horizontalPoints =
            pointsFromTupleList
                [
                    screen.BottomLeftBound.X + borderWidth, screen.BottomLeftBound.Y + height / 2.0
                    screen.TopRightBound.X - borderWidth, screen.BottomLeftBound.Y + height / 2.0
                ]
                |> createBezierCurve
                |> sample horizontalSamples

        let verticalPoints =
            pointsFromTupleList
                [
                    screen.BottomLeftBound.X + width / 2.0, screen.BottomLeftBound.Y + borderWidth
                    screen.BottomLeftBound.X + width / 2.0, screen.TopRightBound.Y - borderWidth
                ]
            |> createBezierCurve
            |> sample verticalSamples

        let curve =
            List.empty
            |> addPointsToCurve screen horizontalPoints axisColour axisThickness
            |> addPointsToCurve screen verticalPoints axisColour axisThickness

        screen
        |> renderCurve RenderingType.Square curve