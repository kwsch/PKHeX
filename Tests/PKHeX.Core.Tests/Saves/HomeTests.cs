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
        return Directory.EnumerateFiles(path, "*.eh1", SearchOption.TopDirectoryOnly);
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

            bool encrypted = HomeCrypto.GetIsEncrypted1(data);
            encrypted.Should().BeTrue();

            var ph1 = new PKH(data);
            ph1.DataVersion.Should().Be(1);

            var decrypted = HomeCrypto.Crypt1(data);
            decrypted.Length.Should().Be(data.Length);
            decrypted.Length.Should().Be(ph1.Data.Length);
            for (int i = 0; i < decrypted.Length; i++)
                decrypted[i].Should().Be(ph1.Data[i]);

            bool check = HomeCrypto.GetIsEncrypted1(decrypted);
            check.Should().BeFalse();

            ph1.Clone().Should().NotBeNull();

            var write = ph1.Rebuild();
            write.Length.Should().Be(decrypted.Length);
            for (int i = 0; i < decrypted.Length; i++)
                decrypted[i].Should().Be(write[i]);

            var encrypt = HomeCrypto.Encrypt(write);
            encrypt.Length.Should().Be(data.Length);
            for (int i = 0; i < data.Length; i++)
                encrypt[i].Should().Be(data[i]);
        }
    }
}
