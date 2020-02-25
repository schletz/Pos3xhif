using System;

namespace LocationDemo
{
    /// <summary>
    /// Speichert Informationen zu einem aufgestellten Accesspoint
    /// </summary>
    class Accesspoint
    {
        public string Mac { get; }
        public Point Location { get; }

        public Accesspoint(string mac, Point location)
        {
            Mac = mac ?? throw new ArgumentNullException(nameof(mac));
            Location = location;
        }
    }
}
