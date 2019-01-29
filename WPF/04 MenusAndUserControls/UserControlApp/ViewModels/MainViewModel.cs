using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UserControlApp.ViewModels
{
    /// <summary>
    /// Viewmodel für das Hauptfenster.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Festlegen des Startzustandes (zu Beginn ausgewählter Menüpunkt)
        /// </summary>
        private string activeMenuitem = "Add";

        /// <summary>
        /// Initialisiert das Command, welches beim Klicken auf das Menü aufgerufen wird.
        /// </summary>
        public MainViewModel()
        {
            ActivateMenuitem = new RelayCommand((param) => ActiveMenuitem = param?.ToString());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ActivateMenuitem { get; private set; }
        /// <summary>
        /// Aktuell ausgewählter Menüpunkt.
        /// </summary>
        public string ActiveMenuitem
        {
            get => activeMenuitem;
            private set
            {
                if (activeMenuitem != value)
                {
                    activeMenuitem = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(ActiveMenuitem)));
                }
            }
        }
    }
}
