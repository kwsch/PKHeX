using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.Tests.Util
{
    [TestClass]
    public class DataUtilTests
    {
        const string TestCategory = "Data Util Tests";

        [TestMethod]
        [TestCategory(TestCategory)]
        public void TestGetPokemonNames()
        {
            var names = PKHeX.Core.Util.GetSpeciesList("en");
            Assert.AreEqual(803, names.Length);
        }
    }
}
