using System;

namespace LocationDemo
{
    /// <summary>
    /// Speichert die gemessene Signalstärke eines Accesspoints
    /// </summary>
    class Signal
    {
        public Signal(Accesspoint accesspoint, float value)
        {
            Accesspoint = accesspoint ?? throw new ArgumentNullException(nameof(accesspoint));
            Value = value;
        }

        public Accesspoint Accesspoint { get; }
        public float Value { get; }
    }

}
