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

namespace UniversalVoting.EventOrganizerTabs
{
    /// <summary>
    /// Interaction logic for TabJudges.xaml
    /// </summary>
    public partial class TabJudges : System.Windows.Controls.UserControl
    {
        public List<Account> account;

        public class Account
        {
            public string judgename { get; set; }
            public string username { get; set; }
            public string password { get; set; }

            public Account(string jname, string uname, string pass)
            {
                judgename = jname;
                username = uname;
                password = pass;

            }
            
        }


        public TabJudges()
        {
            InitializeComponent();


            account = new List<Account>()
            {
                new Account("Kyle","hateydiha","kantahan"),
                new Account("marcuz","malibog","marcuz_pass"),
                new Account("Grace","Ganda","hehe")
            };

            List<SqlDbType> mylist = new List<SqlDbType>();
            mylist.Add(SqlDbType.Bit);
            dgAccounts.ItemsSource = account;

            cmbjudgeoptions.Items.Add("Edit");
            cmbjudgeoptions.Items.Add("Add");
            cmbjudgeoptions.Items.Add("Delete");
        }

        private void cmbjudgeoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbjudgeoptions_SelectionChanged();
        }
        private void cmbjudgeoptions_SelectionChanged()
        {
            txbjudgeuname.Text = txbjudgepword.Text = txbjudgename.Text = "";
            dgAccounts.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txbjudgename.IsEnabled = false;
                txbjudgepword.IsEnabled = false;
                txbjudgeuname.IsEnabled = false;
                dgAccounts.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Edit";
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                txbjudgename.IsEnabled = true;
                txbjudgepword.IsEnabled = true;
                txbjudgeuname.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Add";
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                txbjudgename.IsEnabled = false;
                txbjudgepword.IsEnabled = false;
                txbjudgeuname.IsEnabled = false;
                dgAccounts.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Delete";
            }
            else
            {
                btnjudgeconfirm.Visibility = Visibility.Hidden;
            }
        }

        private void dgAccounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                Account dataRow = (Account)dgAccounts.SelectedItem;
                txbjudgename.Text = dataRow.judgename.ToString();
                txbjudgeuname.Text = dataRow.username.ToString();
                txbjudgepword.Text = dataRow.password.ToString();
                txbjudgename.IsEnabled = true;
                txbjudgepword.IsEnabled = true;
                txbjudgeuname.IsEnabled = true;
            }

        }

        private void btnjudgeconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                Account dataRow = (Account)dgAccounts.SelectedItem;
                dataRow.judgename = txbjudgename.Text.ToString();
                dataRow.username = txbjudgeuname.Text.ToString();
                dataRow.password = txbjudgepword.Text.ToString();
                dgAccounts.SelectedItem = dataRow;
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                account.Add(new Account(txbjudgename.Text, txbjudgeuname.Text, txbjudgepword.Text));
                dgAccounts.IsEnabled = true;
                dgAccounts.Items.Refresh();
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                Account dataRow = (Account)dgAccounts.SelectedItem;
                String s = "Confirm deletion of Account \n\nAccount Name: \t"+ dataRow.judgename.ToString()+"\nUsername: \t" + dataRow.username.ToString()+"\nPassword: \t"+ dataRow.password.ToString();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Confirm Deletion of Account", MessageBoxButtons.YesNo);
                if (dialogResult ==DialogResult.Yes)
                {
                    account.RemoveAt(dgAccounts.SelectedIndex);
                    cmbjudgeoptions.SelectedIndex = -1;
                    dgAccounts.IsEnabled = true;
                    dgAccounts.Items.Refresh();
                    dgAccounts.IsEnabled = false;
                }
            }
            cmbjudgeoptions_SelectionChanged();


        }
    }
}
