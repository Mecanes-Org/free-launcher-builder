using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FreeLauncherBuilder
{
    /// <summary>
    /// Main window of the Launcher Config Builder.
    ///
    /// This application allows the developer to configure
    /// the launcher without manually editing a JSON file.
    ///
    /// The values entered in the UI are converted into
    /// a LauncherConfig object and then serialized to JSON.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Creates the main builder window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            LoadInstalledFonts();
        }


        // =========================================================
        // INITIALIZATION
        // =========================================================

        /// <summary>
        /// Loads all fonts installed on the computer
        /// into the font ComboBox.
        /// </summary>
        private void LoadInstalledFonts()
        {
            // Clear the items that may already exist in the XAML.
            GameFontComboBox.Items.Clear();

            // Fonts.SystemFontFamilies contains every font family
            // installed on the current Windows computer.
            foreach (FontFamily font in Fonts.SystemFontFamilies)
            {
                GameFontComboBox.Items.Add(font.Source);
            }

            // Select a default font if available.
            GameFontComboBox.SelectedItem = "Segoe UI";

            // If Segoe UI was not found for any reason,
            // select the first available font.
            if (GameFontComboBox.SelectedIndex == -1 &&
                GameFontComboBox.Items.Count > 0)
            {
                GameFontComboBox.SelectedIndex = 0;
            }
        }


        // =========================================================
        // FILE SELECTION
        // =========================================================

        /// <summary>
        /// Opens a file dialog that allows the user to select
        /// the executable file of the game.
        /// </summary>
        private void BrowseExecutableButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Game Executable",

                // Only executable files are displayed by default.
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                // Keep the full source path inside the Builder.
                //
                // Example:
                // C:\Projects\LastRun\Release\LastRun.exe
                GameExecutableTextBox.Text = dialog.FileName;
            }
        }


        /// <summary>
        /// Opens a file dialog that allows the user
        /// to select the launcher background image.
        /// </summary>
        private void BrowseBackgroundButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string? selectedFile = SelectImage(
                "Select Launcher Background"
            );

            if (selectedFile != null)
            {
                BackgroundPathTextBox.Text = selectedFile;
            }
        }


        /// <summary>
        /// Opens a file dialog that allows the user
        /// to select a logo image.
        ///
        /// Logo support is prepared in the builder UI,
        /// but the current launcher configuration does
        /// not yet use LogoPath.
        /// </summary>
        private void BrowseLogoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string? selectedFile = SelectImage(
                "Select Launcher Logo"
            );

            if (selectedFile != null)
            {
                LogoPathTextBox.Text = selectedFile;
            }
        }


        /// <summary>
        /// Opens a file dialog that allows the user
        /// to select the launcher icon.
        /// </summary>
        private void BrowseIconButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Launcher Icon",

                Filter =
                    "Icon files (*.ico)|*.ico|" +
                    "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|" +
                    "All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                IconPathTextBox.Text = dialog.FileName;
            }
        }


        /// <summary>
        /// Opens a reusable image selection dialog.
        ///
        /// This method is used by several Browse buttons
        /// to avoid duplicating the same OpenFileDialog code.
        /// </summary>
        /// <param name="title">
        /// Text displayed in the file selection window.
        /// </param>
        /// <returns>
        /// Full path of the selected image,
        /// or null if the user cancels the dialog.
        /// </returns>
        private string? SelectImage(string title)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = title,

                Filter =
                    "Image files (*.png;*.jpg;*.jpeg;*.webp)|" +
                    "*.png;*.jpg;*.jpeg;*.webp|" +
                    "All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return null;
        }


        // =========================================================
        // CONFIG GENERATION
        // =========================================================

        /// <summary>
        /// Generates a LauncherConfig object from all values
        /// currently entered in the builder interface.
        /// </summary>
        /// <returns>
        /// A complete LauncherConfig object.
        /// </returns>
        private LauncherConfig CreateConfigFromUI()
        {
            LauncherConfig config = new LauncherConfig
            {
                LauncherName = LauncherNameTextBox.Text.Trim(),

                GameName = GameNameTextBox.Text.Trim(),

                GameNameFontFamily = GameFontComboBox.SelectedItem?.ToString() ?? "Segoe UI",

                GameExecutable = GameExecutableTextBox.Text.Trim(),

                BackgroundPath = BackgroundPathTextBox.Text.Trim(),

                IconPath = IconPathTextBox.Text.Trim(),

                PlayButtonText = PlayButtonTextTextBox.Text.Trim(),

                PlayButtonBackground = PlayButtonBackgroundTextBox.Text.Trim(),

                PlayButtonForeground = PlayButtonForegroundTextBox.Text.Trim(),

                PlayButtonBorder = PlayButtonBorderTextBox.Text.Trim(),

                PlayButtonHoverBackground = PlayButtonHoverBackgroundTextBox.Text.Trim(),

                PlayButtonHoverForeground = PlayButtonHoverForegroundTextBox.Text.Trim(),

                PlayButtonPressedBackground = PlayButtonPressedBackgroundTextBox.Text.Trim()
            };

            return config;
        }


        /// <summary>
        /// Validates the minimum information required
        /// before generating the launcher configuration.
        /// </summary>
        /// <returns>
        /// True when the configuration can be generated.
        /// Otherwise, false.
        /// </returns>
        private bool ValidateConfig()
        {
            if (string.IsNullOrWhiteSpace(
                LauncherNameTextBox.Text))
            {
                ShowValidationError(
                    "Launcher name cannot be empty."
                );

                return false;
            }

            // if the game name is empty, show a validation error
            //if (string.IsNullOrWhiteSpace( GameNameTextBox.Text) )
            //{
            //    ShowValidationError(
            //        "Game name cannot be empty."
            //    );

            //    return false;
            //}

            if (string.IsNullOrWhiteSpace(
                GameExecutableTextBox.Text))
            {
                ShowValidationError(
                    "Please select the game executable."
                );

                return false;
            }

            return true;
        }


        /// <summary>
        /// Displays a validation error to the developer.
        /// </summary>
        private void ShowValidationError(string message)
        {
            MessageBox.Show(
                message,
                "Invalid Configuration",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        // =========================================================
        // COLOR SELECTION
        // =========================================================
        private void SelectColor(System.Windows.Controls.TextBox targetTextBox)
        {
            using System.Windows.Forms.ColorDialog dialog =
                new System.Windows.Forms.ColorDialog();

            byte alpha = 255;

            try
            {
                System.Windows.Media.Color currentColor =
                    (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString(
                        targetTextBox.Text
                    );

                alpha = currentColor.A;

                dialog.Color = System.Drawing.Color.FromArgb(
                    currentColor.R,
                    currentColor.G,
                    currentColor.B
                );
            }
            catch
            {
                dialog.Color = System.Drawing.Color.Black;
            }

            if (dialog.ShowDialog() ==
                System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = dialog.Color;

                targetTextBox.Text =
                    $"#{alpha:X2}" +
                    $"{selectedColor.R:X2}" +
                    $"{selectedColor.G:X2}" +
                    $"{selectedColor.B:X2}";
            }
        }

        // =========================================================
        // COLOR SELECTION
        // =========================================================
        private void PlayButtonBackgroundColorButton_Click( object sender, RoutedEventArgs e)
        {
            SelectColor(PlayButtonBackgroundTextBox);
        }

        private void PlayButtonForegroundColorButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectColor(PlayButtonForegroundTextBox);
        }

        private void PlayButtonBorderColorButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectColor(PlayButtonBorderTextBox);
        }

        private void PlayButtonHoverBackgroundColorButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectColor(PlayButtonHoverBackgroundTextBox);
        }

        private void PlayButtonHoverForegroundColorButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectColor(PlayButtonHoverForegroundTextBox);
        }

        private void PlayButtonPressedBackgroundColorButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            SelectColor(PlayButtonPressedBackgroundTextBox);
        }


        /// <summary>
        /// Generates the final launcher executable.
        ///
        /// The generation process:
        ///
        /// 1. Reads configuration values from the UI.
        /// 2. Extracts the FreeLauncher.exe template.
        /// 3. Copies launcher assets into the output folder.
        /// 4. Replaces absolute asset paths with relative paths.
        /// 5. Embeds the configuration directly into the generated EXE.
        /// </summary>
        /// <summary>
        /// Generates the final launcher package.
        ///
        /// Generation process:
        ///
        /// 1. Reads all values from the builder UI.
        /// 2. Determines the output directory.
        /// 3. Copies the background image.
        /// 4. Copies the launcher icon.
        /// 5. Copies the complete game directory.
        /// 6. Converts file paths into portable relative paths.
        /// 7. Extracts the FreeLauncher.exe template.
        /// 8. Embeds the configuration inside the generated launcher.
        /// </summary>
        private void GenerateLauncherButton_Click( object sender, RoutedEventArgs e)
        {
            // ---------------------------------------------------------
            // VALIDATION
            // ---------------------------------------------------------

            if (!ValidateConfig())
            {
                return;
            }


            // ---------------------------------------------------------
            // ASK WHERE THE LAUNCHER MUST BE GENERATED
            // ---------------------------------------------------------

            SaveFileDialog dialog =
                new SaveFileDialog
                {
                    Title = "Generate Launcher",

                    FileName = $"{GetSafeFileName(LauncherNameTextBox.Text)}.exe",

                    // FileName = $"{GameNameTextBox.Text.Trim()}Launcher.exe",

                    DefaultExt = ".exe",

                    Filter = "Windows executable (*.exe)|*.exe"
                };


            if (dialog.ShowDialog() != true)
            {
                return;
            }


            try
            {
                // -----------------------------------------------------
                // OUTPUT DIRECTORY
                // -----------------------------------------------------

                string outputDirectory =
                    Path.GetDirectoryName(
                        dialog.FileName
                    )
                    ?? throw new InvalidOperationException(
                        "Unable to determine the output directory."
                    );


                // -----------------------------------------------------
                // CREATE CONFIG OBJECT
                // -----------------------------------------------------

                LauncherConfig config = CreateConfigFromUI();


                // -----------------------------------------------------
                // COPY BACKGROUND
                // -----------------------------------------------------

                config.BackgroundPath = CopyAssetToOutput(
                        BackgroundPathTextBox.Text.Trim(),
                        outputDirectory,
                        "background"
                    );


                // -----------------------------------------------------
                // COPY ICON
                // -----------------------------------------------------

                config.IconPath = CopyAssetToOutput(
                        IconPathTextBox.Text.Trim(),
                        outputDirectory,
                        "icon"
                    );


                // -----------------------------------------------------
                // COPY GAME
                // -----------------------------------------------------

                config.GameExecutable = CopyGameToOutput(
                        GameExecutableTextBox.Text.Trim(),
                        outputDirectory
                    );


                // -----------------------------------------------------
                // EXTRACT LAUNCHER TEMPLATE
                // -----------------------------------------------------

                ExtractLauncherTemplate(
                    dialog.FileName
                );


                // -----------------------------------------------------
                // EMBED FINAL CONFIGURATION
                // -----------------------------------------------------

                EmbedConfigIntoLauncher(
                    dialog.FileName,
                    config
                );


                // -----------------------------------------------------
                // SUCCESS
                // -----------------------------------------------------

                MessageBox.Show(
                    "Launcher generated successfully!\n\n" +
                    $"Output:\n{dialog.FileName}\n\n" +
                    $"Game executable:\n{config.GameExecutable}",
                    "Launcher Generated",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The launcher could not be generated.\n\n" +
                    ex.Message,
                    "Generation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        // ========================================================
        // HELPER METHODS
        // ========================================================
        private string GetSafeFileName(string name)
        {
            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '_');
            }

            return name.Trim();
        }


        /// <summary>
        /// Copies an asset selected by the user into the generated
        /// launcher's Assets folder.
        ///
        /// The method returns the relative path that must be stored
        /// inside the launcher configuration.
        ///
        /// Example:
        /// C:\Images\myBackground.png
        ///
        /// becomes:
        ///
        /// Assets/background.png
        /// </summary>
        private string CopyAssetToOutput( string sourcePath, string outputDirectory, string destinationName)
        {
            // No file selected.
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return "";
            }

            // Make sure the selected file actually exists.
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException(
                    $"The selected asset could not be found:\n{sourcePath}"
                );
            }

            // Create:
            //
            // OutputFolder/Assets
            string assetsDirectory = Path.Combine(outputDirectory, "Assets");

            Directory.CreateDirectory(assetsDirectory);


            // Keep the original file extension.
            //
            // Example:
            // .png
            // .jpg
            // .ico
            string extension = Path.GetExtension(sourcePath);


            // Example:
            //
            // background.png
            string finalFileName = destinationName + extension;


            // Full destination path.
            string destinationPath = Path.Combine(
                    assetsDirectory,
                    finalFileName
                );


            // Avoid copying the file onto itself.
            if (!string.Equals( Path.GetFullPath(sourcePath), Path.GetFullPath(destinationPath), StringComparison.OrdinalIgnoreCase) )
            {
                File.Copy(
                    sourcePath,
                    destinationPath,
                    overwrite: true
                );
            }


            // Return the relative path used by Launcher.exe.
            //
            // Example:
            // Assets/background.png
            return Path.Combine( "Assets", finalFileName );
        }



        /// <summary>
        /// Recursively copies an entire directory and all of its contents
        /// to another location.
        ///
        /// Existing files in the destination directory are overwritten.
        /// </summary>
        /// <param name="sourceDirectory">
        /// Full path of the directory that must be copied.
        /// </param>
        /// <param name="destinationDirectory">
        /// Full path of the destination directory.
        /// </param>
        private void CopyDirectory( string sourceDirectory, string destinationDirectory)
        {
            // Make sure the source directory exists.
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException(
                    $"The game directory could not be found:\n{sourceDirectory}"
                );
            }

            // Create the destination folder if it does not exist.
            Directory.CreateDirectory(destinationDirectory);

            // ---------------------------------------------------------
            // COPY FILES
            // ---------------------------------------------------------

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(file);

                string destinationFile =
                    Path.Combine(
                        destinationDirectory,
                        fileName
                    );

                File.Copy(
                    file,
                    destinationFile,
                    overwrite: true
                );
            }


            // ---------------------------------------------------------
            // COPY SUBDIRECTORIES
            // ---------------------------------------------------------

            foreach (string directory in Directory.GetDirectories(sourceDirectory))
            {
                string directoryName = Path.GetFileName(directory);

                string destinationSubDirectory = Path.Combine(
                        destinationDirectory,
                        directoryName
                    );

                // Recursively copy the subdirectory.
                CopyDirectory(
                    directory,
                    destinationSubDirectory
                );
            }
        }



        /// <summary>
        /// Copies the folder containing the selected game executable
        /// into the generated launcher directory.
        ///
        /// The method returns the relative path to the copied executable.
        ///
        /// Example:
        ///
        /// Selected executable:
        /// C:\Games\LastRun\LastRun.exe
        ///
        /// Generated launcher:
        /// D:\Output\LastRunLauncher.exe
        ///
        /// Result:
        /// D:\Output\LastRun\LastRun.exe
        ///
        /// Returned configuration path:
        /// LastRun/LastRun.exe
        /// </summary>
        private string CopyGameToOutput( string executablePath, string outputDirectory)
        {
            // ---------------------------------------------------------
            // VALIDATE EXECUTABLE
            // ---------------------------------------------------------

            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw new InvalidOperationException(
                    "No game executable was selected."
                );
            }

            if (!File.Exists(executablePath))
            {
                throw new FileNotFoundException(
                    $"The selected game executable could not be found:\n{executablePath}"
                );
            }


            // ---------------------------------------------------------
            // GET GAME DIRECTORY
            // ---------------------------------------------------------

            string? sourceGameDirectory = Path.GetDirectoryName(executablePath);

            if (string.IsNullOrWhiteSpace(sourceGameDirectory))
            {
                throw new InvalidOperationException(
                    "Unable to determine the game directory."
                );
            }


            // Example:
            //
            // C:\Games\LastRun\
            //
            // becomes:
            //
            // LastRun
            string gameFolderName = new DirectoryInfo( sourceGameDirectory ).Name;


            // ---------------------------------------------------------
            // DESTINATION
            // ---------------------------------------------------------

            string destinationGameDirectory = Path.Combine(
                    outputDirectory,
                    gameFolderName
                );


            // Prevent accidentally copying a directory into itself.
            string sourceFullPath = Path.GetFullPath(sourceGameDirectory);

            string destinationFullPath = Path.GetFullPath(destinationGameDirectory);

            if (destinationFullPath.StartsWith( sourceFullPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "The generated launcher cannot be placed inside the game directory."
                );
            }


            // ---------------------------------------------------------
            // COPY GAME
            // ---------------------------------------------------------

            CopyDirectory(
                sourceGameDirectory,
                destinationGameDirectory
            );


            // ---------------------------------------------------------
            // CREATE RELATIVE EXECUTABLE PATH
            // ---------------------------------------------------------

            string executableFileName = Path.GetFileName(executablePath);

            // Use '/' because it makes the JSON easier to read.
            //
            // Example:
            // LastRun/LastRun.exe
            string relativeExecutablePath = $"{gameFolderName}/{executableFileName}";


            return relativeExecutablePath;
        }



        /// <summary>
        /// Extracts the embedded FreeLauncher.exe template
        /// into the requested destination.
        /// </summary>
        private void ExtractLauncherTemplate(string destinationPath)
        {
            // Retrieve the current assembly (the builder application).
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Get all embedded resources.
            string[] resources = assembly.GetManifestResourceNames();

            // Find the launcher template automatically.
            string? resourceName = resources.FirstOrDefault(resource =>
                    resource.EndsWith(
                        ".Templates.FreeLauncher.exe",
                        StringComparison.OrdinalIgnoreCase
                    )
                );

            if (resourceName == null)
            {
                throw new InvalidOperationException(
                    "FreeLauncher.exe was not found as an embedded resource.\n\n" +
                    "Make sure Templates/FreeLauncher.exe has:\n" +
                    "Build Action = Embedded Resource\n\n" +
                    "Embedded resources found:\n" +
                    string.Join("\n", resources)
                );
            }

            // Open the embedded resource stream for reading.
            using Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream == null)
            {
                throw new InvalidOperationException(
                    $"Unable to open launcher template:\n{resourceName}"
                );
            }

            // Create the destination file for writing.
            using FileStream outputStream = new FileStream(
                    destinationPath,
                    FileMode.Create,
                    FileAccess.Write
                );

            resourceStream.CopyTo(outputStream);
        }

        /// <summary>
        /// Appends the launcher configuration directly
        /// to the generated launcher executable.
        ///
        /// File structure after generation:
        ///
        /// [FreeLauncher.exe]
        /// [JSON DATA]
        /// [JSON LENGTH]
        /// [CONFIG MARKER]
        ///
        /// The launcher can later read this information
        /// directly from its own executable.
        /// </summary>
        private void EmbedConfigIntoLauncher(string launcherPath, LauncherConfig config)
        {
            // Unique marker allowing the launcher to detect
            const string configMarker = "LAUNCHER_CONFIG_V1";

            // Convert the LauncherConfig object into JSON.
            string json = JsonSerializer.Serialize( config,
                    new JsonSerializerOptions
                    {
                        WriteIndented = false,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }
                );

            // Convert the JSON text to raw UTF-8 bytes.
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

            // Store the JSON size.
            byte[] jsonLengthBytes = BitConverter.GetBytes(
                    (long)jsonBytes.Length
                );

            // Unique marker allowing the launcher to detect
            // where the embedded configuration ends.
            byte[] markerBytes = Encoding.UTF8.GetBytes(configMarker);


            // Open the generated executable
            // and move to the end of the file.
            using FileStream stream =
                new FileStream(
                    launcherPath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.None
                );

            // Write:
            //
            // JSON
            stream.Write(
                jsonBytes,
                0,
                jsonBytes.Length
            );

            // JSON size
            stream.Write(
                jsonLengthBytes,
                0,
                jsonLengthBytes.Length
            );

            // Marker
            stream.Write(
                markerBytes,
                0,
                markerBytes.Length
            );
        }



    }


    /// <summary>
    /// Represents the configuration expected by the launcher.
    ///
    /// Property names use standard C# PascalCase naming.
    /// During JSON serialization they are automatically
    /// converted to camelCase.
    /// </summary>
    public class LauncherConfig
    {
        /// <summary>
        /// Text displayed in the launcher window title bar.
        /// </summary>
        public string LauncherName { get; set; } = "Launcher";


        /// <summary>
        /// Name of the game displayed inside the launcher.
        /// </summary>
        public string GameName { get; set; } = "My Game";


        /// <summary>
        /// Font used to display the game name.
        /// </summary>
        public string GameNameFontFamily { get; set; } = "Segoe UI";


        /// <summary>
        /// Path to the game executable.
        ///
        /// In the final generated launcher,
        /// this should normally be relative to Launcher.exe.
        ///
        /// Example:
        /// LastRun/LastRun.exe
        /// </summary>
        public string GameExecutable { get; set; } = "";


        /// <summary>
        /// Path to the launcher background image.
        /// </summary>
        public string BackgroundPath { get; set; } = "";


        /// <summary>
        /// Path to the launcher window icon.
        /// </summary>
        public string IconPath { get; set; } = "";


        // =====================================================
        // PLAY BUTTON
        // =====================================================

        /// <summary>
        /// Text displayed inside the Play button.
        /// </summary>
        public string PlayButtonText { get; set; } = "PLAY";


        /// <summary>
        /// Normal background color of the Play button.
        /// </summary>
        public string PlayButtonBackground { get; set; } =
            "#99000000";


        /// <summary>
        /// Normal text color of the Play button.
        /// </summary>
        public string PlayButtonForeground { get; set; } =
            "#FFFF0000";


        /// <summary>
        /// Border color of the Play button.
        /// </summary>
        public string PlayButtonBorder { get; set; } =
            "#FFFF0000";


        /// <summary>
        /// Background color when the mouse
        /// is hovering over the Play button.
        /// </summary>
        public string PlayButtonHoverBackground { get; set; } =
            "#CCAA0000";


        /// <summary>
        /// Text color when the mouse
        /// is hovering over the Play button.
        /// </summary>
        public string PlayButtonHoverForeground { get; set; } =
            "#FFFFFFFF";


        /// <summary>
        /// Background color while the Play button
        /// is being pressed.
        /// </summary>
        public string PlayButtonPressedBackground { get; set; } =
            "#FFFF0000";
    }
}