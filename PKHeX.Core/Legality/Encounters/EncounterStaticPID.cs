namespace PKHeX.Core
{
    internal class EncounterStaticPID : EncounterStatic
    {
        public uint PID { get; set; }
        public bool NSparkle { get; set; }
        public override Shiny Shiny { get; set; } = Shiny.FixedValue;
    }
}