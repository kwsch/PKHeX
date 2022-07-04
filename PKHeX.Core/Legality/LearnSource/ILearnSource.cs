using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PKHeX.Core;

public interface ILearnSource
{
    public LearnMethod GetCanLearn(PKM pk, PersonalInfo pi, EvoCriteria evo, int move, MoveSourceType types = MoveSourceType.All);
    public IEnumerable<int> GetAllMoves(PKM pk, EvoCriteria evo, MoveSourceType types = MoveSourceType.All);

    public Learnset GetLearnset(int species, int form);

    public bool TryGetPersonal(int species, int form, [NotNullWhen(true)] out PersonalInfo? pi);
}
