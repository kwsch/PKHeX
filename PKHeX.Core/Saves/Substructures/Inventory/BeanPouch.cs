using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Reads the bean pouch data from a <see cref="SAV7"/>.
    /// </summary>
    public sealed class BeanPouch
    {
        public const int Count = 15;
        public static readonly string[] BeanIndexNames = GetBeanList();

        public static string[] GetBeanList()
        {
            var colors = Enum.GetNames(typeof(BeanColor));
            var beans = new List<string>();
            // 7 regular, 7 patterned, one rainbow
            beans.AddRange(colors.Select(z => $"{z} Bean"));
            beans.AddRange(colors.Select(z => $"{z} Patterned Bean"));
            beans.Add("Rainbow Bean");
            return beans.ToArray();
        }

        private enum BeanColor
        {
            Red,
            Blue,
            LightBlue,
            Green,
            Yellow,
            Purple,
            Orange,
        }

        private readonly SAV7 SAV;
        public BeanPouch(SAV7 sav) => SAV = sav;

        public Span<byte> Beans => SAV.ResortSave.GetBeans();
        public void SetCountAll(byte val) => SAV.ResortSave.GetBeans().Fill(val);
    }
}
