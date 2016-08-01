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
        /// <summary>
        /// Let's detect and blur some faces!
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // The name of the source image.
            const string sourceImage = "faces.jpg";

            // The name of the destination image
            const string destinationImage = "detectedfaces.jpg";

            // Get the configuration
            var configuration = BuildConfiguration();

            // Detect the faces in the source file
            DetectFaces(sourceImage, configuration["FaceAPIKey"])
                .ContinueWith((task) =>
                {
                    // Save the result of the detection
                    var faceRects = task.Result;

                    Console.WriteLine($"Detected {faceRects.Length} faces");

                    // Blur the detected faces and save in another file
                    BlurFaces(faceRects, sourceImage, destinationImage);

                    Console.WriteLine($"Done!!!");
                });

            Console.ReadLine();
        }

        /// <summary>
        /// Build the confguration
        /// </summary>
        /// <returns>Returns the configuration</returns>
        private static IConfigurationRoot BuildConfiguration()
        {
            // Enable to app to read json setting files
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
            // We use user secrets in Debug mode so API keys are not uploaded to source control 
            builder.AddUserSecrets("cmendible3-dotnetcore.samples-projectOxford");
#endif

            return builder.Build();
        }

        /// <summary>
        /// Blur the detected faces from de source image.
        /// </summary>
        /// <param name="faceRects">The detected faces rectangles</param>
        /// <param name="sourceImage">The source image</param>
        /// <param name="destinationImage">The destination image</param>
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
                    var image = new Image<Color, uint>(stream);

                    // Blur every detected face
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

        /// <summary>
        /// Detect faces calling the Face API
        /// </summary>
        /// <param name="imageFilePath">ource image</param>
        /// <param name="apiKey">Azure Face API Key</param>
        /// <returns>Detected faces rectangles</returns>
        private static async Task<FaceRectangle[]> DetectFaces(string imageFilePath, string apiKey)
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