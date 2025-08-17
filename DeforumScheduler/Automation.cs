using System.Text;

namespace DeforumScheduler;

public class StrengthAutomation
{
    public void AddValue(int frame, double value)
    {
        Values.Add(frame, value);
    }
    
    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var value in Values)
        {
            result.AppendLine($"{value.Key}:({value.Value}),");
        }

        return result.ToString().TrimEnd(',');
    }
    
    private Dictionary<int, double> Values { get; } = new();
}

public class CameraAutomation
{
    private readonly int _bpm;
    private readonly int _fps;
    private readonly double _minValue;
    private readonly double _maxValue;
    private readonly int _automationChangeInBars;
    private readonly Dictionary<int, Sample> _beats;

    public CameraAutomation(int bpm, int fps, double minValue, double maxValue, int automationChangeInBars, Dictionary<int, Sample> beats)
    {
        _bpm = bpm;
        _fps = fps;
        _minValue = minValue;
        _maxValue = maxValue;
        _automationChangeInBars = automationChangeInBars;
        _beats = beats;
    }
    
    public void AddValue(int frame, double value)
    {
        var roundedValue = Math.Round(value, 2);
        Values.Add(frame, roundedValue);
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        var normalizedValues = Normalize(Values, _minValue, _maxValue);
        var smoothValues = SmoothValues(normalizedValues, 5);
        
        var initialAutomationChangeFrame = FrameResolver.GetFrameNumberForBars(_bpm, _automationChangeInBars, _fps);
        var automationChangeFrame = initialAutomationChangeFrame;
        var movementType = GetRandomMovementType();

        var resultValues = new Dictionary<int, double>();
        for (int frame = 0; frame < smoothValues.Count; frame++)
        {
            var movementValue = smoothValues[frame];
            if (frame > automationChangeFrame)
            {
                automationChangeFrame += initialAutomationChangeFrame;
                movementType = GetNextMovementType(movementType);
            }
            
            switch (movementType)
            {
                case MovementType.Positive:
                    movementValue *= 1;
                    break;
                case MovementType.Negative:
                    movementValue *= -1;
                    break;
                case MovementType.None:
                    movementValue = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
    
            resultValues.Add(frame, movementValue);
        }

        foreach (var value in resultValues)
        {
            result.AppendLine($"{value.Key}:({value.Value}),");
        }

        return result.ToString();
    }

    public string ToCsv()
    {
        var result = new StringBuilder();
        var normalizedValues = Normalize(Values, _minValue, _maxValue);
        
        foreach (var value in normalizedValues)
        {
            result.AppendLine($"{value.Key},{value.Value}");
        }
        return result.ToString();
    }
    
    private Dictionary<int, double> Values { get; } = new();
    private Random Random { get; } = new();
    
    private static Dictionary<int, double> Normalize(Dictionary<int, double> values, double lower, double upper)
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
    
    private static Dictionary<int, double> SmoothValues(Dictionary<int, double> framesWithValues, int windowSize)
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

    private MovementType GetRandomMovementType()
    {
        if (Random.NextDouble() > 0.1) // 90% chance of movement
        {
            var randomValue = Random.Next(0, 2);
            return randomValue switch
            {
                0 => MovementType.Negative,
                1 => MovementType.Positive,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        else
        {
            var randomValue = Random.Next(0, 3);
            return randomValue switch
            {
                0 => MovementType.Negative,
                1 => MovementType.Positive,
                2 => MovementType.None,
                _ => throw new ArgumentOutOfRangeException()
            };   
        }
    }
    
    private MovementType GetNextMovementType(MovementType movementType)
    {
        return movementType switch
        {
            MovementType.Positive => MovementType.Negative,
            MovementType.Negative => MovementType.Positive,
            _ => GetRandomMovementType()
        };
    }
}
