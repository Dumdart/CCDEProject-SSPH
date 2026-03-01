Write-Host "Starting Local Development Environment" -ForegroundColor Blue

# Call checkresources.ps1 with -Fix to auto-create missing files
& "$PSScriptRoot\checkresources.ps1" -Fix

if ($LASTEXITCODE -ne 0) {
    Write-Host "Resource check failed - fix issues and retry." -ForegroundColor Red
    exit 1
}

# Build and start Azure Functions
Write-Host "Building Azure Functions..." -ForegroundColor Green
Set-Location PortfolioApi
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Starting Azure Functions..." -ForegroundColor Green
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "func start --port 7071"
} else {
    Write-Host "Azure Functions build failed" -ForegroundColor Red
    Set-Location ..
    exit 1
}

Set-Location ..

# Start Vite
Write-Host "Starting Vite dev server..." -ForegroundColor Green
Set-Location client
Start-Process powershell -ArgumentList "-NoExit", "-Command", "npm run dev"
Set-Location ..

Write-Host "`nDevelopment environment started!" -ForegroundColor Green
Write-Host "Client: http://localhost:5173"
Write-Host "PortfolioApi: http://localhost:7071"
Write-Host "Press Ctrl+C to stop"