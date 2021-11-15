open Mathil.Colours
open Mathil.MathematicalPrimitives
open Mathil.BezierCurves
open Mathil.Polygons
open Mathil.MathematicalConstants
open Mathil.FunctionSampling
open Mathil.Rendering
open Mathil.FileIO
open Mathil.Templates

open System

let vennDiagramExample filepath filename =

    let resolution = (3000, 2000)
    let boundingBox = (createPoint (0.0, 0.0), createPoint (150.0, 100.0))
    let backgroundColor = CSSColour.almond

    let blankScreen = createScreen resolution boundingBox backgroundColor

    let leftCircle =
        createFunction (ellipse 25.0 25.0 60.0 50.0) (0.0, 2.0 * pi)
        |> sample 900

    let rightCircle =
        createFunction (ellipse 25.0 25.0 90.0 50.0) (0.0, 2.0 * pi)
        |> sample 900

    let curve =
        List.empty
        |> addPointsToCurve blankScreen leftCircle CSSColour.black 10
        |> addPointsToCurve blankScreen rightCircle CSSColour.black 10

    let finalScreen =
        blankScreen
        |> renderCurve RenderingType.Round curve
        |> colourFill (createPoint (75.0, 50.0)) (Colour.fromHex "#9b59b6")
        |> colourFill (createPoint (60.0, 50.0)) CSSColour.babyBlue
        |> colourFill (createPoint (90.0, 50.0)) CSSColour.alizarinCrimson

    writeScreenToFile filepath filename blankScreen


let roseExample filepath filename =

    let resolution = (3000, 3000)
    let boundingBox = (createPoint (-1.5, -1.5), createPoint (1.5, 1.5))
    let backgroundColour = CSSColour.white

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let rosePoints =
        createFunction (rose 6.0) (0.0, 2.0 * pi)
        |> sample 8000

    let curve =
        List.empty
        |> addPointsToCurve blankScreen rosePoints CSSColour.black 5

    let finalScreen =
        blankScreen
        |> renderCurve RenderingType.Round curve

    writeScreenToFile filepath filename finalScreen

let fundamentalTheoremOfCalculusIllustrationExample filepath filename =

    let resolution = (4200, 3000)
    let boundingBox = (createPoint (-1.0, -2.0), createPoint (pi + 1.0, 2.0))
    let backgroundColour = Colour.fromHex "#ecf0f1"

    let blankScreen = createScreen resolution boundingBox backgroundColour

    let sinePoints =
        createFunction sine (0, pi)
        |> sample 2000

    let negativeCosinePoints =
        createFunction (fun t -> Point.negy (cosine t)) (0, pi)
        |> sample 2000

    let negativeCosineEndpoints =
        List.append
            [pointToDot blankScreen (Colour.fromHex "#9b59b6") 20 (createPoint (pi, 1.0))]
            [pointToDot blankScreen (Colour.fromHex "#9b59b6") 20 (createPoint (0.0, -1.0))]

    let sineCurve =
        List.empty
        |> addPointsToCurve blankScreen sinePoints (Colour.fromHex "#e74c3c") 5

    let negativeCosineCurve =
        List.empty
        |> addPointsToCurve blankScreen negativeCosinePoints (Colour.fromHex "#9b59b6") 5

    let horizontalAxisPoints =
        createBezierCurve (pointsFromTupleList [-0.25, 0.0; pi + 0.25, 0.0])
        |> sample 1000

    let verticalAxisPoints =
        createBezierCurve (pointsFromTupleList [0.0, -1.75; 0.0, 1.75])
        |> sample 1000

    let horizontalAxis =
        List.empty
        |> addPointsToCurve blankScreen horizontalAxisPoints CSSColour.black 5

    let verticalAxis =
        List.empty
        |> addPointsToCurve blankScreen verticalAxisPoints CSSColour.black 5


    let negativeCosineHorizontalComponentPoints =
        createBezierCurve (pointsFromTupleList [0.0, -1.0; pi, -1.0])
        |> sample 300

    let negativeCosineVerticalComponentPoints =
        createBezierCurve (pointsFromTupleList [pi, -1.0; pi, 1.0])
        |> sample 300

    let greenAngle =
        List.empty
        |> addPointsToCurve blankScreen negativeCosineHorizontalComponentPoints (Colour.fromHex "#2ecc71") 5
        |> addPointsToCurve blankScreen negativeCosineVerticalComponentPoints (Colour.fromHex "#2ecc71") 5

    let finalScreen =
        blankScreen
        |> renderCurve RenderingType.Round sineCurve
        |> renderCurve RenderingType.Square horizontalAxis
        |> colourFill (createPoint (pi / 2.0, 0.5)) (Colour.fromHex "#f2a59d")
        |> renderCurve RenderingType.Square greenAngle
        |> renderCurve RenderingType.Round negativeCosineCurve
        |> renderCurve RenderingType.Square verticalAxis
        |> renderCurve RenderingType.Round negativeCosineEndpoints

    writeScreenToFile filepath filename finalScreen

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
        createFunction (ellipse 1.0 1.0 0.0 0.0) (0.0, 2.0 * pi)
        |> sample 800

    let radius =
        createLine (createPoint (0.0, 0.0), createPoint (cos, sin))
        |> sample 100

    let sineValue =
        createLine (createPoint (cos, 0.0), createPoint (cos, sin))
        |> sample 100

    let cosineValue =
        createLine (createPoint (0.0, sin), createPoint (cos, sin))
        |> sample 100

    let tangentValue =
        createLine (createPoint (cos, sin), createPoint (sec, 0.0))
        |> sample 100

    let tangentDashedLine =
        createDashedLine (createPoint (cos, sin), createPoint (0.0, cosec)) 5
        |> List.map (fun x -> sample 100 x)
        |> List.concat

    let trigLines =
        List.empty
        |> addPointsToCurve blankScreen sineValue (Colour.fromHex "#4cd137") 10
        |> addPointsToCurve blankScreen cosineValue (Colour.fromHex "#9c88ff") 10
        |> addPointsToCurve blankScreen tangentValue CSSColour.orangePeel 10
        |> addPointsToCurve blankScreen tangentDashedLine CSSColour.orangePeel 10

    let circleAndRadius =
        List.empty
        |> addPointsToCurve blankScreen unitCircle (Colour.fromHex "#f5f6fa") 10
        |> addPointsToCurve blankScreen radius (Colour.fromHex "#f5f6fa") 10


    let endpoints =
        List.empty
        |> addPointsToCurve blankScreen [createPoint (cos, 0.0)] (Colour.fromHex "#4cd137") 30 // Sine
        |> addPointsToCurve blankScreen [createPoint (0.0, sin)] (Colour.fromHex "#9c88ff") 30 // Cosine
        |> addPointsToCurve blankScreen [createPoint (sec, 0.0)] CSSColour.orangePeel 30 // Tangent

    let finalScreen =
        blankScreen
        |> renderCurve RenderingType.Round trigLines
        |> cartesianPlane (300, 300) 0.4 (Colour.fromHex "#8e919e") 10 4 0.1 0.1
        |> renderCurve RenderingType.Round circleAndRadius
        |> renderCurve RenderingType.Round endpoints

    writeScreenToFile filepath filename finalScreen
