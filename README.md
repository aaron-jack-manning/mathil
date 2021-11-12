# Mathil

Mathil is a library I have created for drawing neat and consistent images that represent mathematical concepts, with relatively low level control and not too much code.

## Samples:

All examples can also be found in the `Examples` folder above.

#### Venn Diagram

```
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

writeScreenToFile "<path to folder here>" "VennDiagram" blankScreen
```

![VennDiagram](VennDiagram.bmp)

### Rose

```
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

writeScreenToFile "<path to folder here>" "Rose" blankScreen
```

![Rose](Rose.bmp)

### Fundamental Theorem of Calculus Illustration

```
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

writeScreenToFile "<path to folder here>" "FTOC" finalScreen
```

![FTOC](FTOC.bmp)

## Getting Started

To get started, here is a more thorough walkthrough of how the first example above was created.

***full walkthrough to go here***

This is one example of something that can be done using Mathil, for full documentation on all the features of Mathil, see below.

## Documentation

Documentation is available as a PDF download (here)[].

## Q and A

#### Where does the name come from?
**Math**ematical **Il**lustrations. It was the best I could come up with...

#### Why would I use this instead of a vector drawing tool or a standard graphing calculator?
I created this to be used in place of a vector drawing tool because it more easily provides consistent results when many similar illustrations must be created, and because it allows fundamentally mathematical concepts to be illustrated from the mathematical objects themselves (parametric curves, colour mixing, etc..). In terms of graphing calculators, they are generally designed as a tool to use while solving a problem, or as an educational tool. This tool is more catered towards people looking for static illustrations to include in texts.

#### How well supported will this tool be?
As I need new features for my own purposes I will be adding them in, but I do not use this tool for my job or studying so that will be my free time only, and I make no promises about how long I will maintain it. That said, if you have ideas please create an issue and I will occasionally look to see what I can add to make it useful to others, or if you see the framework for something useful to you here but require more features, feel free to fork the repository and work on it yourself.

#### What file formats does this library produce?
All exports are 24bit, uncompressed bitmaps. This is because they are easy to read and write from keeping development and computation time down and well supported enough that they are no trouble to open, view and convert to another format easily. I have no plans to add the capability to export as other image formats.

#### Why has (insert feature) been implemented the way that it has?
I am no expert at computer graphics, and this project has been my way of learning the basics of 2D computer graphics. As such, you may find some of my implementations to be non-standard and strange, because I had serious trouble finding decent resources to understand the standard implementations of what I have done. I just did things the way that seemed most logical to me.
