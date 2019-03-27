using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WeatherDbCrud.Model;

namespace WeatherDbCrud.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Station> Stations
        {
            get
            {
                using (WeatherDb db = new WeatherDb())
                {
                    return db.Stations.ToList();
                }
            }
        }

        private Station currentStation;
        public Station CurrentStation
        {
            get => currentStation;
            set { currentStation = value; PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements))); }
        }

        public IEnumerable<Measurement> Measurements
        {
            get
            {
                if (CurrentStation == null) { return Enumerable.Empty<Measurement>(); }
                using (WeatherDb db = new WeatherDb())
                {
                    db.Stations.Attach(CurrentStation);
                    return CurrentStation.Measurements.ToList();
                }
            }
        }
        public Measurement CurrentMeasurement { get; set; }
        public Measurement NewMeasurement { get; set; } = new Measurement { M_Date = DateTime.Now };

        public void UpdateMeasurement()
        {
            using (WeatherDb db = new WeatherDb())
            {
                db.Measurements.Attach(CurrentMeasurement);
                db.Entry(CurrentMeasurement).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));
        }

        public void DeleteCurrentMeasurement()
        {
            using (WeatherDb db = new WeatherDb())
            {
                db.Measurements.Attach(CurrentMeasurement);
                db.Entry(CurrentMeasurement).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));

        }

        public void AddNewMeasurement()
        {
            using (WeatherDb db = new WeatherDb())
            {
                // Ohne das Anhängen würde der Fremdschlüssel in NewMeasurement nicht korrekt gesetzt
                // werden (M_Station ist dann 0)
                db.Stations.Attach(CurrentStation);
                CurrentStation.Measurements.Add(NewMeasurement);
                db.SaveChanges();
            }
            NewMeasurement = new Measurement { M_Date = DateTime.Now };
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(NewMeasurement)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));
        }
    }
}
