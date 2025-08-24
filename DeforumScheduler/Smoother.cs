namespace DeforumScheduler;

public class Smoother
{
    public Dictionary<int, double> SmoothValues(Dictionary<int, double> framesWithValues, int windowSize)
    {
        var smoothedValues = new Dictionary<int, double>(framesWithValues.Values.Count);
        for (int i = 0; i < framesWithValues.Values.Count; i++)
        {
            // Determine the window range for smoothing (avoid out-of-bound indices)
            int start = Math.Max(i - windowSize / 2, 0);
            int end = Math.Min(i + windowSize / 2, framesWithValues.Values.Count - 1);

            // Get the subarray within the window range
            double[] window = framesWithValues.Values.Skip(start).Take(end - start + 1).ToArray();
            
            // Calculate the average for this window
            smoothedValues.Add(i, window.Average());
        }
        return smoothedValues;
    }
}