using Brumak_Client.Audio;
using Brumak_Client.Network;
using Brumak_Shared.Character.Model;
using Brumak_Shared.Classes.Static;
using Brumak_Shared.Drawings.Entity;
using Brumak_Shared.Drawings.Skin;
using Brumak_Shared.Network.Frames.Characters;
using Brumak_Shared.Network.Frames.Servers;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace Brumak_Client.Forms
{
    public partial class CharacterCreation : Window
    {
        public static CharacterCreation Instance { get; set; } = null!;

        private static readonly string[] SkinPalette = ["#C8B48C", "#F0C890", "#8B6040", "#FDDBB4", "#5C3317"];
        private static readonly string[] ClothPalette = ["#783C28", "#2A4A8A", "#2A7A3A", "#7A2A7A", "#1A1A2A"];
        private static readonly string[] AccentPalette = ["#50321E", "#1A2A5A", "#1A5A2A", "#5A1A5A", "#444444"];
        private static readonly string[] Custom1Palette = ["#4488CC", "#CC4444", "#44CC88", "#CCAA44", "#8844CC"];
        private static readonly string[] Custom2Palette = ["#CC4444", "#44CC88", "#CCAA44", "#4488CC", "#CC8844"];

        private static readonly string[] RandomNames =
        [
            "Aldric", "Seraphine", "Doran", "Lyria", "Torvan",
            "Elaris", "Goran", "Mira", "Balen", "Sylvara"
        ];

        private readonly List<Border> _classBorders = [];
        private readonly PreviewEntity _previewEntity = new();

        private readonly string[] _selectedColors =
            ["#C8B48C", "#783C28", "#50321E", "#4488CC", "#CC4444"];

        private int _selectedClassIndex = 0;
        private int _activeColorSlot = 0;
        private string _direction = "S";

        private static readonly string[] SlotNames = ["PIEL", "ROPA PRINCIPAL", "ACENTO", "COLOR 1", "COLOR 2"];
        private readonly List<string> _customColors = [];

        public event Action<CharacterFrame>? OnCharacterFrameMessage;

        #region "Constructor"
        public CharacterCreation()
        {
            InitializeComponent();
            Instance = this;
            DataContext = AccountSingleton.Instance;

            Closed += (_, _) => Instance = null!;

            OnCharacterFrameMessage += frame =>
            {
                Dispatcher.Invoke(() =>
                {
                    switch (frame)
                    {
                        case CreateCharacterSuccessFrame createCharacter:
                            Application.Current.MainWindow = CharacterSelection.Instance;
                            CharacterSelection.Instance.Show();
                            Hide();
                            break;

                        case CharacterErrorFrame error:
                            MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            CreateBtn.IsEnabled = true;
                            break;
                    }
                });
            };

            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((sender, _) =>
            {
                if (sender is FrameworkElement el && el.Tag is "silent") return;
                AudioManager.Instance.UI.Play(@"/Assets/Audio/Ui/Click.ogg");
            }));

            CharacterNameInput.TextChanged += (_, _) =>
                CreateBtn.IsEnabled = CharacterNameInput.Text.Trim().Length >= 3;

            BuildClassGrid();
            BuildColorPalettes();
            SelectClass(0);
        }
        #endregion

        #region "Ask Frames"
        public void AskCharacterCreationFrame()
        {
            NetworkManager.WorldClientManager.Send(new CreateCharacterFrame()
            {
                AccountId = AccountSingleton.Instance.Id,
                ServerId = CharacterSelection.Instance.CurrentServer.Id,
                Name = CharacterNameInput.Text.Trim(),
                Skin = Classes.Get(_selectedClassIndex).First().Skin,
                Class = Classes.Get(_selectedClassIndex).First().Class,
                SkinHexColors = [.. _selectedColors],
            });
        }
        #endregion

        #region "Class Grid"
        private void BuildClassGrid()
        {
            for (int i = 0; i < Classes.Get().Count(); i++)
            {
                int index = i;
                var cls = Classes.Get(i).FirstOrDefault();

                var icon = new TextBlock
                {
                    Text = cls.Icon,
                    FontSize = 22,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 4)
                };

                var name = new TextBlock
                {
                    Text = cls.Name,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center
                };

                var stack = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                stack.Children.Add(icon);
                stack.Children.Add(name);

                var border = new Border
                {
                    Style = (Style)FindResource("ClassItem"),
                    Child = stack,
                    Width = 110,
                    Height = 70,
                    Margin = new Thickness(0, 0, 6, 6),
                    Padding = new Thickness(6)
                };

                border.MouseLeftButtonDown += (_, _) => SelectClass(index);
                _classBorders.Add(border);
                ClassGrid.Children.Add(border);
            }
        }

        private void SelectClass(int index)
        {
            _selectedClassIndex = index;
            var cls = Classes.Get(index).FirstOrDefault();

            for (int i = 0; i < _classBorders.Count; i++)
                _classBorders[i].Style = (Style)FindResource(
                    i == index ? "ClassItemSelected" : "ClassItem");

            SelectedClassName.Text = cls.Name;
            SelectedClassDesc.Text = cls.Description;

            _previewEntity.Skin = (int)cls.Skin;
            RefreshPreview();
        }
        #endregion

        #region "Color Palettes"
        private void BuildColorPalettes()
        {
            BuildPalette(SkinColorPanel, SkinPalette, 0);
            BuildPalette(ClothColorPanel, ClothPalette, 1);
            BuildPalette(AccentColorPanel, AccentPalette, 2);
            BuildPalette(Custom1ColorPanel, Custom1Palette, 3);
            BuildPalette(Custom2ColorPanel, Custom2Palette, 4);
        }

        private void BuildPalette(WrapPanel panel, string[] colors, int slot)
        {
            bool first = true;
            foreach (var hex in colors)
            {
                var swatch = new Border
                {
                    Style = first ? (Style)FindResource("ColorSwatchSelected")
                                       : (Style)FindResource("ColorSwatch"),
                    Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(hex)),
                    Margin = new Thickness(0, 0, 4, 4),
                    Tag = hex
                };
                swatch.MouseLeftButtonDown += (_, _) => OnColorSelected(slot, hex, panel, swatch);
                panel.Children.Add(swatch);
                first = false;
            }
        }

        private void OnColorSelected(int slot, string hex, WrapPanel panel, Border selected)
        {
            _activeColorSlot = slot;
            _selectedColors[slot] = hex;

            foreach (Border b in panel.Children)
                b.Style = (Style)FindResource("ColorSwatch");
            selected.Style = (Style)FindResource("ColorSwatchSelected");

            UpdateHexUI(hex);
            _previewEntity.SkinHexColors = [.. _selectedColors];
            RefreshPreview();
        }

        private void UpdateHexUI(string hex)
        {
            HexInput.Text = hex;

            try
            {
                var color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
                HexPreview.Background = new SolidColorBrush(color);

                double lum = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
                HexPreviewLabel.Foreground = lum > 128
                    ? new SolidColorBrush(Color.FromRgb(0x11, 0x11, 0x11))
                    : new SolidColorBrush(Colors.White);
                HexPreviewLabel.Text = hex.ToUpper();

                ActiveSlotPreview.Background = new SolidColorBrush(color);
                ActiveSlotName.Text = SlotNames[_activeColorSlot];
            }
            catch { }
        }

        private void HexInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var hex = HexInput.Text.Trim();
            if (!hex.StartsWith('#') || (hex.Length != 7 && hex.Length != 4)) return;

            try
            {
                var color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
                HexPreview.Background = new SolidColorBrush(color);

                double lum = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;
                HexPreviewLabel.Foreground = lum > 128
                    ? new SolidColorBrush(Color.FromRgb(0x11, 0x11, 0x11))
                    : new SolidColorBrush(Colors.White);
                HexPreviewLabel.Text = hex.ToUpper();
            }
            catch { }
        }

        private void ApplyHex_Click(object sender, RoutedEventArgs e)
        {
            var hex = HexInput.Text.Trim();
            if (!hex.StartsWith('#') || (hex.Length != 7 && hex.Length != 4)) return;

            try { System.Windows.Media.ColorConverter.ConvertFromString(hex); }
            catch { return; }

            _selectedColors[_activeColorSlot] = hex;
            UpdateHexUI(hex);

            if (!_customColors.Contains(hex))
            {
                _customColors.Insert(0, hex);
                if (_customColors.Count > 8) _customColors.RemoveAt(_customColors.Count - 1);
                RebuildCustomColors();
            }

            _previewEntity.SkinHexColors = [.. _selectedColors];
            RefreshPreview();
        }

        private void RebuildCustomColors()
        {
            CustomColorPanel.Children.Clear();
            foreach (var hex in _customColors)
            {
                var h = hex;
                var swatch = new Border
                {
                    Style = (Style)FindResource("ColorSwatch"),
                    Background = new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(h)),
                    Margin = new Thickness(0, 0, 4, 4),
                    Tag = h
                };
                swatch.MouseLeftButtonDown += (_, _) =>
                {
                    HexInput.Text = h;
                    _selectedColors[_activeColorSlot] = h;
                    UpdateHexUI(h);
                    _previewEntity.SkinHexColors = [.. _selectedColors];
                    RefreshPreview();
                };
                CustomColorPanel.Children.Add(swatch);
            }
        }
        #endregion

        #region "Preview"
        private void RefreshPreview()
        {
            try
            {
                var drawer = SkinDrawer.Instance.GetSkin(_previewEntity);

                const int w = 180, h = 240;
                using var bmp = new Bitmap(w, h);
                using var g = Graphics.FromImage(bmp);

                g.Clear(System.Drawing.Color.Transparent);

                var dir = _direction switch
                {
                    "N" => MageDrawer.Direction.N,
                    "E" => MageDrawer.Direction.E,
                    "W" => MageDrawer.Direction.W,
                    _ => MageDrawer.Direction.S
                };

                drawer.DrawCentered(g, new System.Drawing.Size(w, h));

                var src = BitmapToImageSource(bmp);
                CharacterPreviewHost.Background = new ImageBrush(src)
                {
                    Stretch = Stretch.Uniform
                };
            }
            catch { }
        }

        private static BitmapSource BitmapToImageSource(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    handle,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(handle);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private void RotateLeft_Click(object sender, RoutedEventArgs e)
        {
            _direction = _direction switch { "S" => "E", "E" => "N", "N" => "W", "W" => "S", _ => "S" };
            RefreshPreview();
        }

        private void RotateRight_Click(object sender, RoutedEventArgs e)
        {
            _direction = _direction switch { "S" => "W", "W" => "N", "N" => "E", "E" => "S", _ => "S" };
            RefreshPreview();
        }
        #endregion

        #region "Window Events"
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = CharacterSelection.Instance;
            CharacterSelection.Instance.Show();
            Hide();
        }

        private void RandomName_Click(object sender, RoutedEventArgs e)
        {
            var rng = new Random();
            var name = RandomNames[rng.Next(RandomNames.Length)];
            var num = rng.Next(100, 9999);
            CharacterNameInput.Text = $"{name}{num}";
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateBtn.IsEnabled = false;
            AskCharacterCreationFrame();
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e)
            => AudioMixerPanel.Toggle();
        #endregion

        #region "Raise Events"
        public void RaiseCharacterFrameMessage(CharacterFrame frame)
        {
            OnCharacterFrameMessage?.Invoke(frame);
        }
        #endregion
    }
}