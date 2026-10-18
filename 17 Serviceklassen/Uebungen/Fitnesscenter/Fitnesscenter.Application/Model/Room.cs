#pragma warning disable CS8618
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fitnesscenter.Model;

/// <summary>
/// A room of the fitness center in which training sessions take place.
/// </summary>
public class Room
{
    public Room(string name, string type)
    {
        Name = name;
        Type = type;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } // e.g. "Cardio", "Kraft", "Yoga"
    [JsonIgnore]
    public List<TrainingSession> TrainingSessions { get; } = new();
}

#pragma warning restore CS8618
