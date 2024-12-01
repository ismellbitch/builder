using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace ImageConverter
{
    public interface IImageBuilder
    {
        void SetSourcePath(string path);
        void ConvertImage();
        string GetResultPath();
    }

    public class ImageBuilder : IImageBuilder
    {
        private string _sourcePath;
        private string _destinationPath;

        public void SetSourcePath(string path)
        {
            _sourcePath = path;
        }

        public void ConvertImage()
        {
            if (File.Exists(_sourcePath))
            {
                string extension = Path.GetExtension(_sourcePath).ToLower();
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_sourcePath);
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (extension == ".png")
                {
                    _destinationPath = Path.Combine(desktopPath, $"{fileNameWithoutExtension}.jpg");
                    using (Image image = Image.FromFile(_sourcePath))
                    {
                        image.Save(_destinationPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                else if (extension == ".jpg" || extension == ".jpeg")
                {
                    _destinationPath = Path.Combine(desktopPath, $"{fileNameWithoutExtension}.png");
                    using (Image image = Image.FromFile(_sourcePath))
                    {
                        image.Save(_destinationPath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                else
                {
                    throw new InvalidOperationException("This format is not supproted. Use .png or .jpg.");
                }
            }
            else
            {
                throw new FileNotFoundException("There's no file on your path");
            }
        }

        public string GetResultPath()
        {
            return _destinationPath;
        }
    }

    // Директор, который управляет процессом
    public class ImageConverterDirector
    {
        private readonly IImageBuilder _imageBuilder;

        public ImageConverterDirector(IImageBuilder imageBuilder)
        {
            _imageBuilder = imageBuilder;
        }

        public void Convert(string path)
        {
            _imageBuilder.SetSourcePath(path);
            _imageBuilder.ConvertImage();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Type file path: ");
            string filePath = Console.ReadLine();

            try
            {
                IImageBuilder imageBuilder = new ImageBuilder();
                ImageConverterDirector director = new ImageConverterDirector(imageBuilder);

                director.Convert(filePath);

                string resultPath = imageBuilder.GetResultPath();
                Console.Write($"Convertation completed! Result saved on path: {resultPath}");
            }
            catch (Exception ex)
            {
                Console.Write($"Error: {ex.Message}");
            }
        }
    }
}
