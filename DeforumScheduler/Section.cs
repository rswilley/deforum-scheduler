namespace DeforumScheduler;

public class Section
{
    public required string Timestamp { get; init; }
    public SectionType Type { get; init; }
}

public enum SectionType
{
    Intro,
    NewTrack,
    Chorus,
    Breakdown,
    End
}