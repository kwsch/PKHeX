namespace PKHeX.Core
{
    /// <summary>
    /// Object has Trainer ownership
    /// </summary>
    public interface ITrainerID
    {
        int TID { get; set; }
        int SID { get; set; }
    }
}
