using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Xunit;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Core.Tests.Saves;

public static class HomeTests
{
    private static IEnumerable<string> GetHomeEncrypted()
    {
        var folder = TestUtil.GetRepoPath();
        var path = Path.Combine(folder, "TestData");
        return Directory.EnumerateFiles(path, "*.eh3", SearchOption.TopDirectoryOnly);
    }

    [Fact]
    public static void CheckCrypto1()
    {
        var paths = GetHomeEncrypted();
        foreach (var f in paths)
        {
            var data = File.ReadAllBytes(f);

            var oldCHK = ReadUInt32LittleEndian(data.AsSpan(0xA, 4));
            var chk = HomeCrypto.GetChecksum1(data);
            oldCHK.Should().Be(chk);

            var version = ReadUInt16LittleEndian(data);
            bool encrypted = HomeCrypto.GetIsEncrypted(data, version);
            encrypted.Should().BeTrue();

            var ph1 = new PKH(data);
            HomeCrypto.IsKnownVersion(ph1.DataVersion).Should().BeTrue();

            var decrypted = HomeCrypto.Crypt(data);
            decrypted.Length.Should().Be(data.Length);
            decrypted.Length.Should().Be(ph1.Data.Length);
            for (int i = 0; i < decrypted.Length; i++)
                decrypted[i].Should().Be(ph1.Data[i]);

            bool check = HomeCrypto.GetIsEncrypted(decrypted, version);
            check.Should().BeFalse();

            ph1.Clone().Should().NotBeNull();

            var write = ph1.Rebuild();
            write.Length.Should().Be(decrypted.Length);
            for (int i = 0; i < decrypted.Length; i++)
                write[i].Should().Be(decrypted[i], $"Offset {i:X2}");

            var encrypt = HomeCrypto.Encrypt(write);
            encrypt.Length.Should().Be(data.Length);
            for (int i = 0; i < data.Length; i++)
                encrypt[i].Should().Be(data[i], $"Offset {i:X2}");
        }
    }
}
