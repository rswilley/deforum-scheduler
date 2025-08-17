namespace DeforumScheduler;

public interface ISampleResolver
{
    Dictionary<int, Sample> ProcessFrames(Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> framesWithEnergy, int bpm, int fps);
}

public class SampleResolver : ISampleResolver
{
    public Dictionary<int, Sample> ProcessFrames(Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> framesWithEnergy, int bpm, int fps)
    {
        var samples = new Dictionary<int, Sample>();
        var peakDetector = new PeakDetector();
        var kickSamples = framesWithEnergy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.LowFreqEnergy);
        var kickPeaks = peakDetector.DetectPeaks(kickSamples);
        var drops = new DropDetector().DetectDrops(kickPeaks);
        var snarePeaks = peakDetector.DetectPeaks(framesWithEnergy.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.HighFreqEnergy));
        
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
}

public class Sample
{
    public double KickValue { get; init; }
    public double SnareValue { get; init; }
    public bool IsKickPeak { get; init; }
    public bool IsSnarePeak { get; init; }
    public bool IsDrop { get; init; }
}
