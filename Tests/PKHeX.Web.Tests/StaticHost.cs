using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PKHeX.Web.Tests;

/// <summary>Serves only a supplied Release wwwroot. No save-processing endpoints.</summary>
internal sealed class StaticHost : IDisposable
{
    private readonly HttpListener listener = new();
    private readonly string root;
    public string Url { get; }

    public StaticHost(string root)
    {
        this.root = Path.GetFullPath(root);
        var reservation = new TcpListener(IPAddress.Loopback, 0);
        reservation.Start();
        var port = ((IPEndPoint)reservation.LocalEndpoint).Port;
        reservation.Stop();
        Url = $"http://127.0.0.1:{port}/";
        listener.Prefixes.Add(Url);
        listener.Start();
        _ = Serve();
    }

    private async Task Serve()
    {
        while (listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await listener.GetContextAsync();
            }
            catch (Exception) when (!listener.IsListening)
            {
                return;
            }
            try
            {
                var urlPath = Uri.UnescapeDataString(context.Request.Url!.AbsolutePath);
                var subpath = urlPath.StartsWith("/PKHeX/", StringComparison.Ordinal);
                if (subpath)
                {
                    urlPath = urlPath[6..];
                }
                var file = Path.GetFullPath(Path.Combine(root, urlPath.TrimStart('/')));
                if (file == root)
                {
                    file = Path.Combine(root, "index.html");
                }
                if (!file.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.Ordinal) || !File.Exists(file) || context.Request.HttpMethod != "GET")
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    continue;
                }
                var bytes = await File.ReadAllBytesAsync(file);
                // The only deployment transformation is the documented static base href.
                if (subpath && Path.GetFileName(file) == "index.html")
                {
                    bytes = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes).Replace("<base href=\"/\"", "<base href=\"/PKHeX/\"", StringComparison.Ordinal));
                }
                context.Response.ContentType = Path.GetExtension(file) switch
                {
                    ".html" => "text/html", ".js" => "text/javascript", ".css" => "text/css",
                    ".json" => "application/json", ".wasm" => "application/wasm", _ => "application/octet-stream",
                };
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self'; connect-src 'self'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'";
                context.Response.ContentLength64 = bytes.Length;
                await context.Response.OutputStream.WriteAsync(bytes);
                context.Response.Close();
            }
            catch
            {
                context.Response.Abort();
            }
        }
    }

    public void Dispose()
    {
        listener.Stop();
        listener.Close();
    }
}
