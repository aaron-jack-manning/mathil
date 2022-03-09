namespace Mathil

open System

open MathematicalObjects
open Colours
open MathematicalObjects.Types

module Rendering =

    module private Function =

        let sample (samples : int) (func : Function) : Point list =

            let lerpScalar (startPoint : float, endPoint : float) : (float -> float) =
                (fun t -> (1.0 - t) * startPoint + t * endPoint)

            let start, finish = func.Domain

            [0..(samples - 1)]
            |> List.map (fun x -> float x / float (samples - 1))
            |> List.map (lerpScalar (start, finish))
            |> List.map func.Rule


    module private Polygon =

        let getBounds (polygon : Polygon) : Point * Point =
    
            let minimumX =
                (polygon.Vertices
                |> List.minBy (fun p -> p.X)).X

            let minimumY =
                (polygon.Vertices
                |> List.minBy (fun p -> p.Y)).Y

            let maximumX =
                (polygon.Vertices
                |> List.maxBy (fun p -> p.X)).X

            let maximumY =
                (polygon.Vertices
                |> List.maxBy (fun p -> p.Y)).Y

            (
                { X = minimumX; Y = minimumY },
                { X = maximumX; Y = maximumY }
            )

        type private Orientation =
            | Clockwise
            | CounterClockwise
            | Collinear

        let private getOrientation (a : Point) (b : Point) (c : Point) : Orientation =
    
            if (c.Y - a.Y) * (b.X - a.X) < (b.Y - a.Y) * (c.X - a.X) then
                Clockwise
            elif (c.Y - a.Y) * (b.X - a.X) = (b.Y - a.Y) * (c.X - a.X) then
                Collinear
            else
                CounterClockwise

        let private withinBounds (point : Point) (bounds : Point * Point) =
    
            let bottomLeft, topRight = bounds

            bottomLeft.X <= point.X
            && point.X <= topRight.X
            && bottomLeft.Y <= point.Y
            && point.Y <= topRight.Y

        let private checkLineSegmentsIntersect (segment1 : Point * Point) (segment2 : Point * Point) : bool =
    
            let p1, q1 = segment1
            let p2, q2 = segment2

            let o1 = getOrientation p1 q1 p2
            let o2 = getOrientation p1 q1 q2
            let o3 = getOrientation p2 q2 p1
            let o4 = getOrientation p2 q2 q1

            o1 <> o2 && o3 <> o4
            || (o1 = Collinear && withinBounds p2 (p1, q1))
            || (o2 = Collinear && withinBounds q2 (p1, q1))
            || (o3 = Collinear && withinBounds p1 (p2, q2))
            || (o4 = Collinear && withinBounds q1 (p2, q2))

        let getSides (polygon : Polygon) : (Point * Point) list =
            let ends = (List.last polygon.Vertices, List.head polygon.Vertices)
    
            let middle =
                polygon.Vertices
                |> List.pairwise

            ends :: middle

        let private checkEndpoint (polygonSide : Point * Point) (line : Point * Point) : bool =
        
            let lineStart, lineEnd = line
            let endpoint1, endpoint2 = polygonSide

            getOrientation endpoint1 lineStart lineEnd = Collinear

        let isInsidePolygon (bottomLeft : Point) (topRight : Point) (sides : (Point * Point) list) (point : Point) : bool =

            let buffer = 0.1

            let testLine =
                (point,
                {
                    X = point.X + (topRight.X - bottomLeft.X + buffer)
                    Y = point.Y
                })

            let numberOfIntersections =
                sides
                |> List.map (fun x -> checkLineSegmentsIntersect x testLine && point.Y <> topRight.Y && point.Y <> bottomLeft.Y && not (checkEndpoint x testLine))
                |> List.sumBy (fun x -> if x then 1 else 0)

            let liesOnAnySide =
                not
                    (sides
                    |> List.map (fun (a, b) -> Collinear = getOrientation a b point)
                    |> List.forall (fun x -> x = false))

            (numberOfIntersections) % 2 = 1
            || (liesOnAnySide && withinBounds point (bottomLeft, topRight))


    // Types -----------------------------------------------------------

    /// Represents an image.
    type Screen =
        {
            Pixels : Colour [,]
            HorizontalResolution : int
            VerticalResolution : int
            BottomLeftBound : Point
            TopRightBound : Point
        }

    /// Represents the coordinates of a given pixel.
    [<StructuredFormatDisplay("({H}, {V})")>]
    type PixelCoordinates =
        {
            H : int
            V : int
        }

    /// Represents the way a dot is rendered.
    type RenderingType =
        | Square = 0
        | Round = 1


    // Screen Functions -----------------------------------------------------------

    /// Calculates the aspect ratio in the resolution and in the bounds, to check how significantly the mathematical coordinates system is squeezed before rendering the image.
    let aspectRatios (screen : Screen) : float * float =
        let resolutionRatio = float screen.HorizontalResolution / float screen.VerticalResolution
        let boundsRatio = (screen.TopRightBound.X - screen.BottomLeftBound.X) / (screen.TopRightBound.Y - screen.BottomLeftBound.Y)
        (resolutionRatio, boundsRatio)

    /// Calculates the line thickness as a proportion of the average of the horizontal and vertical resolutions. Allows line thickness to be scaled appropriates upon changing an image's resolution.
    let calculateLineThickness (resolution : int * int) (proportion : float) : int =
        let horizontal, vertical = resolution

        int (proportion * float ((horizontal + vertical) / 2))

    /// Creates a blank screen based on the resolution, bounds and colour.
    let createScreen (horizontalResolution : int, verticalResolution : int) (bounds : Point * Point) (defaultColour : Colour) : Screen =
        let pixels =
            ([|
                for i = 0  to horizontalResolution - 1 do
                    [|
                        for j = 0 to verticalResolution - 1 do
                            defaultColour
                    |]
            |] |> array2D)
        let bottomLeft, topRight = bounds
        {
            Pixels = pixels;
            HorizontalResolution = horizontalResolution;
            VerticalResolution = verticalResolution;
            BottomLeftBound = bottomLeft;
            TopRightBound = topRight
        }


    // Rendering Utilities -----------------------------------------------------------

    let private lerpScalar (startPoint : float, endPoint : float) : (float -> float) =
        (fun t -> (1.0 - t) * startPoint + t * endPoint)

    let private pointToPixelCoordinates (screen : Screen) (point : Point) : PixelCoordinates =

        let horizontalParameter = (point.X - screen.BottomLeftBound.X) / (screen.TopRightBound.X - screen.BottomLeftBound.X)
        let verticalParameter = (point.Y - screen.BottomLeftBound.Y) / (screen.TopRightBound.Y - screen.BottomLeftBound.Y)

        let point : Point =
            {
                X = lerpScalar (0.0, float screen.HorizontalResolution) horizontalParameter
                Y = lerpScalar (0.0, float screen.VerticalResolution) verticalParameter
            }

        { H = int point.X; V = int point.Y }

    let private pixelCoordinatesToPoint (screen : Screen) (coordinates : PixelCoordinates) : Point =
    
        let horizontalParameter = float coordinates.H / float screen.HorizontalResolution
        let verticalParameter = float coordinates.V / float screen.VerticalResolution

        {
            X = lerpScalar (screen.BottomLeftBound.X, screen.TopRightBound.X) horizontalParameter
            Y =  lerpScalar (screen.BottomLeftBound.Y, screen.TopRightBound.Y) verticalParameter    
        }

    let private withinScreen (coordinates : PixelCoordinates) (screen : Screen) =
        coordinates.H >= 0
        && coordinates.V >= 0
        && coordinates.H < screen.HorizontalResolution
        && coordinates.V < screen.VerticalResolution


    // Renderers -----------------------------------------------------------

    let private renderDotFromCoordinates (coordinate : PixelCoordinates) (colour : Colour) (radius : int) (renderingType : RenderingType) (screen : Screen) : Screen =
    
        let distanceFormula (x1, y1) (x2, y2) =
            Math.Sqrt(float (pown (x2 - x1) 2 + pown (y2 - y1) 2))

        for i = coordinate.H - radius to coordinate.H + radius do
            for j = coordinate.V - radius to coordinate.V + radius do

                let distance = distanceFormula (coordinate.H, coordinate.V) (i, j)

                let onScreen = withinScreen { H = i; V = j } screen

                if renderingType = RenderingType.Square then
                    if onScreen then
                        screen.Pixels[i, j] <- colour
                elif renderingType = RenderingType.Round then
                    if onScreen && distance <= float radius then
                        screen.Pixels[i, j] <- colour

        screen

    /// Renders a point.
    let renderPoint (point : Point) (colour : Colour) (radius : int) (screen : Screen) : Screen =
        let pointLocation = pointToPixelCoordinates screen point

        renderDotFromCoordinates pointLocation colour radius RenderingType.Round screen

    /// Renders many points if the same rendering settings are desired for all of them.
    let renderManyPoints (points : Point list) (colour : Colour) (radius : int) (screen : Screen) : Screen =
        let mutable newScreen = screen

        for point in points do
            newScreen <- renderPoint point colour radius newScreen
    
        newScreen

    /// Renders any type implicitly convertable to Function, including line segments, circles and Bezier curves.
    let renderFunction (func : Function) (colour : Colour) (thickness : int) (samples : int) (renderingType : RenderingType) (screen : Screen) : Screen =
    
        let centrePixels =
            func
            |> Function.sample samples
            |> List.map (pointToPixelCoordinates screen)
    
        let mutable newScreen = screen

        for coordinate in centrePixels do
            newScreen <- renderDotFromCoordinates coordinate colour thickness renderingType newScreen

        newScreen

    /// Renders many functions if the same rendering settings are desired for all of them.
    let renderManyFunctions (funcs : Function list) (colour : Colour) (thickness : int) (samples : int) (renderingType : RenderingType) (screen : Screen) : Screen =
        let mutable newScreen = screen

        for func in funcs do
            newScreen <- renderFunction func colour thickness samples renderingType newScreen

        newScreen

    /// Fills a region of a screen which has a solid colour with another solid colour.
    let colourFill (startingPoint : Point) (desiredColour  : Colour) (screen : Screen) : Screen =

        let startingLocation = pointToPixelCoordinates screen startingPoint
        let initialColour = screen.Pixels[startingLocation.H, startingLocation.V]

        if initialColour = desiredColour then
            failwith "the specified colour cannot match the colour at the specified location."

        let mutable currentChecks = List.singleton startingLocation
        while not (currentChecks |> List.isEmpty) do

            let current = List.head currentChecks

            let onScreen = withinScreen current screen

            if onScreen then
                if screen.Pixels[current.H, current.V] = initialColour then
                    screen.Pixels[current.H, current.V] <- desiredColour

                    currentChecks <- List.removeAt 0 currentChecks

                    currentChecks <- { H = current.H; V = current.V + 1} :: currentChecks
                    currentChecks <- { H = current.H + 1; V = current.V} :: currentChecks
                    currentChecks <- { H = current.H; V = current.V - 1} :: currentChecks
                    currentChecks <- { H = current.H - 1; V = current.V} :: currentChecks
                else
                    currentChecks <- List.removeAt 0 currentChecks
            else
                currentChecks <- List.removeAt 0 currentChecks

        screen

    /// Completes a colour fill on many distinct starting points if the same colour is desired for all of them.
    let colourFillMany (startingPoints : Point list) (desiredColour : Colour) (screen : Screen) : Screen =
        let mutable newScreen = screen

        for startingPoint in startingPoints do
            newScreen <- colourFill startingPoint desiredColour newScreen

        newScreen

    /// Renders a solid polygon of the specified colour, independent of the background. Use this instead of colourFill when other elements may already rendered where the polygon should go.
    let renderSolidPolygon (polygon : Polygon) (desiredColour : Colour) (screen : Screen) : Screen =
    
        // Calculating polygon metadata outside of isInsidePolygon for better performance.
        let sides = Polygon.getSides polygon
        let bottomLeftPoint, topRightPoint = Polygon.getBounds polygon
        
        let bottomLeftPixelLocation = pointToPixelCoordinates screen bottomLeftPoint
        let topRightPixelLocation = pointToPixelCoordinates screen topRightPoint

        for i = bottomLeftPixelLocation.H to topRightPixelLocation.H do
            for j = bottomLeftPixelLocation.V to topRightPixelLocation.V do
                let currentCoordinates = { H = i; V = j }
                let currentPoint = pixelCoordinatesToPoint screen currentCoordinates

                let onScreen = withinScreen currentCoordinates screen

                if currentPoint |> Polygon.isInsidePolygon bottomLeftPoint topRightPoint sides && onScreen then
                    screen.Pixels[i, j] <- desiredColour

        screen

    /// Renders many solid poligons if the same rendering settings are desired for all of them.
    let renderManySolidPolygons (polygons : Polygon list) (desiredColour : Colour) (screen : Screen) : Screen =
        let mutable newScreen = screen
    
        for polygon in polygons do
            newScreen <- renderSolidPolygon polygon desiredColour newScreen
        
        newScreen

    /// Renders a vector.
    let renderVector (vector : Vector) (colour : Colour) (thickness : int) (samples : int) (renderingType : RenderingType) (screen : Screen) : Screen =   

        screen
        |> renderSolidPolygon vector.ArrowHead colour
        |> renderFunction vector.Line colour thickness samples renderingType 


    /// Renders many vectors if the same rendering settings are desired for all of them.
    let renderManyVectors (vectors : Vector list) (colour : Colour) (thickness : int) (samples : int) (renderingType : RenderingType) (screen : Screen) : Screen =
        let mutable newScreen = screen

        for vector in vectors do
            newScreen <- renderVector vector colour thickness  samples renderingType newScreen

        newScreen

    /// Renders the sides of a polygon.
    let renderPolygonSides (polygon : Polygon) (colour : Colour) (thickness : int) (samplesPerSide : int) (renderingType : RenderingType) (screen : Screen) : Screen =
        renderManyFunctions polygon.Edges colour thickness samplesPerSide renderingType screen

    /// Renders the sides of many polygons if the same rendering settings are desired for all of them.
    let renderManyPolygonsSides (polygons : Polygon list) (colour : Colour) (thickness : int) (samplesPerSide : int) (renderingType : RenderingType) (screen : Screen) : Screen =
        let mutable newScreen = screen
        
        for polygon in polygons do
            newScreen <- renderPolygonSides polygon colour thickness samplesPerSide renderingType newScreen

        newScreen

    /// Renders a dashed line.
    let renderDashedLine (dashedLine : DashedLine) (colour : Colour) (thickness : int) (samplesPerDash : int) (renderingType : RenderingType) (screen : Screen) : Screen =
        renderManyFunctions dashedLine colour thickness samplesPerDash renderingType screen