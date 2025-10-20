namespace EventDispatcher.Interfaces
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IReadOnlyList<IEvent> events, CancellationToken cancellationToken);
        Task DispatchAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
