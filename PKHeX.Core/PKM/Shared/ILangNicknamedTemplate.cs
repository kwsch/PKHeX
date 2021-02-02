namespace PKHeX.Core
{
    internal interface ILangNicknamedTemplate
    {
        string GetNickname(int language);
        bool GetIsNicknamed(int language);

        bool CanBeAnyLanguage();
        bool CanHaveLanguage(int language);
    }
}
