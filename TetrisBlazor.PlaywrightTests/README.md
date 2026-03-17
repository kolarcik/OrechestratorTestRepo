Playwright E2E tests

Prerequisites:
- Install Playwright browsers: run scripts/install-playwright.sh

Run tests:
- dotnet test TetrisBlazor.PlaywrightTests

If 'playwright' CLI is not available, you can install browsers via npm:
- npm i -D @playwright/test
- npx playwright install
