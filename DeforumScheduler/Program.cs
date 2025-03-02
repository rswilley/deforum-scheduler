using DeforumScheduler;

var energyReader = new AudioEnergyReader();
var shortTimeEnergy =
    energyReader.ComputeShortTimeEnergy(
        "C:\\Users\\rober\\Music\\StemRoller\\JNTN - Unscathed (Original Mix)\\drums.wav");

int fps = 12; // Frames per second
int bpm = 128; // Beats per minute
double secondsPerBeat = 60.0 / bpm; // Time per beat in seconds
double secondsPerBar = 240.0 / bpm;

double framesPerBeat = secondsPerBeat * fps; // Frames per beat

var track = new Track(fps);

double onBeatValue = 0.55;
double offBeatValue = 0.65;

//var kickDrumDetector = new KickDrumDetector();

// foreach (var section in track.Sections)
// {
//     for (int frame = section.StartFrame; frame < section.EndFrame; frame++)
//     {
//         double timeInSeconds = (double)frame / fps;
//         double modTime;
//         
//         if (section.Type == SectionType.Breakdown)
//         {
//             modTime = timeInSeconds % secondsPerBar;
//         }
//         else
//         {
//             modTime = timeInSeconds % secondsPerBeat;
//         }
//
//         // Check if the frame is close to a beat time
//         double frameValue = (modTime < (1.0 / fps)) ? onBeatValue : offBeatValue;
//
//         Console.WriteLine($"{frame}: ({frameValue}),");
//     }
// }

// foreach (var section in track.Sections)
// {
//     if (section.Type == SectionType.Breakdown)
//     { 
//         Console.WriteLine($"{section.StartFrame}: ((-0.10 * cos((128 / 240 * 3.141 * (t + 0) / 12))**10 + 0.65)),");
//     } else if (section.Type is SectionType.Intro or SectionType.Chorus)
//     {
//         Console.WriteLine($"{section.StartFrame}: ((-0.10 * cos((128 / 60 * 3.141 * (t + 0) / 12))**10 + 0.65)),");
//     }
//     else if (section.Type == SectionType.NewTrack)
//     { 
//         Console.WriteLine($"{section.StartFrame - 2}: (0.65),");
//         Console.WriteLine($"{section.StartFrame - 1}: (0.25),");
//         Console.WriteLine($"{section.StartFrame}: ((-0.10 * cos((128 / 60 * 3.141 * (t + 0) / 12))**10 + 0.65)),");
//     }
// }

int totalFrames = (track.DurationInSeconds + 1) * fps;
Console.WriteLine(totalFrames);

TimeSpan GetTime(Section section)
{
    return TimeSpan.Parse(section.Timestamp);
}