using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace FaxanaduRando
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select ROM",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "nes",
                Filter = "nes files (*.nes)|*.nes",
                FilterIndex = 2,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                pathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void NewSeedButton_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();
            int seed = random.Next(int.MaxValue);
            seedTextBox.Text = seed.ToString();
        }

        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (pathTextBox.Text == null || pathTextBox.Text.Length == 0)
                return;

            if (!File.Exists(pathTextBox.Text))
                return;

            if (!int.TryParse(seedTextBox.Text, out int seed))
            {
                MessageBox.Show("Incorrect seed format", "Error");
                return;
            }

            if (Randomizer.GeneralOptions.ShuffleTowers && Randomizer.GeneralOptions.IncludeEvilOnesFortress &&
                !Randomizer.GeneralOptions.MoveFinalRequirements)
            {
                if (MessageBox.Show("You have selected to include the Evil One's fortress in the tower shuffle, but you have not selected to move the final requirements. This might result in extra requirements not actually being required. Continue?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                var randomizer = new Randomizer.Randomizer();
                string message;
                bool result = randomizer.Randomize(pathTextBox.Text, flagsTextBox.Text, seed, out message);
                if (!result)
                {
                    MessageBox.Show(message, "Failed");
                    return;
                }
                MessageBox.Show(message, "Success");
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to create rom", "Error");
            }
        }

        private void flagPresetComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Controls.ComboBox))
            {
                var box = (System.Windows.Controls.ComboBox)sender;
                if (box.SelectedIndex == 0)
                {
                    flagsTextBox.Text = "30B9FFF05k02v1k";
                }
                else if (box.SelectedIndex == 1)
                {
                    flagsTextBox.Text = "7E9FFFF0za0cFbk";
                }
                else if (box.SelectedIndex == 2)
                {
                    flagsTextBox.Text = "7E9FFFF1zc00Pak";
                }
                else if (box.SelectedIndex == 3)
                {
                    flagsTextBox.Text = "5010FE00Ac00Z9bk";
                }
                else if (box.SelectedIndex == 4)
                {
                    flagsTextBox.Text = "7E99DFC0ze0bPak";
                }
                else if (box.SelectedIndex == 5)
                {
                    flagsTextBox.Text = "7E9FFFF1ucma0aa";
                }
                else if (box.SelectedIndex == 6)
                {
                    flagsTextBox.Text = "3EDFFFF1Al02v1k";
                }
            }
        }
    }
}
