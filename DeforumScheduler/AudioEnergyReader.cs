using System.Text;
using NAudio.Dsp;
using NAudio.Wave;

namespace DeforumScheduler;

public interface IAudioEnergyReader
{
    IEnumerable<Energy> ComputeShortTimeEnergy(string filePath, bool saveToFile);
}

public class AudioEnergyReader : IAudioEnergyReader
{
    public IEnumerable<Energy> ComputeShortTimeEnergy(string filePath, bool saveToFile = false)
    {
        using var reader = new AudioFileReader(filePath);
        
        int sampleRate = reader.WaveFormat.SampleRate;
        int bufferSize = 1024; // Small buffer for better transient detection
        float[] buffer = new float[bufferSize];
        BiQuadFilter lowPassFilter = BiQuadFilter.LowPassFilter(sampleRate, 150, 1); // 150 Hz cutoff

        int bytesRead;
        double time = 0;

        var results = new List<Energy>();
        var csv = new StringBuilder();
        
        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            // Apply the low-pass filter
            for (int i = 0; i < bytesRead; i++)
            {
                buffer[i] = lowPassFilter.Transform(buffer[i]);
            }

            // Compute Short-Time Energy (STE)
            double energy = buffer.Take(bytesRead).Select(x => x * x).Sum();
            var timeInSeconds = Convert.ToDouble($"{time:F3}");

            var item = new Energy
            {
                TimeInSeconds = timeInSeconds,
                Value = energy
            };
            results.Add(item);
            
            if (saveToFile)
            {
                csv.AppendLine($"{item.TimeInSeconds},{item.Value}");
            }

            time += (double)bufferSize / sampleRate;
        }

        if (saveToFile)
        {
            File.WriteAllText("output.csv", csv.ToString());
        }

        return results;
    }
}

public class Energy
{
    public double TimeInSeconds { get; init; }
    public double Value { get; init; }
}