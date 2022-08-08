using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace DJIDrone.Commandes
{
    /// <summary>
    /// La valeur par défaut est 'true'.
    /// </summary>
    public class Relais : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Appelé quand RaiseCanExecuteChanged est appelé
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Crée une commande qui peut s'exécuter
        /// </summary>
        /// <param name="execute">La logique d'exécution.</param>
        public Relais(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Crée une nouvelle commande
        /// </summary>
        /// <param name="execute">La logique d'exécution.</param>
        /// <param name="canExecute">Statut.</param>
        public Relais(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }
        public void Execute(object parameter)
        {
            _execute();
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
