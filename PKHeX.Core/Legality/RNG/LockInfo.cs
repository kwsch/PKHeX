namespace PKHeX.Core
{
    public enum LockInfo
    {
        /// <summary>
        /// PID matches the required parameters.
        /// </summary>
        Pass,

        /// <summary>
        /// PID did not match the required Nature.
        /// </summary>
        Nature,

        /// <summary>
        /// PID did not match the required Gender.
        /// </summary>
        Gender,
    }
}
