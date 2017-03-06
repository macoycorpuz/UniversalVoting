﻿using System;
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
        public List<Account> allaccounts;
        public List<Account> eventaccounts;

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
            //IDatabase clsDatabase = new Database();
            //clsDatabase.ExecuteCommand("Select * From Critera");  
            //dgAccounts.ItemsSource = clsDatabase.Data.DefaultView;         

            #region  intializers

            eventaccounts = new List<Account>();
            allaccounts = new List<Account>()
            {
                new Account("Kyle","hateydiha","kantahan"),
                new Account("marcuz","malibog","marcuz_pass"),
                new Account("Grace","Ganda","hehe")
            };

            List<SqlDbType> mylist = new List<SqlDbType>();
            mylist.Add(SqlDbType.Bit);

            dgAllAccounts.ItemsSource = allaccounts;
            dgEventAccounts.ItemsSource = eventaccounts;

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
            txbjudgeuname.Text = txbjudgepword.Text = txbjudgename.Text = "";
            dgEventAccounts.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txbjudgename.IsEnabled = false;
                txbjudgepword.IsEnabled = false;
                txbjudgeuname.IsEnabled = false;
                dgEventAccounts.IsEnabled = true;
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
                //edit a judge
                Account dataRow = (Account)dgEventAccounts.SelectedItem;
                dataRow.judgename = txbjudgename.Text.ToString();
                dataRow.username = txbjudgeuname.Text.ToString();
                dataRow.password = txbjudgepword.Text.ToString();
                dgEventAccounts.SelectedItem = dataRow;
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                //adding a judge      
                eventaccounts.Add(new Account(txbjudgename.Text, txbjudgeuname.Text, txbjudgepword.Text));
                dgEventAccounts.IsEnabled = true;
                dgEventAccounts.Items.Refresh();
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                //deleting a judge
                Account dataRow = (Account)dgEventAccounts.SelectedItem;
                String s = "Confirm deletion of Account \n\nAccount Name: \t"+ dataRow.judgename.ToString()+"\nUsername: \t" + dataRow.username.ToString()+"\nPassword: \t"+ dataRow.password.ToString();
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
            lblerrorindicator1.Content = "";
        }

        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {

            bool doesnotexist = true;

            Account dataRow = (Account)dgAllAccounts.SelectedItem;
            for(int x = 0; x<eventaccounts.Count();x++)
            {
                if ((dataRow.judgename == eventaccounts[x].judgename || dataRow.username == eventaccounts[x].username | dataRow.password == eventaccounts[x].password))
                {
                    doesnotexist = true;
                    lblerrorindicator1.Content = "Account Already exists";
                    return;
                }

            }
            if (doesnotexist)
            {
                lblerrorindicator1.Content = "Account Added";
                eventaccounts.Add(new Account(dataRow.judgename.ToString(), dataRow.username.ToString(), dataRow.password.ToString()));
                dgEventAccounts.Items.Refresh();
            }
        }
    }
}
