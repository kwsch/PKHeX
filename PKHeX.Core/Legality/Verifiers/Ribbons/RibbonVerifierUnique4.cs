namespace PKHeX.Core;

/// <summary>
/// Parsing logic for <see cref="IRibbonSetUnique4"/>.
/// </summary>
public static class RibbonVerifierUnique4
{
    public static void Parse(this IRibbonSetUnique4 r, RibbonVerifierArguments args, ref RibbonResultList list)
    {
        var evos = args.History;
        if (!RibbonRules.IsAllowedBattleFrontier4(evos))
            FlagAnyAbility(r, ref list);

        if (RibbonRules.IsAllowedContest3(evos))
            AddMissingContest3(r, ref list);
        else
            FlagAnyContest3(r, ref list);

        if (RibbonRules.IsAllowedContest4(evos))
            AddMissingContest4(r, ref list);
        else
            FlagAnyContest4(r, ref list);
    }

    public static void ParseEgg(this IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        FlagAnyAbility(r, ref list);
        FlagAnyContest3(r, ref list);
        FlagAnyContest4(r, ref list);
    }

    private static void AddMissingContest3(IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        static void CheckSet(bool Master, bool Hyper, bool Super, bool Initial, ref RibbonResultList list, RibbonIndex3 index)
        {
            bool top = Master;
            if (Hyper)
                top = true;
            else if (top)
                list.Add((RibbonIndex3)((byte)index + 2));

            if (Super)
                top = true;
            else if (top)
                list.Add((RibbonIndex3)((byte)index + 1));

            if (top && !Initial)
                list.Add(index);
        }
        CheckSet(r.RibbonG3CoolMaster,   r.RibbonG3CoolHyper,   r.RibbonG3CoolSuper,   r.RibbonG3Cool,   ref list, RibbonIndex3.Cool);
        CheckSet(r.RibbonG3BeautyMaster, r.RibbonG3BeautyHyper, r.RibbonG3BeautySuper, r.RibbonG3Beauty, ref list, RibbonIndex3.Beauty);
        CheckSet(r.RibbonG3CuteMaster ,  r.RibbonG3CuteHyper,   r.RibbonG3CuteSuper,   r.RibbonG3Cute,   ref list, RibbonIndex3.Cute);
        CheckSet(r.RibbonG3SmartMaster,  r.RibbonG3SmartHyper,  r.RibbonG3SmartSuper,  r.RibbonG3Smart,  ref list, RibbonIndex3.Smart);
        CheckSet(r.RibbonG3ToughMaster,  r.RibbonG3ToughHyper,  r.RibbonG3ToughSuper,  r.RibbonG3Tough,  ref list, RibbonIndex3.Tough);
    }

    private static void AddMissingContest4(IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        static void CheckSet(bool Master, bool Hyper, bool Super, bool Initial, ref RibbonResultList list, RibbonIndex4 index)
        {
            bool top = Master;
            if (Hyper)
                top = true;
            else if (top)
                list.Add((RibbonIndex4)((byte)index + 2));

            if (Super)
                top = true;
            else if (top)
                list.Add((RibbonIndex4)((byte)index + 1));

            if (top && !Initial)
                list.Add(index);
        }
        CheckSet(r.RibbonG3CoolMaster,   r.RibbonG3CoolHyper,   r.RibbonG3CoolSuper,   r.RibbonG3Cool,   ref list, RibbonIndex4.Cool);
        CheckSet(r.RibbonG3BeautyMaster, r.RibbonG3BeautyHyper, r.RibbonG3BeautySuper, r.RibbonG3Beauty, ref list, RibbonIndex4.Beauty);
        CheckSet(r.RibbonG3CuteMaster ,  r.RibbonG3CuteHyper,   r.RibbonG3CuteSuper,   r.RibbonG3Cute,   ref list, RibbonIndex4.Cute);
        CheckSet(r.RibbonG3SmartMaster,  r.RibbonG3SmartHyper,  r.RibbonG3SmartSuper,  r.RibbonG3Smart,  ref list, RibbonIndex4.Smart);
        CheckSet(r.RibbonG3ToughMaster,  r.RibbonG3ToughHyper,  r.RibbonG3ToughSuper,  r.RibbonG3Tough,  ref list, RibbonIndex4.Tough);
    }

