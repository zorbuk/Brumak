using Brumak_ORM.Database;
using Brumak_ORM.Game.Account.Controller;
using Brumak_ORM.Game.Character.Controller;
using Brumak_ORM.Game.Generic;
using Brumak_ORM.Game.Server.Controller;
using Brumak_Shared.Metrics;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Brumak_ORM
{
    public class Controllers
    {
        private static readonly Logger _logger = new("ORM", typeof(Controllers), showLogs: true, saveLogs: false);
        private static readonly ConcurrentDictionary<Type, IGenericController> _controllers = new();

        #region "Controllers"
        public static AccountController? GetAccountController
        {
            get => Get<AccountController>();
        }
        public static ServerController? GetServerController
        {
            get => Get<ServerController>();
        }
        public static CharacterController? GetCharacterController
        {
            get => Get<CharacterController>();
        }
        public static CharacterExperiencesController? GetCharacterExperiencesController
        {
            get => Get<CharacterExperiencesController>();
        }
        public static CharacterWorldPositionController? GetCharacterWorldPositionController
        {
            get => Get<CharacterWorldPositionController>();
        }
        #endregion

        public static void Register(IGenericController controller)
        {
            ArgumentNullException.ThrowIfNull(controller);
            _controllers[controller.GetType()] = controller;
            _logger.Log("Controller registered for type " + controller.GetType());
        }

        public static void Register<TController>(TController controller)
        where TController : IGenericController
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            _controllers[typeof(TController)] = controller;
        }

        public static void RegisterAllControllers(Type type)
        {
            var serviceProvider = Services.ServiceProvider;

            var definitions = new[]
            {
                new { ControllerType = typeof(AccountController), ContextType = (Type?)typeof(AuthDbContext) },
                new { ControllerType = typeof(ServerController), ContextType = (Type?)typeof(AuthDbContext) },
                new { ControllerType = typeof(CharacterController), ContextType = (Type?)typeof(WorldDbContext) },
                new { ControllerType = typeof(CharacterExperiencesController), ContextType = (Type?)typeof(WorldDbContext) },
                new { ControllerType = typeof(CharacterWorldPositionController), ContextType = (Type?)typeof(WorldDbContext) },

            };

            foreach (var def in definitions.Where(t => t.ContextType == type))
            {
                IGenericController controller;

                if (def.ContextType is not null)
                {
                    var constructor = def.ControllerType.GetConstructor([def.ContextType, typeof(IServiceProvider)])
                                      ?? throw new InvalidOperationException($"No constructor ({def.ContextType.Name}, IServiceProvider) found for {def.ControllerType}");

                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService(def.ContextType);

                    controller = constructor.Invoke([context, serviceProvider]) as IGenericController
                                 ?? throw new InvalidOperationException($"Failed to create {def.ControllerType}");
                }
                else
                {
                    var constructor = def.ControllerType.GetConstructor([typeof(IServiceProvider)])
                                      ?? throw new InvalidOperationException($"No constructor (IServiceProvider) found for {def.ControllerType}");

                    controller = constructor.Invoke([serviceProvider]) as IGenericController
                                 ?? throw new InvalidOperationException($"Failed to create {def.ControllerType}");
                }

                _controllers[def.ControllerType] = controller;
                _logger.Log($"Controller registered: {def.ControllerType.Name}");
            }
        }

        public static T Get<T>() where T : class, IGenericController
        {
            if (_controllers.TryGetValue(typeof(T), out var controller))
            {
                return (T)controller;
            }

            throw new InvalidOperationException($"No controller of type {typeof(T)} is registered.");
        }
    }
}
