using DeforumScheduler;

namespace TestProject1;

public class KickDrumDetectorTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        var kicks = subject.ConvertTimeToFrames(LoadTestTrack(), 12);
    }

    private IKickDrumDetector GetSubject() => new KickDrumDetector();

    private static List<Energy> LoadTestTrack()
    {
        var lines = File.ReadAllLines(Path.Combine("TestData", "Track1.csv"));

        var results = new List<Energy>();
        foreach (var line in lines)
        {
            var values = line.Split(',');
            var seconds = Convert.ToDouble(values[0]);
            var energy = Convert.ToDouble(values[1]);
            
            results.Add(new Energy
            {
                TimeInSeconds = seconds,
                Value = energy
            });
        }

        return results;
    }
}