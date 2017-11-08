using Microsoft.VisualStudio.TestTools.UnitTesting;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var newSave = new SAV7(save.Write(false, false));
            Assert.IsTrue(newSave.ChecksumsValid, "Checksums are not valid.");
            Assert.AreEqual(save.ChecksumInfo, newSave.ChecksumInfo, "Checksums changed since saving without modification.");
        }
    }
}
