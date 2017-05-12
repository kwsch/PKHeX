using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.Tests.Saves.Substructures
{
    [TestClass]
    public class MemeCryptoTests
    {
        [TestMethod]
        public void CanUseMemeCrypto()
        {
            Assert.IsTrue(MemeCrypto.CanUseMemeCrypto());
        }
    }
}
