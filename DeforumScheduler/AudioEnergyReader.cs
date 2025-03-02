using System.Text;
using NAudio.Wave;

namespace DeforumScheduler;

public interface IAudioEnergyReader
{
    Dictionary<int, double> ComputeEnergyPerFrame(string filePath, int fps, bool saveToFile = false);
}

public class AudioEnergyReader : IAudioEnergyReader
{
    public Dictionary<int, double> ComputeEnergyPerFrame(string filePath, int fps, bool saveToFile = false)
    {
        Dictionary<int, double> results = new Dictionary<int, double>();
        StringBuilder csv = null!;
        if (saveToFile)
        {
            csv = new StringBuilder();
        }
        
        using var reader = new AudioFileReader(filePath);
        int sampleRate = reader.WaveFormat.SampleRate;  // e.g., 44100 Hz
        int channels = reader.WaveFormat.Channels;      // Stereo or mono
        double durationSeconds = reader.TotalTime.TotalSeconds;
        int totalFrames = (int)(durationSeconds * fps); // Total frames in video

        // Read entire audio file into buffer
        int totalSamples = (int)(sampleRate * durationSeconds) * channels;
        float[] buffer = new float[totalSamples];
        int samplesRead = reader.Read(buffer, 0, buffer.Length);

        // Convert to mono if needed
        float[] monoSamples = new float[samplesRead / channels];
        for (int i = 0; i < monoSamples.Length; i++)
        {
            monoSamples[i] = buffer[i * channels]; // Take only the left channel
        }

        int samplesPerFrame = sampleRate / fps; // Samples corresponding to 1 video frame
        
        for (int frame = 0; frame < totalFrames; frame++)
        {
            double energy = 0;
            int startSample = frame * samplesPerFrame;

            // Sum squared values within the frame's sample range
            for (int i = 0; i < samplesPerFrame && (startSample + i) < monoSamples.Length; i++)
            {
                float sample = monoSamples[startSample + i];
                energy += sample * sample;
            }

            // Store energy value for this frame
            results.Add(frame, energy);
            
            if (saveToFile)
            {
                csv.AppendLine($"{frame},{energy}");
            }
        }
        
        if (saveToFile)
        {
            File.WriteAllText("frames.csv", csv.ToString());
        }

        return results;
    }
}