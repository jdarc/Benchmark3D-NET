# .NET Benchmark
.NET version of the benchmark to compare and contrast the relatively performance 
of various different languages/runtimes. The test model consists of *149,837* vertices and *299,707* triangles.

## Requirements
- .NET Core 5.0
- Avalonia UI framework

## Running the benchmark
```
dotnet run -c Release
```

## Rules
I have followed these simple rules when developing these benchmarks:

- Leverage platform & language features
- Keep the code clean and easy to understand
- Avoid 3rd party frameworks, toolkits or libraries if possible
- Have fun

## Caveat
Rendering is multi-threaded and executed without any locking or synchronisation, this 
should result in considerable tearing and rendering artefacts but these 
don't manifest themselves on my system (Razer Blade 15 Advanced Edition RZ09-0330), 
I suspect this is a consequence of the high polygon count of the model, the small size of the 
polygons and their distribution.

# Attribution
Mountain King model by Pierre-Antoine (https://sketchfab.com/pa)

# License
Copyright 2020 Jean d'Arc

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.