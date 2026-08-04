using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PKHeX.Avalonia.Tests;

/// <summary>Minimal fake <see cref="HttpMessageHandler"/> for tests that need to stub an HTTP response without touching the network.</summary>
internal sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(responder(request));
}
