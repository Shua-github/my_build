using MemeFactory.Core.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;

namespace MemeProcessorLibrary
{
    public class MemeProcessor
    {
        // 修改后的异步方法，接收输入的Base64字符串和处理参数，返回处理后的GIF的Base64字符串
        public static async Task<string> ProcessMemeAsync(string base64InputImage, int directionHorizontal, int directionVertical, int minMoves)
        {
            // 将Base64字符串转换为字节数组
            byte[] inputImageBytes = Convert.FromBase64String(base64InputImage);

            // 使用ImageSharp加载GIF图像
            using var inputStream = new MemoryStream(inputImageBytes);
            using var baseImage = await Image.LoadAsync(inputStream);
            using var baseSequence = await baseImage.ExtractFrames()
                .ToSequenceAsync();

            // 处理
            using var result = await baseSequence
                .Sliding(directionHorizontal: directionHorizontal, directionVertical: directionVertical, minMoves: minMoves)
                .AutoComposeAsync();

            // 将结果图像保存到内存流中
            using var memoryStream = new MemoryStream();
            await result.Image.SaveAsync(memoryStream, new GifEncoder());

            // 将内存流转换为Base64字符串
            byte[] resultImageBytes = memoryStream.ToArray();
            string base64Result = Convert.ToBase64String(resultImageBytes);

            // 返回处理后的GIF的Base64字符串
            return base64Result;
        }
    }
}
