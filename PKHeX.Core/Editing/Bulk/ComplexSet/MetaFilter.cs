using System;

namespace PKHeX.Core
{
    /// <inheritdoc cref="IComplexFilter"/>
    public sealed class MetaFilter : IComplexFilterMeta
    {
        private readonly string Property;
        private readonly Func<object, StringInstruction, bool> FilterPKM;

        public MetaFilter(
            string property,
            Func<object, StringInstruction, bool> filterPkm)
        {
            Property = property;
            FilterPKM = filterPkm;
        }

        public bool IsMatch(string prop) => prop == Property;
        public bool IsFiltered(object pkm, StringInstruction cmd) => FilterPKM(pkm, cmd);
    }
}
