namespace DeforumScheduler;

public interface IBeatDetector
{
    Dictionary<int, Beat> DetectBeats(Dictionary<int, double> frames, int fps);
}

public class BeatDetector : IBeatDetector
{
    public Dictionary<int, Beat> DetectBeats(Dictionary<int, double> frames, int fps)
    {
        var beats = new Dictionary<int, Beat>();
        var peaks = DetectPeaks(frames);
        
        for (double i = 0; i < frames.Count; i += fps)
        {
            var minFrame = i;
            var maxFrame = i + (fps - 1);
            
            var framesInRange = frames.Where(e => e.Key >= minFrame && e.Key <= maxFrame).ToList();
            foreach (var frame in framesInRange)
            {
                beats.Add(frame.Key, new Beat
                {
                    IsPeak = peaks.ContainsKey(frame.Key),
                    Value = frame.Value
                });
            }
        }
        
        return beats;
    }
    
    private static Dictionary<int, double> DetectPeaks(IReadOnlyDictionary<int, double> frames)
    {
        var peaks = new Dictionary<int, double>();

        for (var i = 0; i < frames.Count - 1; i++)
        {
            var currentValue = frames[i];
            var previousValue = 0.0;
            if (i != 0)
            {
                previousValue = frames[i - 1];
            }
            var nextValue = frames[i + 1];
            
            if (currentValue > previousValue && currentValue > nextValue)
            {
                peaks.Add(i, frames[i]);
            }
        }
        
        return peaks;
    }
}

public class Beat
{
    public double Value { get; init; }
    public bool IsPeak { get; init; }
}