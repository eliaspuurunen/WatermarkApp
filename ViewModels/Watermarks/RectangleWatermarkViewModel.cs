using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WatermarkApp
{
    public class RectangleWatermarkViewModel : WatermarkViewModelBase
    {
        private double gradientAngle;
        private GradientMode gradientMode;
        private double fillColor2Position = 0.25;
        private double fillColorPosition = 0.75;
        private Color fillColor2;
        private Color fillColor;
        private double? height;
        private double? width;
        private Brush fill;

        [JsonIgnore]
        public ICommand ClearWidth { get; }

        [JsonIgnore]
        public ICommand ClearHeight { get; }


        public Color FillColor
        {
            get { return fillColor; }
            set
            {
                if (fillColor == value)
                    return;
                fillColor = value;
                OnPropertyChange(nameof(FillColor));
                OnPropertyChange(nameof(Fill));
            }
        }


        public Color FillColor2
        {
            get { return fillColor2; }
            set
            {
                if (fillColor2 == value)
                    return;
                fillColor2 = value;
                OnPropertyChange(nameof(FillColor2));
                OnPropertyChange(nameof(Fill));
            }
        }


        public double FillColorPosition
        {
            get { return fillColorPosition; }
            set
            {
                if (fillColorPosition == value)
                    return;
                fillColorPosition = value;
                OnPropertyChange(nameof(FillColorPosition));
                OnPropertyChange(nameof(Fill));
            }
        }


        public double FillColor2Position
        {
            get { return fillColor2Position; }
            set
            {
                if (fillColor2Position == value)
                    return;
                fillColor2Position = value;
                OnPropertyChange(nameof(FillColor2Position));
                OnPropertyChange(nameof(Fill));
            }
        }

        
        public double GradientAngle
        {
            get { return gradientAngle; }
            set
            {
                if (gradientAngle == value)
                    return;
                gradientAngle = value;
                OnPropertyChange(nameof(GradientAngle));
                OnPropertyChange(nameof(Fill));
            }
        }
        

        public GradientMode GradientMode
        {
            get { return gradientMode; }
            set
            {
                if (gradientMode == value)
                    return;
                gradientMode = value;
                OnPropertyChange(nameof(GradientMode));
                OnPropertyChange(nameof(Fill));
            }
        }

        [JsonIgnore]
        public Brush Fill
        {
            get
            {
                var gradientStops = new GradientStopCollection(new[]{
                            new GradientStop(this.FillColor, this.FillColorPosition),
                            new GradientStop(this.FillColor2, this.FillColor2Position)
                        });

                switch (this.GradientMode)
                {
                    case GradientMode.None:
                        return new SolidColorBrush(this.FillColor);
                    case GradientMode.Linear:
                        var toReturn = new LinearGradientBrush(gradientStops);
                        // At 0, should be   (0,   0),   (1,   1)
                        // At 45, should be  (0,   0.5), (1,   0.5)
                        // At 90, should be  (0,   1),   (1,   0)
                        // At 135, should be (0.5, 1),   (0.5, 0)
                        // At 180, should be (1,   1),   (0,   0)
                        // At 225, should be (1,   0.5), (0,   0.5)
                        // At 270, should be (1,   0),   (0,   1)
                        // At 315, should be (0.5, 0),   (0.5, 1)
                        // At 360, should be (0,   0),   (1,   1)

                        var startX = GetStartXFromAngle(this.GradientAngle);
                        var startY = GetStartXFromAngle((this.GradientAngle + 90.0) % 360.0);

                        var endX = GetStartXFromAngle((this.GradientAngle + 180.0) % 360.0);
                        var endY = GetStartXFromAngle((this.GradientAngle + 270.0) % 360.0);

                        toReturn.StartPoint = new Point(startX, startY);
                        toReturn.EndPoint = new Point(endX, endY);

                        return toReturn;
                    case GradientMode.Radial:
                        return new RadialGradientBrush(gradientStops);
                    default:
                        return null;
                }
            }
        }

        private static double GetStartXFromAngle(double angle)
        {
            return angle >= 0 && angle <= 90
                ? 0.0
                : angle >= 180 && angle <= 270
                    ? 1.0
                    : angle > 90 && angle < 180
                        ? (angle % 90.0) / 90.0
                            : (360.0 - angle) / 90.0;

        }

        public double? Width
        {
            get { return width; }
            set
            {
                if (width == value)
                    return;
                width = value;
                OnPropertyChange(nameof(Width));
            }
        }


        public double? Height
        {
            get { return height; }
            set
            {
                if (height == value)
                    return;
                height = value;
                OnPropertyChange(nameof(Height));
            }
        }


        public RectangleWatermarkViewModel(WindowViewModel parent)
            : base(parent)
        {
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;

            this.ClearHeight = ReactiveUI.ReactiveCommand.Create(() => this.Height = null);
            this.ClearWidth = ReactiveUI.ReactiveCommand.Create(() => this.Width = null);
        }
    }
}
