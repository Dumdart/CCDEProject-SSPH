param([switch]$Fix)

Write-Host "Checking resources..." -ForegroundColor Blue

$issues = @()

# Check local.settings.json
if (!(Test-Path "./PortfolioApi/local.settings.json")) {
    Write-Host "Warning: local.settings.json not found" -ForegroundColor Red
    $issues += "local.settings.json"
    if ($Fix) {
        Write-Host "Creating template local.settings.json..."
@"
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "API_KEY": "your-local-api-key",

    "COSMOS_ENDPOINT": "your-local-cosmos-endpoint",
    "COSMOS_KEY": "your-local-cosmos-key (primary)",
    "COSMOS_DATABASE_ID": "PortfolioDB",
    "COSMOS_CONTAINER_ID": "PageViews",

    "GEMINI_API_KEY": "your-gemini-api-key"
  }
}
"@ | Out-File -FilePath "./PortfolioApi/local.settings.json" -Encoding UTF8
    }
}

# Check .env.local
if (!(Test-Path "./client/.env.local")) {
    Write-Host "Warning: .env.local not found" -ForegroundColor Red
    $issues += ".env.local"
    if ($Fix) {
        Write-Host "Creating template .env.local..."
@"
VITE_API_URL=http://localhost:7071
VITE_API_KEY=your-local-api-key
"@ | Out-File -FilePath "./client/.env.local" -Encoding UTF8
    }
}

if ($issues.Count -eq 0) {
    Write-Host "All resources OK." -ForegroundColor Green
    exit 0
} else {
    Write-Host "Issues found: $($issues -join ', ')" -ForegroundColor Yellow
    exit 1
}