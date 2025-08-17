using DeforumScheduler;

namespace TestProject1;

public class AudioEnergyReaderTests
{
    [Fact]
    public void ComputeEnergyPerFrame_ByDefault_ShouldWriteCsv()
    {
        var subject = GetSubject();
        var energy = subject.ComputeEnergyPerFrame(@"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\drums.wav", 12, string.Empty);
    }
    
    [Fact]
    public void ComputeFrequencyBandEnergies_ByDefault_ShouldWriteCsv()
    {
        var subject = GetSubject();
        var energy = subject.ComputeFrequencyBandEnergies(@"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\drums.wav", 12, "drums-frames.csv");
    }
    
    private IAudioEnergyReader GetSubject() => new AudioEnergyReader();
}