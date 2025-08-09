using System.Text;

namespace DeforumScheduler;

// public class StrengthAutomation : AutomationBase
// {
//     public StrengthAutomation(double defaultValue)
//     {
//         DefaultValue = defaultValue;
//         Multiplier = 0;
//     }
// }

/// <summary>
/// 2D & 3D operator to move canvas left/right in pixels per frame
/// </summary>
// public class TranslationXAutomation : AutomationBase
// {
//     public TranslationXAutomation(double defaultValue, double multiplier)
//     {
//         DefaultValue = defaultValue;
//         Multiplier = multiplier;
//     }
// }

/// <summary>
/// 2D & 3D operator to move canvas up/down in pixels per frame
/// </summary>
// public class TranslationYAutomation : AutomationBase
// {
//     public TranslationYAutomation(double defaultValue, double multiplier)
//     {
//         DefaultValue = defaultValue;
//         Multiplier = multiplier;
//     }
// }

/// <summary>
/// 3D operator to move canvas towards/away from view [speed set by FOV]
/// </summary>
public class TranslationZAutomation : AutomationBase
{
    public TranslationZAutomation(int fps, double minValue, double maxValue, int automationChangeInSeconds)
    {
        Fps = fps;
        MinValue = minValue;
        MaxValue = maxValue;
        AutomationChangeInSeconds = automationChangeInSeconds;
    }
}

/// <summary>
/// 3D operator to tilt canvas up/down in degrees per frame (+ = up, - = down)
/// </summary>
public class Rotation3DXAutomation : AutomationBase
{
    public Rotation3DXAutomation(int fps, double minValue, double maxValue, int automationChangeInSeconds)
    {
        Fps = fps;
        MinValue = minValue;
        MaxValue = maxValue;
        AutomationChangeInSeconds = automationChangeInSeconds;
    }
}

/// <summary>
/// 3D operator to tilt canvas left/right in degrees per frame (+ = right, - = left)
/// </summary>
public class Rotation3DYAutomation : AutomationBase
{
    public Rotation3DYAutomation(int fps, double minValue, double maxValue, int automationChangeInSeconds)
    {
        Fps = fps;
        MinValue = minValue;
        MaxValue = maxValue;
        AutomationChangeInSeconds = automationChangeInSeconds;
    }
}

/// <summary>
/// 3D operator to roll canvas clockwise/anticlockwise (+ = left, - = right)
/// </summary>
// public class Rotation3DZAutomation : AutomationBase
// {
//     public Rotation3DZAutomation(double defaultValue, double multiplier)
//     {
//         DefaultValue = defaultValue;
//         Multiplier = multiplier;
//     }
// }

public abstract class AutomationBase
{
    public int Fps { get; init; }
    protected double MinValue { get; init; }
    protected double MaxValue { get; init; }
    protected int AutomationChangeInSeconds { get; init; }
    
    public void AddValue(int frame, double value)
    {
        var roundedValue = Math.Round(value, 2);
        Values.Add(frame, roundedValue);
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        var normalizedValues = Normalize(Values, MinValue, MaxValue);
        
        var automationChangeFrame = AutomationChangeInSeconds * Fps;
        var movementType = GetRandomMovementType();

        var resultValues = new Dictionary<int, double>();
        for (int frame = 0; frame < normalizedValues.Count; frame++)
        {
            var movementValue = normalizedValues[frame];
            if (frame > automationChangeFrame)
            {
                automationChangeFrame += AutomationChangeInSeconds * Fps;
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
        var normalizedValues = Normalize(Values, MinValue, MaxValue);
        
        foreach (var value in normalizedValues)
        {
            result.AppendLine($"{value.Key},{value.Value}");
        }
        return result.ToString();
    }
    
    private Dictionary<int, double> Values { get; init; } = new();
    private Random Random { get; init; } = new();
    
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

    private MovementType GetRandomMovementType()
    {
        var randomValue = Random.Next(0, 2);
        return randomValue switch
        {
            0 => MovementType.Negative,
            1 => MovementType.Positive,
            _ => throw new ArgumentOutOfRangeException()
        };
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