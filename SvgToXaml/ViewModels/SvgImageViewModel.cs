using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using SvgConverter;
using SvgToXaml.Command;
using SvgToXaml.Infrastructure;

namespace SvgToXaml.ViewModels
{
    public static class ColorMapKeys
    {
        // Light theme colors
        public const string LightNormal = "#787474";
        public const string LightHover = "#201c1c";
        public const string LightPressed = "#0874fc";
        public const string LightPressedRed = "#FF3333"; // RED variant

        // Dark theme colors
        public const string DarkNormal = "#DDDDDD";
        public const string DarkHover = "#EFEFEF";
        public const string DarkPressed = "#00a8ff";
        public const string DarkPressedRed = "#FF3333"; // RED variant
    }

    public class SvgImageViewModel : ImageBaseViewModel
    {
        private ConvertedSvgData _convertedSvgData;


        public SvgImageViewModel(string filepath) : base(filepath)
        {
            CreatePressedIcon = new DelegateCommand<object>((obj) => CreatePressedIconExecute(obj));
            CreateNormalIcon = new DelegateCommand<object>((obj) => CreateNormalIconExecute(obj));
            CreateHoverIcon = new DelegateCommand<object>((obj) => CreateHoverIconExecute(obj));
        }


        private string GetColorBasedOnTheme(string theme, string context)
        {
            switch (context)
            {
                case "Hover":
                    return theme == "Light" ? ColorMapKeys.LightHover : ColorMapKeys.DarkHover;
                case "Normal":
                    return theme == "Light" ? ColorMapKeys.LightNormal : ColorMapKeys.DarkNormal;
                case "Pressed":
                    return theme == "Light" ? ColorMapKeys.LightPressed : ColorMapKeys.DarkNormal;
                default:
                    return string.Empty;
            }
        }

        private (string xaml, string action) CreateIconExecute(object obj, string context)
        {
            if (obj is string str)
            {
                string color = GetColorBasedOnTheme(str, context);
                if (!string.IsNullOrEmpty(color))
                {
                    var xaml = ReplaceBrush(Xaml, color);
                    return (xaml, context);
                }
            }

            return (Xaml, context);
        }

        private void CreateHoverIconExecute(object obj)
        {
            SaveXamlFile(CreateIconExecute(obj, "Hover"));
        }

        private void CreateNormalIconExecute(object obj)
        {
            SaveXamlFile(CreateIconExecute(obj, "Normal"));
        }

        private void CreatePressedIconExecute(object obj)
        {
            SaveXamlFile(CreateIconExecute(obj, "Pressed"));
        }

        private void SaveXamlFile((string xaml, string action) xamlContent)
        {
            // Create and configure the SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "XAML files (*.xaml)|*.xaml", // Save as .xaml file
                DefaultExt = ".xaml",
                FileName = $"{Prefix}_{xamlContent.action}.xaml" // Default file name
            };

            // Show the dialog and check if the user selects a path
            if (saveFileDialog.ShowDialog() is DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                // Write the XAML content to the selected file
                File.WriteAllText(filePath, xamlContent.xaml);
            }
        }

        public static string ReplaceBrush(string xamlContent, string newBrushValue)
        {
            // Regular expression to find the Brush attribute with a hex color value
            string pattern = @"Brush=""#[A-Fa-f0-9]{8}""";

            // Replace the matched Brush value with the new one
            string replacedXaml = Regex.Replace(xamlContent, pattern, $"Brush=\"{newBrushValue}\"");

            return replacedXaml;
        }


        public SvgImageViewModel(ConvertedSvgData convertedSvgData)
            : this(convertedSvgData.Filepath)
        {
            _convertedSvgData = convertedSvgData;

        }

        public static SvgImageViewModel DesignInstance
        {
            get
            {
                var imageSource = new DrawingImage(new GeometryDrawing(Brushes.Black, null, new RectangleGeometry(new Rect(new Size(10, 10)), 1, 1)));
                var data = new ConvertedSvgData { ConvertedObj = imageSource, Filepath = "FilePath", Svg = "<svg/>", Xaml = "<xaml/>" };
                return new SvgImageViewModel(data);
            }
        }

        protected override ImageSource GetImageSource()
        {
            return ConvertedSvgData?.ConvertedObj as ImageSource;
        }

        protected override string GetSvgDesignInfo()
        {
            if (PreviewSource is DrawingImage)
            {
                var di = (DrawingImage)PreviewSource;
                if (di.Drawing is DrawingGroup)
                {
                    var dg = (DrawingGroup)di.Drawing;
                    var bounds = dg.ClipGeometry?.Bounds ?? dg.Bounds;
                    return $"{bounds.Width:#.##}x{bounds.Height:#.##}";
                }
            }
            return null;
        }

        public override bool HasXaml => true;
        public override bool HasSvg => true;

        public string Svg => ConvertedSvgData?.Svg;
        public string Xaml => ConvertedSvgData?.Xaml;
    }
}
