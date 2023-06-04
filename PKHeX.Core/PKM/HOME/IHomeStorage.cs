namespace PKHeX.Core;

public interface IHomeStorage
{
    bool Exists(ulong tracker);
    PKH GetEntity<T>(T pk) where T : PKM;
}

public sealed class HomeStorageFacade : IHomeStorage
{
    public bool Exists(ulong tracker) => false;
    public PKH GetEntity<T>(T pk) where T : PKM => PKH.ConvertFromPKM(pk);
}
