using System;
using System.Collections.Generic;
using System.Diagnostics;

using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    public class BatchEditor
    {
        private int Modified { get; set; }
        private int Iterated { get; set; }
        private int Errored { get; set; }

        public bool ProcessPKM(PKM pkm, IEnumerable<StringInstruction> Filters, IEnumerable<StringInstruction> Instructions)
        {
            if (pkm.Species <= 0)
                return false;
            if (!pkm.Valid || pkm.Locked)
            {
                Iterated++;
                var reason = pkm.Locked ? "Locked." : "Not Valid.";
                Debug.WriteLine($"{MsgBEModifyFailBlocked} {reason}");
                return false;
            }

            var r = BatchEditing.TryModifyPKM(pkm, Filters, Instructions);
            if (r != ModifyResult.Invalid)
                Iterated++;
            if (r == ModifyResult.Error)
                Errored++;
            if (r != ModifyResult.Modified)
                return false;

            pkm.RefreshChecksum();
            Modified++;
            return true;
        }

        public string GetEditorResults(ICollection<StringInstructionSet> sets)
        {
            int ctr = Modified / sets.Count;
            int len = Iterated / sets.Count;
            string maybe = sets.Count == 1 ? string.Empty : "~";
            string result = string.Format(MsgBEModifySuccess, maybe, ctr, len);
            if (Errored > 0)
                result += $"{Environment.NewLine}{maybe}" + string.Format(MsgBEModifyFailError, Errored);
            return result;
        }
    }
}
