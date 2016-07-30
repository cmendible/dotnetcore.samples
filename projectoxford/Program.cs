namespace ConsoleApplication
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ImageProcessorCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.ProjectOxford.Face;
    using Microsoft.ProjectOxford.Face.Contract;

    public class Program
    {
        public static void Main(string[] args)
        {
            const string sourceImage = "faces.jpg";
            const string destinationImage = "detectedfaces.jpg";

            var configuration = BuildConfiguration();

            UploadAndDetectFaces(sourceImage, configuration["FaceAPIKey"])
                .ContinueWith((task) =>
                {
                    var faceRects = task.Result;

                    Console.WriteLine($"Detected {faceRects.Length} faces");

                    BlurFaces(faceRects, sourceImage, destinationImage);

                    Console.WriteLine($"Done!!!");
                });

            Console.ReadLine();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
            builder.AddUserSecrets("cmendible3-dotnetcore.samples-projectOxford");
#endif

            return builder.Build();
        }

        private static void BlurFaces(FaceRectangle[] faceRects, string sourceImage, string destinationImage)
        {
            if (File.Exists(destinationImage))
            {
                File.Delete(destinationImage);
            }

            if (faceRects.Length > 0)
            {
                using (FileStream stream = File.OpenRead("faces.jpg"))
                using (FileStream output = File.OpenWrite(destinationImage))
                {
                    Image image = new Image(stream);

                    foreach (var faceRect in faceRects)
                    {
                        var rectangle = new Rectangle(
                            faceRect.Left,
                            faceRect.Top,
                            faceRect.Width,
                            faceRect.Height);

                        image = image.BoxBlur(20, rectangle);
                    }

                    image.SaveAsJpeg(output);
                }
            }

        }

        private static async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath, string apiKey)
        {
            var faceServiceClient = new FaceServiceClient(apiKey);

            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            {
                return new FaceRectangle[0];
            }
        }
    }
}