using Brumak_Shared.Character.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_Shared.Classes.Static
{
    public static class Classes
    {
        private static readonly ClassInfo[] _classes = [
            new ClassInfo
            {
                Name        = "GUERRERO",
                Description = "Maestro del combate cuerpo a cuerpo. Resistente y devastador, el Guerrero lidera la batalla con fuerza bruta y armadura pesada.",
                Skin        = SkinEnum.Class_Warrior,
                Class       = SkinEnum.Class_Warrior,
                Icon        = "⚔"
            },
            new ClassInfo
            {
                Name        = "MAGO",
                Description = "Maestro de los elementos y las artes arcanas. Capaz de devastar enemigos a distancia con hechizos de gran poder.",
                Skin        = SkinEnum.Class_Mage,
                Class       = SkinEnum.Class_Mage,
                Icon        = "✨"
            },
            new ClassInfo
            {
                Name        = "ARQUERO",
                Description = "Experto en combate a distancia. El Arquero utiliza su agilidad y puntería para eliminar enemigos antes de que se acerquen.",
                Skin        = SkinEnum.Class_Archer,
                Class       = SkinEnum.Class_Archer,
                Icon        = "🏹"
            },
            new ClassInfo
            {
                Name        = "INVOCADOR",
                Description = "Invocador de criaturas. El invocador convoca bestias y espíritus para luchar a su lado en la batalla.",
                Skin        = SkinEnum.Class_Summoner,
                Class       = SkinEnum.Class_Summoner,
                Icon        = "🌀"
            },
            new ClassInfo
            {
                Name        = "ENCANTADOR",
                Description = "Maestro de los encantamientos y el apoyo mágico. El Encantador fortalece a sus aliados y debilita a sus enemigos con hechizos de apoyo.",
                Skin        = SkinEnum.Class_Enchanter,
                Class       = SkinEnum.Class_Enchanter,
                Icon        = "🔮"
            },
             new ClassInfo
            {
                Name        = "SACERDOTE",
                Description = "Sanador y protector. El Sacerdote utiliza su fe y magia sagrada para curar heridas y proteger a sus aliados en la batalla.",
                Skin        = SkinEnum.Class_Priest,
                Class       = SkinEnum.Class_Priest,
                Icon        = "⛪"
            }
        ];

        public static ClassInfo[] Get(int index = -1) => index != -1 ? [_classes[index]] : _classes;
        public static string GetIconByClassEnum(SkinEnum classEnum) => Get().FirstOrDefault(c => c.Class == classEnum)?.Icon ?? throw new Exception($"Class {classEnum} not found");
        public static string GetNameByClassEnum(SkinEnum classEnum) => Get().FirstOrDefault(c => c.Class == classEnum)?.Name ?? throw new Exception($"Class {classEnum} not found");
    }
}
