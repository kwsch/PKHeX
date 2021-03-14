namespace PKHeX.Core
{
    /// <summary>
    /// Type of PID-Nature roll algorithm the frame is obtained from.
    /// </summary>
    public enum FrameType
    {
        None,

        /// <summary> Generation 3 PID-Nature roll algorithm </summary>
        MethodH,

        /// <summary> Generation 4 PID-Nature roll algorithm present for D/P/Pt </summary>
        MethodJ,

        /// <summary> Generation 4 PID-Nature roll algorithm present for HG/SS </summary>
        MethodK,
    }
}
