using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core;

/// <summary>
/// Logic for generating a large amount of <see cref="PKM"/> data.
/// </summary>
public static class BulkGenerator
{
    /// <summary>
    /// Gets a list of <see cref="PKM"/> data that are valid for the provided <see cref="sav"/>.
    /// </summary>
    public static List<PKM> GetLivingDex(this SaveFile sav)
    {
        var speciesToGenerate = GetAll(1, sav.MaxSpeciesID);
        speciesToGenerate = speciesToGenerate.Where(sav.Personal.IsSpeciesInGame);
        return GetLivingDex(sav, speciesToGenerate);
    }

    private static IEnumerable<ushort> GetAll(ushort min, ushort max)
    {
        for (ushort i = min; i <= max; i++)
            yield return i;
    }

    private static List<PKM> GetLivingDex(SaveFile sav, IEnumerable<ushort> speciesToGenerate)
    {
        return sav.GetLivingDex(speciesToGenerate, sav.BlankPKM);
    }

    extension(ITrainerInfo tr)
    {
        public List<PKM> GetLivingDex(IEnumerable<ushort> speciesToGenerate, PKM blank)
        {
            var result = new List<PKM>();
            var destType = blank.GetType();
            foreach (var s in speciesToGenerate)
            {
                var pk = blank.Clone();
                pk.Species = s;
                pk.Gender = pk.GetSaneGender();

                var pi = pk.PersonalInfo;
                for (byte f = 0; f < pi.FormCount; f++)
                {
                    var entry = tr.GetLivingEntry(pk, s, f, destType);
                    if (entry is null)
                        continue;
                    result.Add(entry);
                }
            }

            return result;
        }

        public PKM? GetLivingEntry(PKM template, ushort species, byte form, Type destType)
        {
            template.Species = species;
            template.Form = form;
            template.Gender = template.GetSaneGender();

            var moves = ArrayPool<ushort>.Shared.Rent(4);
            var memory = moves.AsMemory(0, 4);
            var span = memory.Span;
            template.GetMoves(span);
            var first = EncounterMovesetGenerator.GenerateEncounters(template, tr, memory).FirstOrDefault();
            span.Clear();
            ArrayPool<ushort>.Shared.Return(moves);
            if (first is null)
                return null;

            var pk = first.ConvertToPKM(tr);
            var result = EntityConverter.ConvertToType(pk, destType, out _);
            if (result is null)
                return null;

            result.Species = species;
            result.Form = form;
            result.CurrentLevel = 100;

            result.Heal();
            return result;
        }
    }
}
