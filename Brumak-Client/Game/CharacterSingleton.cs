using Brumak_Shared.Character.Model;
using System.ComponentModel;

namespace Brumak_Client.Game
{
    public class CharacterSingleton : INotifyPropertyChanged
    {
        private static CharacterSingleton _instance = new();
        public static CharacterSingleton Instance => _instance;

        private CharacterDto _character;

        public int Id => _character.Id;
        public string Name => _character.Name;
        public SkinEnum Skin => _character.Skin;
        public SkinEnum Class => _character.Class;
        public int Level => Brumak_Shared.Experience.Static.Level.GetLevelFromExperience(_character.CharacterExperiences.Experience);
        public List<string> SkinHexColors => _character.SkinHexColors;

        public void SetCharacter(CharacterDto character)
        {
            _character = character;
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SkinHexColors));
            OnPropertyChanged(nameof(Skin));
            OnPropertyChanged(nameof(Class));
            OnPropertyChanged(nameof(Level));
        }

        public void Clear()
        {
            _character = null!;
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SkinHexColors));
            OnPropertyChanged(nameof(Skin));
            OnPropertyChanged(nameof(Class));
            OnPropertyChanged(nameof(Level));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
