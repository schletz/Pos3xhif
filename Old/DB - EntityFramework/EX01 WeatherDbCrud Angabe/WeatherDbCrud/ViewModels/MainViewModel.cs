using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WeatherDbCrud.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Liefert die aktuelle Systemzeit auf Sekunden genau. Verwende diese Methode, damit die Zeit
        /// in die Datenbank geschrieben werden kann. Bei der Verwendung von DateTime.Now werden die
        /// Nachkommastellen abgeschnitten und der Datensatz wird nicht mehr gefunden.
        /// </summary>
        /// <returns></returns>
        private static DateTime GetCurrentTime()
        {
            DateTime time = new DateTime(DateTime.Now.Ticks / TimeSpan.TicksPerSecond * TimeSpan.TicksPerSecond);
            return time;
        }
    }
}
