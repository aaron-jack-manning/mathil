namespace Mathil

open System

open Colours
open MathematicalPrimitives

module Rendering =

    /// Represents an image.
    type Screen = { Pixels : Colour [,]; HorizontalResolution : int; VerticalResolution : int; BottomLeftBound : Point; TopRightBound : Point }

    /// Represents the coordinates of a given pixel.
    type PixelCoordinates = { H : int; V : int }

    /// Represents a dot to be rendered: the primitive of a curve.
    type Dot = { Colour : Colour; Radius : int; Location : PixelCoordinates }

    /// Represents a series of dots.
    type Curve = Dot list

    /// Represents the way a dot is rendered.
    type RenderingType =
        | Square = 0
        | Round = 1


    /// Calculates the aspect ratio in the resolution and in the bounds, to check how significantly the mathematical coordinates system is squeezed before rendering the image.
    let aspectRatios (screen : Screen) : float * float =
        let resolutionRatio = float screen.HorizontalResolution / float screen.VerticalResolution
        let boundsRatio = (screen.TopRightBound.X - screen.BottomLeftBound.X) / (screen.TopRightBound.Y - screen.BottomLeftBound.Y)
        (resolutionRatio, boundsRatio)

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

    let private convertCoordinates (point : Point) (screen : Screen) : PixelCoordinates =

        let lerpScalar (startPoint : float, endPoint : float) : (float -> float) =
            (fun t -> (1.0 - t) * startPoint + t * endPoint)

        let horizontalParameter = (point.X - screen.BottomLeftBound.X) / (screen.TopRightBound.X - screen.BottomLeftBound.X)
        let verticalParameter = (point.Y - screen.BottomLeftBound.Y) / (screen.TopRightBound.Y - screen.BottomLeftBound.Y)

        let point : Point = { X = lerpScalar (0.0, float screen.HorizontalResolution) horizontalParameter; Y = lerpScalar (0.0, float screen.VerticalResolution) verticalParameter }

        { H = int point.X; V = int point.Y }

    let pointToDot (screen : Screen) (colour : Colour) (radius : int) (point : Point) : Dot =
        {
            Colour = colour
            Radius = radius
            Location = convertCoordinates point screen
        }

    /// Adds a list of points to a Curve (list of Dots).
    let addPointsToCurve (screen : Screen) (points: Point list) (colour : Colour) (radius : int) (curve : Curve) : Curve =

        let addDotsToCurve (curve : Curve) (dots : Dot list) : Curve =
            curve @ dots

        points
        |> List.map (pointToDot screen colour radius)
        |> addDotsToCurve curve

    /// Renders a Curve onto a Screen.
    let renderCurve (renderingType : RenderingType) (curve : Curve) (screen : Screen) : Screen =
    
        let distanceFormula (x1, y1) (x2, y2) =
            Math.Sqrt(float (pown (x2 - x1) 2 + pown (y2 - y1) 2))
    
        for dot in curve do
            for i = dot.Location.H - dot.Radius to dot.Location.H + dot.Radius do
                for j = dot.Location.V - dot.Radius to dot.Location.V + dot.Radius do

                    let distance = distanceFormula (dot.Location.H, dot.Location.V) (i, j)

                    let withinScreen =
                        i >= 0
                        && j >= 0
                        && i < screen.HorizontalResolution
                        && j < screen.VerticalResolution

                    if renderingType = RenderingType.Square then
                        if withinScreen then
                            screen.Pixels.[i, j] <- dot.Colour
                    elif renderingType = RenderingType.Round then
                        if withinScreen && distance <= float dot.Radius then
                            screen.Pixels.[i, j] <- dot.Colour

        screen

    /// Calculates the line thickness as a proportion of the average of the horizontal and vertical resolutions. Allows line thickness to be scaled appropriates upon changing an image's resolution.
    let calculateLineThickness (resolution : int * int) (proportion : float) =
        let horizontal, vertical = resolution
    
        int (proportion * float ((horizontal + vertical) / 2))

    /// Fills a region of a screen which has a solid colour with another solid colour.
    let colourFill (startingPoint : Point) (desiredColour  : Colour) (screen : Screen) =
    
        let startingLocation = convertCoordinates startingPoint screen
        let initialColour = screen.Pixels.[startingLocation.H, startingLocation.V]

        let mutable currentChecks = List.singleton startingLocation
        while not (currentChecks |> List.isEmpty) do

            let current = currentChecks.[0]

            let withinScreen =
                current.H >= 0
                && current.V >= 0
                && current.H < screen.HorizontalResolution
                && current.V < screen.VerticalResolution

            if withinScreen then
                if screen.Pixels.[current.H, current.V] = initialColour then
                    screen.Pixels.[current.H, current.V] <- desiredColour

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