using Brumak_ORM.Database;
using Brumak_ORM.Game.Cached.Controller;
using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Generic.Cache;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterExperiencesController(WorldDbContext context, IServiceProvider serviceProvider)
        : CachedController<Brumak_Shared.Character.Model.CharacterExperiences, WorldDbContext>(context, serviceProvider), IGenericController, ICacheFlusher
    {
        public string Name => "CharacterExperiencesController";

        protected override int GetEntityId(Brumak_Shared.Character.Model.CharacterExperiences entity)
            => entity.CharacterId;

        #region "Generic Controller Functions"
        public override bool Create(Brumak_Shared.Character.Model.CharacterExperiences experiencies)
            => base.Create(experiencies);

        public override bool Delete(int characterId)
            => base.Delete(characterId);

        public override void Save()
            => base.Save();
        #endregion

        #region "Custom Controller Functions"
        public Brumak_Shared.Character.Model.CharacterExperiences? GetByCharacter(int characterId)
            => GetAll().FirstOrDefault(x => x.CharacterId == characterId);

        public bool AddExperience(int characterId, long amount)
        {
            var entry = GetByCharacter(characterId);
            if (entry == null) return false;

            entry.Experience += amount;
            return Update(entry);
        }
        #endregion
    }
}