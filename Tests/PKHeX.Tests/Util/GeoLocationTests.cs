using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;

namespace PKHeX.Tests.Util
{
    [TestClass]
    public class GeoLocationTests
    {
        private const string GeoLocationCategory = "GeoLocation Tests";

        [TestMethod]
        [TestCategory(GeoLocationCategory)]
        public void CountryFetch()
        {
            string japan = GeoLocation.GetCountryName("en", 1);
            Assert.AreEqual("Japan", japan);

            string argentina = GeoLocation.GetCountryName(LanguageID.English, 10);
            Assert.AreEqual("Argentina", argentina);
        }

        [TestMethod]
        [TestCategory(GeoLocationCategory)]
        public void RegionFetch()
        {
            string tokyo = GeoLocation.GetRegionName("en", 1, 2);
            Assert.AreEqual("Tokyo", tokyo);

            string bermuda = GeoLocation.GetRegionName(LanguageID.Korean, 186, 1);
            Assert.AreEqual("버뮤다", bermuda);
        }
    }
}
