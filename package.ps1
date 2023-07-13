$distPath = "dist"
$srcLocation = "src"

Write-Progress "Creating package directory." "Initializing..."

if (!(Test-Path $distPath))
{
    mkdir $distPath
}
else
{
    Get-ChildItem $distPath\* -Include * -Recurse | Remove-Item
}

Push-Location $srcLocation

Write-Progress "Restoring dependencies"

dotnet restore

Write-Progress "Building packages"

dotnet build --configuration Release

Pop-Location

Write-Progress "Moving packages to $distPath folder"

dir -Path . -Filter *.nupkg -Recurse | %{ Move-Item $_.FullName $distPath }

Write-Progress "Moving debug symbols to $distPath folder"

dir -Path . -Filter *.snupkg -Recurse | %{ Move-Item $_.FullName $distPath }