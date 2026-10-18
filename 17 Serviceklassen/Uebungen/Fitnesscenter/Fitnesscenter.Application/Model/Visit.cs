#pragma warning disable CS8618
using System;

namespace Fitnesscenter.Model;

/// <summary>
/// A visit of a member. The visit starts at the check-in and ends when the member leaves.
/// </summary>
public class Visit
{
    protected Visit() { }

    public Visit(Member member, DateTime start, DateTime? end)
    {
        Member = member;
        Start = start;
        End = end;
    }

    public int Id { get; set; }
    public Member Member { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; } // null while the member is still in the fitness center
}

#pragma warning restore CS8618