    private static void FlagAnyAbility(IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        if (r.RibbonAbility)
            list.Add(RibbonIndex4.Ability);
        if (r.RibbonAbilityGreat)
            list.Add(RibbonIndex4.AbilityGreat);
        if (r.RibbonAbilityDouble)
            list.Add(RibbonIndex4.AbilityDouble);
        if (r.RibbonAbilityMulti)
            list.Add(RibbonIndex4.AbilityMulti);
        if (r.RibbonAbilityPair)
            list.Add(RibbonIndex4.AbilityPair);
    }

    private static void FlagAnyContest3(IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        static void CheckSet(bool Master, bool Hyper, bool Super, bool Initial, ref RibbonResultList list, RibbonIndex3 index)
        {
            if (Master)
                list.Add((RibbonIndex3)((byte)index + 3));
            if (Hyper)
                list.Add((RibbonIndex3)((byte)index + 2));
            if (Super)
                list.Add((RibbonIndex3)((byte)index + 1));
            if (Initial)
                list.Add(index);
        }
        CheckSet(r.RibbonG3CoolMaster,   r.RibbonG3CoolHyper,   r.RibbonG3CoolSuper,   r.RibbonG3Cool,   ref list, RibbonIndex3.Cool);
        CheckSet(r.RibbonG3BeautyMaster, r.RibbonG3BeautyHyper, r.RibbonG3BeautySuper, r.RibbonG3Beauty, ref list, RibbonIndex3.Beauty);
        CheckSet(r.RibbonG3CuteMaster ,  r.RibbonG3CuteHyper,   r.RibbonG3CuteSuper,   r.RibbonG3Cute,   ref list, RibbonIndex3.Cute);
        CheckSet(r.RibbonG3SmartMaster,  r.RibbonG3SmartHyper,  r.RibbonG3SmartSuper,  r.RibbonG3Smart,  ref list, RibbonIndex3.Smart);
        CheckSet(r.RibbonG3ToughMaster,  r.RibbonG3ToughHyper,  r.RibbonG3ToughSuper,  r.RibbonG3Tough,  ref list, RibbonIndex3.Tough);
    }

    private static void FlagAnyContest4(IRibbonSetUnique4 r, ref RibbonResultList list)
    {
        static void CheckSet(bool Master, bool Hyper, bool Super, bool Initial, ref RibbonResultList list, RibbonIndex4 index)
        {
            if (Master)
                list.Add((RibbonIndex4)((byte)index + 3));
            if (Hyper)
                list.Add((RibbonIndex4)((byte)index + 2));
            if (Super)
                list.Add((RibbonIndex4)((byte)index + 1));
            if (Initial)
                list.Add(index);
        }
        CheckSet(r.RibbonG3CoolMaster,   r.RibbonG3CoolHyper,   r.RibbonG3CoolSuper,   r.RibbonG3Cool,   ref list, RibbonIndex4.Cool);
        CheckSet(r.RibbonG3BeautyMaster, r.RibbonG3BeautyHyper, r.RibbonG3BeautySuper, r.RibbonG3Beauty, ref list, RibbonIndex4.Beauty);
        CheckSet(r.RibbonG3CuteMaster ,  r.RibbonG3CuteHyper,   r.RibbonG3CuteSuper,   r.RibbonG3Cute,   ref list, RibbonIndex4.Cute);
        CheckSet(r.RibbonG3SmartMaster,  r.RibbonG3SmartHyper,  r.RibbonG3SmartSuper,  r.RibbonG3Smart,  ref list, RibbonIndex4.Smart);
        CheckSet(r.RibbonG3ToughMaster,  r.RibbonG3ToughHyper,  r.RibbonG3ToughSuper,  r.RibbonG3Tough,  ref list, RibbonIndex4.Tough);
    }
}
