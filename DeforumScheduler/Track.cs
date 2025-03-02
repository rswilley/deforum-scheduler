namespace DeforumScheduler;

public class Track
{
    public int DurationInSeconds { get; private set; }
    public List<SectionModel> Sections { get; } = [];
    
    public Track(int fps)
    {
        var sections = GetSections();
        var startTime = GetTime(sections.Single(s => s.Type == SectionType.Intro));
        
        DurationInSeconds = (int)(GetTime(sections.Single(s => s.Type == SectionType.End)) -
                                  startTime).TotalSeconds;


        var delta = startTime - TimeSpan.Zero;
        
        for (var i = 0; i < sections.Count - 1; i++)
        {
            var ts = GetTime(sections[i]) - delta;
            var nextTs = GetTime(sections[i + 1]) - delta;
            
            if (i == 0)
            {
                Sections.Add(new SectionModel
                {
                    StartFrame = 0,
                    EndFrame = (int)nextTs.TotalSeconds * fps,
                    Type = sections[i].Type
                });
            }
            else
            {
                Sections.Add(new SectionModel
                {
                    StartFrame = (int)ts.TotalSeconds * fps,
                    EndFrame = (int)nextTs.TotalSeconds * fps,
                    Type = sections[i].Type
                });
            }
        }
    }
    
    private TimeSpan GetTime(Section section)
    {
        return TimeSpan.Parse(section.Timestamp);
    }
    
    private List<Section> GetSections()
    {
        var sections = new List<Section>
        {
            new()
            {
                Timestamp = "00:41:14",
                Type = SectionType.Intro
            },
            new()
            {
                Timestamp = "00:41:44",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:41:59",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:42:40",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:42:44",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:43:14",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:44:14",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:45:14",
                Type = SectionType.NewTrack
            },
            new()
            {
                Timestamp = "00:46:41",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:47:00",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:49:00",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:49:37",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:50:37",
                Type = SectionType.NewTrack
            },
            new()
            {
                Timestamp = "00:51:07",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:51:37",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:53:07",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:54:37",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:55:07",
                Type = SectionType.NewTrack
            },
            new()
            {
                Timestamp = "00:55:37",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:56:07",
                Type = SectionType.Chorus
            },
            new()
            {
                Timestamp = "00:57:07",
                Type = SectionType.Breakdown
            },
            new()
            {
                Timestamp = "00:58:07",
                Type = SectionType.NewTrack
            },
            new()
            {
                Timestamp = "01:00:36",
                Type = SectionType.End
            }
        };
        return sections;
    }
}

public class SectionModel
{
    public int StartFrame { get; set; }
    public int EndFrame { get; set; }
    public SectionType Type { get; set; }
}