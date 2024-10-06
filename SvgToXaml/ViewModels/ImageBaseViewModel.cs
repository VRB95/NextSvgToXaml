using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using SvgConverter;
using SvgToXaml.Command;

namespace SvgToXaml.ViewModels
{
    public abstract class ImageBaseViewModel : ViewModelBase
    {
        private string m_prefix ="AddIconPrefix";

        protected ImageBaseViewModel(string filepath)
        {
            Filepath = filepath;

            m_convertedSvgData = ConverterLogic.ConvertSvg(Filepath, ResultMode.DrawingImage);

            OpenDetailCommand = new DelegateCommand(OpenDetailExecute);
            OpenFileCommand = new DelegateCommand(OpenFileExecute);

        }

        public string Filepath { get; }
        public string Filename => Path.GetFileName(Filepath);
        public ImageSource PreviewSource => GetImageSource();
        public ICommand OpenDetailCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand CreatePressedIcon { get; set; }
        public ICommand CreateNormalIcon { get; set; }
        public ICommand CreateHoverIcon { get; set; }

        protected abstract ImageSource GetImageSource();
        public abstract bool HasXaml { get; }
        public abstract bool HasSvg { get; }
        public string SvgDesignInfo => GetSvgDesignInfo();

        public string Prefix
        {
            get { return m_prefix; }
            set
            {
                m_prefix = value;
                OnPropertyChanged("prefix");
            }
        }

        private void OpenDetailExecute()
        {
            OpenDetailWindow(this);
        }

        public static void OpenDetailWindow(ImageBaseViewModel imageBaseViewModel)
        {
            new DetailWindow { DataContext = imageBaseViewModel }.Show();
        }

        private void OpenFileExecute()
        {
            Process.Start(Filepath);
        }


        private ConvertedSvgData m_convertedSvgData;
        public ConvertedSvgData ConvertedSvgData
        {
            get { return m_convertedSvgData; }
            set
            {
                m_convertedSvgData = value;
                OnPropertyChanged("ConvertedSvgData");
            }
        }

        protected abstract string GetSvgDesignInfo();
    }
}