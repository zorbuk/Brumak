using Brumak_ORM.Database;
using Brumak_ORM.Game.Cached.Controller;
using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Generic.Cache;

namespace Brumak_ORM.Game.Server.Controller
{
    public class ServerController(AuthDbContext context, IServiceProvider serviceProvider)
        : CachedController<Brumak_Shared.Server.Model.Server, AuthDbContext>(context, serviceProvider), IGenericController, ICacheFlusher
    {
        public string Name => "ServerController";

        protected override int GetEntityId(Brumak_Shared.Server.Model.Server entity)
            => entity.Id;

        #region "Generic Controller Functions"
        public override bool Create(Brumak_Shared.Server.Model.Server server)
        {
            if (string.IsNullOrWhiteSpace(server.Name))
                return false;

            return base.Create(server);
        }

        public override bool Delete(int id)
            => base.Delete(id);

        public override void Save()
            => base.Save();
        #endregion

        #region "Custom Controller Functions"
        #endregion
    }
}