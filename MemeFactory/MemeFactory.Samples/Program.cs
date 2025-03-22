using MemeFactory.Core.Processing;
using MemeFactory.Core.Utilities;
using SixLabors.ImageSharp;

// load resources
using var baseImage = await Image.LoadAsync("resources/base.gif");
using var baseSequence = await baseImage.ExtractFrames()
    // the Sequence class can manage the disposal of all frames
    .ToSequenceAsync();

// process
using var result = await baseSequence
    .Sliding(directionHorizontal: 1, directionVertical: 1)
    // generate final image
    .AutoComposeAsync(1);

// output
await result.Image.SaveAsync("result." + result.Extension, result.Encoder);
