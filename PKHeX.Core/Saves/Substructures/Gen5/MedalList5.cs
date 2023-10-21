namespace PKHeX.Core;

public sealed class MedalList5: SaveBlock<SAV5B2W2> {

    const int MAX_MEDALS = 255;
    private readonly Medal5[] Medals;

    public MedalList5(SAV5B2W2 SAV, int offset) : base(SAV) {
        Offset = offset;
        Medals = new Medal5[MAX_MEDALS];
        for (int i = 0; i < MAX_MEDALS; i++)
            Medals[i] = new Medal5(Data, Offset, i);
    }

    public Medal5 this[int index]
    {
        get => Medals[index];
        set => Medals[index] = value;
    }

}
