using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DJIDrone.ViewModels
{
    /// <summary>
    /// Implémentation de base de <voir cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Exécute lorsque cette propriété est modifiée.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Liée à l'event <voir cref="PropertyChanged"/>.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string changedPropertyName = "")
        {
            this.PropertyChangedOverride(changedPropertyName);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(changedPropertyName));
        }

        /// <summary>
        /// Point d'entrée.
        /// </summary>
        protected virtual void PropertyChangedOverride(string changedPropertyName)
        {
        }
    }
}
