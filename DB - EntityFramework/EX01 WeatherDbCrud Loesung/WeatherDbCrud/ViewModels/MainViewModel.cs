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

        public IEnumerable<Station> Stations
        {
            get => db.Stations.ToList();
        }

        private Station currentStation;
        public Station CurrentStation
        {
            get => currentStation;
            set { currentStation = value; PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements))); }
        }

        public IEnumerable<Measurement> Measurements
        {
            get => CurrentStation?.Measurements.ToList() ?? Enumerable.Empty<Measurement>();
        }
        public Measurement CurrentMeasurement { get; set; }
        public Measurement NewMeasurement { get; set; } = new Measurement { M_Date = GetCurrentTime() };

        public void UpdateMeasurement()
        {
            if (CurrentMeasurement == null) { return; }
            db.SaveChanges();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));
        }

        public void DeleteCurrentMeasurement()
        {
            CurrentStation.Measurements.Remove(CurrentMeasurement);
            db.SaveChanges();
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));
        }

        public void AddNewMeasurement()
        {
            CurrentStation.Measurements.Add(NewMeasurement);
            db.SaveChanges();
            NewMeasurement = new Measurement { M_Date = GetCurrentTime() };
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(NewMeasurement)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Measurements)));
        }

        private static DateTime GetCurrentTime()
        {
            DateTime time = new DateTime(DateTime.Now.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
            return time;
        }
    }
}
