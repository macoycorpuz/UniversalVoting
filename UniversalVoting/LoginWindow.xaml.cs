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
        DataTable _eventorganizers;
        DataTable _events;
        DataTable _judges;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            bool IsCorrect = false;
            bool IsFinalize = false;
            _clsDb = new Database();
            _eventorganizers = new DataTable();
            _events = new DataTable();
            _judges = new DataTable();

            _clsDb.ExecuteCommand("SELECT * FROM Judge");
            if (_clsDb.Data.Rows.Count > 0)
                _judges = _clsDb.Data;
            foreach (DataRow j in _judges.Rows)
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
            if (_clsDb.Data.Rows.Count > 0)
                _eventorganizers = _clsDb.Data;
            foreach (DataRow o in _eventorganizers.Rows)
            {
                if (txtUsername.Text == o.Field<string>(1).ToString() && txtPassword.Password.ToString() == o.Field<string>(2).ToString())
                {
                    IsCorrect = true;
                    _clsDb.ExecuteCommand("SELECT E.IsFinalize FROM EVENT AS E INNER JOIN EVENTORGANIZER AS EO ON EO.EventID = E.EventID WHERE EO.adminUname = N'" + o.Field<string>(1).ToString() + "'");
                    if (!_clsDb.Data.Rows[0].Field<bool>(0))
                    {
                        this.Hide();
                        EventOrganizerWindow organizer = new EventOrganizerWindow(o.Field<int>(3));
                        organizer.ShowDialog();
                        organizer = null;
                        this.Show();
                    }
                    else
                        MessageBox.Show("The event has been finalized!");
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
            if (popup.IsOpen == true)
                popup.IsOpen = false;
            else
                popup.IsOpen = true;
        }

        private void generateAsBtn_Click(object sender, RoutedEventArgs e)
        {

            if ((txbefname.Text == "") || (txbelname.Text == "") || (txbeventname.Text == "") || (txbeventpass.Text =="")||( txbeventuname.Text == ""))
            {
                popup.IsOpen = false;
                var x1 = MessageBox.Show("Fields Incomplete ","", MessageBoxButton.OK);
                if (x1 == MessageBoxResult.OK)
                {
                    popup.IsOpen = true;
                    return;
                }


            }

            popup.IsOpen = false;
            var x = MessageBox.Show("Create Event?","Warning",MessageBoxButton.YesNo);
            if (x == MessageBoxResult.Yes)
            {
               
                #region conditions for event creation
                IDatabase clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckEventPerson",
                    "@efname", txbefname.Text,
                    "@elname", txbelname.Text);
                if (clsDatabase.Data.Rows.Count > 0)
                {
                    popup.IsOpen = false;
                   var x1 = MessageBox.Show("Person Name already Exists","",MessageBoxButton.OK);
                    if(x1 ==MessageBoxResult.OK )
                    {
                        popup.IsOpen = true;
                        return;
                    }
                }
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckEventAccount",
                    "@epname", txbeventuname.Text,
                    "@euname", txbeventpass.Text);
                if (clsDatabase.Data.Rows.Count > 0)
                {
                    popup.IsOpen = false;
                    var x1 = MessageBox.Show("Event Account already Exists", "", MessageBoxButton.OK);
                    if (x1 == MessageBoxResult.OK)
                    {
                        popup.IsOpen = true;
                        return;
                    }
                }
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckEventName",
                    "@ename", txbeventname.Text);
                if (clsDatabase.Data.Rows.Count > 0)
                {
                    popup.IsOpen = false;
                    var x1 = MessageBox.Show("Event Name already Exists", "", MessageBoxButton.OK);
                    if (x1 == MessageBoxResult.OK)
                    {
                        popup.IsOpen = true;
                        return;
                    }
                }
                #endregion
              

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCreateEvent",
                    "@efname", txbefname.Text,
                    "@elname", txbelname.Text,
                    "@epname", txbeventuname.Text,
                    "@euname", txbeventpass.Text,
                    "@ename", txbeventname.Text);
                MessageBox.Show("Account Created");
                txbefname.Text=txbelname.Text= txbeventname.Text = txbeventpass.Text = txbeventuname.Text = "";
            }
            else
            {
                popup.IsOpen = true;
            }
        }

        private void cancelbtn_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
            txbefname.Text = txbelname.Text = txbeventname.Text = txbeventpass.Text = txbeventuname.Text = "";
        }
    }
}
