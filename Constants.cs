using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace WatermarkApp
{
    public static class Constants
    {
        public static IEnumerable<HorizontalAlignment> HorizontalOptions { get; }
            = new[] { HorizontalAlignment.Left, HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Stretch };

        public static IEnumerable<VerticalAlignment> VerticalOptions { get; }
            = new[] { VerticalAlignment.Top, VerticalAlignment.Center, VerticalAlignment.Bottom, VerticalAlignment.Stretch };

        public static IEnumerable<GradientMode> GradientModes { get; }
            = new[] { GradientMode.None, GradientMode.Linear, GradientMode.Radial };
    }
}
