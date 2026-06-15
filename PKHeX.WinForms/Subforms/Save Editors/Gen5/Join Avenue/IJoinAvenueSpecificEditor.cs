using PKHeX.Core;

namespace PKHeX.WinForms;

internal interface IJoinAvenueSpecificEditor<in T> where T : class, IJoinAvenueEntity5
{
    void LoadObject(T entity);
    void SaveObject(T entity);
}
