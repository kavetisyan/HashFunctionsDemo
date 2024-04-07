using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HashFunctionsWPF
{
    public partial class MainWindow : Window
    {
        #region "Ctor"
        public MainWindow()
        {
            InitializeComponent();
            rBtnCollisionC.IsChecked = true;
        }
        #endregion "Ctor"

        #region "Properties"
        private string _inputHash;
        public string InputHash
        {
            get => _inputHash;
            set
            {
                if (_inputHash != value)
                {
                    _inputHash = value;
                    txtInputHash.Text = _inputHash;
                }
            }
        }

        private string _hashToAttack;
        public string HashToAttack
        {
            get => _hashToAttack;
            set
            {
                if (_hashToAttack != value)
                {
                    _hashToAttack = value;
                    txtHashToAttack.Text = _hashToAttack;
                }
            }
        }

        private string _attackResult;
        public string AttackResult
        {
            get => _attackResult;
            set
            {
                if (_attackResult != value)
                {
                    _attackResult = value;
                    txtAttackResult.Text = _attackResult;
                }
            }
        }

        private string _attackHash;
        public string AttackHash
        {
            get => _attackHash;
            set
            {
                if (_attackHash != value)
                {
                    _attackHash = value;
                    txtAttackHash.Text = _attackHash;
                }
            }
        }

        private double _totalTime;
        public double TotalTime
        {
            get => _totalTime;
            set
            {
                if(_totalTime != value)
                {
                    _totalTime = value;
                    txtTime.Text = $"{_totalTime} seconds";
                }
            }
        }
        #endregion "Properties"


        #region "Events"
        private async void CalculateHash_Click(object sender, RoutedEventArgs e)
        {
            string input = txtInput.Text;
            if (!string.IsNullOrEmpty(input))
            {
                string hash = HashHelper.CalculateHash(input);
                InputHash = hash;
                HashToAttack = InputHash.Substring(0, 6);
            }
            else
            {
                MessageBox.Show("Please enter some input data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Attack_Click(object sender, RoutedEventArgs e)
        {
            string originalInput = txtInput.Text;
            if (!string.IsNullOrEmpty(originalInput))
            {
                SwitchBusyIndicator();
               
                Stopwatch stopwatch = Stopwatch.StartNew();
                string attackedInput = await Task.Run(() => PerformAttack());
                stopwatch.Stop();

                SwitchBusyIndicator();

                string attackedHash = HashHelper.CalculateHash(attackedInput);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    AttackResult = attackedInput;
                    AttackHash = attackedHash;
                    TotalTime = stopwatch.Elapsed.TotalSeconds;
                });
            }
            else
            {
                MessageBox.Show("Please enter some input data to attack.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var a = sender as RadioButton;
            if (sender != null)
            {
                if (a.Content.ToString() == "Collision Attack")
                {
                    rBtnCollisionC.IsChecked = true;
                    rBtnCollisionS.IsChecked = true;
                    CollusionGrid.Visibility = Visibility.Visible;
                    SaltGrid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    rBtnSaltC.IsChecked = true;
                    rBtnSaltS.IsChecked = true;
                    CollusionGrid.Visibility = Visibility.Collapsed;
                    SaltGrid.Visibility = Visibility.Visible;
                }
            }
        }
        private void CalculateSaltHash_Click(object sender, RoutedEventArgs e)
        {
            string input = txtInputSalt.Text;
            if (!string.IsNullOrEmpty(input))
            {
                byte[] salt1 = HashHelper.GenerateSalt();
                byte[] salt2 = HashHelper.GenerateSalt();

                string hash1 = HashHelper.HashWithSalt(input, salt1);
                string hash2 = HashHelper.HashWithSalt(input, salt2);

                txtSalt1.Text = Convert.ToBase64String(salt1);
                txtHash1.Text = hash1;
                txtSalt2.Text = Convert.ToBase64String(salt2);
                txtHash2.Text = hash2;
            }
            else
            {
                MessageBox.Show("Please enter some input data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearCollusion_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid(CollusionGrid);
        }

        private void ClearSalt_Click(object sender, RoutedEventArgs e)
        {
            ClearGrid(SaltGrid);
        }
        #endregion "Events"

        #region "Methods"
        private string PerformAttack()
        {
            string attackedInput = string.Empty;
            int count = 0;
            while (true)
            {
                attackedInput = count.ToString();
                string hashString = HashHelper.CalculateHash(attackedInput);
                if (hashString.Substring(0, 6) == HashToAttack)
                {
                    break;
                }
                count++;
            }
            return attackedInput;
        }

        private void SwitchBusyIndicator()
        {
            BusyOverlay.Visibility = CollusionGrid.Visibility;
            CollusionGrid.Visibility = CollusionGrid.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ClearGrid(Grid grid)
        {
            foreach (var child in grid.Children)
            {
                if (child is TextBox textBox)
                {
                    textBox.Text = string.Empty;
                }
            }
        }
        #endregion "Methods"
    }
}
