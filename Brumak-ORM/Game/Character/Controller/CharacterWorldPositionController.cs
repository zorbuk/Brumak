using Brumak_ORM.Database;
using Brumak_ORM.Game.Cached.Controller;
using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Generic.Cache;
using System.Drawing;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterWorldPositionController(WorldDbContext context, IServiceProvider serviceProvider)
        : CachedController<Brumak_Shared.Character.Model.CharacterWorldPosition, WorldDbContext>(context, serviceProvider), IGenericController, ICacheFlusher
    {
        public string Name => "CharacterWorldPositionController";

        protected override int GetEntityId(Brumak_Shared.Character.Model.CharacterWorldPosition entity)
            => entity.CharacterId;

        #region "Generic Controller Functions"
        public override bool Create(Brumak_Shared.Character.Model.CharacterWorldPosition position)
            => base.Create(position);

        public override bool Delete(int characterId)
            => base.Delete(characterId);

        public override void Save()
            => base.Save();
        #endregion

        #region "Custom Controller Functions"
        public Brumak_Shared.Character.Model.CharacterWorldPosition? GetByCharacter(int characterId)
            => GetAll().FirstOrDefault(x => x.CharacterId == characterId);

        public bool UpdatePosition(int characterId, int mapId, int positionX, int positionY)
        {
            var entry = GetByCharacter(characterId);
            if (entry == null) return false;

            entry.MapId = mapId;
            entry.PositionX = positionX;
            entry.PositionY = positionY;
            entry.UpdatedAt = DateTime.UtcNow;
            return Update(entry);
        }
        #endregion
    }
}