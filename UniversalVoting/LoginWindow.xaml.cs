using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace UniversalVoting
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        IDatabase _clsDb;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool IsCorrect = false;
            _clsDb = new Database();
            _clsDb.ExecuteCommand("SELECT * FROM Judge");
            foreach (DataRow j in _clsDb.Data.Rows)
            {
                if (txtUsername.Text == j.Field<string>(2).ToString() && txtPassword.Password.ToString() == j.Field<string>(3).ToString())
                {
                    IsCorrect = true;
                    this.Hide();
                    JudgeWindow judge = new JudgeWindow(j.Field<int>(0));
                    judge.ShowDialog();
                    judge = null;
                    this.Show();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    lblError.Content = null;
                }
            }

            _clsDb.ExecuteCommand("SELECT * FROM EventOrganizer");
            foreach (DataRow o in _clsDb.Data.Rows)
            {
                if (txtUsername.Text == o.Field<string>(1).ToString() && txtPassword.Password.ToString() == o.Field<string>(2).ToString())
                {
                    IsCorrect = true;
                    this.Hide();
                    EventOrganizerWindow organizer = new EventOrganizerWindow(o.Field<int>(3));
                    organizer.ShowDialog();
                    organizer = null;
                    this.Show();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    lblError.Content = null;
                }
            }

            if (!IsCorrect)
            {
                lblError.Content = "Invalid Username and Password!";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
