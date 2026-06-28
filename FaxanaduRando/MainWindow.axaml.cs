using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.IO;

namespace FaxanaduRando
{
    public partial class MainWindow : Window
    {
        private bool _updatingFlags = false;

        public MainWindow()
        {
            InitializeComponent();
            // Apply the first preset now that all controls are initialized
            flagsTextBox.Text = "38DFFF5A05k02v1ncoH";
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

        private void SyncStaticPropertiesFromUI()
        {
            // General options
            Randomizer.GeneralOptions.FastText = fastTextCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.FastStart = fullHealthCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.DragonSlayerRequired = dragonSlayerRequiredCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.PendantRodRubyRequired = pendantRodRubyRequiredCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.MoveSpringQuestRequirement = moveSpringQuestRequirementCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.ShuffleTowers = shuffleTowersCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.ShuffleWorlds = shuffleWorldsCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.UpdateMiscText = updateMiscTextCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.GenerateSpoilerLog = generateSpoilerLogCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.QuickSeed = quickSeedCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.AllowLoweringRespawn = allowLoweringRespawnCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.PreventKnockbackOnLadders = preventKnockbackOnLaddersCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.FlexibleItems = flexibleItemsCheckbox.IsChecked == true;
            Randomizer.GeneralOptions.DarkTowers = darknessCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.RandomizeTitles = randomizeTitlesCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.AddKillSwitch = addKillSwitchCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.IncludeEvilOnesFortress = includeEvilOnesFortressCheckBox.IsChecked == true;
            Randomizer.GeneralOptions.HintSetting = (Randomizer.GeneralOptions.Hints)hintsComboBox.SelectedIndex;
            Randomizer.GeneralOptions.MiscDoorSetting = (Randomizer.GeneralOptions.MiscDoors)miscDoorsComboBox.SelectedIndex;
            Randomizer.GeneralOptions.DoorTypeSetting = (Randomizer.GeneralOptions.DoorTypeShuffle)doorTypeComboBox.SelectedIndex;
            Randomizer.GeneralOptions.RandomizeScreens = (Randomizer.GeneralOptions.ScreenRandomization)randomizeScreensComboBox.SelectedIndex;
            Randomizer.GeneralOptions.ShuffleSegments = (Randomizer.GeneralOptions.SegmentShuffle)shuffleSegmentsComboBox.SelectedIndex;

            // Enemy options
            Randomizer.EnemyOptions.RandomizeExperience = randomizeEnemyExperiencesCheckBox.IsChecked == true;
            Randomizer.EnemyOptions.RandomizeRewards = randomizeRewardsCheckBox.IsChecked == true;
            Randomizer.EnemyOptions.RandomizeMagicImmunities = randomizeMagicImmunitiesCheckBox.IsChecked == true;
            Randomizer.EnemyOptions.TryToMoveBosses = tryToMoveBossesCheckBox.IsChecked == true;
            Randomizer.EnemyOptions.EnemySet = (Randomizer.EnemyOptions.EnemySetType)enemySetComboBox.SelectedIndex;
            Randomizer.EnemyOptions.EnemyHPSetting = (Randomizer.EnemyOptions.EnemyHP)enemyHPComboBox.SelectedIndex;
            Randomizer.EnemyOptions.EnemyDamageSetting = (Randomizer.EnemyOptions.EnemyDamage)enemyDamageComboBox.SelectedIndex;
            Randomizer.EnemyOptions.AISetting = (Randomizer.EnemyOptions.AIShuffle)aiComboBox.SelectedIndex;
            Randomizer.EnemyOptions.AIPropertySetting = (Randomizer.EnemyOptions.AIProperrtyRandomization)aiPropertyComboBox.SelectedIndex;

            // Item options
            Randomizer.ItemOptions.GuaranteeElixirNearFortress = guaranteeElixirNearFortressCheckbox.IsChecked == true;
            Randomizer.ItemOptions.FixPendantBug = fixPendantBugCheckBox.IsChecked == true;
            Randomizer.ItemOptions.BuffGloves = buffGlovesCheckBox.IsChecked == true;
            Randomizer.ItemOptions.BuffHourglass = buffHourglassCheckBox.IsChecked == true;
            Randomizer.ItemOptions.RandomizeBarRank = randomizeBarRank.IsChecked == true;
            Randomizer.ItemOptions.GuaranteeStartingSpell = guaranteeStartingSpell.IsChecked == true;
            Randomizer.ItemOptions.GuaranteeMattock = guaranteeMattock.IsChecked == true;
            Randomizer.ItemOptions.ReplacePoison = replacePoisonCheckBox.IsChecked == true;
            Randomizer.ItemOptions.AlwaysSpawnSmallItems = alwaysSpawnSmallItemsCheckBox.IsChecked == true;
            Randomizer.ItemOptions.RandomizeItemNames = randomizeItemNamesCheckBox.IsChecked == true;
            Randomizer.ItemOptions.IncludeSomeEolisDoors = includeSomeEolisDoorsCheckBox.IsChecked == true;
            Randomizer.ItemOptions.MattockUsage = (Randomizer.ItemOptions.MattockUsages)mattockUsageComboBox.SelectedIndex;
            Randomizer.ItemOptions.StartingWeapon = (Randomizer.ItemOptions.StartingWeaponOptions)startingWeaponComboBox.SelectedIndex;
            Randomizer.ItemOptions.WingbootDurationSetting = (Randomizer.ItemOptions.WingBootDurations)wingBootDurationComboBox.SelectedIndex;
            Randomizer.ItemOptions.ShieldSetting = shieldSettingsComboBox.SelectedIndex;
            Randomizer.ItemOptions.BigItemSpawns = (Randomizer.ItemOptions.BigItemSpawning)bigItemSpawnsComboBox.SelectedIndex;
            Randomizer.ItemOptions.ShuffleItems = (Randomizer.ItemOptions.ItemShuffle)itemShuffleComboBox.SelectedIndex;
            Randomizer.ItemOptions.RandomizeKeys = (Randomizer.ItemOptions.KeyRandomization)keyRandomizationComboBox.SelectedIndex;
            Randomizer.ItemOptions.MultipleGifts = (Randomizer.ItemOptions.MultipleGiftOptions)multipleGiftsComboBox.SelectedIndex;
            Randomizer.ItemOptions.SmallKeyLimit = (Randomizer.ItemOptions.KeyLimit)smallKeyLimitComboBox.SelectedIndex;
            Randomizer.ItemOptions.BigKeyLimit = (Randomizer.ItemOptions.KeyLimit)bigKeyLimitComboBox.SelectedIndex;

            // Extra options
            Randomizer.ExtraOptions.RandomizePalettes = randomizePalettesCheckbox.IsChecked == true;
            Randomizer.ExtraOptions.RandomizeSounds = randomizeSoundEffectsCheckbox.IsChecked == true;
            Randomizer.ExtraOptions.AppendSuffix = addSuffixCheckbox.IsChecked == true;
            Randomizer.ExtraOptions.MusicSetting = (Randomizer.Music)musicComboBox.SelectedIndex;

            // Text options
            Randomizer.TextOptions.UseCustomText = useCustomTextCheckBox.IsChecked == true;
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

            SyncStaticPropertiesFromUI();

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
                bool success = randomizer.Randomize(pathTextBox.Text, customTextPathTextBox.Text, flagsTextBox.Text, seed, out message);
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

            if (sender is ComboBox box)
            {
                // Beginner
                if (box.SelectedIndex == 0)
                {
                    flagsTextBox.Text = "38DFFF5A05k02v1ncoH";
                }
                // Standard
                else if (box.SelectedIndex == 1)
                {
                    flagsTextBox.Text = "38CFFF7A0za0cGalcmH";
                }
                // Race (typical)
                else if (box.SelectedIndex == 2)
                {
                    flagsTextBox.Text = "7ECFFF7A0za0cFakcmH";
                }
                // Race (classic)
                else if (box.SelectedIndex == 3)
                {
                    flagsTextBox.Text = "580867000Am00a1nmoH";
                }
                // Challenge mode
                else if (box.SelectedIndex == 4)
                {
                    flagsTextBox.Text = "FECC377A0ze0bPakmoH";
                }
                // Chaos mode
                else if (box.SelectedIndex == 5)
                {
                    flagsTextBox.Text = "7ECFFF7E0ucba0a012b";
                }
                // Extra fast
                else if (box.SelectedIndex == 6)
                {
                    flagsTextBox.Text = "3ECFFFFA0Al02v1n2k2";
                }
            }
        }

        private void FlagsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updatingFlags)
                return;

            var text = flagsTextBox.Text;
            if (string.IsNullOrEmpty(text))
                return;

            var values = Randomizer.FlagConverter.ParseFlags(text);
            if (values.Length == 0)
                return;

            _updatingFlags = true;
            try
            {
                var checkBoxes = new CheckBox[]
                {
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
                };

                int boolCount = 33;
                for (int i = 0; i < boolCount && i < values.Length && i < checkBoxes.Length; i++)
                {
                    checkBoxes[i].IsChecked = (bool)values[i];
                }

                var comboBoxes = new ComboBox[]
                {
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
                };

                for (int i = 0; i < comboBoxes.Length && (boolCount + i) < values.Length; i++)
                {
                    comboBoxes[i].SelectedIndex = (int)values[boolCount + i];
                }
            }
            finally
            {
                _updatingFlags = false;
            }
        }
    }
}
