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
    /// Логика взаимодействия для Report.xaml
    /// </summary>
    public partial class Report : Page
    {
        private User _name;
        public Report(User name)
        {
            InitializeComponent();
            _name = name;
                var totalBets = ConnectionClass.connect.GameSessions
    .Where(gs => gs.UserID == name.UserID && gs.BetAmount.HasValue)
    .Sum(gs => gs.BetAmount.Value);

            var totalWins = ConnectionClass.connect.GameSessions
                .Where(gs => gs.UserID == name.UserID && gs.WinAmount.HasValue)
                .Sum(gs => gs.WinAmount.Value);

            TotalBetsTextBlock.Text = $"{totalBets:C}";
            TotalWinsTextBlock.Text = $"{totalWins:C}";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
