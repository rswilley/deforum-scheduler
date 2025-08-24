using System.Text;

namespace DeforumScheduler;

public class StrengthAutomation(double minValue, double maxValue, double dropValue)
{
    public void AddValue(int frame, Sample sample)
    {
        if (frame == 0)
        {
            Values.Add(frame, maxValue);
            return;
        }
        
        var value = GetValue(sample);
        Values.TryAdd(frame, value);
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

    private double GetValue(Sample sample)
    {
        double value;
        if (sample.IsDrop)
        {
            value = dropValue;
        } else if (sample.IsKickPeak)
        {
            value = minValue;
        }
        else
        {
            value = maxValue;   
        }

        return value;
    }
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
        var normalizedValues = new Normalizer().Normalize(Values, _minValue, _maxValue);
        var windowSize = FrameResolver.GetFrameNumberForBars(_bpm, 0.25, _fps);
        var smoothValues = new Smoother().SmoothValues(normalizedValues, windowSize);
        
        var drops = _beats.Where(b => b.Value.IsDrop).Select(drop => drop.Value).ToList();
        var dropIndex = 0;
        var seekDrop = drops[dropIndex];
        
        var initialAutomationChangeFrame = FrameResolver.GetFrameNumberForBars(_bpm, _automationChangeInBars, _fps);
        var automationChangeFrame = initialAutomationChangeFrame;
        var movementType = GetRandomMovementType();

        var resultValues = new Dictionary<int, double>();
        for (int frame = 0; frame < smoothValues.Count; frame++)
        {
            var movementValue = smoothValues[frame];
            if (frame == seekDrop?.Frame)
            {
                // reset automation change frame if we find a drop
                automationChangeFrame = seekDrop.Frame + initialAutomationChangeFrame;
                movementType = GetNextMovementType(movementType);
                ++dropIndex;
                seekDrop = drops.ElementAtOrDefault(dropIndex);

            } else if (frame > automationChangeFrame)
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
        var normalizedValues = new Normalizer().Normalize(Values, _minValue, _maxValue);
        
        foreach (var value in normalizedValues)
        {
            result.AppendLine($"{value.Key},{value.Value}");
        }
        return result.ToString();
    }
    
    private Dictionary<int, double> Values { get; } = new();
    private Random Random { get; } = new();

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
