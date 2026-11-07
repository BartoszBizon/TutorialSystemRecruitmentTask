using System;
using System.Collections.Generic;
using System.ComponentModel;
using ArtifexMundi.Client.Mvvm;
using UnityEngine.UI;

namespace ArtifexMundi.Client.Binding
{
    /// <summary>
    /// Binding layer between ViewModel and View:
    /// - ViewModel -> View: "Event (OnPropertyChanged)" -> "Set property"
    /// - View -> ViewModel: "Event" (Button.onClick) -> "RelayCommand"
    /// Disposing the set removes every subscription (call it in OnDestroy).
    /// </summary>
    public sealed class BindingSet : IDisposable
    {
        private readonly List<Action> _unbinders = new List<Action>();

        /// <summary>Calls setter now and every time the property changes.</summary>
        public BindingSet Bind<T>(INotifyPropertyChanged source, string propertyName, Func<T> getter, Action<T> setter)
        {
            PropertyChangedEventHandler handler = (_, args) =>
            {
                if (args.PropertyName == propertyName)
                    setter(getter());
            };

            source.PropertyChanged += handler;
            _unbinders.Add(() => source.PropertyChanged -= handler);
            setter(getter());
            return this;
        }

        public BindingSet BindCommand(Button button, IRelayCommand command)
        {
            void OnClick() => command.Execute();
            void OnCanExecuteChanged() => button.interactable = command.CanExecute();

            button.onClick.AddListener(OnClick);
            command.CanExecuteChanged += OnCanExecuteChanged;
            _unbinders.Add(() =>
            {
                button.onClick.RemoveListener(OnClick);
                command.CanExecuteChanged -= OnCanExecuteChanged;
            });
            OnCanExecuteChanged();
            return this;
        }

        public BindingSet BindEvent<T>(Action<Action<T>> subscribe, Action<Action<T>> unsubscribe, IRelayCommand<T> command)
        {
            Action<T> handler = command.Execute;
            subscribe(handler);
            _unbinders.Add(() => unsubscribe(handler));
            return this;
        }

        public void Dispose()
        {
            foreach (var unbind in _unbinders)
                unbind();
            _unbinders.Clear();
        }
    }
}
