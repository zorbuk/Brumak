using Brumak_ORM.Game.Generic;
using Brumak_Shared.Metrics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brumak_ORM
{
    public class Controllers
    {
        private static readonly Logger _logger = new("ORM", typeof(Controllers));
        private static readonly ConcurrentDictionary<Type, IGenericController> _controllers = new();

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

        public static void RegisterAllControllers()
        {
            var controllerDefinitions = new[]
            {
                // Placeholder
                new { ControllerType = typeof(IGenericController), ContextType = typeof(DbContext) },
            };

            var serviceProvider = Services.ServiceProvider;

            foreach (var def in controllerDefinitions.Where(cd => cd.ControllerType != typeof(IGenericController)))
            {
                var context = serviceProvider.GetRequiredService(def.ContextType);

                var constructor = def.ControllerType.GetConstructor([def.ContextType, typeof(IServiceProvider)])
                                  ?? throw new InvalidOperationException($"No suitable constructor found for {def.ControllerType}");

                var controller = constructor.Invoke([context, serviceProvider]) as IGenericController
                                 ?? throw new InvalidOperationException($"Failed to create {def.ControllerType}");

                _controllers[def.ControllerType] = controller;
                _logger.Log($"Controller registered for type {def.ControllerType}");
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
