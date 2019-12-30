using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WatermarkApp
{
    public class WindowViewModel : ViewModelBase
    {
        public ObservableCollection<ImageViewModel> Images { get; }
            = new ObservableCollection<ImageViewModel>();

        public ObservableCollection<WatermarkViewModelBase> Watermarks { get; }
            = new ObservableCollection<WatermarkViewModelBase>();


        public ImageViewModel SelectedImage
        {
            get { return selectedImage; }
            set
            {
                if (selectedImage == value)
                    return;
                selectedImage = value;
                OnPropertyChange(nameof(SelectedImage));
            }
        }


        public WatermarkViewModelBase SelectedWatermark
        {
            get { return selectedWatermark; }
            set
            {
                if (selectedWatermark == value)
                    return;
                selectedWatermark = value;
                OnPropertyChange(nameof(SelectedWatermark));
            }
        }


        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;
                isBusy = value;
                OnPropertyChange(nameof(IsBusy));
            }
        }


        public ICommand Browse { get; }

        public ICommand AddWatermark { get; }

        public ICommand AddText { get; }

        public ICommand AddEffect { get; }

        public ICommand ExportImage { get; }

        public ICommand Export { get; }

        public ICommand LoadSettings { get; }

        public ICommand SaveSettings { get; }

        public ICommand Exit { get; }

        private bool isBusy;
        private WatermarkViewModelBase selectedWatermark;
        private ImageViewModel selectedImage;
        private string rootPath;
        private MainWindow window;

        public string RootPath
        {
            get { return rootPath; }
            set
            {
                if (rootPath == value)
                    return;
                rootPath = value;
                OnPropertyChange(nameof(RootPath));
                this.OnUpdateRootPath();
            }
        }

        public WindowViewModel(MainWindow window)
        {
            this.window = window;

            this.Browse = ReactiveUI.ReactiveCommand.Create(() =>
            {
                using (var ofd = new System.Windows.Forms.FolderBrowserDialog())
                {
                    var result = ofd.ShowDialog();
                    if (result != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }

                    this.RootPath = ofd.SelectedPath;
                }
            });

            this.AddWatermark = ReactiveUI.ReactiveCommand.Create(() =>
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Image Files(*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG)|*.BMP;*.JPG;*.JPEG;*.PNG;*.GIF|All files (*.*)|*.*";
                var result = ofd.ShowDialog();
                if (result != true)
                {
                    return;
                }

                this.Watermarks.Add(new ImageWatermarkViewModel(ofd.FileName, this));
            });

            this.AddText = ReactiveUI.ReactiveCommand.Create(() =>
            {
                this.Watermarks.Add(new TextWatermarkViewModel(this));
            });

            this.AddEffect = ReactiveUI.ReactiveCommand.Create(() =>
            {
                this.Watermarks.Add(new RectangleWatermarkViewModel(this));
            });

            this.Exit = ReactiveUI.ReactiveCommand.Create(() =>
            {
                this.window.Close();
            });

            this.Export = ReactiveUI.ReactiveCommand.Create(this.OnExport);
            this.ExportImage = ReactiveUI.ReactiveCommand.Create(() => this.OnExportImage(true));
            this.LoadSettings = ReactiveUI.ReactiveCommand.Create(this.OnLoadSettings);
            this.SaveSettings = ReactiveUI.ReactiveCommand.Create(this.OnSaveSettings);
        }

        public void CleanUpTempImages()
        {
            this.IsBusy = true;
            var tempImages = this.Images.Where(x => x.IsTempFile);

            this.Images.Clear();

            foreach (var item in tempImages)
            {
                File.Delete(item.Path);
            }
            this.IsBusy = false;
        }

        private async void OnUpdateRootPath()
        {
            this.CleanUpTempImages();

            this.IsBusy = true;

            var images = new List<string>();

            foreach (var globPattern in new[] { "*.jpg", "*.jpeg" })
            {
                var searchResult = Directory.GetFiles(this.RootPath, globPattern);
                if (searchResult.Any())
                {
                    images.AddRange(searchResult);
                }
            }

            var results = images.Select(x => Task.Run(() => ImageViewModel.BuildViewModelFromPath(x)));
            await Task.WhenAll(results);

            foreach (var item in results.Select(x => x.Result))
            {
                this.Images.Add(item);
            }

            this.IsBusy = false;
        }

        private void OnExport()
        {
            string path = this.RootPath;
            using (var ofd = new System.Windows.Forms.FolderBrowserDialog())
            {
                ofd.SelectedPath = path;
                var result = ofd.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                path = ofd.SelectedPath;
            }

            this.IsBusy = true;
            var currentImage = this.SelectedImage;

            foreach (var image in this.Images.Where(x => x.IsSelected))
            {
                this.SelectedImage = image;
                this.window.RequestUpdateLayout();
                System.Threading.Thread.Sleep(50);

                var newFileNameParts = new FileInfo(image.Path).Name.Split('.');

                var newName =
                    System.IO.Path.Combine(path, $"{string.Join('.', newFileNameParts.Take(newFileNameParts.Length - 1))}_processed.jpg");

                using (var fileStream = new StreamWriter(new FileStream(newName, FileMode.OpenOrCreate, FileAccess.Write)))
                {
                    var snapshot = this.window.ExportImage();
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(snapshot));
                    encoder.Save(fileStream.BaseStream);
                }
            }

            this.SelectedImage = currentImage;
            this.IsBusy = false;
            System.Windows.MessageBox.Show("Export complete.");
        }

        private void OnExportImage(bool showAfterSave)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "JPEG Files (*.jpeg)|*.jpeg"
            };
            var result = sfd.ShowDialog();

            if (result != true)
            {
                return;
            }

            var snapshot = this.window.ExportImage();
            this.IsBusy = true;

            using (var file = sfd.OpenFile())
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.QualityLevel = 80;
                encoder.Frames.Add(BitmapFrame.Create(snapshot));
                encoder.Save(file);
            }

            this.IsBusy = false;
            MessageBox.Show("Export complete.");
        }

        private void OnLoadSettings()
        {
            OpenFileDialog sfd = new OpenFileDialog
            {
                Filter = "The Image Exporter Files (*.empex)|*.empex"
            };
            var result = sfd.ShowDialog();

            if (result != true)
            {
                return;
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            this.IsBusy = true;
            var toLoad = JsonConvert.DeserializeObject<ObservableCollection<WatermarkViewModelBase>>(File.ReadAllText(sfd.FileName), settings);

            this.Watermarks.Clear();

            foreach (var item in toLoad)
            {
                item.SetupCommands(this);
                this.Watermarks.Add(item);
            }
            this.IsBusy = false;
        }

        private void OnSaveSettings()
        {

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "The Image Exporter Files (*.empex)|*.empex"
            };
            var result = sfd.ShowDialog();

            if (result != true)
            {
                return;
            }

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            this.IsBusy = true;
            var toSave = JsonConvert.SerializeObject(this.Watermarks, settings);

            File.WriteAllText(sfd.FileName, toSave);
            this.IsBusy = false;
        }

        public void MoveRequested(int direction, WatermarkViewModelBase target)
        {
            var index = this.Watermarks.IndexOf(target);
            if (index == 0 && direction == -1)
            {
                return;
            }

            if (index == this.Watermarks.Count - 1 && direction == 1)
            {
                return;
            }

            this.Watermarks.Remove(target);
            this.Watermarks.Insert(index + direction, target);
        }
    }
}
