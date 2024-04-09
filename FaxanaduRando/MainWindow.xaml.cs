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

        private void CustomTextBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title = "Select Custom Text File",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                customTextPathTextBox.Text = openFileDialog.FileName;
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

            if (Randomizer.GeneralOptions.RandomizeScreens != Randomizer.GeneralOptions.ScreenRandomization.Unchanged &&
                !Randomizer.GeneralOptions.AddKillSwitch)
            {
                if (MessageBox.Show("It is recommended that you turn on the 'Add kill switch' flag when doing screen randomization to prevent softlocks. Continue?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(customTextPathTextBox.Text) &&
                !Randomizer.TextOptions.UseCustomText)
            {
                if (MessageBox.Show("Custom text won't be used unless the 'Use custom text' flag is checked. Continue?", "Warning", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    return;
                }
            }

            try
            {
                var randomizer = new Randomizer.Randomizer();
                string message;
                bool result = randomizer.Randomize(pathTextBox.Text, customTextPathTextBox.Text, flagsTextBox.Text, seed, out message);
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
                    flagsTextBox.Text = "38DFFF315k02v1ncoH";
                }
                else if (box.SelectedIndex == 1)
                {
                    flagsTextBox.Text = "78CFFF35za0cGalcmH";
                }
                else if (box.SelectedIndex == 2)
                {
                    flagsTextBox.Text = "7ECFFF35zc0cFakcmH";
                }
                else if (box.SelectedIndex == 3)
                {
                    flagsTextBox.Text = "58086700Am00a1nmoH";
                }
                else if (box.SelectedIndex == 4)
                {
                    flagsTextBox.Text = "FECC3735ze0bPakmoH";
                }
                else if (box.SelectedIndex == 5)
                {
                    flagsTextBox.Text = "7ECFFF37ucba0a012b";
                }
                else if (box.SelectedIndex == 6)
                {
                    flagsTextBox.Text = "3ECFFF75Al02v1n2k2";
                }
            }
        }
    }
}
