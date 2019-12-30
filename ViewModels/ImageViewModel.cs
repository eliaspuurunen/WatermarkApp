using ImageMagick;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WatermarkApp
{
    public class ImageViewModel : ViewModelBase
    {
        public string Path { get; private set; }

        public ImageSource Image { get; }

        public bool IsTempFile { get; private set; }

        private bool isSelected = true;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected == value)
                    return;
                isSelected = value;
                OnPropertyChange(nameof(IsSelected));
            }
        }

        public static async Task<ImageViewModel> BuildViewModelFromPath(string path)
        {
            var realPath = await Task.Run<Tuple<string, bool>>(async () =>
            {
                if (path.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase)
                    || path.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var img = new MagickImage(path))
                    {
                        img.AutoOrient();   // Fix orientation
                        img.Strip();        // remove all EXIF information

                        if (img.Width > 1920 || img.Height > 1920)
                        {
                            if (img.Width > img.Height)
                            {
                                img.Scale(1920, 1080);
                            }
                            else
                            {
                                img.Scale(1080, 1920);
                            }
                        }

                        var tempName = System.IO.Path.GetTempFileName() + ".jpg";
                        img.Write(tempName);

                        return new Tuple<string, bool>(tempName, true);
                    }
                }
                else
                {
                    return new Tuple<string, bool>(path, false);
                }
            });

            return new ImageViewModel(realPath.Item1)
            {
                IsTempFile = realPath.Item2
            };
        }

        public ImageViewModel(string path)
        {
            this.Path = path;
        }
    }
}
