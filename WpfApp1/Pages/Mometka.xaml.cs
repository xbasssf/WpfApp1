using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.DB;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для Mometka.xaml
    /// </summary>
    public partial class Mometka : Page
    {
        private string _name;
        private decimal _betAmount;
        private string _choice; // "Орёл" или "Решка"
        private Random _random;

        public Mometka(string name)
        {
            InitializeComponent();
            _name = name;
            _random = new Random();
        }

        private async void HeadsButton_Click(object sender, RoutedEventArgs e)
        {
            await PlayGame("Орёл");
        }

        private async void TailsButton_Click(object sender, RoutedEventArgs e)
        {
            await PlayGame("Решка");
        }

        private async Task PlayGame(string choice)
        {
            _choice = choice;


            if (!decimal.TryParse(BetAmountTextBox.Text, out _betAmount) || _betAmount <= 0)
            {
                MessageBox.Show("Введите корректную сумму ставки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);
            if (user == null)
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.Balance < _betAmount)
            {
                MessageBox.Show("На счету недостаточно средств для ставки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            user.Balance -= Convert.ToInt32(_betAmount);
            ConnectionClass.connect.SaveChanges();

            // Генерация результата броска монетки
            string[] coinSides = { "Орёл", "Решка" };
            string coinResult = coinSides[_random.Next(0, 2)];

            // Определение выигрыша
            decimal winAmount = 0;
            if (_choice == coinResult)
            {
                // Например, коэффициент выигрыша 2x
                winAmount = _betAmount * 2;
                user.Balance += Convert.ToInt32(winAmount);
                ConnectionClass.connect.SaveChanges();
                ResultTextBlock.Text = $"Выпало: {coinResult}. Вы выиграли {winAmount:C}";
                ResultTextBlock.Foreground = Brushes.Green;
            }
            else
            {
                ResultTextBlock.Text = $"Выпало: {coinResult}. Вы проиграли {_betAmount:C}(";
                ResultTextBlock.Foreground = Brushes.Red;
            }

            var gameSession = new GameSessions
            {
                UserID = user.UserID,
                GameID = 2, // Предполагаем, что GameID=1 соответствует игре "Монетка"
                Date = DateTime.Now,
                BetAmount = _betAmount,
                WinAmount = winAmount > 0 ? (decimal?)winAmount : null
            };
            ConnectionClass.connect.GameSessions.Add(gameSession);
            ConnectionClass.connect.SaveChanges();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var user = ConnectionClass.connect.User.FirstOrDefault(u => u.Username == _name);
            NavigationService.Navigate(new Menu(user.Balance, user.Username));
        }
    }
}
