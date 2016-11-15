using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PKHeX.Reflection
{
    public class EnumItemNotInListException : Exception
    {
        public EnumItemNotInListException(string missingItem)
        {
            this.MissingItem = missingItem;
        }

        public string MissingItem { get; set; }
    }
}
