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
  
    public partial class TabJudges : System.Windows.Controls.UserControl 
    {
        private int _tabeventid;
        IDatabase clsDatabase;

        public TabJudges(int _myeveid)
        {
            InitializeComponent();
            _tabeventid = _myeveid;
            RefreshDataGrids();
            
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
            else if (cmbjudgeoptions.SelectedIndex == 3)
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
            unameavail.Content = "";
            if (dgEventAccounts.SelectedIndex == -1)
                return;
            if ((cmbjudgeoptions.SelectedIndex == 0)||(cmbjudgeoptions.SelectedIndex == 2))
            {
                DataRowView dataRow = (DataRowView)dgEventAccounts.SelectedItem;
                txbfname.Text = dataRow[0].ToString();
                txblname.Text = dataRow[1].ToString();
                txbjudgeuname.Text = dataRow[2].ToString();
                txbjudgepword.Text = dataRow[3].ToString();
                txblname.IsEnabled = true;
                txbfname.IsEnabled = true;
                txbjudgepword.IsEnabled = true;
                txbjudgeuname.IsEnabled = true;

            }

        }

        private void btnjudgeconfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow;
           

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                #region edit           
                if (unameavail.Content.ToString() == "Username Taken already")
                {
                    System.Windows.MessageBox.Show("Username already taken..");
                    return;
                }

                 dataRow = (DataRowView)dgEventAccounts.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("spCheckPersonExistanceinEvent", 
                    "@fname", dataRow[0].ToString(), 
                    "@lname", dataRow[1].ToString(),
                    "@uname", dataRow[2].ToString(),
                    "@pass", dataRow[3].ToString(),
                    "@eventid",_tabeventid);

               string _personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("spUpdatePersonJudgeDetails",
                    "@fname", txbfname.Text,
                    "@lname", txblname.Text,
                    "@uname", txbjudgeuname.Text,
                    "@pass", txbjudgepword.Text,
                    "@eventid", _tabeventid,
                    "@personid",_personid
                    );

                if (!(clsDatabase.HasError))
                {
                    unameavail.Content = "";
                    System.Windows.MessageBox.Show("Save Success");
                }
                RefreshDataGrids();
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                #region add
                if (unameavail.Content.ToString() == "Username Taken already")
                {
                    System.Windows.MessageBox.Show("Username already taken..");
                    return;
                }

                dataRow = (DataRowView)dgEventAccounts.SelectedItem;

                clsDatabase = new Database();
                  clsDatabase.ExecuteStoredProc("spAddPersonToEventJudges",
                    "@fname", txbfname.Text,
                    "@lname", txblname.Text,
                    "@uname", txbjudgeuname.Text,
                    "@pass", txbjudgepword.Text,
                    "@_eventid", _tabeventid);

                if (!(clsDatabase.HasError))
                {
                    System.Windows.MessageBox.Show("Account Creation Success");
                    dgEventAccounts.IsEnabled = true;
                    cmbjudgeoptions.SelectedIndex = -1;
                    RefreshDataGrids();
                    unameavail.Content = "";
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                #region delete from event
                dataRow = (DataRowView)dgEventAccounts.SelectedItem;
            
                String s = "Confirm deletion of Account \n\nAccount Name: \t"+
                    dataRow[0].ToString()+" "+ dataRow[1].ToString() + 
                    "\nUsername: \t" + dataRow[2].ToString()+"\nPassword: \t"+
                    dataRow[3].ToString();

                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Confirm Deletion of Account", MessageBoxButtons.YesNo);
                if (dialogResult ==DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("spRemoveJudgefromEventJudges",
                      "@fname", dataRow[0].ToString(),
                      "@lname", dataRow[1].ToString(),
                      "@uname", dataRow[2].ToString(),
                      "@pass", dataRow[3].ToString());

                    cmbjudgeoptions.SelectedIndex = -1;
                    dgEventAccounts.IsEnabled = true;
                    RefreshDataGrids();
                    dgEventAccounts.IsEnabled = false;
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                #region delete from database
                dataRow = (DataRowView)dgEventAccounts.SelectedItem;

                String s = "Confirm deletion of Account From Database?? \n\nAccount Name: \t" +
                    dataRow[0].ToString() + " " + dataRow[1].ToString() +
                    "\nUsername: \t" + dataRow[2].ToString() + "\nPassword: \t" +
                    dataRow[3].ToString();

                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Confirm Deletion of Account", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();

                    clsDatabase.ExecuteStoredProc("spRemovePersonfromDatabase",
                      "@fname", dataRow[0].ToString(),
                      "@lname", dataRow[1].ToString(),
                      "@uname", dataRow[2].ToString(),
                      "@pass", dataRow[3].ToString());

                    if (!(clsDatabase.HasError))
                    {
                        System.Windows.MessageBox.Show("Deletion Successful");
                        cmbjudgeoptions.SelectedIndex = -1;
                        dgEventAccounts.IsEnabled = true;
                        RefreshDataGrids();
                        dgEventAccounts.IsEnabled = false;
                    }
                }
                #endregion
            }
            cmbjudgeoptions_SelectionChanged();
            
        }

        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {
            if ((dgAllAccounts.SelectedIndex == -1)||(dgAllAccounts.SelectedItem==null))
                return;

            DataRowView dataRow = (DataRowView)dgAllAccounts.SelectedItem;
            clsDatabase.ExecuteStoredProc("spAddJudgeToEventJudges", 
                    "@fname", dataRow[0].ToString(), 
                    "@lname", dataRow[1].ToString(),
                    "@uname", dataRow[2].ToString(),
                    "@pass", dataRow[3].ToString(),
                    "@eventid",_tabeventid);
        
            RefreshDataGrids();

        }

        public void RefreshDataGrids()
        {
            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spViewNotEventJudges","@_eventid", _tabeventid);
            dgAllAccounts.ItemsSource = clsDatabase.Data.DefaultView;
      
            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spViewEventJudges", "@_eventid", _tabeventid);
            dgEventAccounts.ItemsSource = clsDatabase.Data.DefaultView;
        }

        public void passingidvalue(int x)
        {
            _tabeventid = x;
        }

        private void txbjudgeuname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IDatabase clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spCheckUnameavailability",
                 "@judgechars", txbjudgeuname.Text);
            if (clsDatabase.Data.Rows.Count == 0)
                unameavail.Content = "Username Available";
            else
                unameavail.Content = "Username Taken already";
            
        }

        }
    }

