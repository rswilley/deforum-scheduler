namespace DeforumScheduler;

public class PeakDetector
{
    public Dictionary<int, double> DetectPeaks(Dictionary<int, double> frames)
    {
        var normalizedKicks = new Normalizer().Normalize(frames, 0, 1);
        var peaks = new Dictionary<int, double>();

        for (var i = 0; i < frames.Count - 1; i++)
        {
            var currentValue = normalizedKicks[i];
            var previousValue = 0.0;
            if (i != 0)
            {
                previousValue = normalizedKicks[i - 1];
            }
            var nextValue = normalizedKicks[i + 1];
            
            if (currentValue > previousValue && currentValue > nextValue)
            {
                peaks.Add(i, normalizedKicks[i]);
            }
        }
        
        return peaks;
    }
}