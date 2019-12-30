using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace WatermarkApp
{
    public class TextWatermarkViewModel : WatermarkViewModelBase
    {

        private double shadowWeight;
        private Color shadowColour;
        private bool shadow;
        private Color colour;
        private FontFamily font;
        private double size = 30;
        private string text;
        private double margin = 6;

        public TextWatermarkViewModel(WindowViewModel parent)
            : base(parent)
        {
            this.Colour = Colors.Black;
            this.Text = "New Text";
        }

        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;
                text = value;
                OnPropertyChange(nameof(Text));
            }
        }


        public bool Shadow
        {
            get { return shadow; }
            set
            {
                if (shadow == value)
                    return;
                shadow = value;
                OnPropertyChange(nameof(Shadow));

                if (!shadow)
                {
                    this.ShadowColour = Colors.Transparent;
                    this.ShadowWeight = 0;
                }
            }
        }


        public Color ShadowColour
        {
            get { return shadowColour; }
            set
            {
                if (shadowColour == value)
                    return;
                shadowColour = value;
                OnPropertyChange(nameof(ShadowColour));
                OnPropertyChange(nameof(ShadowBrush));
            }
        }


        public double ShadowWeight
        {
            get { return shadowWeight; }
            set
            {
                if (shadowWeight == value)
                    return;
                shadowWeight = value;
                OnPropertyChange(nameof(ShadowWeight));
            }
        }


        public FontFamily Font
        {
            get { return font; }
            set
            {
                if (font == value)
                    return;
                font = value;
                OnPropertyChange(nameof(Font));
            }
        }

        public double Size
        {
            get { return size; }
            set
            {
                if (size == value)
                    return;
                size = value;
                OnPropertyChange(nameof(Size));
            }
        }

        public double Margin
        {
            get { return margin; }
            set
            {
                if (margin == value)
                    return;
                margin = value;
                OnPropertyChange(nameof(Margin));
                OnPropertyChange(nameof(ImageMargin));
            }
        }

        public Thickness ImageMargin
        {
            get
            {
                return new Thickness(this.Margin);
            }
        }


        public Color Colour
        {
            get { return colour; }
            set
            {
                if (colour == value)
                    return;
                colour = value;
                OnPropertyChange(nameof(Colour));
                OnPropertyChange(nameof(FontBrush));
            }
        }

        [JsonIgnore]
        public Brush FontBrush => new SolidColorBrush(this.Colour);

        [JsonIgnore]
        public Brush ShadowBrush => new SolidColorBrush(this.shadowColour);
    }
}
