# Mathil

## Introduction

Mathil is a Rust library for drawing and animating illustrations that are intrinsically mathematical, in a programmatic way with a relatively minimal amount of code, and still a fair amount of low level control.

## Examples

The following images and videos were all created using Mathil, with source code that can be found in the `examples` folder.

### Venn Diagram

![Venn Diagram](examples/venn-diagram/output.png)
[Source Code](examples/geometric-representations-of-trig-functions/main.rs)

### Geometric Representations of Trigonometric Functions

![Geometric Representations of Trigonometric Functions](examples/geometric-representations-of-trig-functions/thumbnail.png)
[Source Code](examples/geometric-representations-of-trig-functions/main.rs)
[High Quality Video](https://www.youtube.com/watch?v=SYb_QUx1BE0)

### Rose

![Rose](examples/rose/thumbnail.png)
[Source Code](examples/rose/main.rs)
[High Quality Video](https://www.youtube.com/watch?v=SlcujkXnz8M)

### Curve Intersection

![Curve Intersection](examples/curve-intersection/thumbnail.png)
[Source Code](examples/curve-intersection/main.rs)
[High Quality Video](https://www.youtube.com/watch?v=x_0A-ukn5gE)

## Setup

The easiest way to get up and running using Mathil is to clone this repository, open `src/mathil/src/main.rs` and begin writing your code in there. The source code for the examples above are provided in the form of the main file to use in this project.

Then, with cargo installed, run your code with the following command:

```
cargo run --release
```

While debug and release profiles are configured, due to the nature of Mathil being a library for drawing graphics to write to a file, rather than to integrate into a larger code base, it's recommended that you run in release mode under most circumstances due to the huge performance improvements that come with greater optimization. As such, overflow checks are still enabled in release mode. Check `Cargo.toml` for specific details.

## Getting Started

To help get started, here is an explanation for how a frame of the geometric representations of trigonometric functions example can be created (the code is simplified for this explanation).

Note that this explanation is simply to get a high level understanding of the library, not including all the specifics. If you have followed on with this and want to learn about all the specific functionality of Mathil, see the *Full Documentation* section below, and the source code for the examples.

If you haven't already, get your `main.rs` file setup according to the *setup* section above.

The unchanged file should look like this:

```
#![allow(dead_code, unused_variables, unused_imports)]

mod mathil;
use mathil::{colours::Colour, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};

fn main() {
    
}
```

All of the required modules, types and functions are imported, so we are just going to write the code we need in `main`.

Let's begin by setting our horizontal and vertical resolution, and specifying the constant of the angle we will use between the positive horizontal axis and the radius.

```
let horizontal_resolution : u16 = 3840;
let vertical_resolution : u16 = 2160;

let angle = PI / 4.0;
```

The way we will construct the animation will make it clear that the image will be (generally speaking) independent of this resolution, so we can change that if we would like to later.

Next, let's create a new `Screen`. A `Screen` in Mathil represents an image, or a frame of video. When creating one, we need to specify the initial colour and the bounding box.

```
Screen::new(
    horizontal_resolution, vertical_resolution,
    Point::new(-3.555, -2.0), Point::new(3.555, 2.0),
    Colour::from_hex("#2f3640")
)
```

The bounding box is defined by the two provided points. This just represents the bottom left and top right points in the coordinate system we will be using when drawing. Notice how the aspect ratio is roughly equal to that of the specified resolution, to minimize stretching of the image. Points are created with the `Point::new` function which takes in an `x` and `y` coordinate each as an `f32`.

The background colour is specified here using a hex code. Colours can be created from a hex code string `Colour::from_hex`, RGB values `Colour::from_rgb` or as one of the predefined colours from the CSS standard provided in the `css_colours` module.

The `Screen` type implements a function called `render` which takes in some mathematical object which can be rendered, and the settings that are associated with it. Note also that `render` takes ownership of and then returns the screen. This means it is typically best to call render many times in sequence on the result of the previous. In this case, we have the unit circle, the cartesian plane, the lines representing each trig function and the endpoints to render. So let's do those one by one, noting that things rendered later on may cover up things rendered earlier on.

```
Screen::new(
    horizontal_resolution, vertical_resolution,
    Point::new(-3.555, -2.0), Point::new(3.555, 2.0),
    Colour::from_hex("#2f3640")
)
.render(
    // Sine Line
    Function::new_line_segment(
        Point::new(angle.cos(), 0.0),
        Point::new(angle.cos(), angle.sin()),
        (0.0, 1.0)
    ),
    FunctionRenderSettings::new(
        Colour::from_hex("#4cd137"),
        10,
        100,
        RenderingType::RoundAntiAliased(2.0)
    )
)
```

In this case, we are rendering a `Function`. A `Function` in Mathil is just a parametric function from an `f32` to a `Point` and a domain on which to sample that function. We can create a function on our own with any rule we like, but some tools exist to make rendering certain standard functions, such as a line segment, easier. We are using such a function here called `new_line_segment` which creates a line segment from the endpoints and the domain. Notice that the domain here is specified as `(0.0, 1.0)`, since the line segment is internally just dividing the interval. The reason this is not hidden in the implementation is to make drawing such a line in an animation easier.

Since we have provided a `Function` we also need to provide `FunctionRenderSettings` which specify how to display the function. This includes the colour, line thickness, number of samples and rendering type.

The line thickness provided is one of the few things in Mathil which is based on the number of pixels not the coordinate system defined on the screen. See the full documentation for more information on this.

The number of samples just determines how many times the function is sampled when rendering it.

`RenderingType` is an enum with options of `Square`, `RoundAliased` and `RoundAntialised(f32)`. The first two options render each sample as a square and circle respectively. The third option renders each sample as a circle but also adds an anti-aliasing factor. In general, a value closer to 0 will give a softer edge and a larger value will approach the `RoundAliased` option. We're going to go with 2.0 for this line, which is a good place to start for reasonably thin lines.

We can now do the same thing with the lines representing the cosine and tangent of the angle. Consider the following code directly following our function call to render the previous line segment.

```
.render(
    // Cosine Line
    Function::new_line_segment(
        Point::new(0.0, angle.sin()),
        Point::new(angle.cos(), angle.sin()),
        (0.0, 1.0)
    ),
    FunctionRenderSettings::new(
        Colour::from_hex("#9c88ff"),
        10,
        100,
        RenderingType::RoundAntiAliased(2.0)
    )
)
.render(
    // Tangent Line
    Function::new_line_segment(
        Point::new(angle.cos(), angle.sin()),
        Point::new(1.0 / angle.cos(), 0.0),
        (0.0, 1.0)
    ),
    FunctionRenderSettings::new(
        css_colours::ORANGE_PEEL,
        10,
        100,
        RenderingType::RoundAntiAliased(2.0)
    )
)
```

Next we are going to create and render the cartesian plane. Internally, a cartesian plane in Mathil is just a convenient wrapper for four instances of the `Vector` type in Mathil. The cartesian plane is defined by the lower left bound, upper right bound and the location of the origin. We also need to specify the width and height of the arrows on the ends.

```
.render(
    CartesianPlane::new(
        Point::new(-1.6, -1.6),
        Point::new(1.6, 1.6),
        Point::new(0.0, 0.0),
        0.13,
        0.13
    ),
    CartesianPlaneRenderSettings::new(
        css_colours::WHITE,
        10,
        100
    )
)
```

Next we can render the unit circle, which can be created using the new_circle function implemented on `Function` as follows:

```
.render(
    // Unit Circle
    Function::new_ellipse(
        1.0, 1.0,
        Point::new(0.0, 0.0),
        (0.0, TAU)
    ),
    FunctionRenderSettings::new(
        Colour::from_rgb(240, 240, 240),
        10,
        800,
        RenderingType::RoundAntiAliased(2.0)
    )
)
```



At this point, even if you have not seen all of the features of Mathil, you should be starting to get the hang of how rendering objects works.

Here is the code required to render the remaining items, being the radius and the endpoist of the line we drew earlier.

```
.render(
    // Radius
    Function::new_line_segment(
        Point::new(0.0, 0.0),
        Point::new(angle.cos(), angle.sin()),
        (0.0, 1.0)
    ),
    FunctionRenderSettings::new(
        Colour::from_hex("#f5f6fa"),
        10,
        100,
        RenderingType::RoundAntiAliased(2.0)
    )
)
.render(
    // Sine Endpoint
    Point::new(angle.cos(), 0.0),
    PointRenderSettings::new(
        Colour::from_hex("#4cd137"),
        30,
        RenderingType::RoundAntiAliased(10.0),
    )
)
.render(
    // Cosine Endpoint
    Point::new(0.0, angle.sin()),
    PointRenderSettings::new(
        Colour::from_hex("#9c88ff"),
        30,
        RenderingType::RoundAntiAliased(10.0)
    )
)
.render(
    // Tangent Endpoint
    Point::new(1.0 / angle.cos(), 0.0),
    PointRenderSettings::new(
        css_colours::ORANGE_PEEL,
        30,
        RenderingType::RoundAntiAliased(10.0)
    )
)
```

In general, renderable objects depend on two type primitives, `Point` and `Polygon` (in its fill form). A `Function` is then just rendered as a series of points from where the function was sampled, a `Vector` is just a line segment (a special case of a function) along with a `Polygon`, a `CartesianPlane` is just many vectors, and a `DashedLine` is many line segments.

The `Function` type, as mentioned earlier, can be created directly from the rule, or can be created using one of the helper functions including `new_line_segment`, `new_bezier_curve`, `new_ellipse` or `new_circle`.

Now that we have our image drawn up, we need to write it to a file. Mathil supports outputs as uncompressed bitmaps, or as pngs. The `write_to_bitmap` and `write_to_png` methods implemented on the screen handle these separately, and both take in as input the folder to write the file to, and the filename (without the extension).

This leaves our final main file as follows:

```
#![allow(dead_code, unused_variables, unused_imports)]

mod mathil;
use mathil::{colours::Colour, utilities::*, constants::*, rendering::*, maths_objects::*, colours::css_colours, animation::*};


fn main() {
    let horizontal_resolution : u16 = 3840;
    let vertical_resolution : u16 = 2160;

    let angle = PI / 4.0;

    Screen::new(
        horizontal_resolution, vertical_resolution,
        Point::new(-3.555, -2.0), Point::new(3.555, 2.0),
        Colour::from_hex("#2f3640")
    )
    .render(
        // Sine Line
        Function::new_line_segment(
            Point::new(angle.cos(), 0.0),
            Point::new(angle.cos(), angle.sin()),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            Colour::from_hex("#4cd137"),
            10,
            100,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .render(
        // Cosine Line
        Function::new_line_segment(
            Point::new(0.0, angle.sin()),
            Point::new(angle.cos(), angle.sin()),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            Colour::from_hex("#9c88ff"),
            10,
            100,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .render(
        // Tangent Line
        Function::new_line_segment(
            Point::new(angle.cos(), angle.sin()),
            Point::new(1.0 / angle.cos(), 0.0),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            css_colours::ORANGE_PEEL,
            10,
            100,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .render(
        CartesianPlane::new(
            Point::new(-1.6, -1.6),
            Point::new(1.6, 1.6),
            Point::new(0.0, 0.0),
            0.13,
            0.13
        ),
        CartesianPlaneRenderSettings::new(
            css_colours::WHITE,
            10,
            100
        )
    )
    .render(
        // Unit Circle
        Function::new_ellipse(
            1.0, 1.0,
            Point::new(0.0, 0.0),
            (0.0, TAU)
        ),
        FunctionRenderSettings::new(
            Colour::from_rgb(240, 240, 240),
            10,
            800,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .render(
        // Radius
        Function::new_line_segment(
            Point::new(0.0, 0.0),
            Point::new(angle.cos(), angle.sin()),
            (0.0, 1.0)
        ),
        FunctionRenderSettings::new(
            Colour::from_hex("#f5f6fa"),
            10,
            100,
            RenderingType::RoundAntiAliased(2.0)
        )
    )
    .render(
        // Sine Endpoint
        Point::new(angle.cos(), 0.0),
        PointRenderSettings::new(
            Colour::from_hex("#4cd137"),
            30,
            RenderingType::RoundAntiAliased(10.0),
        )
    )
    .render(
        // Cosine Endpoint
        Point::new(0.0, angle.sin()),
        PointRenderSettings::new(
            Colour::from_hex("#9c88ff"),
            30,
            RenderingType::RoundAntiAliased(10.0)
        )
    )
    .render(
        // Tangent Endpoint
        Point::new(1.0 / angle.cos(), 0.0),
        PointRenderSettings::new(
            css_colours::ORANGE_PEEL,
            30,
            RenderingType::RoundAntiAliased(10.0)
        )
    )
    .write_to_png("C:\\", "trig-drawing");
}
```

Now just run and see the output!

## Dependencies

Originally this library only supported writing to 24-bit uncompressed bitmaps, which is a file type I chose due to the simplicity of the standard and therefore the ease of implementation. Due to the fact that the library is now oriented towards ease of animation, outputs of a more common filetype with smaller filesizes was required, hence the ability to now write to PNG images. This is through the `png` crate which is documented [here](https://crates.io/crates/png). This crate and it's dependencies are only dependencies of this project. Eventually I plan on writing my own PNG authoring code.

## Full Documentation

Full documentation is not yet available, but will be very soon.

## Q and A

#### Where does the name come from?

**Math**ematical **Il**lustrations. It was the best I could come up with...

#### How well supported will this tool be?

This project is now at the stage where it is capable enough for me to regularly use it in my own projects, and as such I am implementing new features as I see a need for them in my own workflow. I still have plans to implement a better colour mixing tool with which you can track my progress [here](https://github.com/aaron-jack-manning/colour-mixing-experiments), and a font rendering tool. If you have ideas please create an issue and I will occasionally look to see what I can add to make it useful to others, or if you see the framework for something useful to you here but require more features, feel free to fork the repository and turn it into something catered to your needs.

#### I have found a bug in the code, or error in documentation, what should I do?

If you find a mistake in anything I have written here, or a bug in my code, I would greatly appreciate it if you could create an issue on this repository so I can fix it, even if you do fix it before then in your local copy.

#### Why has (insert feature) been implemented the way that it has?

I am no expert at computer graphics, and this project has been my way of learning the basics of 2D computer graphics. As such, you may find some of my implementations to be non-standard and strange, because I had serious trouble finding decent resources to understand the standard implementations of what I have done. I just did things the way that seemed most logical to me.

#### What license is this issued under?

Mathil is released with the GPLv2 license. Please see `LICENSE` at the top level of this repository.

tldr: if you write and release code that uses Mathil, it must also be licensed under GPLv2, but feel free to use Mathil with your own code to create images and videos for commercial purposes and keep the code you have written private.

#### What is the project in archive?

Mathil originally started as just a static illustration library in F#. I migrated the project to Rust while I was first learning it since itt would give better performance when attempting to do animations, and traits provide a much neater implementation for rendering objects. The original F# version is provided there along with the README file as it was when it was last changed, which includes documentation for it, but it is recommended that you use the version provided here unless you plan on maintaining it yourself, since I will not be updating that version if bugs are found, and it is now many features behind the current version.