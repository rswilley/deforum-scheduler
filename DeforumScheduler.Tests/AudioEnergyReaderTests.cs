using DeforumScheduler;

namespace TestProject1;

public class AudioEnergyReaderTests
{
    [Fact]
    public void Test1()
    {
        var subject = GetSubject();
        var energy = subject.ComputeShortTimeEnergy(@"C:\Users\rober\Music\StemRoller\JNTN - Unscathed (Original Mix)\drums.wav", true);
    }
    
    private IAudioEnergyReader GetSubject() => new AudioEnergyReader();
}