open Mathil.Colours
open Mathil.MathematicalObjects
open Mathil.Rendering
open Mathil.CompoundShapes
open Mathil.Bitmap

open System

let vennDiagramExample filepath filename =

    let resolution = (3000, 2000)
    let boundingBox = (createPoint (0.0, 0.0), createPoint (150.0, 100.0))
    let backgroundColour = CSSColour.almond
    
    let blankScreen = createScreen resolution boundingBox backgroundColour
    
    let leftCircle =
        createCircle 25.0 (createPoint (60.0, 50.0))

    let rightCircle =
        createCircle 25.0 (createPoint (90.0, 50.0))
    
    blankScreen
    |> renderManyFunctions [leftCircle; rightCircle] CSSColour.black 10 900 RenderingType.Round
    |> colourFill (createPoint (75.0, 50.0)) (Colour.fromHex "#9b59b6")
    |> colourFill (createPoint (60.0, 50.0)) CSSColour.babyBlue
    |> colourFill (createPoint (90.0, 50.0)) CSSColour.alizarinCrimson
    |> saveScreenToBitmap filepath filename

let roseExample filepath filename coefficient =

    let resolution = (3000, 3000)
    let boundingBox = (createPoint (-1.5, -1.5), createPoint (1.5, 1.5))
    let backgroundColour = CSSColour.white

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let rose =
        createFunction (Parametric.rose (float coefficient)) (0.0, 2.0 * Constants.pi)

    let circleRadius = 1.1

    let radialLines =
        [
            for i = 0 to (coefficient * 2) do
                if (coefficient % 2 = 1 && i % 2 = 0) || coefficient % 2 = 0 then
                    yield createLineSegment (createPoint (0.0, 0.0)) (createPoint (circleRadius * Math.Cos(float i * Constants.pi / (float (coefficient))), circleRadius * Math.Sin(float i * Constants.pi / (float (coefficient)))))
        ]

    let circle =
        createCircle circleRadius (createPoint (0.0, 0.0))

    blankScreen
    |> renderFunction rose CSSColour.black 5 8000 RenderingType.Round
    |> renderCartesianPlane CSSColour.black 4 2000 0.05 0.05 0.3
    |> renderManyFunctions radialLines CSSColour.black 2 1000 RenderingType.Round
    |> renderFunction circle CSSColour.black 2 5000 RenderingType.Round
    |> saveScreenToBitmap filepath filename

let fundamentalTheoremOfCalculusIllustrationExample filepath filename =

    let resolution = (4200, 3000)
    let boundingBox = (createPoint (-1.0, -2.0), createPoint (Constants.pi + 1.0, 2.0))
    let backgroundColour = Colour.fromHex "#ecf0f1"

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let sineFunction =
        createFunction Parametric.sin (0, Constants.pi)

    let negativeCosineFunction =
        createFunction (fun t -> Point.negateY (Parametric.cos t)) (0, Constants.pi)

    let negativeCosineEndpoints =
        [
            createPoint (Constants.pi, 1.0)
            createPoint (0.0, -1.0)
        ]

    let horizontalAxis = createVector (createPoint (Constants.pi + 0.25, 0.0)) (createPoint (-0.25, 0.0)) 0.1 0.1
    let verticalAxis = createVector (createPoint (0.0, 1.75)) (createPoint (0.0, -1.75)) 0.1 0.1

    let greenAngle =
        [
            createDashedLine (createPoint (0.0, -1.0)) (createPoint (Constants.pi, -1.0)) 8
            createDashedLine (createPoint (Constants.pi, -1.0)) (createPoint (Constants.pi, 1.0)) 5
        ]
        |> List.concat

    blankScreen
    |> renderFunction sineFunction (Colour.fromHex "#e74c3c") 5 2000 RenderingType.Round
    |> renderManyVectors [horizontalAxis; verticalAxis] CSSColour.black 5 1000 RenderingType.Square
    |> renderManyFunctions greenAngle (Colour.fromHex "#2ecc71") 5 300 RenderingType.Round
    |> renderManyPoints negativeCosineEndpoints (Colour.fromHex "#9b59b6") 20
    |> colourFill (createPoint (Constants.pi / 2.0, 0.5)) (Colour.fromHex "#f2a59d")
    |> renderFunction negativeCosineFunction (Colour.fromHex "#9b59b6") 5 2000 RenderingType.Round
    |> saveScreenToBitmap filepath filename

