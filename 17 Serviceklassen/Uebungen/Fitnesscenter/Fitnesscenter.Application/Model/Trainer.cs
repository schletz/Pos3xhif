#pragma warning disable CS8618
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fitnesscenter.Model;

/// <summary>
/// A trainer who offers training sessions in his or her specialization.
/// </summary>
public class Trainer
{
    public Trainer(string firstName, string lastName, string specialization, bool isExternal)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        IsExternal = isExternal;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public bool IsExternal { get; set; }
    [JsonIgnore]
    public List<TrainingSession> TrainingSessions { get; } = new();
}

#pragma warning restore CS8618
