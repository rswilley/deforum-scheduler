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
            var kicksInRange = energyInRange.OrderByDescending(e => e.Value).Take(beatsPerSecond);

            kicks.AddRange(kicksInRange.OrderBy(k => k.TimeInSeconds).Select(k => new Frame
            {
                FrameIndex = (int)(k.TimeInSeconds * fps),
                Timestamp = TimeSpan.FromSeconds(k.TimeInSeconds),
                Value = k.Value
            }));
        }

        //TODO: 1) if two kicks side by side (frame 11, 12) take the higher
        //      2) dedupe frames
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
    public double FrameIndex { get; init; }
    public TimeSpan Timestamp { get; init; }
    public double Value { get; init; }
}