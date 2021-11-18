namespace Mathil

open Colours
open MathematicalPrimitives
open BezierCurves
open Lines
open FunctionSampling
open Rendering

module Templates =

    /// Adds a cartesian plane to the specified image.
    let cartesianPlane (samples : int * int) (borderWidth : float) (axisColour : Colour) (axisThickness : int) (arrowEdgeThickness) (arrowLength : float) (arrowWidth : float) (screen : Screen) : Screen =
    
        let horizontalSamples, verticalSamples = samples

        let width = screen.TopRightBound.X - screen.BottomLeftBound.X
        let height = screen.TopRightBound.Y - screen.BottomLeftBound.Y

        let horizontalCentre = screen.BottomLeftBound.X + width / 2.0
        let verticalCentre = screen.BottomLeftBound.Y + height / 2.0

        let horizontalPoints =
            pointsFromTupleList
                [
                    screen.BottomLeftBound.X + borderWidth, verticalCentre
                    screen.TopRightBound.X - borderWidth, verticalCentre
                ]
                |> createBezierCurve
                |> sample horizontalSamples

        let verticalPoints =
            pointsFromTupleList
                [
                    horizontalCentre, screen.BottomLeftBound.Y + borderWidth
                    horizontalCentre, screen.TopRightBound.Y - borderWidth
                ]
            |> createBezierCurve
            |> sample verticalSamples



        let axisEndpoint = horizontalCentre + (width / 2.0) - borderWidth 
        let halfArrowWidth = arrowWidth / 2.0
        let arrowEndpoint = axisEndpoint + arrowLength

        let arrows : Polygon list =
            [
                [ // Top
                   createPoint (horizontalCentre + halfArrowWidth, verticalCentre + axisEndpoint)
                   createPoint (horizontalCentre, verticalCentre + arrowEndpoint)
                   createPoint (horizontalCentre - halfArrowWidth, verticalCentre + axisEndpoint)
                ]

                [ // Bottom
                    createPoint (horizontalCentre + halfArrowWidth, verticalCentre - axisEndpoint)
                    createPoint (horizontalCentre - halfArrowWidth, verticalCentre - axisEndpoint)
                    createPoint (horizontalCentre, verticalCentre - arrowEndpoint)
                ]

                [ // Right
                    createPoint (horizontalCentre + axisEndpoint, verticalCentre + halfArrowWidth)
                    createPoint (horizontalCentre + axisEndpoint, verticalCentre - halfArrowWidth)
                    createPoint (horizontalCentre + arrowEndpoint, verticalCentre)
                ]

                [ // Left
                    createPoint (horizontalCentre - axisEndpoint, verticalCentre + halfArrowWidth)
                    createPoint (horizontalCentre - arrowEndpoint, verticalCentre)
                    createPoint (horizontalCentre - axisEndpoint, verticalCentre - halfArrowWidth)
                ]
            
            ]

        let curve =
            List.empty
            |> addPointsToCurve screen horizontalPoints axisColour axisThickness
            |> addPointsToCurve screen verticalPoints axisColour axisThickness

        screen
        |> renderCurve RenderingType.Round curve
        |> renderManySolidPolygons arrows axisColour 