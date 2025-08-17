namespace DeforumScheduler;

public class DropDetector
{
    public Dictionary<int, double> DetectDrops(Dictionary<int, double> kickPeaks)
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