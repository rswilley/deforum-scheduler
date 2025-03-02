using DeforumScheduler;

namespace TestProject1;

public class AudioEnergyReaderTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        var energy = subject.ComputeEnergyPerFrame(@"C:\Users\rober\Music\StemRoller\JNTN - Unscathed (Original Mix)\drums.wav", 12, true);
    }
    
    private IAudioEnergyReader GetSubject() => new AudioEnergyReader();
}