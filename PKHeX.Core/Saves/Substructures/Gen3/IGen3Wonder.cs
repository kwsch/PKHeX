namespace PKHeX.Core
{
    public interface IGen3Wonder
    {
        int WonderOffset { get; }
        WonderNews3 WonderNews { get; set; }
        WonderCard3 WonderCard { get; set; }
        WonderCard3Extra WonderCardExtra { get; set; }
    }
}
