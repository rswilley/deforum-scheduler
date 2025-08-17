using DeforumScheduler;

namespace TestProject1;

public class DropDetectorTests
{
    [Fact]
    public void DetectDrops_Track1_ReturnsNineDrops()
    {
        var framesWithValues = new TestDataLoader().LoadTestTrack(Path.Combine("TestData", "Track1", "low-frequency-drums-frames.csv"));
        var kickPeaks = new PeakDetector().DetectPeaks(framesWithValues);
        
        var subject = GetSubject();
        var drops = subject.DetectDrops(kickPeaks);
        
        Assert.Equal(9, drops.Count);
        Assert.Equal(366, drops.ElementAt(0).Key);
        Assert.Equal(732, drops.ElementAt(1).Key);
        Assert.Equal(1098, drops.ElementAt(2).Key);
        Assert.Equal(1829, drops.ElementAt(3).Key);
        Assert.Equal(2606, drops.ElementAt(4).Key);
        Assert.Equal(2789, drops.ElementAt(5).Key);
        Assert.Equal(2972, drops.ElementAt(6).Key);
        Assert.Equal(3338, drops.ElementAt(7).Key);
        Assert.Equal(3703, drops.ElementAt(8).Key);
    }
    
    private DropDetector GetSubject() => new DropDetector();
}