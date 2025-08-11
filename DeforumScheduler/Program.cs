using DeforumScheduler;

const int fps = 12; // Frames per second
const int bpm = 128; // Beats per minute

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
var bass = stemResults.Single(r => r.Key == StemType.Bass).Value;
var other = stemResults.Single(r => r.Key == StemType.Other).Value;

var strengthAutomation = new StrengthAutomation();
var translationXAutomation = new CameraAutomation(bpm, fps, 0.04, 0.4, 8, beats);
var translationYAutomation = new CameraAutomation(bpm, fps, 0.06, 0.6, 8, beats);
var translationZAutomation = new CameraAutomation(bpm, fps, 1, 5, 32, beats);
var rotation3DXAutomation = new CameraAutomation(bpm, fps, 0.14, 1.4, 16, beats);
var rotation3DYAutomation = new CameraAutomation(bpm, fps, 0.16, 1.6, 16, beats);

for (int frame = 0; frame < beats.Count; frame++)
{
    var beat = beats[frame];
    var kickValue = kicksAndSnares[frame].LowFreqEnergy;
    var snareValue = kicksAndSnares[frame].HighFreqEnergy;
    var bassValue = bass[frame];
    var otherValue = other[frame];

    strengthAutomation.AddValue(frame, beat.IsPeak ? 0.55 : 0.65);
    translationXAutomation.AddValue(frame, otherValue);
    translationYAutomation.AddValue(frame, otherValue);
    translationZAutomation.AddValue(frame, bassValue);
    rotation3DXAutomation.AddValue(frame, kickValue);
    rotation3DYAutomation.AddValue(frame, snareValue);
}

var tasks = new List<Task>
{
    File.WriteAllTextAsync("strength-schedule.txt", strengthAutomation.ToString()),
    File.WriteAllTextAsync("trans-x-schedule.txt", translationXAutomation.ToString()),
    File.WriteAllTextAsync("trans-y-schedule.txt", translationYAutomation.ToString()),
    File.WriteAllTextAsync("trans-z-schedule.txt", translationZAutomation.ToString()),
    File.WriteAllTextAsync("trans-3dx-schedule.txt", rotation3DXAutomation.ToString()),
    File.WriteAllTextAsync("trans-3dy-schedule.txt", rotation3DYAutomation.ToString()),
};

await Task.WhenAll(tasks);