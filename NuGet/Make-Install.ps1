# Configuration

$NUGET_FEED="$env:USERPROFILE\.nuget\packages"
$NUGET_EXE="NuGet.exe" # À supposer que NuGet.exe soit dans le $PATH

# Détermination de l'identifiant du package
$NUSPEC = [xml](Get-Content (Get-ChildItem -Filter *.nuspec))
$ID_NUPKG = $NUSPEC.package.metadata.id
$VERSION_NUPKG = $NUSPEC.package.metadata.version
$VERSION_DOTNET = $NUSPEC.package.metadata.dependencies.group.targetFramework

# Nettoyage
Remove-Item -Path *.nupkg -Force -ErrorAction Ignore
Remove-Item -Path lib -Recurse -Force -ErrorAction Ignore
mkdir lib -Force > $null

# Assemblage des fichiers à compiler
Copy-Item ../*/bin/Debug/* -Recurse -Destination .\lib\  -Force -Exclude *json,*pdb

# Compilation du NuPkg
& $NUGET_EXE Pack -Exclude *.ps1 -Exclude *\*\*.deps.json #> $null

# Suppression de l'ancien paquet dans le feed NuGet s'il existe
$OUTDIR = Join-Path $NUGET_FEED (Join-Path $ID_NUPKG $VERSION_NUPKG)
Remove-Item -Path $OUTDIR -Recurse -Force -ErrorAction Ignore

# Déploiement du nouveau paquet
& $NUGET_EXE Add $ID_NUPKG'.'$VERSION_NUPKG.nupkg -Source $NUGET_FEED -Expand

if ($?) {
	"Le déploiement a réussi."
} else {
	"Le déploiement a échoué ; vérifiez qu'il ne reste pas de projet ouvert dans le Studio UiPath utilisant cette bibliothèque."
}
