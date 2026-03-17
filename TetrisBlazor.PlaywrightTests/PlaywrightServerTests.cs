using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace TetrisBlazor.PlaywrightTests
{
    public class PlaywrightServerTests : IAsyncLifetime
    {
        private Process _serverProcess;

        public async Task InitializeAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --project ./TetrisBlazor.Server --urls http://localhost:5000",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            _serverProcess = Process.Start(psi);

            using var client = new HttpClient();
            var timeout = TimeSpan.FromSeconds(30);
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout)
            {
                try
                {
                    var resp = await client.GetAsync("http://localhost:5000");
                    if (resp.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch
                {
                    // ignore and retry
                }

                await Task.Delay(500);
            }

            // If we get here, server didn't start
            throw new Exception("TetrisBlazor.Server did not respond within timeout");
        }

        public Task DisposeAsync()
        {
            try
            {
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    _serverProcess.Kill(true);
                    _serverProcess.WaitForExit(5000);
                }
            }
            catch
            {
                // ignore cleanup errors
            }

            return Task.CompletedTask;
        }

        [Fact]
        public async Task HomePageLoadsAndContainsStart()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("http://localhost:5000", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });

            // Wait for either a Start button or an h1 to appear
            var start = page.Locator("text=Start");
            var header = page.Locator("h1");

            var found = false;
            try
            {
                if (await start.IsVisibleAsync()) found = true;
            }
            catch { }

            try
            {
                if (!found && await header.IsVisibleAsync()) found = true;
            }
            catch { }

            Assert.True(found, "Expected Start button or h1 to be visible on the main page");
        }
    }
}
