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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System.Windows.Forms;
using UniversalVoting;

namespace UniversalVoting.EventOrganizerTabs
{
    /// <summary>
    /// Interaction logic for TabJudges.xaml
    /// </summary>
    public partial class TabJudges : System.Windows.Controls.UserControl
    {
        private List<SqlParameter> prm;
        public List<Account> allaccounts;
        public List<Account> eventaccounts;
        private int _tabeventid =1 ;
        IDatabase clsDatabase;
        public class Account
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string judgeUname { get; set; }
            public string judgePword { get; set; }
            public bool existing { get; set; }

            public Account(string fname,string lname, string uname, string pass)
            {
                FirstName = fname;
                LastName = lname;
                judgeUname = uname;
                judgePword = pass;
                existing = false;

            }

            public Account(string fname, string lname, string uname, string pass,bool exist)
            {
                FirstName = fname;
                LastName = lname;
                judgeUname = uname;
                judgePword = pass;
                existing = exist;

            }


        }

        public TabJudges()
        {
            InitializeComponent();
            clsDatabase = new Database();
            clsDatabase.ExecuteCommand("Select * From [vwdgallaccounts]");  
            dgAllAccounts.ItemsSource = clsDatabase.Data.DefaultView;
            
            prm = new List<SqlParameter>()
            {
                new SqlParameter("@_eventid",SqlDbType.Int) {Value = _tabeventid }
            };
            clsDatabase.ExecuteStoredProcedure("[spViewEventJudges]", prm);
            dgEventAccounts.ItemsSource = clsDatabase.Data.DefaultView;


            #region  intializers

            eventaccounts = new List<Account>();
            
          // dgEventAccounts.ItemsSource = eventaccounts;

            cmbjudgeoptions.Items.Add("Edit");
            cmbjudgeoptions.Items.Add("Add");
            cmbjudgeoptions.Items.Add("Remove");
            cmbjudgeoptions.Items.Add("Cancel");

            #endregion
        }

