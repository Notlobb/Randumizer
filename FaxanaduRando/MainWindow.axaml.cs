using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using FaxanaduRando.Randomizer;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FaxanaduRando
{
    public partial class MainWindow : Window
    {
        private bool _updatingFlags = false;

        // contains settings that are part of flag serialization
        private object[] _settings;

        private ComboBox[] _comboBoxes;
        private CheckBox[] _checkBoxes;

        private CheckBox[] _extraCheckBoxes;
        private ComboBox[] _extraComboBoxes;

        private static readonly string[] Presets =
            [
                "38DFFF5A05k02v1ncoH", // Beginner
                "78CFFF5A0za0cGalcmH", // Standard
                "7ECFFF5A0zc0cFakcmH", // Race (typical)
                "580867000Am00a1nmoH", // Race (classic)
                "FECC375A0ze0bPakmoH", // Challenge mode
                "7ECFFF7E0ucba0a012b", // Chaos mode
                "3ECFFFDA0Al02v1n2k2", // Extra fast
            ];

        public MainWindow()
        {
            InitializeComponent();
            InitializeGUIElements();

            // read default constructor setting values
            _settings = FlagsCodec.ReadSettings();
            // read option defaults first so an invalid preset can't leave the GUI
            // in an undefined state if the flag schema is evolving during development
            flagsTextBox.Text = Presets[0];
            ApplyFlagString();
        }

        private void UpdateCheckBox(int index, bool value)
        {
            _checkBoxes[index].IsChecked = value;
        }

        private void UpdateComboBox(int index, int value)
        {
            _comboBoxes[index].SelectedIndex = value;
        }

        private void PopulateGUIElementsFromSettings()
        {
            // serialized settings
            for (int i = 0; i < _checkBoxes.Length; i++)
                UpdateCheckBox(i, (bool)_settings[i]);
            for (int i = 0; i < _comboBoxes.Length; ++i)
                UpdateComboBox(i, (int)_settings[i + FlagsCodec.BoolCount]);

            // Cosmetic settings
            randomizePalettesCheckbox.IsChecked = ExtraOptions.RandomizePalettes;
            randomizeSoundEffectsCheckbox.IsChecked = ExtraOptions.RandomizeSounds;
            addSuffixCheckbox.IsChecked = ExtraOptions.AppendSuffix;
            musicComboBox.SelectedIndex = (int)ExtraOptions.MusicSetting;
            soundtrackComboBox.SelectedIndex = (int)ExtraOptions.SoundtrackSetting;
        }

        private void InitializeGUIElements()
        {
            // Must match the schema order in FlagsCodec exactly
            // Only the element counts are validated automatically
            _checkBoxes = [
                    fastTextCheckBox,
                    fullHealthCheckBox,
                    dragonSlayerRequiredCheckBox,
                    pendantRodRubyRequiredCheckBox,
                    moveSpringQuestRequirementCheckBox,
                    shuffleTowersCheckBox,
                    shuffleWorldsCheckBox,
                    updateMiscTextCheckBox,
                    generateSpoilerLogCheckBox,
                    quickSeedCheckBox,
                    allowLoweringRespawnCheckBox,
                    preventKnockbackOnLaddersCheckBox,
                    randomizeEnemyExperiencesCheckBox,
                    randomizeRewardsCheckBox,
                    randomizeMagicImmunitiesCheckBox,
                    tryToMoveBossesCheckBox,
                    guaranteeElixirNearFortressCheckbox,
                    fixPendantBugCheckBox,
                    buffGlovesCheckBox,
                    buffHourglassCheckBox,
                    randomizeBarRank,
                    guaranteeStartingSpell,
                    guaranteeMattock,
                    replacePoisonCheckBox,
                    alwaysSpawnSmallItemsCheckBox,
                    randomizeItemNamesCheckBox,
                    flexibleItemsCheckbox,
                    includeEvilOnesFortressCheckBox,
                    darknessCheckBox,
                    randomizeTitlesCheckBox,
                    includeSomeEolisDoorsCheckBox,
                    addKillSwitchCheckBox,
                    useCustomTextCheckBox,
            ];

            _comboBoxes = [
                    hintsComboBox,
                    miscDoorsComboBox,
                    doorTypeComboBox,
                    enemySetComboBox,
                    enemyHPComboBox,
                    enemyDamageComboBox,
                    mattockUsageComboBox,
                    startingWeaponComboBox,
                    wingBootDurationComboBox,
                    shieldSettingsComboBox,
                    bigItemSpawnsComboBox,
                    itemShuffleComboBox,
                    keyRandomizationComboBox,
                    randomizeScreensComboBox,
                    multipleGiftsComboBox,
                    aiComboBox,
                    shuffleSegmentsComboBox,
                    smallKeyLimitComboBox,
                    bigKeyLimitComboBox,
                    aiPropertyComboBox,
             ];

            // extra checkboxes - for boolean options not part of flag serialization
            // order irrelevant as long as they update the corresponding option directly
            _extraCheckBoxes = [
                    randomizePalettesCheckbox,
                    randomizeSoundEffectsCheckbox,
                    addSuffixCheckbox,
                ];

            // extra comboboxes - for non-bool options not part of flag serialization
            // order irrelevant as long as they update the corresponding option directly
            _extraComboBoxes = [
                musicComboBox,
                soundtrackComboBox,
                ];

            // validate counts for serialized options against schema
            if (_checkBoxes.Length != FlagsCodec.BoolCount)
                throw new InvalidProgramException("CheckBox count does not match settings schema");
            if (_comboBoxes.Length != FlagsCodec.Entries.Count - FlagsCodec.BoolCount)
                throw new InvalidProgramException("ComboBox count does not match settings schema");

            foreach (var cb in _checkBoxes)
            {
                cb.IsCheckedChanged += CheckBoxChanged;
            }

            foreach (var combo in _comboBoxes)
            {
                combo.SelectionChanged += ComboBoxChanged;
            }

            // hook the extra settings up to the same controls
            foreach (var cb in _extraCheckBoxes)
            {
                cb.IsCheckedChanged += CheckBoxChanged;
            }

            foreach (var combo in _extraComboBoxes)
            {
                combo.SelectionChanged += ComboBoxChanged;
            }
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select ROM",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("NES files") { Patterns = new[] { "*.nes" } },
                    new FilePickerFileType("All files") { Patterns = new[] { "*" } }
                }
            });

            if (files.Count > 0)
            {
                pathTextBox.Text = files[0].Path.LocalPath;
            }
        }

        private async void CustomTextBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Custom Text File",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Text files") { Patterns = new[] { "*.txt" } },
                    new FilePickerFileType("All files") { Patterns = new[] { "*" } }
                }
            });

            if (files.Count > 0)
            {
                customTextPathTextBox.Text = files[0].Path.LocalPath;
            }
        }

        private void NewSeedButton_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            int seed = random.Next(int.MaxValue);
            seedTextBox.Text = seed.ToString();
        }

        private async void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            string inputRomFileName = pathTextBox.Text;
            string customTextFileName = String.IsNullOrEmpty(customTextPathTextBox.Text) ? null : customTextPathTextBox.Text;

            if (!int.TryParse(seedTextBox.Text, out int seed))
            {
                await ShowErrorMessageBox("Incorrect seed format");
                return;
            }

            if (string.IsNullOrEmpty(inputRomFileName) || !File.Exists(inputRomFileName))
            {
                await ShowErrorMessageBox("Please supply a valid ROM file for randomization");
                return;
            }

            // write current settings to live option class members as they are needed from this point
            FlagsCodec.ApplySettings(_settings);

            if (Randomizer.GeneralOptions.RandomizeScreens != Randomizer.GeneralOptions.ScreenRandomization.Unchanged &&
                !Randomizer.GeneralOptions.AddKillSwitch)
            {
                if (!await ShowWarningPrompt(
                    "It is recommended that you turn on the 'Add kill switch' flag when doing screen randomization to prevent softlocks. Continue?"))
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(customTextFileName) &&
                !Randomizer.TextOptions.UseCustomText)
            {
                if (!await ShowWarningPrompt(
                    "Custom text won't be used unless the 'Use custom text' flag is checked. Continue?"))
                {
                    return;
                }
            }

            if (TextOptions.UseCustomText &&
                (string.IsNullOrEmpty(customTextFileName) || !File.Exists(customTextFileName)))
            {
                await ShowErrorMessageBox("Please supply a path to a text file when using custom text. For a reference to the format of the file, check the Readme file");
                return;
            }

            try
            {
                string flags = FlagsCodec.Serialize();

                var randomizer = new Randomizer.Randomizer();
                var randomizationResult = randomizer.Randomize(
                    File.ReadAllBytes(inputRomFileName),
                    customTextFileName == null ? Array.Empty<string>() : File.ReadAllLines(customTextFileName),
                    flags, seed);

                string outFileName = Randomizer.Randomizer.GetOutputFilename(inputRomFileName, seed, flags, randomizationResult.FileNameSuffix);

                File.WriteAllBytes(outFileName, randomizationResult.Rom);
                if (GeneralOptions.GenerateSpoilerLog)
                    File.WriteAllLines(outFileName.Replace(".nes", ".txt"), randomizationResult.SpoilerLog);

                await ShowSuccessMessageBox("Randomized ROM created at " + outFileName);
            }
            catch (RandomizationException ex)
            {
                await ShowFailureMessageBox(ex.Message);
            }
            catch (Exception ex)
            {
                await ShowErrorMessageBox($"Failed to create rom: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void flagPresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flagsTextBox == null)
                return;

            if (sender is ComboBox box &&
                box.SelectedIndex >= 0 &&
                box.SelectedIndex < Presets.Length)
            {
                flagsTextBox.Text = Presets[box.SelectedIndex];
                ApplyFlagString();
            }
        }

        private void FlagsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFlagString();
        }

        private void ApplyFlagString()
        {
            if (_updatingFlags)
                return;

            var text = flagsTextBox.Text;
            try
            {
                _settings = FlagsCodec.Deserialize(text);
                flagsTextBox.ClearValue(TextBox.BorderBrushProperty);
                flagsTextBox.ClearValue(TextBox.BorderThicknessProperty);
                flagsTextBox.ClearValue(TextBox.BackgroundProperty);
            }
            catch
            {
                flagsTextBox.BorderBrush = Brushes.Red;
                flagsTextBox.Background = Brushes.MistyRose;
                flagsTextBox.BorderThickness = new Thickness(2);
                return;
            }

            _updatingFlags = true;
            try
            {
                PopulateGUIElementsFromSettings();
            }
            finally
            {
                _updatingFlags = false;
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            if (_updatingFlags)
                return;

            var checkBox = (CheckBox)sender!;

            if (Array.IndexOf(_extraCheckBoxes, checkBox) >= 0)
            {
                // extra options not part of serialization
                ExtraOptions.RandomizePalettes = randomizePalettesCheckbox.IsChecked == true;
                ExtraOptions.RandomizeSounds = randomizeSoundEffectsCheckbox.IsChecked == true;
                ExtraOptions.AppendSuffix = addSuffixCheckbox.IsChecked == true;
            }
            else
                UpdateSetting(checkBox);
        }

        private void ComboBoxChanged(object sender, RoutedEventArgs e)
        {
            if (_updatingFlags)
                return;

            var comboBox = (ComboBox)sender!;

            if (Array.IndexOf(_extraComboBoxes, comboBox) >= 0)
            {
                // extra options not part of serialization
                ExtraOptions.MusicSetting = (Music)musicComboBox.SelectedIndex;
                ExtraOptions.SoundtrackSetting = (Soundtrack)soundtrackComboBox.SelectedIndex;
            }
            else
                UpdateSetting(comboBox);
        }

        private void UpdateSetting(Control control)
        {
            if (control is CheckBox checkBox)
            {
                int index = Array.IndexOf(_checkBoxes, checkBox);
                _settings[index] = checkBox.IsChecked == true;
            }
            else if (control is ComboBox comboBox)
            {
                int comboIndex = Array.IndexOf(_comboBoxes, comboBox);
                int settingIndex = FlagsCodec.BoolCount + comboIndex;

                var type = FlagsCodec.Entries[settingIndex].Type;

                _settings[settingIndex] = type.IsEnum
                    ? Enum.ToObject(type, comboBox.SelectedIndex)
                    : comboBox.SelectedIndex;
            }

            flagsTextBox.Text = FlagsCodec.Serialize(_settings);
        }

        private Task ShowMessageBox(string title, string message, Icon icon)
        {
            return MessageBoxManager.GetMessageBoxStandard(
                title,
                message,
                ButtonEnum.Ok,
                icon)
            .ShowWindowDialogAsync(this);
        }

        private async Task<bool> ShowWarningPrompt(string message)
        {
            var result = await MessageBoxManager.GetMessageBoxStandard(
                "Warning",
                message,
                ButtonEnum.YesNo,
                MsBox.Avalonia.Enums.Icon.Warning)
            .ShowWindowDialogAsync(this);

            return result == ButtonResult.Yes;
        }

        private Task ShowErrorMessageBox(string message) =>
            ShowMessageBox("Error", message, MsBox.Avalonia.Enums.Icon.Error);

        private Task ShowFailureMessageBox(string message) =>
            ShowMessageBox("Failed", message, MsBox.Avalonia.Enums.Icon.Warning);

        private Task ShowSuccessMessageBox(string message) =>
            ShowMessageBox("Success", message, MsBox.Avalonia.Enums.Icon.Success);

    }

}
