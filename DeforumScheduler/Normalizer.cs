namespace DeforumScheduler;

public class Normalizer
{
    public Dictionary<int, double> Normalize(Dictionary<int, double> values, double lower, double upper)
    {
        double min = double.MaxValue;
        double max = double.MinValue;

        // Find the min and max values in the array
        foreach (var item in values)
        {
            if (item.Value < min) min = item.Value;
            if (item.Value > max) max = item.Value;
        }

        // Normalize the values
        var normalizedValues = new Dictionary<int, double>(values.Count);
        for (int i = 0; i < values.Count; i++)
        {
            normalizedValues[i] = Math.Round(lower + ((values[i] - min) / (max - min)) * (upper - lower), 2);
        }

        return normalizedValues;
    }
}