using EventDispatcher.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EventDispatcher.Concretes
{
    public class ReflectionDispatcherAdapter : IEventDispatcher
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ReflectionDispatcherAdapter> logger;

        public ReflectionDispatcherAdapter(IServiceProvider serviceProvider, ILogger<ReflectionDispatcherAdapter> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task DispatchAsync(IReadOnlyList<IEvent> events, CancellationToken cancellationToken)
        {
            foreach (var @event in events)
                await DispatchAsync(@event, cancellationToken);
        }

        public async Task DispatchAsync(IEvent @event, CancellationToken cancellationToken)
        {
            var eventType = @event.GetType();

            var handleEventMethod = typeof(ReflectionDispatcherAdapter)
                .GetMethod(nameof(HandleEventAsync), BindingFlags.Instance | BindingFlags.Public)!
                .MakeGenericMethod(eventType);

            var task = (Task)handleEventMethod.Invoke(this, new object[] { @event, cancellationToken })!;

            await task;
        }

        public async Task HandleEventAsync<T>(T @event, CancellationToken cancellationToken) where T : IEvent
        {
            var handlers = serviceProvider.GetServices<IEventHandler<T>>();

            foreach (var handler in handlers)
                await handler.HandleAsync(@event, cancellationToken);
        }
    }
}
