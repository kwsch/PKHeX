using PKHeX.Core;
using PKHeX.Avalonia.Services;

namespace PKHeX.Avalonia.Tests;

public class QrCodeTests
{
    private readonly QrCodeService _service = new();

    [Fact]
    public void GenerateAndDecode_RoundTripsAsciiMessage()
    {
        const string message = "http://lunarcookies.github.io/b1s1.html#dGVzdA==";
        var png = _service.GeneratePng(message);

        Assert.True(png.Length > 0);
        Assert.Equal(message, _service.DecodePng(png));
    }

    [Fact]
    public void GenerateAndDecode_RoundTripsPk7ToIdenticalBytes()
    {
        // Gen 7 QR payloads are raw bytes stored char-per-byte (ISO-8859-1), the hardest case.
        var pk = new PK7
        {
            Species = (ushort)Species.Rowlet,
            CurrentLevel = 5,
            Move1 = (ushort)Move.Tackle,
            PID = 0x12345678,
            EncryptionConstant = 0x87654321,
        };
        pk.RefreshChecksum();

        var message = QRMessageUtil.GetMessage(pk);
        var png = _service.GeneratePng(message);
        var decodedMessage = _service.DecodePng(png);

        Assert.NotNull(decodedMessage);
        var decoded = QRMessageUtil.GetPKM(decodedMessage, EntityContext.Gen7);
        Assert.NotNull(decoded);
        var decodedPk7 = Assert.IsType<PK7>(decoded);
        Assert.Equal(pk.Species, decodedPk7.Species);
        Assert.Equal(pk.PID, decodedPk7.PID);
        Assert.Equal(pk.EncryptionConstant, decodedPk7.EncryptionConstant);
        Assert.Equal(pk.Data[..pk.SIZE_STORED].ToArray(), decodedPk7.Data[..pk.SIZE_STORED].ToArray());
    }

    [Fact]
    public void Decode_NonQrImage_ReturnsNull()
    {
        // A 1x1 transparent PNG.
        var png = Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");
        Assert.Null(_service.DecodePng(png));
    }

    [Fact]
    public void Decode_GarbageBytes_ReturnsNull()
    {
        Assert.Null(_service.DecodePng([1, 2, 3, 4]));
    }
}
