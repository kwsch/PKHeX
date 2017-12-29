using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;

namespace PKHeX.Tests.Saves
{
    [TestClass]
    public class SMTests
    {
        public const string TestCategory = "SM Save Data Tests";

        private SAV7 GetSave()
        {
            return new SAV7(Properties.Resources.SM_Project_802);
        }

        [TestMethod]
        [TestCategory(TestCategory)]
        public void TestChecksumRead()
        {
            Assert.IsTrue(GetSave().ChecksumsValid, "Checksums are not valid.");
        }

        [TestMethod]
        [TestCategory(TestCategory)]
        public void TestChecksumUpdate()
        {
            var save = GetSave();
            var saveChecksumInfo = save.ChecksumInfo;
            var newSave = new SAV7(save.Write(false, false));
            Assert.AreEqual(saveChecksumInfo, save.ChecksumInfo, "Checksum info modified on Write");
            Assert.IsTrue(save.ChecksumsValid, "Checksum not valid after write");
            Assert.IsTrue(newSave.ChecksumsValid, "Checksums are not valid after open");
            Assert.AreEqual(save.ChecksumInfo, newSave.ChecksumInfo, "Checksums changed since write and open");
        }
    }
}
