using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static PKHeX.Core.MessageStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Carries out a batch edit and contains information summarizing the results.
    /// </summary>
    public sealed class BatchEditor
    {
        private int Modified { get; set; }
        private int Iterated { get; set; }
        private int Errored { get; set; }

        /// <summary>
        /// Tries to modify the <see cref="PKM"/>.
        /// </summary>
        /// <param name="pkm">Object to modify.</param>
        /// <param name="filters">Filters which must be satisfied prior to any modifications being made.</param>
        /// <param name="modifications">Modifications to perform on the <see cref="pkm"/>.</param>
        /// <returns>Result of the attempted modification.</returns>
        public bool ProcessPKM(PKM pkm, IEnumerable<StringInstruction> filters, IEnumerable<StringInstruction> modifications)
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

            var result = BatchEditing.TryModifyPKM(pkm, filters, modifications);
            if (result != ModifyResult.Invalid)
                Iterated++;
            if (result == ModifyResult.Error)
                Errored++;
            if (result != ModifyResult.Modified)
                return false;

            pkm.RefreshChecksum();
            Modified++;
            return true;
        }

        /// <summary>
        /// Gets a message indicating the overall result of all modifications performed across multiple Batch Edit jobs.
        /// </summary>
        /// <param name="sets">Collection of modifications.</param>
        /// <returns>Friendly (multi-line) string indicating the result of the batch edits.</returns>
        public string GetEditorResults(ICollection<StringInstructionSet> sets)
        {
            if (sets.Count == 0)
                return MsgBEInstructionNone;
            int ctr = Modified / sets.Count;
            int len = Iterated / sets.Count;
            string maybe = sets.Count == 1 ? string.Empty : "~";
            string result = string.Format(MsgBEModifySuccess, maybe, ctr, len);
            if (Errored > 0)
                result += Environment.NewLine + maybe + string.Format(MsgBEModifyFailError, Errored);
            return result;
        }

        public static BatchEditor Execute(IList<string> lines, IEnumerable<PKM> data)
        {
            var editor = new BatchEditor();
            var sets = StringInstructionSet.GetBatchSets(lines).ToArray();
            foreach (var pk in data)
            {
                foreach (var set in sets)
                    editor.ProcessPKM(pk, set.Filters, set.Instructions);
            }

            return editor;
        }
    }
}
