using DeforumScheduler;

namespace TestProject1;

public class AudioEnergyReaderTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        var energy = subject.ComputeEnergyPerFrame(@"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\drums.wav", 12, string.Empty);
    }
    
    private IAudioEnergyReader GetSubject() => new AudioEnergyReader();
}