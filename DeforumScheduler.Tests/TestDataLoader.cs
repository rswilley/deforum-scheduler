namespace TestProject1;

public class TestDataLoader
{
    public Dictionary<int, double> LoadTestTrack(string filePath)
    {
        var lines = File.ReadAllLines(filePath);

        var results = new Dictionary<int, double>();
        foreach (var line in lines)
        {
            var values = line.Split(',');
            var frame = Convert.ToInt32(values[0]);
            var energy = Convert.ToDouble(values[1]);
            
            results.Add(frame, energy);
        }

        return results;
    }
}