using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;

namespace PKHeX.Tests.Legality
{
    [TestClass]
    public class LegalityTest
    {
        private const string LegalityTestCategory = "PKM PIDIV Matching Tests";

        [TestMethod]
        [TestCategory(LegalityTestCategory)]
        public void BadwordTest()
        {
            string[] phrases =
            {
                "censor", "buttnugget", "18넘"
            };
            foreach (var phrase in phrases)
                Assert.IsTrue(WordFilter.IsFiltered(phrase, out _), $"Word not filtered: {phrase}.");
        }
    }
}
