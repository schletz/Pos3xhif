#pragma warning disable CS8618
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Fitnesscenter.Model;

/// <summary>
/// A customer of the fitness center with a Basic or Premium membership.
/// </summary>
public class Member
{
    public Member(string firstName, string lastName, string email, string membershipType, DateTime activeSince, bool isActive)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        MembershipType = membershipType;
        ActiveSince = activeSince;
        IsActive = isActive;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MembershipType { get; set; } // "Basic" or "Premium"
    public DateTime ActiveSince { get; set; }
    public bool IsActive { get; set; }
    [JsonIgnore]
    public List<Visit> Visits { get; } = new();
    [JsonIgnore]
    public List<Participation> Participations { get; } = new();
}

#pragma warning restore CS8618
