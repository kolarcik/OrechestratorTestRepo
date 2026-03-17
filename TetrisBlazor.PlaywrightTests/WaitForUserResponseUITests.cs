using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace TetrisBlazor.PlaywrightTests
{
    public class WaitForUserResponseUITests : IAsyncLifetime
    {
        private Process _serverProcess;
        private const int PromptTimeoutMs = 8000; // configurable timeout used by tests

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
                // ignore
            }

            return Task.CompletedTask;
        }

        // Helper to open page and wait for prompt
        private async Task<IPage> NavigateAndWaitForPromptAsync(IPlaywright playwright, int timeoutMs = PromptTimeoutMs)
        {
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await page.GotoAsync("http://localhost:5000", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 30000 });

            // Wait for the prompt element; selectors below are configurable and may need adjustment
            await page.Locator("#response-prompt").WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
            return page;
        }

        [Fact]
        public async Task Success_WhenUserResponds_AppProceeds()
        {
            using var playwright = await Playwright.CreateAsync();
            var page = await NavigateAndWaitForPromptAsync(playwright);

            // Provide input and submit
            var input = page.Locator("#response-input");
            var submit = page.Locator("#response-submit");

            await input.FillAsync("expected answer");
            await submit.ClickAsync();

            // Assert success indicator appears
            await page.Locator(".response-success").WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
            Assert.True(await page.Locator(".response-success").IsVisibleAsync());

            await page.Context.CloseAsync();
        }

        [Fact]
        public async Task Timeout_WhenNoResponse_ShowsTimeoutState()
        {
            using var playwright = await Playwright.CreateAsync();
            var page = await NavigateAndWaitForPromptAsync(playwright);

            // Do not interact; wait for timeout indicator which app should show
            await page.Locator(".response-timeout").WaitForAsync(new LocatorWaitForOptions { Timeout = PromptTimeoutMs + 5000 });
            Assert.True(await page.Locator(".response-timeout").IsVisibleAsync());

            await page.Context.CloseAsync();
        }

        [Fact]
        public async Task Cancel_WhenUserCancels_AppShowsCancelledState()
        {
            using var playwright = await Playwright.CreateAsync();
            var page = await NavigateAndWaitForPromptAsync(playwright);

            var cancel = page.Locator("#response-cancel");
            await cancel.ClickAsync();

            await page.Locator(".response-cancelled").WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
            Assert.True(await page.Locator(".response-cancelled").IsVisibleAsync());

            await page.Context.CloseAsync();
        }
    }
}