let trigGeometricRepresentationExample filepath filename angle =

    let cos = Math.Cos(angle)
    let sin = Math.Sin(angle)
    let sec = 1.0 / cos
    let cosec = 1.0 / sin
    
    let resolution = (3000, 3000)
    let boundingBox = (createPoint (-2.0, -2.0), createPoint (2.0, 2.0))
    let backgroundColor = Colour.fromHex "#2f3640"
    
    let blankScreen = createScreen resolution boundingBox backgroundColor
    
    let unitCircle =
        createCircle 1.0 (createPoint (0.0, 0.0))
    let radius =
        createLineSegment (createPoint (0.0, 0.0)) (createPoint (cos, sin))
    
    let sineLine =
        createLineSegment (createPoint (cos, 0.0)) (createPoint (cos, sin))
    let cosineLine =
        createLineSegment (createPoint (0.0, sin)) (createPoint (cos, sin))
    let tangentLine =
        createLineSegment (createPoint (cos, sin)) (createPoint (sec, 0))
    let tangentDashedLine =
        createDashedLine (createPoint (cos, sin)) (createPoint (0.0, cosec)) 5
    
    let cosineEndpoint = createPoint (0.0, sin)
    let sineEndpoint = createPoint (cos, 0.0)
    let tangentEndpoint = createPoint (sec, 0.0)
    
    let sineColour = Colour.fromHex "#4cd137"
    let cosineColour = Colour.fromHex "#9c88ff"
    let tangentColour = CSSColour.orangePeel
    let offWhite = Colour.fromHex "#f5f6fa"
    
    blankScreen
    |> renderFunction sineLine sineColour 10 100 RenderingType.Round
    |> renderFunction cosineLine cosineColour 10 100 RenderingType.Round
    |> renderFunction tangentLine tangentColour 10 100 RenderingType.Round
    |> renderDashedLine tangentDashedLine tangentColour 10 40 RenderingType.Round
    |> renderFunction unitCircle offWhite 10 800 RenderingType.Round
    |> renderFunction radius offWhite 10 100 RenderingType.Round
    |> renderCartesianPlane offWhite 10 300 0.1 0.1 0.4
    |> renderPoint sineEndpoint sineColour 30
    |> renderPoint cosineEndpoint cosineColour 30
    |> renderPoint tangentEndpoint tangentColour 30
    |> saveScreenToBitmap filepath filename

let addingComplexNumbersExample filepath filename =
    
    let resolution = (3000, 3000)
    let boundingBox = (createPoint (-6.0, -6.0), createPoint (6.0, 6.0))
    let backgroundColour = CSSColour.darkLavender

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let minorAxisLines =
        [
            for i in [-4..4] do
                yield
                    createLineSegment (createPoint (float i, -4.0)) (createPoint (float i, 4.0))

                yield
                    createLineSegment (createPoint (-4.0, float i)) (createPoint (4.0, float i))
        ]

    let linesToComplexPoints =
        [
            createLineSegment (createPoint (-3.0, 1.0)) (createPoint (0.0, 0.0))
            createLineSegment (createPoint (1.0, 2.0)) (createPoint (0.0, 0.0))
        ]

    let complexPoints =
        createPoints [-3.0, 1.0; 1.0, 2.0; -2.0, 3.0]

    let parallelogram =
        createPolygon (createPoints [0.0, 0.0; 1.0, 2.0; -2.0, 3.0; -3.0, 1.0])

    blankScreen
    |> renderSolidPolygon parallelogram (Colour.fromHex "#e7b864")
    |> renderManyFunctions minorAxisLines CSSColour.white 2 600 RenderingType.Square
    |> renderCartesianPlane CSSColour.white 10 150 0.2 0.2 1.0
    |> renderManyFunctions linesToComplexPoints CSSColour.orangeWebColor 10 400 RenderingType.Round
    |> renderManyPoints complexPoints CSSColour.orangeWebColor 40
    |> saveScreenToBitmap filepath filename

let riemannSumExample filepath filename =

    let resolution = (5000, 2000)
    let boundingBox = (createPoint (0.0, 0.0), createPoint (500.0, 200.0))
    let backgroundColour = CSSColour.almond
    
    let blankScreen = createScreen resolution boundingBox backgroundColour

    let shift = 275.0

    let axis =
        [
            createVector (createPoint (200, 25)) (createPoint (25, 25)) 5 5
            createVector (createPoint (200.0 + shift, 25)) (createPoint (25.0 + shift, 25)) 5 5
            createVector (createPoint (25, 175)) (createPoint (25, 25)) 5 5
            createVector (createPoint (25.0 + shift, 175)) (createPoint (25.0 + shift, 25)) 5 5
        ]

    let transitionArrow =
        createVector (createPoint (260, 100)) (createPoint (240, 100)) 5 5

    let leftFunction =
        createBezierCurve (createPoints [25, 60; 100, 150; 130, 40; 200, 100])
    let rightFunction =
        createBezierCurve (createPoints [25.0 + shift, 60; 100.0 + shift, 150; 130.0 + shift, 40; 200.0 + shift, 100])

    let functionBounds =
        [
            createLineSegment (createPoint (200.0, 100)) (createPoint (200.0, 25))
            createLineSegment (createPoint (200.0 + shift, 100)) (createPoint (200.0 + shift, 25))
        ]
    
    let numberOfColumns = 10.0

    let columns =
        [
            for i = 0 to int numberOfColumns - 1 do
                let columnWidth = 175.0 / numberOfColumns
                let leftSide = 25.0 + columnWidth * (float i)
                let rightSide = leftSide + columnWidth

                let functionPointLeft = leftFunction.Rule (columnWidth * (float i) / 175.0)
                let functionPointRight = leftFunction.Rule (columnWidth * (float i + 1.0) / 175.0)

                yield createPolygon (functionPointLeft :: (createPoints [functionPointLeft.X, 25.0; functionPointRight.X, 25.0; functionPointRight.X, functionPointLeft.Y]))
        ]

    let red1 = Colour.fromHex "#c0392b"
    let red2 = Colour.fromHex "#e74c3c"
    let red3 = Colour.fromHex "#f2a59d"

    blankScreen
    |> renderManySolidPolygons columns red3
    |> renderManyPolygonsSides columns red2 5 400 RenderingType.Square
    |> renderManyFunctions [leftFunction; rightFunction] red1 5 1000 RenderingType.Round
    |> renderManyFunctions functionBounds red1 5 1000 RenderingType.Square
    |> renderManyVectors (transitionArrow :: axis) CSSColour.black 5 500 RenderingType.Square
    |> colourFill (createPoint (400.0, 50.0)) red3
    |> saveScreenToBitmap filepath filename