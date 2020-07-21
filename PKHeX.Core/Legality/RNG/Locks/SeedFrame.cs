namespace PKHeX.Core
{
    /// <summary>
    /// Message Passing for frame results.
    /// </summary>
    internal readonly struct SeedFrame
    {
        public readonly uint PID;
        public readonly int FrameID;

        internal SeedFrame(uint pid, int frame)
        {
            PID = pid;
            FrameID = frame;
        }
    }
}