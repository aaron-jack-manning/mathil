open Mathil.Colours
open Mathil.MathematicalObjects
open Mathil.Rendering
open Mathil.CompoundShapes
open Mathil.Bitmap

open System

let vennDiagramExample filepath filename =

    let resolution = (3000, 2000)
    let boundingBox = (createPoint (0.0, 0.0), createPoint (150.0, 100.0))
    let backgroundColor = CSSColour.almond
    
    let blankScreen = createScreen resolution boundingBox backgroundColor
    
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
        createFunction (p_rose (float coefficient)) (0.0, 2.0 * pi)

    let circleRadius = 1.1

    let radialLines =
        [
            for i = 0 to (coefficient * 2) do
                if (coefficient % 2 = 1 && i % 2 = 0) || coefficient % 2 = 0 then
                    yield createLineSegment (createPoint (0.0, 0.0)) (createPoint (circleRadius * Math.Cos(float i * pi / (float (coefficient))), circleRadius * Math.Sin(float i * pi / (float (coefficient)))))
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
    let boundingBox = (createPoint (-1.0, -2.0), createPoint (pi + 1.0, 2.0))
    let backgroundColour = Colour.fromHex "#ecf0f1"

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let sineFunction =
        createFunction p_sin (0, pi)

    let negativeCosineFunction =
        createFunction (fun t -> negateYPoint (p_cos t)) (0, pi)

    let negativeCosineEndpoints =
        [
            createPoint (pi, 1.0)
            createPoint (0.0, -1.0)
        ]

    let horizontalAxis = createVector (createPoint (pi + 0.25, 0.0)) (createPoint (-0.25, 0.0)) 0.1 0.1
    let verticalAxis = createVector (createPoint (0.0, 1.75)) (createPoint (0.0, -1.75)) 0.1 0.1

    let greenAngle =
        [
            createDashedLine (createPoint (0.0, -1.0)) (createPoint (pi, -1.0)) 8
            createDashedLine (createPoint (pi, -1.0)) (createPoint (pi, 1.0)) 5
        ]
        |> List.concat

    blankScreen
    |> renderFunction sineFunction (Colour.fromHex "#e74c3c") 5 2000 RenderingType.Round
    |> renderManyVectors [horizontalAxis; verticalAxis] CSSColour.black 5 1000 RenderingType.Square
    |> renderManyFunctions greenAngle (Colour.fromHex "#2ecc71") 5 300 RenderingType.Round
    |> renderManyPoints negativeCosineEndpoints (Colour.fromHex "#9b59b6") 20
    |> colourFill (createPoint (pi / 2.0, 0.5)) (Colour.fromHex "#f2a59d")
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
        createCircle 1.0 (createPoint (0.0, 0.0)) // 800 samples
    let radius =
        createLineSegment (createPoint (0.0, 0.0)) (createPoint (cos, sin)) // 100 samples
    
    let sineLine =
        createLineSegment (createPoint (cos, 0.0)) (createPoint (cos, sin)) // 100 samples
    let cosineLine =
        createLineSegment (createPoint (0.0, sin)) (createPoint (cos, sin)) // 100 samples
    let tangentLine =
        createLineSegment (createPoint (cos, sin)) (createPoint (sec, 0)) // 100 samples
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