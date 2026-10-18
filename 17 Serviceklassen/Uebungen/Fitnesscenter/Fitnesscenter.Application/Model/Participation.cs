#pragma warning disable CS8618
namespace Fitnesscenter.Model;

/// <summary>
/// The registration of a member for a training session, optionally with a rating of the trainer.
/// </summary>
public class Participation
{
    protected Participation() { }

    public Participation(TrainingSession session, Member member, int? rating)
    {
        TrainingSession = session;
        Member = member;
        Rating = rating;
    }

    public int Id { get; set; }
    public TrainingSession TrainingSession { get; set; }
    public Member Member { get; set; }
    public int? Rating { get; set; } // 1 (bad) to 5 (excellent), null if not rated yet
}

#pragma warning restore CS8618
