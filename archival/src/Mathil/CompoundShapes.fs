namespace Mathil

open Colours
open MathematicalObjects
open Rendering

module CompoundShapes =

    /// Adds a cartesian plane to the specified image.
    let renderCartesianPlane (colour : Colour) (thickness : int) (samples : int) (arrowHeight : float) (arrowWidth : float) (borderWidth : float) (screen : Screen) : Screen =

        let width = screen.TopRightBound.X - screen.BottomLeftBound.X
        let height = screen.TopRightBound.Y - screen.BottomLeftBound.Y

        let horizontalCentre = screen.BottomLeftBound.X + width / 2.0
        let verticalCentre = screen.BottomLeftBound.Y + height / 2.0

        let positiveHorizontal =
            createVector (createPoint (screen.TopRightBound.X - borderWidth, verticalCentre)) (createPoint (horizontalCentre, verticalCentre)) arrowWidth arrowHeight

        let negativeHorizontal =
            createVector (createPoint (screen.BottomLeftBound.X + borderWidth, verticalCentre)) (createPoint (horizontalCentre, verticalCentre)) arrowWidth arrowHeight

        let positiveVertical =
            createVector (createPoint (horizontalCentre, screen.TopRightBound.Y - borderWidth)) (createPoint (horizontalCentre, verticalCentre)) arrowWidth arrowHeight

        let negativeVertical =
            createVector (createPoint (horizontalCentre, screen.BottomLeftBound.Y + borderWidth)) (createPoint (horizontalCentre, verticalCentre)) arrowWidth arrowHeight

        screen
        |> renderManyVectors [positiveVertical; negativeVertical; positiveHorizontal; negativeHorizontal] colour thickness samples RenderingType.Square