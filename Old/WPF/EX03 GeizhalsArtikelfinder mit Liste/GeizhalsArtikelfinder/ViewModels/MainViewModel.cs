using GeizhalsArtikelfinder.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;

namespace GeizhalsArtikelfinder.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GeizhalsDb db = GeizhalsDb.FromXml("Geizhals.xml");
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
        public MainViewModel()
        {
            ArticleSearchCommand = new RelayCommand(SearchArticle);
        }
        /// <summary>
        /// Property für das Binding des Vondatum Suchfeldes
        /// </summary>
        public string DateTo { get; set; }
        /// <summary>
        /// Property für das Binding des Bisdatum Suchfeldes
        /// </summary>
        public string DateFrom { get; set; }

        // Kurzform für 
        // public IEnumerable<Artikel> Artikels
        // { get {return db.Artikels; } }
        /// <summary>
        /// Binding für die Liste, liefert einfach eine Liste der Artikel.
        /// Hier können ganze LINQ Abfragen reinkommen (sortieren, filtern, ...).
        /// </summary>
        public IEnumerable<Artikel> Artikels => db.Artikels;
        private Artikel selectedArticle;
        /// <summary>
        /// "Eventhandler", wenn ein Artikel gewählt wurde.
        /// </summary>
        public Artikel SelectedArticle
        {
            get => selectedArticle;
            set
            {
                // Verhindert ein Setzen von null am Anfang (wenn selectedArticle
                // auch null ist) und wenn sich nichts geändert hat.
                if (selectedArticle != value)
                {
                    selectedArticle = value;
                    SearchArticle();
                }
            }
        }

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
            if (!DateTime.TryParse(DateFrom, out DateTime dateFrom))
            {
                dateFrom = DateTime.MinValue;
            }

            if (!DateTime.TryParse(DateTo, out DateTime dateTo))
            {
                dateTo = DateTime.Now;
            }

            long ean = SelectedArticle.Ean;
            // Frage hier die Artikeldaten mit LINQ ab und erstelle eine Instanz von Articledata.
            // Danach speichere Sie in CurrentArticle und rufe PropertyChanged auf.
            // Achte darauf, dass keine Fehler geworfen werden, wenn kein Artikel gefunden wird.
            CurrentArticle = (
                from an in db.Angebote
                where an._Artikel.Ean == ean && an.Datum >= dateFrom && an.Datum < dateTo.AddDays(1)
                group an by an._Artikel into g
                select new Articledata
                {
                    Ean = ean,
                    Name = g.Key.Name,
                    MinPrice = g.Min(a => a.Preis),
                    AvgPrice = g.Average(a => a.Preis),
                    MaxPrice = g.Max(a => a.Preis)
                }).FirstOrDefault();

            // WICHTIG: NICHT VERGESSEN
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentArticle)));

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
