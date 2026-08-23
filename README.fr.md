# Free Launcher Builder

Une application **WPF Windows** qui permet de **configurer et générer un lanceur de jeu entièrement autonome** sans écrire une seule ligne de code.

Au lieu de modifier manuellement des fichiers de configuration JSON, vous remplissez une interface graphique et le générateur produit un **unique EXE portable** qui lance votre jeu — configuration incluse.

## Fonctionnalités

- **Lanceur personnalisable** — configurez le titre de la fenêtre, le nom du jeu et la police d'affichage.
- **Style du bouton Play** — texte, couleur de fond, de texte et de bordure, ainsi que les états survol et appui (avec sélecteur de couleur intégré).
- **Fond et icône** — choisissez une image de fond et une icône pour le lanceur.
- **Détection du jeu** — sélectionnez votre exécutable ; le dossier du jeu entier est copié automatiquement.
- **Build portable** — tous les chemins d'actifs et du jeu sont convertis en chemins relatifs pour que le lanceur généré fonctionne depuis n'importe quel emplacement.
- **Configuration intégrée** — le modèle `FreeLauncher.exe` est extrait et la configuration JSON est injectée directement dans l'EXE généré (`[FreeLauncher.exe][JSON][taille JSON][marqueur]`).
- **Validation** — les champs obligatoires minimums sont vérifiés avant la génération.

## Prérequis

- [SDK .NET 8](https://dotnet.microsoft.com/download) (avec support WPF / Windows Forms sur Windows)
- Windows 10/11

## Build

```bash
dotnet restore
dotnet build -c Release
```

Le projet est configuré pour publier un exécutable **autonome (self-contained) en fichier unique** :

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Utilisation

1. Lancez le générateur.
2. Renseignez le nom du lanceur, le nom du jeu et sélectionnez l'exécutable du jeu.
3. Sélectionnez éventuellement une image de fond, une icône, une police et personnalisez les couleurs du bouton Play.
4. Cliquez sur **Générer le lanceur**, choisissez une destination, puis le générateur crée :
   - le `Launcher.exe` autonome (avec la configuration intégrée),
   - un dossier `Assets/` contenant le fond et l'icône,
   - le dossier du jeu copié à côté.

Tout est placé de manière relative à l'EXE généré : le dossier est donc **portable**. Compressez-le, déplacez-le, partagez-le.

## Comment fonctionne le lanceur généré

- `Templates/FreeLauncher.exe` est le moteur de lanceur pré-compilé, intégré en tant que ressource dans ce générateur.
- Lors de la génération, le générateur :
  1. valide la saisie,
  2. copie le fond et l'icône dans `Assets/`,
  3. copie récursivement le dossier du jeu,
  4. convertit les chemins absolus en chemins relatifs portables,
  5. extrait `FreeLauncher.exe`,
  6. ajoute la configuration sérialisée à la fin de l'EXE.

Structure du fichier généré :

```
Launcher.exe
Assets/
  background.png
  icon.ico
VotreJeu/
  VotreJeu.exe
  ...
```

## Structure du projet

```
FreeLauncherBuilder/
├── App.xaml / App.xaml.cs        # Point d'entrée de l'application
├── MainWindow.xaml               # Interface du générateur
├── MainWindow.xaml.cs            # Logique du générateur, modèle de config & génération EXE
├── Templates/
│   └── FreeLauncher.exe          # Modèle de lanceur intégré (Git LFS)
└── FreeLauncherBuilder.csproj
```

## Modèle de configuration

La configuration JSON générée (`LauncherConfig`) comprend :

| Champ | Description |
| --- | --- |
| `launcherName` | Texte affiché dans la barre de titre du lanceur |
| `gameName` | Nom du jeu affiché dans le lanceur |
| `gameNameFontFamily` | Police utilisée pour le nom du jeu |
| `gameExecutable` | Chemin relatif vers l'exécutable du jeu |
| `backgroundPath` | Chemin relatif vers l'image de fond |
| `iconPath` | Chemin relatif vers l'icône du lanceur |
| `playButtonText` | Texte à l'intérieur du bouton Play |
| `playButtonBackground` / `Foreground` / `Border` | Couleurs normales du bouton |
| `playButtonHoverBackground` / `Foreground` | Couleurs à l'état survol |
| `playButtonPressedBackground` | Couleur à l'état appui |

## Licence

Projet personnel — aucune licence spécifiée.

---

🇬🇧 Read the documentation in English: [README.md](README.md)