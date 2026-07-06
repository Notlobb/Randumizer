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
            // TODO: Recalculate flags strings now that more data (1 bool and 3 enums) are encoded
            // in branch armor-weapon-stat-randomization
            // all have been set a serialization of the default-constructed option values
                "38DFFF5205k02v1ncoH00", // Beginner
                "38DFFF5205k02v1ncoH00", // Standard
                "38DFFF5205k02v1ncoH00", // Race (typical)
                "38DFFF5205k02v1ncoH00", // Race (classic)
                "38DFFF5205k02v1ncoH00", // Challenge mode
                "38DFFF5205k02v1ncoH00", // Chaos mode
                "38DFFF5205k02v1ncoH00", // Extra fast
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
                    useWeaponIndoorsCheckBox,
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
                    weaponStatComboBox,
                    magicStatComboBox,
                    armorStatComboBox,
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
            if (pathTextBox.Text == null || pathTextBox.Text.Length == 0)
                return;

            if (!File.Exists(pathTextBox.Text))
                return;

            if (!int.TryParse(seedTextBox.Text, out int seed))
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", "Incorrect seed format").ShowWindowDialogAsync(this);
                return;
            }

            // write current settings to live option class members as they are needed from this point
            FlagsCodec.ApplySettings(_settings);

            if (Randomizer.GeneralOptions.RandomizeScreens != Randomizer.GeneralOptions.ScreenRandomization.Unchanged &&
                !Randomizer.GeneralOptions.AddKillSwitch)
            {
                var result = await MessageBoxManager.GetMessageBoxStandard("Warning",
                    "It is recommended that you turn on the 'Add kill switch' flag when doing screen randomization to prevent softlocks. Continue?",
                    ButtonEnum.YesNo).ShowWindowDialogAsync(this);
                if (result == ButtonResult.No)
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(customTextPathTextBox.Text) &&
                !Randomizer.TextOptions.UseCustomText)
            {
                var result = await MessageBoxManager.GetMessageBoxStandard("Warning",
                    "Custom text won't be used unless the 'Use custom text' flag is checked. Continue?",
                    ButtonEnum.YesNo).ShowWindowDialogAsync(this);
                if (result == ButtonResult.No)
                {
                    return;
                }
            }

            try
            {
                var randomizer = new Randomizer.Randomizer();
                string message;
                bool success = randomizer.Randomize(pathTextBox.Text, customTextPathTextBox.Text, FlagsCodec.Serialize(), seed, out message);
                if (!success)
                {
                    await MessageBoxManager.GetMessageBoxStandard("Failed", message).ShowWindowDialogAsync(this);
                    return;
                }
                await MessageBoxManager.GetMessageBoxStandard("Success", message).ShowWindowDialogAsync(this);
            }
            catch (Exception ex)
            {
                await MessageBoxManager.GetMessageBoxStandard("Error", $"Failed to create rom: {ex.Message}\n{ex.StackTrace}").ShowWindowDialogAsync(this);
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

    }


}
