using Brumak_Shared.Character.Model;
using System.Drawing;

namespace Brumak_Shared.Drawings.Skin
{
    public class SkinDrawer
    {
        public static SkinDrawer Instance { get; } = new SkinDrawer();

        private readonly Dictionary<SkinEnum, Func<ISkinDrawer>> _skinDrawerFactories = new()
        {
            { SkinEnum.Class_Mage, () => new MageDrawer()  },
            { SkinEnum.Class_Warrior, () => new WarriorDrawer() }
        };

        public ISkinDrawer GetSkin(IAbstractEntity entity)
        {
            if (entity == null)
                throw new Exception("Entity cannot be null");

            SkinEnum skin = (SkinEnum)entity.Skin;
            if (_skinDrawerFactories.TryGetValue(skin, out var factory))
                return factory().SetColors(
                    ColorTranslator.FromHtml(entity.SkinHexColors[0]),
                    ColorTranslator.FromHtml(entity.SkinHexColors[1]),
                    ColorTranslator.FromHtml(entity.SkinHexColors[2])
                );

            throw new Exception("No drawer found for the specified skin");
        }
    }
}
