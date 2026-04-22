using Brumak_ORM.Database;
using Brumak_ORM.Game.Cached.Controller;
using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Generic.Cache;

namespace Brumak_ORM.Game.Character.Controller
{
    public class CharacterController(WorldDbContext context, IServiceProvider serviceProvider)
        : CachedController<Brumak_Shared.Character.Model.Character, WorldDbContext>(context, serviceProvider), IGenericController, ICacheFlusher
    {
        public string Name => "CharacterController";

        protected override int GetEntityId(Brumak_Shared.Character.Model.Character entity)
            => entity.Id;

        #region "Generic Controller Functions"
        public override bool Create(Brumak_Shared.Character.Model.Character character)
        {
            if (string.IsNullOrWhiteSpace(character.Name))
                return false;

            if (!base.Create(character))
                return false;

            Controllers.GetCharacterExperiencesController?.Create(new Brumak_Shared.Character.Model.CharacterExperiences
            {
                CharacterId = character.Id,
                Experience = 0
            });

            Controllers.GetCharacterWorldPositionController?.Create(new Brumak_Shared.Character.Model.CharacterWorldPosition
            {
                CharacterId = character.Id,
                MapId = 1,
                PositionX = 0,
                PositionY = 0
            });

            return true;
        }

        public override bool Delete(int id)
            => base.Delete(id);

        public override void Save()
            => base.Save();
        #endregion

        #region "Custom Controller Functions"
        public List<Brumak_Shared.Character.Model.Character> GetCharacters(int accountId, int serverId)
            => [.. GetAll().Where(x => x.AccountId == accountId && x.ServerId == serverId)];
        #endregion
    }
}