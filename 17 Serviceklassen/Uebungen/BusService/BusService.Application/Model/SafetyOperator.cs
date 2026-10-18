using System;
using System.Collections.Generic;

namespace BusService.Application.Model;

/// <summary>
/// A person on board of an autonomous bus who takes over control if necessary.
/// The certification is valid until (and including) CertifiedUntil.
/// </summary>
public class SafetyOperator
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected SafetyOperator() { }
#pragma warning restore CS8618

    public SafetyOperator(string firstName, string lastName, DateOnly certifiedUntil)
    {
        FirstName = firstName;
        LastName = lastName;
        CertifiedUntil = certifiedUntil;
    }

    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly CertifiedUntil { get; set; }
    public List<Trip> Trips { get; } = new();
}
