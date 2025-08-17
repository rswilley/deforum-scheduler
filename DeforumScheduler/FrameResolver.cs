namespace DeforumScheduler;

public static class FrameResolver
{
    public static int GetFrameNumberForBars(double bpm, double bars, double fps, int beatsPerBar = 4)
    {
        // Duration (seconds) for the requested bars
        double seconds = bars * beatsPerBar * (60.0 / bpm);

        // Convert seconds to frames
        double frames = seconds * fps;

        // Round to nearest frame
        return (int)Math.Round(frames, MidpointRounding.AwayFromZero);
    }
}