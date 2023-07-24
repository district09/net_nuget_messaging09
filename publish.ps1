$distPath = "dist"

$nugetApikey = $env:NUGET_API_KEY

if (-not$nugetApikey)
{
    Write-Warning "NUGET_API_KEY not set"
    $nugetApikey = Read-Host -Prompt "enter nuget api key"
}

Push-Location $distPath

Write-Progress "Pushing nuget packages to nuget.org"

dotnet nuget push *.nupkg --api-key $nugetApikey --skip-duplicates --source https://api.nuget.org/v3/index.json

Pop-Location
