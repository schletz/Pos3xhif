//   AP1 (0|30)
//    +-------------------+
//    |                   |
//    | C3.01             |
//    | (0|20) - (10|30)  |
//    |                   |
//    +-------------------+ AP2 (10|20)
//    |                   |
//    | C3.02             |
//    | (0|10) - (10|20)  |
//    |                   |
//    +-------------------+
//    |                   |
//    | C3.03             |
//    | (0|0) - (10|10)   |
//    |                   |
//    +-------------------+
//   AP3 (0|0)
//

using System;
using Bogus.Distributions.Gaussian;

namespace LocationDemo
{
    /// <summary>
    /// Speichert den Punkt in unserem virtuellen Stockwert und ermittelt den Raum aufgrund der
    /// Koordinaten und die erwartete Signalstärke.
    /// </summary>
    readonly struct Point
    {
        public float X { get; }
        public float Y { get; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
        public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
        // https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#positional-patterns
        public string Room => this switch
        {
            var (x, y) when x < 10 && y < 10 => "C3.03",
            var (x, y) when x < 10 && y < 20 => "C3.02",
            var (x, y) when x < 10 && y < 30 => "C3.01",
            _ => "Unknown"
        };
        public double Distance(Point desc) => Math.Sqrt((desc.X - X) * (desc.X - X) + (desc.Y - Y) * (desc.Y - Y));
        /// <summary>
        /// Berechnet die virtuelle Signalstärke.
        /// </summary>
        /// <param name="ap">Der Accesspoint, dessen Signalstärke ermittelt wird.</param>
        /// <param name="biasFunc">Funktion, dessen Wert zur berechneten Signalstärke hinzugefügt wird.</param>
        /// <returns></returns>
        public float SignalStrength(Accesspoint ap, Func<float, float> biasFunc)
        {
            float strength = (float)(100.0 * 1 / Math.Max(0.1, ap.Location.Distance(this)));
            return strength + biasFunc(strength);
        }
        public float SignalStrength(Accesspoint ap) => SignalStrength(ap, x => x);
    }
}
