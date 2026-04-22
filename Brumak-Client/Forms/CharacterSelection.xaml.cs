using Brumak_Client.Audio;
using Brumak_Client.Network;
using Brumak_Shared.Character.Model;
using Brumak_Shared.Classes.Static;
using Brumak_Shared.Drawings.Entity;
using Brumak_Shared.Drawings.Skin;
using Brumak_Shared.Experience.Static;
using Brumak_Shared.Network.Frames.Characters;
using Brumak_Shared.Server.Model;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Brumak_Client.Forms
{
    public partial class CharacterSelection : Window
    {
        public static CharacterSelection Instance { get; set; } = null!;

        public ServerInfo CurrentServer;
        private CharacterDto[] _characters = [];
        private int _selectedIndex = -1;
        private readonly List<Border> _characterBorders = [];
        private bool _isGoingBack = false;

        private readonly PreviewEntity _previewEntity = new();
        private string _direction = "S";

        public event Action<CharacterFrame>? OnCharacterFrameMessage;

        #region "Constructor"
        public CharacterSelection(ServerInfo server)
        {
            InitializeComponent();
            Instance = this;
            CurrentServer = server;
            DataContext = AccountSingleton.Instance;

            ServerNameLabel.Text = CurrentServer.Name.ToUpper();

            ServerSplashArt.Source = new BitmapImage(
                new Uri(CurrentServer.SplashArt, UriKind.Relative));

            Closed += (_, _) => Instance = null!;

            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler((sender, _) =>
            {
                if (sender is FrameworkElement el && el.Tag is "silent") return;
                AudioManager.Instance.UI.Play(@"/Assets/Audio/Ui/Click.ogg");
            }));

            OnCharacterFrameMessage += frame =>
            {
                Dispatcher.Invoke(() =>
                {
                    switch (frame)
                    {
                        case GetCharactersSuccessFrame getCharacters:
                            _characters = [.. getCharacters.Characters.Select(x => new CharacterDto(new Character {
                                Id = x.Id,
                                Name = x.Name,
                                Skin = x.Skin,
                                Class = x.Class,
                                SkinHexColors = x.SkinHexColors,
                                CreatedAt = x.CreatedAt,
                                AccountId = AccountSingleton.Instance.Id,
                                ServerId = CurrentServer.Id
                            }, x.CharacterExperiences))];

                            string? previousSelection = _selectedIndex >= 0 && _selectedIndex < _characters.Length
                                ? _characters[_selectedIndex].Name
                                : null;

                            RebuildCharacterList();

                            int restoredIndex = previousSelection != null
                                ? Array.FindIndex(_characters, c => c.Name == previousSelection)
                                : -1;

                            if (restoredIndex >= 0)
                                SelectCharacter(restoredIndex);
                            else
                                ClearSelection();
                            break;

                        case CreateCharacterSuccessFrame createCharacter:
                            var newCharacter = new Character
                            {
                                Id = createCharacter.Character.Id,
                                Name = createCharacter.Character.Name,
                                Skin = createCharacter.Character.Skin,
                                Class = createCharacter.Character.Class,
                                SkinHexColors = createCharacter.Character.SkinHexColors,
                                CreatedAt = createCharacter.Character.CreatedAt,
                                AccountId = AccountSingleton.Instance.Id,
                                ServerId = CurrentServer.Id,
                            };

                            _characters = [.. _characters, new CharacterDto(newCharacter, createCharacter.Character.CharacterExperiences)];

                            RebuildCharacterList();

                            int newIndex = Array.FindIndex(_characters, c => c.Id == newCharacter.Id);
                            if (newIndex >= 0)
                                SelectCharacter(newIndex);
                            break;

                        case DeleteCharacterSuccessFrame deleteCharacter:
                            int deletedIndex = Array.FindIndex(_characters, c => c.Id == deleteCharacter.CharacterId);

                            _characters = [.. _characters.Where(c => c.Id != deleteCharacter.CharacterId)];

                            RebuildCharacterList();

                            if (_characters.Length == 0)
                                ClearSelection();
                            else
                            {
                                int nextIndex = Math.Min(deletedIndex, _characters.Length - 1);
                                SelectCharacter(nextIndex);
                            }
                            break;

                        case CharacterErrorFrame error:
                            MessageBox.Show(error.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;
                    }
                });
            };
        }
        #endregion

        #region "Ask Frames"
        public void AskCharactersFrame()
        {
            NetworkManager.WorldClientManager.Send(new GetCharactersFrame() { AccountId = AccountSingleton.Instance.Id , ServerId = CurrentServer.Id });
        }
        public void AskDeleteCharacterFrame()
        {
            NetworkManager.WorldClientManager.Send(new DeleteCharacterFrame() { CharacterId = _characters[_selectedIndex].Id });
        }
        #endregion

        #region "Character List"
        private void RebuildCharacterList()
        {
            CharacterListPanel.Children.Clear();
            _characterBorders.Clear();

            for (int i = 0; i < _characters.Length; i++)
            {
                int index = i;
                var ch = _characters[i];

                var avatar = new Border
                {
                    Width = 34,
                    Height = 34,
                    CornerRadius = new CornerRadius(3),
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x33, 0xFF, 0xAA, 0x44)),
                    BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x44, 0xFF, 0xAA, 0x44)),
                    BorderThickness = new Thickness(1),
                    Child = new TextBlock
                    {
                        Text = Classes.GetIconByClassEnum(ch.Class),
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xAA, 0x44)),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                var nameText = new TextBlock
                {
                    Text = ch.Name,
                    FontSize = 13,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xFF, 0xFF)),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    VerticalAlignment = VerticalAlignment.Center
                };

                var classBadge = new Border
                {
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
                    BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x22, 0xFF, 0xFF, 0xFF)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2),
                    Padding = new Thickness(5, 2, 5, 2),
                    VerticalAlignment = VerticalAlignment.Center,
                    Child = new TextBlock
                    {
                        Text = Classes.GetNameByClassEnum(ch.Class),
                        FontSize = 10,
                        Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF))
                    }
                };

                var levelText = new TextBlock
                {
                    Text = Level.GetLevelFromExperience(ch.CharacterExperiences.Experience).ToString(),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xAA, 0x44)),
                    FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 36,
                    TextAlignment = TextAlignment.Right
                };

                var infoStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
                infoStack.Children.Add(nameText);
                infoStack.Children.Add(classBadge);

                var rowGrid = new Grid();
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                rowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                Grid.SetColumn(avatar, 0);
                Grid.SetColumn(infoStack, 1);
                Grid.SetColumn(levelText, 2);
                rowGrid.Children.Add(avatar);
                rowGrid.Children.Add(infoStack);
                rowGrid.Children.Add(levelText);

                avatar.Margin = new Thickness(0, 0, 10, 0);
                levelText.Margin = new Thickness(10, 0, 0, 0);

                var border = new Border
                {
                    Style = (Style)FindResource("CharacterRow"),
                    Child = rowGrid
                };

                border.MouseLeftButtonDown += (_, _) => SelectCharacter(index);
                _characterBorders.Add(border);
                CharacterListPanel.Children.Add(border);
            }
        }

        private void SelectCharacter(int index)
        {
            if (index < 0 || index >= _characters.Length) return;

            _selectedIndex = index;
            var ch = _characters[index];

            SelectedCharacterName.Text = ch.Name;
            SelectedCharacterClass.Text = Classes.GetNameByClassEnum(ch.Class);
            SelectedCharacterLevel.Text = Level.GetLevelFromExperience(ch.CharacterExperiences.Experience).ToString();
            _previewEntity.Skin = (int)ch.Skin;
            _previewEntity.SkinHexColors = ch.SkinHexColors.ToArray();

            PlayBtn.IsEnabled = true;
            PlayBtn.Opacity = 1.0;
            DeleteCharacterBtn.IsEnabled = true;

            for (int i = 0; i < _characterBorders.Count; i++)
                _characterBorders[i].Style = (Style)FindResource(
                    i == index ? "CharacterRowSelected" : "CharacterRow");

            RefreshPreview();
        }

        private void ClearSelection()
        {
            _selectedIndex = -1;
            SelectedCharacterName.Text = "";
            SelectedCharacterClass.Text = "";
            SelectedCharacterLevel.Text = "";
            PlayBtn.IsEnabled = false;
            PlayBtn.Opacity = 0.4;
            DeleteCharacterBtn.IsEnabled = false;

            foreach (var b in _characterBorders)
                b.Style = (Style)FindResource("CharacterRow");

            RefreshPreview();
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
        #endregion

        #region "Window Events"
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _isGoingBack = true;
            Application.Current.MainWindow = ServerSelection.Instance;
            if (NetworkManager.AuthClientManager == null)
            {
                ClientFrameDispatcher.Initialize();
                _ = Task.Run(MainWindow.Instance.InitializeAuthNetworkAsync);
            }
            ServerSelection.Instance.ServerListPanel.Children.Clear();
            ServerSelection.Instance.SelectedServer.Visibility = Visibility.Hidden;
            ServerSelection.Instance.AskServersFrame();
            ServerSelection.Instance.Show();
            Close();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (_characters.Length >= 5)
            {
                MessageBox.Show("Has alcanzado el límite de 5 personajes.", "Límite alcanzado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CharacterCreation.Instance = new();
            Application.Current.MainWindow = CharacterCreation.Instance;
            CharacterCreation.Instance.Show();
            Hide();
        }

        private void DeleteCharacter_Click(object sender, RoutedEventArgs e)
        {
            AskDeleteCharacterFrame();
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e)
            => AudioMixerPanel.Toggle();

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (!_isGoingBack)
                Application.Current.Shutdown();
        }
        #endregion

        #region "Raise Events"
        public void RaiseCharacterFrameMessage(CharacterFrame frame)
            => OnCharacterFrameMessage?.Invoke(frame);
        #endregion
    }
}