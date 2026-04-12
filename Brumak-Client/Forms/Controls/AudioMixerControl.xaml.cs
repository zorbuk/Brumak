using Brumak_Client.Audio;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Brumak_Client.Forms.Controls
{
    public partial class AudioMixerControl : UserControl
    {
        private readonly AudioManager _audio = AudioManager.Instance;

        private bool _isDragging;
        private Point _dragStart;
        private double _originX, _originY;

        #region "Constructor"
        public AudioMixerControl()
        {
            InitializeComponent();
            BuildChannels();
        }
        #endregion

        #region "Build"
        private void BuildChannels()
        {
            var channels = new[] { _audio.Master }.Concat(_audio.AllChannels);

            foreach (var channel in channels)
            {
                bool isMaster = channel.Name == "Master";

                ChannelList.Children.Add(BuildRow(channel, isMaster));

                if (isMaster)
                    ChannelList.Children.Add(new Rectangle
                    {
                        Height = 1,
                        Fill = new SolidColorBrush(Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
                        Margin = new Thickness(0, 8, 0, 8)
                    });
            }
        }

        private Grid BuildRow(AudioChannel channel, bool isMaster)
        {
            var label = new TextBlock
            {
                Text = channel.Name.ToUpper(),
                FontSize = 10,
                FontWeight = isMaster ? FontWeights.Bold : FontWeights.Normal,
                Foreground = new SolidColorBrush(isMaster
                    ? Colors.White
                    : Color.FromArgb(0x99, 0xFF, 0xFF, 0xFF)),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 46
            };

            var pct = new TextBlock
            {
                Text = $"{(int)(channel.Volume * 100)}%",
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(isMaster
                    ? Color.FromRgb(0xD1, 0x36, 0x39)
                    : Color.FromArgb(0x88, 0xFF, 0xFF, 0xFF)),
                VerticalAlignment = VerticalAlignment.Center,
                Width = 34,
                TextAlignment = TextAlignment.Right
            };

            var slider = new Slider
            {
                Minimum = 0,
                Maximum = 1,
                Value = channel.Volume,
                Style = (Style)FindResource("AudioSlider"),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0),
                Tag = channel
            };

            slider.ValueChanged += (_, e) =>
            {
                pct.Text = $"{(int)(e.NewValue * 100)}%";

                if (channel.Name == "Master")
                    _audio.SetMasterVolume((float)e.NewValue);
                else
                    channel.Volume = (float)e.NewValue;

                _audio.SaveSettings();
            };

            var grid = new Grid { Margin = new Thickness(0, 0, 0, isMaster ? 0 : 8) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(label, 0);
            Grid.SetColumn(slider, 1);
            Grid.SetColumn(pct, 2);

            grid.Children.Add(label);
            grid.Children.Add(slider);
            grid.Children.Add(pct);

            return grid;
        }
        #endregion

        #region "Drag"
        private void DragHandle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _dragStart = e.GetPosition(Window.GetWindow(this));
            _originX = PanelTranslate.X;
            _originY = PanelTranslate.Y;
            DragHandle.CaptureMouse();
            e.Handled = true;
        }

        private void DragHandle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var current = e.GetPosition(Window.GetWindow(this));
            PanelTranslate.X = _originX + (current.X - _dragStart.X);
            PanelTranslate.Y = _originY + (current.Y - _dragStart.Y);
        }

        private void DragHandle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            DragHandle.ReleaseMouseCapture();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            => e.Handled = true;
        #endregion

        #region "Events"
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            PanelTranslate.X = 0;
            PanelTranslate.Y = 0;
        }
        #endregion

        #region "Toggle"
        public void Toggle()
        {
            Visibility = Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

            if (Visibility == Visibility.Visible)
            {
                PanelTranslate.X = 0;
                PanelTranslate.Y = 0;
            }
        }
        #endregion
    }
}