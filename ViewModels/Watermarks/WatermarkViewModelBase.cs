using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace WatermarkApp
{
    public class WatermarkViewModelBase : ViewModelBase
    {
        private VerticalAlignment verticalAlignment;
        private HorizontalAlignment horizontalAlignment;

        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set
            {
                if (horizontalAlignment == value)
                    return;
                horizontalAlignment = value;
                OnPropertyChange(nameof(HorizontalAlignment));
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                if (verticalAlignment == value)
                    return;
                verticalAlignment = value;
                OnPropertyChange(nameof(VerticalAlignment));
            }
        }


        [JsonIgnore]
        public ICommand MoveUp { get; private set; }
        [JsonIgnore]
        public ICommand MoveDown { get; private set; }
        [JsonIgnore]
        public ICommand Remove { get; private set; }

        public WatermarkViewModelBase(WindowViewModel parent)
        {
            this.SetupCommands(parent);
        }

        public void SetupCommands(WindowViewModel parent)
        {
            this.Remove = ReactiveUI.ReactiveCommand.Create(() => parent.Watermarks.Remove(this));

            this.MoveDown = ReactiveUI.ReactiveCommand.Create(() =>
            {
                parent.MoveRequested(1, this);
            });

            this.MoveUp = ReactiveUI.ReactiveCommand.Create(() =>
            {
                parent.MoveRequested(-1, this);
            });
        }
    }
}
