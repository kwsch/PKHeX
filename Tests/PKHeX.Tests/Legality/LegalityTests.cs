using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using PKHeX.Core;

namespace PKHeX.Tests.Legality
{
    [TestClass]
    public class LegalityTest
    {
        private const string LegalityWordCategory = "PKM Wordfilter Tests";
        private const string LegalityValidCategory = "PKM Validity Tests";

        static LegalityTest()
        {
            if (!EncounterEvent.Initialized)
                EncounterEvent.RefreshMGDB();
        }

        [TestMethod]
        [TestCategory(LegalityWordCategory)]
        public void BadwordTest()
        {
            string[] phrases =
            {
                "censor", "buttnugget", "18넘"
            };
            foreach (var phrase in phrases)
                Assert.IsTrue(WordFilter.IsFiltered(phrase, out _), $"Word not filtered: {phrase}.");
        }

        [TestMethod]
        [TestCategory(LegalityValidCategory)]
        public void VerifyLegalityTest()
        {
            var folder = Directory.GetCurrentDirectory();
            while (!folder.EndsWith(nameof(Tests)))
                folder = Directory.GetParent(folder).FullName;

            folder = Path.Combine(folder, "Legality");
            Legal.AllowGBCartEra = true;
            VerifyAll(folder, "Legal", true);
            VerifyAll(folder, "Illegal", false);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void VerifyAll(string folder, string name, bool IsValid)
        {
            var path = Path.Combine(folder, name);
            Assert.IsTrue(Directory.Exists(path), $"Folder does not exist: {folder}.");
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                Assert.IsNotNull(fi, $"Invalid file: {file}");
                Assert.IsTrue(PKX.IsPKM(fi.Length), $"Invalid file in {fi.Directory.Name} folder.");

                var data = File.ReadAllBytes(file);
                var format = PKX.GetPKMFormatFromExtension(file[file.Length - 1], -1);
                if (format > 10)
                    format = 6;
                var pkm = PKMConverter.GetPKMfromBytes(data, prefer: format);
                Assert.IsNotNull(pkm, $"Failed to load PKM: {new FileInfo(file).Name}.");

                Legal.AllowGBCartEra = fi.DirectoryName.Contains("GBCartEra");
                Legal.AllowGen1Tradeback = fi.DirectoryName.Contains("1 Tradeback");
                var legality = new LegalityAnalysis(pkm);
                Assert.IsTrue(legality.Valid == IsValid, $"Failed to validate PKM as {(IsValid ? "Valid" : "Invalid")}: {fi.Directory.Name}\\{fi.Name}.");
            }
        }
    }
}
