using DeforumScheduler;

namespace TestProject1;

public class SampleResolverTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        var framesWithValues = new TestDataLoader().LoadTestTrack(Path.Combine("TestData", "Track1", "drums-frames.csv"));
        //var beats = subject.DetectBeats(LoadTestTrack(), 12);
    }

    private ISampleResolver GetSubject() => new SampleResolver();
}