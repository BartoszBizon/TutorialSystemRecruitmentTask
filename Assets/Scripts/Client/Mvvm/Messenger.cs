using System;
using System.Collections.Generic;

namespace ArtifexMundi.Client.Mvvm
{
    /// <summary>
    /// "Message" arrow (Model -> ViewModel). Models publish, ViewModels subscribe - they never reference each other.
    /// </summary>
    public interface IMessenger
    {
        IDisposable Subscribe<TMessage>(Action<TMessage> handler);
        void Publish<TMessage>(TMessage message);
    }

    public sealed class Messenger : IMessenger
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();

        public IDisposable Subscribe<TMessage>(Action<TMessage> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            if (!_handlers.TryGetValue(typeof(TMessage), out var list))
            {
                list = new List<Delegate>();
                _handlers.Add(typeof(TMessage), list);
            }

            list.Add(handler);
            return new Subscription(() => list.Remove(handler));
        }

        public void Publish<TMessage>(TMessage message)
        {
            if (!_handlers.TryGetValue(typeof(TMessage), out var list))
                return;

            // Copy - handlers may (un)subscribe while being notified.
            foreach (var handler in list.ToArray())
                ((Action<TMessage>)handler).Invoke(message);
        }

        private sealed class Subscription : IDisposable
        {
            private Action _unsubscribe;

            public Subscription(Action unsubscribe) => _unsubscribe = unsubscribe;

            public void Dispose()
            {
                _unsubscribe?.Invoke();
                _unsubscribe = null;
            }
        }
    }
}
