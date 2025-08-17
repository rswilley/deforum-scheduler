namespace DeforumScheduler;

public interface IPeakDetector
{
    Dictionary<int, Sample> DetectPeaks(Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> framesWithEnergy, int bpm, int fps);
}

public class PeakDetector : IPeakDetector
{
    public Dictionary<int, Sample> DetectPeaks(Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> framesWithEnergy, int bpm, int fps)
    {
        var samples = new Dictionary<int, Sample>();
        var kickSamples = framesWithEnergy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.LowFreqEnergy);
        var kickPeaks = DetectPeaks(kickSamples);
        var drops = DetectDrops(kickPeaks);
        var snarePeaks = DetectPeaks(framesWithEnergy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HighFreqEnergy));
        
        for (int i = 0; i < framesWithEnergy.Count; i += fps)
        {
            var minFrame = i;
            var maxFrame = i + (fps - 1);
            
            var framesInRange = framesWithEnergy.Where(e => e.Key >= minFrame && e.Key <= maxFrame).ToList();
            foreach (var frame in framesInRange)
            {
                var lowFreqEnergy = framesWithEnergy[frame.Key].LowFreqEnergy;
                var highFreqEnergy = framesWithEnergy[frame.Key].HighFreqEnergy;
                
                samples.Add(frame.Key, new Sample
                {
                    KickValue = lowFreqEnergy,
                    SnareValue = highFreqEnergy,
                    IsKickPeak = kickPeaks.ContainsKey(frame.Key),
                    IsSnarePeak = snarePeaks.ContainsKey(frame.Key),
                    IsDrop = drops.ContainsKey(frame.Key)
                });
            }
        }
        
        return samples;
    }
    
    private static Dictionary<int, double> DetectPeaks(Dictionary<int, double> frames)
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

    private static Dictionary<int, double> DetectDrops(Dictionary<int, double> kickPeaks)
    {
        var drops = new Dictionary<int, double>();

        var iteration = 0;
        var currentValue = 0.0;
        
        foreach (var peak in kickPeaks)
        {
            var previousValue = iteration == 0 
                ? 0.0 
                : currentValue;

            currentValue = peak.Value;

            if (previousValue >= 0.05 && currentValue >= previousValue * 3)
            {
                drops.Add(peak.Key, peak.Value);
            }
            
            ++iteration;
        }

        return drops;
    }
}

public class Sample
{
    public double KickValue { get; init; }
    public double SnareValue { get; init; }
    public bool IsKickPeak { get; init; }
    public bool IsSnarePeak { get; init; }
    public bool IsDrop { get; init; }
}
