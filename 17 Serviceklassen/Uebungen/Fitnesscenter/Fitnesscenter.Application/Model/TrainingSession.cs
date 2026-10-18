#pragma warning disable CS8618
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fitnesscenter.Model;

/// <summary>
/// A guided training of a trainer at a given time in a given room.
/// </summary>
public class TrainingSession
{
    protected TrainingSession() { }

    public TrainingSession(Trainer trainer, Room room, DateTime time, string type, int durationMinutes, int maxParticipants)
    {
        Trainer = trainer;
        Room = room;
        Time = time;
        Type = type;
        DurationMinutes = durationMinutes;
        MaxParticipants = maxParticipants;
    }

    public int Id { get; set; }
    public Trainer Trainer { get; set; }
    public Room Room { get; set; }
    public DateTime Time { get; set; }
    public string Type { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; }
    [JsonIgnore]
    public List<Participation> Participations { get; } = new();
}

#pragma warning restore CS8618
