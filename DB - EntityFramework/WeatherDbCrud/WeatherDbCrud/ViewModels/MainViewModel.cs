using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WeatherDbCrud.Model;

namespace WeatherDbCrud.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly WeatherDb db = new WeatherDb();

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<Station> Stations => db.Stations.ToList();
        private Station currentStation;
        public Station CurrentStation
        {
            get => currentStation;
            set { currentStation = value; PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentStation))); }
        }
        public Measurement CurrentMeasurement { get; set; }
        public Measurement NewMeasurement { get; set; } = new Measurement { M_Date = DateTime.Now };

        public void SaveData()
        {
            try { db.SaveChanges(); } catch { }

        }

        public void DeleteCurrentMeasurement()
        {
            if (CurrentMeasurement == null) { return; }
            db.Measurements.Remove(CurrentMeasurement);
            try { db.SaveChanges(); } catch { }
        }

        public void AddNewMeasurement()
        {
            CurrentStation?.Measurements.Add(NewMeasurement);
            try { db.SaveChanges(); } catch { }
            NewMeasurement = new Measurement { M_Date = DateTime.Now };
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(NewMeasurement)));
        }
    }
}
