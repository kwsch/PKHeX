using System;

namespace PKHeX.Core
{
    /// <inheritdoc cref="IComplexFilter"/>
    public class ComplexFilter : IComplexFilter
    {
        private readonly string Property;
        private readonly Func<PKM, StringInstruction, bool> FilterPKM;
        private readonly Func<BatchInfo, StringInstruction, bool> FilterBulk;

        public ComplexFilter(
            string property,
            Func<PKM, StringInstruction, bool> filterPkm,
            Func<BatchInfo, StringInstruction, bool> filterBulk)
        {
            Property = property;
            FilterPKM = filterPkm;
            FilterBulk = filterBulk;
        }

        public bool IsMatch(string prop) => prop == Property;
        public bool IsFiltered(PKM pkm, StringInstruction cmd) => FilterPKM(pkm, cmd);
        public bool IsFiltered(BatchInfo info, StringInstruction cmd) => FilterBulk(info, cmd);
    }
}
