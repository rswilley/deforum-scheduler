using DeforumScheduler;

namespace TestProject1;

public class PeakDetectorTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        //var beats = subject.DetectBeats(LoadTestTrack(), 12);
    }

    private IPeakDetector GetSubject() => new PeakDetector();

    private static Dictionary<int, double> LoadTestTrack()
    {
        var lines = File.ReadAllLines(Path.Combine("TestData", "Track1", "drums-frames.csv"));

        var results = new Dictionary<int, double>();
        foreach (var line in lines)
        {
            var values = line.Split(',');
            var frame = Convert.ToInt32(values[0]);
            var energy = Convert.ToDouble(values[1]);
            
            results.Add(frame, energy);
        }

        return results;
    }
}