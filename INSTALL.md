# Compilation

Ouvrez la solution dans Visual Studio et lancez la compilation des 3 projets qu'elle contient.

# Déploiement

Placez-vous dans le répertoire `NuGet` et exécutez le script `Make-Install.ps1`.
Ce script exécute les étapes suivantes:

 1. Rassemblement des fichiers `.dll` produits à l'étape de compilation dans un nouveau dossier `lib`.
 2. Génération d'un fichier `.nupkg` à partir des fichiers `.dll` et du manifeste `.nuspec`. L'exécutable `NuGet.exe` peut être récupéré à cette adresse: https://dist.nuget.org/win-x86-commandline/latest/nuget.exe.
 3. Déploiement du NuGet généré à l'étape précédente vers un emplacement approprié (par défault: `~\.nuget\packages\<nom-du-NuGet>\<version-du-NuGet>\`). La bibliothèque est prête à être utilisée dans le Studio UiPath.
 
**Important**: pour la réussite du déploiement, tout projet UiPath utilisant actuellement la même version de la bibliothèque regénérée doit être fermé au préalable.