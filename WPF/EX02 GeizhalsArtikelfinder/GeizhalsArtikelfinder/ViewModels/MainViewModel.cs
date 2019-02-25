using GeizhalsArtikelfinder.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace GeizhalsArtikelfinder.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        // Todo: Erstellen der DB Instanz.
        private readonly GeizhalsDb db;
        /// <summary>
        /// Todo: Initialisiere das Command im Konstruktor und weise ihm eine Instanz von RelayCommand
        ///       zu, sodass die Methode SearchArticle aufgerufen wird.
        /// </summary>
        public ICommand ArticleSearchCommand { get; }
        /// <summary>
        /// Aufrufbeispiel, für Propertyname ist der Name des Public Properties zu verwenden: 
        ///     PropertyChanged(this, new PropertyChangedEventArgs(nameof(Propertyname)));
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Todo: Initialisieren von ArticleSearchCommand
        /// </summary>
        public MainViewModel()
        {
            
        }

        /// <summary>
        /// Property für das Binding des EAN Suchfeldes
        /// </summary>
        public string Ean { get; set; }
        /// <summary>
        /// Property für das Binding des Vondatum Suchfeldes
        /// </summary>
        public string DateTo { get; set; }
        /// <summary>
        /// Property für das Binding des Bisdatum Suchfeldes
        /// </summary>
        public string DateFrom { get; set; }
        /// <summary>
        /// Property für das Binding der Ergebnisfelder
        /// </summary>
        public Articledata CurrentArticle { get; private set; }
        /// <summary>
        /// Sucht einen mit den in den Properties Ean, DateTo und DateFrom angegebenen Suchparametern.
        /// Wenn DateFrom leer oder NULL ist, so ist dieses Suchfeld zu ignorieren.
        /// Wenn DateTo leer oder NULL ist, so ist dieses Suchfeld zu ignorieren.
        /// </summary>
        private void SearchArticle()
        {

        }
    }

    /// <summary>
    /// DTO für die Darstellung des Suchergebnisses.
    /// </summary>
    public class Articledata
    {
        public long Ean { get; set; }
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal AvgPrice { get; set; }
    }
}
