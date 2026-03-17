#!/usr/bin/env bash
set -e
if command -v playwright >/dev/null 2>&1; then
  echo "Using playwright CLI to install browsers"
  playwright install
else
  echo "Playwright CLI not found. You can install via dotnet tool install --global Microsoft.Playwright.CLI and run 'playwright install'"
  echo "Or use npm: npm i -D @playwright/test && npx playwright install"
fi
