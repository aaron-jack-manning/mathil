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

## Dependencies

Originally this library only supported writing to 24-bit uncompressed bitmaps, which is a file type I chose due to the simplicity of the standard and therefore the ease of implementation. Due to the fact that the library is now oriented towards ease of animation, outputs of a more common filetype with smaller filesizes was required, hence the ability to now write to PNG images. This is through the `png` crate which is documented [here](https://crates.io/crates/png). This crate and it's dependencies are only dependencies of this project. Eventually I plan on writing my own PNG authoring code.

## Full Documentation

Full documentation is available [here](documentation/main.pdf).

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

tldr: if you write and release code that uses Mathil, it must also be licensed under GPLv2, but feel free to use Mathil with your own code to create images and videos for personal or commercial purposes and keep the code you have written private.

#### What is the project in archive?

Mathil originally started as just a static illustration library in F#. I migrated the project to Rust while I was first learning it since itt would give better performance when attempting to do animations, and traits provide a much neater implementation for rendering objects. The original F# version is provided there along with the README file as it was when it was last changed, which includes documentation for it, but it is recommended that you use the version provided here unless you plan on maintaining it yourself, since I will not be updating that version if bugs are found, and it is now many features behind the current version.