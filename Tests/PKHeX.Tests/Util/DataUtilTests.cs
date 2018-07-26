using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.AreEqual(808, names.Length);
        }
    }
}
