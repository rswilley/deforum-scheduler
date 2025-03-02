namespace DeforumScheduler;

public interface IKickDrumDetector
{
    IEnumerable<Frame> ConvertTimeToFrames(IEnumerable<Energy> energy, int fps);
}

public class KickDrumDetector : IKickDrumDetector
{
    public IEnumerable<Frame> ConvertTimeToFrames(IEnumerable<Energy> energy, int fps)
    {
        var energyList = energy.ToList();
        var totalTime = energyList.Last().TimeInSeconds;
        var beatsPerSecond = 128 / 60;
        
        var kicks = new List<Frame>();
        
        for (double i = 0; i < totalTime; i++)
        {
            var minRange = i;
            var maxRange = i + 1;
            
            var energyInRange = energyList.Where(e => e.TimeInSeconds >= minRange && e.TimeInSeconds <= maxRange).ToList();
            var kicksInRange = GetAdaptiveRangeThreshold(energyInRange);

            //kicks.AddRange(kicksInRange.OrderBy(k => k.TimeInSeconds));
        }

        return kicks;
    }

    private static IEnumerable<Energy> GetAdaptiveRangeThreshold(IReadOnlyCollection<Energy> energy)
    {
        double minValue = energy.Select(e => e.Value).Min();
        double maxValue = energy.Select(e => e.Value).Max();
        double threshold = minValue + (maxValue - minValue) * 0.7; // Top 30% range

        return energy.Where(e => e.Value >= threshold).ToList();
    }
}

public class Frame
{
    public int Index { get; init; }
    public TimeSpan Timestamp { get; init; }
    public double Value { get; init; }
}