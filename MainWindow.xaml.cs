using ImageMagick;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WatermarkApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new WindowViewModel(this);
        }

        public RenderTargetBitmap ExportImage()
        {
            return FrameworkHelpers.GetImage(this.layoutWindow);
        }
        public void RequestUpdateLayout()
        {
            this.layoutWindow.UpdateLayout();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            (this.DataContext as WindowViewModel).CleanUpTempImages();
        }
    }
}
