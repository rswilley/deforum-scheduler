using DeforumScheduler;

const int fps = 12; // Frames per second

var energyReader = new AudioEnergyReader();
var stemFiles = new Dictionary<StemType, string>
{
    { StemType.Drums, @"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\drums.wav" },
    { StemType.Bass, @"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\bass.wav" },
    { StemType.Other, @"C:\Users\rober\Music\JNTN - Unscathed (Original Mix)\other.wav" },
};

var stemResults = new Dictionary<StemType, Dictionary<int, double>>();
foreach (var file in stemFiles)
{
    var framesToEnergy = energyReader.ComputeEnergyPerFrame(file.Value, fps, "");
    stemResults.Add(file.Key, framesToEnergy);
}

var kicksAndSnares = energyReader.ComputeFrequencyBandEnergies(stemFiles.Single(f => f.Key == StemType.Drums).Value, fps);
var beats = new BeatDetector().DetectBeats(stemResults.Single(r => r.Key == StemType.Drums).Value, fps);
var drums = stemResults.Single(r => r.Key == StemType.Drums).Value;
var bass = stemResults.Single(r => r.Key == StemType.Bass).Value;
var other = stemResults.Single(r => r.Key == StemType.Other).Value;

const int automationChangeSeconds = 15;

// var strengthAutomation = new StrengthAutomation(0.65);
// var translationXAutomation = new TranslationXAutomation(0.04, multiplier);
// var translationYAutomation = new TranslationYAutomation(0.06, multiplier);
var translationZAutomation = new TranslationZAutomation(12, 1, 5, automationChangeSeconds);
var rotation3DXAutomation = new Rotation3DXAutomation(12, 0.14, 1, automationChangeSeconds);
var rotation3DYAutomation = new Rotation3DYAutomation(12, 0.16, 1, automationChangeSeconds);

for (int frame = 0; frame < beats.Count; frame++)
{
    var beat = beats[frame];
    //var drumValue = drums[0];
    var snareValue = kicksAndSnares[frame].HighFreqEnergy;
    var bassValue = bass[frame];
    var otherValue = other[frame];

    //strengthAutomation.AddValue(frame, beat.IsPeak ? 0.55 : 0.65, 1);
    //translationXAutomation.AddValue(frame, otherValue, sign);
    //translationYAutomation.AddValue(frame, otherValue, sign);
    translationZAutomation.AddValue(frame, bassValue);
    rotation3DXAutomation.AddValue(frame, snareValue);
    rotation3DYAutomation.AddValue(frame, snareValue);
}

var tasks = new List<Task>
{
    //File.WriteAllTextAsync("strength-schedule.csv", strengthAutomation.ToCsv()),
    //File.WriteAllTextAsync("trans-x-schedule.csv", translationXAutomation.ToCsv()),
    //File.WriteAllTextAsync("trans-y-schedule.csv", translationYAutomation.ToCsv()),
    File.WriteAllTextAsync("trans-z-schedule.txt", translationZAutomation.ToString()),
    File.WriteAllTextAsync("trans-3dx-schedule.txt", rotation3DXAutomation.ToString()),
    File.WriteAllTextAsync("trans-3dy-schedule.txt", rotation3DYAutomation.ToString()),
};

await Task.WhenAll(tasks);