        private void cmbjudgeoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbjudgeoptions_SelectionChanged();
        }
        private void cmbjudgeoptions_SelectionChanged()
        {
            txbjudgeuname.Text = txbjudgepword.Text = txblname.Text = txbfname.Text= "";
            unameavail.Content = "";
           dgEventAccounts.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txblname.IsEnabled = false;
                txbfname.IsEnabled = false;
                txbjudgepword.IsEnabled = false;
                txbjudgeuname.IsEnabled = false;
                dgEventAccounts.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Edit";
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                txblname.IsEnabled = true;
                txbfname.IsEnabled = true;
                txbjudgepword.IsEnabled = true;
                txbjudgeuname.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Add";
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                txblname.IsEnabled = false;
                txbfname.IsEnabled = false;
                txbjudgepword.IsEnabled = false;
                txbjudgeuname.IsEnabled = false;
                dgEventAccounts.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else
            {
                cmbjudgeoptions.SelectedIndex = -1;
                btnjudgeconfirm.Visibility = Visibility.Hidden;
            }
        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                Account dataRow = (Account)dgEventAccounts.SelectedItem;
                txbfname.Text = dataRow.FirstName.ToString();
                txblname.Text = dataRow.LastName.ToString();
                txbjudgeuname.Text = dataRow.judgeUname.ToString();
                txbjudgepword.Text = dataRow.judgePword.ToString();
                txblname.IsEnabled = true;
                txbfname.IsEnabled = true;
                txbjudgepword.IsEnabled = true;
                txbjudgeuname.IsEnabled = true;
            }

        }

        private void btnjudgeconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                //edit a judge
                Account dataRow = (Account)dgEventAccounts.SelectedItem;
                dataRow.FirstName = txbfname.Text;
                dataRow.LastName = txblname.Text;
                dataRow.judgeUname = txbjudgeuname.Text.ToString();
                dataRow.judgePword = txbjudgepword.Text.ToString();
                dgEventAccounts.SelectedItem = dataRow;
                dgEventAccounts.Items.Refresh();
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                //adding a judge      

                if (txbfname.Text == "" || txbjudgepword.Text == "" || txbjudgeuname.Text == "" || txblname.Text == "")
                    return;

                eventaccounts.Add(new Account(txbfname.Text,txblname.Text, txbjudgeuname.Text, txbjudgepword.Text));
                dgEventAccounts.IsEnabled = true;
                dgEventAccounts.Items.Refresh();
                cmbjudgeoptions.SelectedIndex = -1;
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                //deleting a judge
                Account dataRow = (Account)dgEventAccounts.SelectedItem;
                String s = "Confirm deletion of Account \n\nAccount Name: \t"+ dataRow.FirstName.ToString()+" "+ dataRow.LastName.ToString() + "\nUsername: \t" + dataRow.judgeUname.ToString()+"\nPassword: \t"+ dataRow.judgePword.ToString();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Confirm Deletion of Account", MessageBoxButtons.YesNo);
                if (dialogResult ==DialogResult.Yes)
                {
                    eventaccounts.RemoveAt(dgEventAccounts.SelectedIndex);
                    cmbjudgeoptions.SelectedIndex = -1;
                    dgEventAccounts.IsEnabled = true;
                    dgEventAccounts.Items.Refresh();
                    dgEventAccounts.IsEnabled = false;
                }
            }
            cmbjudgeoptions_SelectionChanged();
            
        }

        private void dgAllAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {
            bool doesnotexist = true;
            if ((dgAllAccounts.SelectedIndex == -1)||(dgAllAccounts.SelectedItem==null))
                return;

            DataRowView dataRow = (DataRowView)dgAllAccounts.SelectedItem;
            //Go to Database and Check if Account to be added is already in EventJudge


            for(int x = 0; x<eventaccounts.Count();x++)
            {
                if ((dataRow[1].ToString() == eventaccounts[x].LastName || dataRow[0].ToString() == eventaccounts[x].FirstName || dataRow[2].ToString() == eventaccounts[x].judgeUname ))
                {
                    doesnotexist = true;
                    lblerrorindicator1.Visibility = Visibility.Visible;
                    lblerrorindicator1.Content = "Account Already exists";
                    return;
                }

            }
            if (doesnotexist)
            {
                lblerrorindicator1.Visibility = Visibility.Visible;
                lblerrorindicator1.Content = "Account Added";
                eventaccounts.Add(new Account(dataRow[0].ToString(), dataRow[1].ToString(), dataRow[2].ToString(), dataRow[3].ToString(), true));
                dgEventAccounts.Items.Refresh();
               
            }
        }

        public void passingidvalue(int x)
        {
            _tabeventid = x;
        }


        private void txbjudgeuname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IDatabase clsDatabase = new Database();
            prm = new List<SqlParameter>() { new SqlParameter("@judgechars", SqlDbType.NVarChar) { Value = txbjudgeuname.Text } };
            clsDatabase.ExecuteStoredProcedure("[spCheckUnameavailability]", prm);
            if (clsDatabase.Data.Rows.Count == 0)
                unameavail.Content = "Username Available";
            else
                unameavail.Content = "Username Taken already";


        }

        private void btnjudgesave_Click(object sender, RoutedEventArgs e)
        {
            foreach(Account x in eventaccounts)
            {
                if (x.existing == true)
                {
                    IDatabase clsdatabase = new Database();
                    prm = new List<SqlParameter>()
                    { new SqlParameter("@judgeUname",SqlDbType.NVarChar){ Value = x.judgeUname},
                        new SqlParameter("@eventid",SqlDbType.Int) {Value = _tabeventid }};
                    clsdatabase.ExecuteStoredProcedure("[spGetJudgeID]", prm);

                }
                else if (x.existing == false)
                {

                    IDatabase clsdatabase = new Database();
                    prm = new List<SqlParameter>()
                    {
                        new SqlParameter("@fname",SqlDbType.NVarChar){ Value = x.FirstName},
                        new SqlParameter("@lname",SqlDbType.NVarChar) {Value = x.LastName },
                        new SqlParameter("@uname", SqlDbType.NVarChar) { Value = x.judgeUname },
                        new SqlParameter("@pass", SqlDbType.NVarChar) { Value = x.judgePword },
                        new SqlParameter("@_eventid", SqlDbType.Int) { Value = _tabeventid }
                      };
                    clsdatabase.ExecuteStoredProcedure("[spAddPersonToEventJudges]", prm);
                }

                System.Windows.MessageBox.Show("Save Success");

            }
        }
    }
}
