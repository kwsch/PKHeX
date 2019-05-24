using FluentAssertions;
using PKHeX.Core;
using System.IO;
using Xunit;

namespace PKHeX.Tests.Saves
{
    public static class SMTests
    {
        private static SAV7 GetSave()
        {
            var folder = TestUtil.GetRepoPath();
            var path = Path.Combine(folder, "TestData", "SM Project 802.main");
            return new SAV7SM(File.ReadAllBytes(path));
        }

        [Fact]
        public static void ChecksumsValid()
        {
            GetSave().ChecksumsValid.Should().BeTrue();
        }

        [Fact]
        public static void ChecksumsUpdate()
        {
            var save = GetSave();
            var originalChecksumInfo = save.ChecksumInfo;
            var newSave = new SAV7SM(save.Write());

            save.ChecksumInfo.Should().BeEquivalentTo(originalChecksumInfo, "because the checksum should have been modified");
            save.ChecksumsValid.Should().BeTrue("because the checksum should be valid after write");
            newSave.ChecksumsValid.Should().BeTrue("because the checksums should be valid after reopening the save");
            newSave.ChecksumInfo.Should().BeEquivalentTo(save.ChecksumInfo, "because the checksums should be the same since write and open");
        }
    }
}
