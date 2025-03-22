using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MemeFactory.Core.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;

namespace MemeProcessorLibrary
{
    public class MemeProcessor
    {
        // 现有的异步方法保持不变
        public static async Task<string> ProcessMemeAsync(string base64InputImage, int directionHorizontal, int directionVertical, int minMoves, int Interval)
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
                .AutoComposeAsync(Interval);

            // 将结果图像保存到内存流中
            using var memoryStream = new MemoryStream();
            await result.Image.SaveAsync(memoryStream, new GifEncoder());

            // 将内存流转换为Base64字符串
            byte[] resultImageBytes = memoryStream.ToArray();
            string base64Result = Convert.ToBase64String(resultImageBytes);

            // 返回处理后的GIF的Base64字符串
            return base64Result;
        }

        // 添加的同步方法，供非托管代码调用
        [UnmanagedCallersOnly(EntryPoint = "ProcessMeme")]
        public static IntPtr ProcessMeme(IntPtr base64InputImagePtr, int directionHorizontal, int directionVertical, int minMoves, int Interval)
        {
            try
            {
                string base64InputImage = Marshal.PtrToStringAnsi(base64InputImagePtr) ?? string.Empty;
                if (string.IsNullOrEmpty(base64InputImage))
                    return IntPtr.Zero;

                // 使用Task.Run来异步调用ProcessMemeAsync并同步等待结果
                string result = Task.Run(async () => await ProcessMemeAsync(base64InputImage, directionHorizontal, directionVertical, minMoves, Interval))
                    .GetAwaiter().GetResult();

                // 将处理后的结果字符串直接转换为非托管内存（无需手动管理内存）
                IntPtr resultPtr = Marshal.StringToHGlobalAnsi(result);  // 将字符串转换为非托管内存

                return resultPtr;
            }
            catch
            {
                // 异常时返回空指针
                return IntPtr.Zero;
            }
        }
    }
}
