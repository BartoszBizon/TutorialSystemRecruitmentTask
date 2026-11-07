using System;

namespace ArtifexMundi.Client.Mvvm
{
    /// <summary>
    /// "RelayCommand (mvvm)" arrow: Binding -> ViewModel. View never calls ViewModel methods directly.
    /// (Not to be confused with Shared Logic ICommand - this one is a UI action.)
    /// </summary>
    public interface IRelayCommand
    {
        event Action CanExecuteChanged;
        bool CanExecute();
        void Execute();
    }

    public interface IRelayCommand<in T>
    {
        void Execute(T parameter);
    }

    public sealed class RelayCommand : IRelayCommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event Action CanExecuteChanged;

        public bool CanExecute() => _canExecute?.Invoke() ?? true;

        public void Execute()
        {
            if (CanExecute())
                _execute();
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke();
    }

    public sealed class RelayCommand<T> : IRelayCommand<T>
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public void Execute(T parameter) => _execute(parameter);
    }
}
