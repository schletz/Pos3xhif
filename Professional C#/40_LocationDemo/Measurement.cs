using System;
using System.Collections.Generic;

namespace LocationDemo
{
    /// <summary>
    /// Speichert den Messwert, der die Signalstärke aller Accesspoints im Messbereich beinhaltet.
    /// </summary>
    class Measurement
    {
        public Measurement(string device, DateTime date, Point location, IEnumerable<Signal> signals)
        {
            Device = device ?? throw new ArgumentNullException(nameof(device));
            Date = date;
            Signals = signals ?? throw new ArgumentNullException(nameof(signals));
            Location = location;
        }

        public string Device { get; }
        public DateTime Date { get; }
        public Point Location { get; }
        public IEnumerable<Signal> Signals { get; }
    }
}
