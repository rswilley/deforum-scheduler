using System.Text;
using NAudio.Wave;

namespace DeforumScheduler;

public interface IAudioEnergyReader
{
    Dictionary<int, double> ComputeEnergyPerFrame(string filePath, int fps, string fileName = "");
    Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> ComputeFrequencyBandEnergies(string filePath, int fps);
}

public class AudioEnergyReader : IAudioEnergyReader
{
    public Dictionary<int, double> ComputeEnergyPerFrame(string filePath, int fps, string fileName = "")
    {
        Dictionary<int, double> results = new Dictionary<int, double>();
        StringBuilder csv = null!;
        bool saveToFile = !string.IsNullOrEmpty(fileName);
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
            File.WriteAllText(fileName, csv.ToString());
        }

        return results;
    }
    
    public Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)> ComputeFrequencyBandEnergies(
        string filePath, int fps)
    {
        const float lowPassCutoff = 150.0f;  // Typical kick drum frequencies are below 150 Hz
        const float highPassCutoff = 2000.0f;  // Typical snare frequencies are above 2000 Hz
    
        var results = new Dictionary<int, (double LowFreqEnergy, double HighFreqEnergy)>();
        
        using var reader = new AudioFileReader(filePath);
        int sampleRate = reader.WaveFormat.SampleRate;
        int channels = reader.WaveFormat.Channels;
        double durationSeconds = reader.TotalTime.TotalSeconds;
        int totalFrames = (int)(durationSeconds * fps);

        // Read entire audio file into buffer
        int totalSamples = (int)(sampleRate * durationSeconds) * channels;
        float[] buffer = new float[totalSamples];
        int samplesRead = reader.Read(buffer, 0, buffer.Length);

        // Convert to mono if needed
        float[] monoSamples = new float[samplesRead / channels];
        for (int i = 0; i < monoSamples.Length; i++)
        {
            monoSamples[i] = buffer[i * channels];
        }

        // Apply filters
        float[] lowPassSamples = ApplyLowPassFilter(monoSamples, sampleRate, lowPassCutoff);
        float[] highPassSamples = ApplyHighPassFilter(monoSamples, sampleRate, highPassCutoff);

        int samplesPerFrame = sampleRate / fps;

        for (int frame = 0; frame < totalFrames; frame++)
        {
            double lowFreqEnergy = 0;
            double highFreqEnergy = 0;
            int startSample = frame * samplesPerFrame;

            for (int i = 0; i < samplesPerFrame && (startSample + i) < monoSamples.Length; i++)
            {
                float lowSample = lowPassSamples[startSample + i];
                float highSample = highPassSamples[startSample + i];
                
                lowFreqEnergy += lowSample * lowSample;
                highFreqEnergy += highSample * highSample;
            }

            results.Add(frame, (lowFreqEnergy, highFreqEnergy));
        }

        return results;
    }

    private float[] ApplyLowPassFilter(float[] samples, int sampleRate, float cutoffFrequency)
    {
        float[] filtered = new float[samples.Length];
        float dt = 1.0f / sampleRate;
        float rc = 1.0f / (2.0f * MathF.PI * cutoffFrequency);
        float alpha = dt / (rc + dt);
        
        filtered[0] = samples[0];
        for (int i = 1; i < samples.Length; i++)
        {
            filtered[i] = filtered[i - 1] + (alpha * (samples[i] - filtered[i - 1]));
        }
        
        return filtered;
    }

    private float[] ApplyHighPassFilter(float[] samples, int sampleRate, float cutoffFrequency)
    {
        float[] filtered = new float[samples.Length];
        float dt = 1.0f / sampleRate;
        float rc = 1.0f / (2.0f * MathF.PI * cutoffFrequency);
        float alpha = rc / (rc + dt);
        
        filtered[0] = samples[0];
        for (int i = 1; i < samples.Length; i++)
        {
            filtered[i] = alpha * (filtered[i - 1] + samples[i] - samples[i - 1]);
        }
        
        return filtered;
    }
}