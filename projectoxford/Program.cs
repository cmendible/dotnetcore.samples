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
        static FaceServiceClient faceServiceClient;

        public static void Main(string[] args)
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
            builder.AddUserSecrets("cmendible3-dotnetcore.samples-projectOxford");
#endif

            var configuration = builder.Build();

            var apiKey = configuration["FaceAPIKey"];

            if (File.Exists("detectedfaces.jpg"))
            {
                File.Delete("detectedfaces.jpg");
            }

            faceServiceClient = new FaceServiceClient(apiKey);

            var faceRects = UploadAndDetectFaces("faces.jpg").Result;

            Console.WriteLine($"Detected {faceRects.Length} faces");

            if (faceRects.Length > 0)
            {
                using (FileStream stream = File.OpenRead("faces.jpg"))
                using (FileStream output = File.OpenWrite("detectedfaces.jpg"))
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

        private static async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath)
        {
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
