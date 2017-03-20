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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalVoting;

namespace UniversalVoting.EventOrganizerTabs
{
    /// <summary>
    /// Interaction logic for TabContestants.xaml
    /// </summary>
    public partial class TabContestants : System.Windows.Controls.UserControl
    {
       
        private int _tabeventid;
        IDatabase clsDatabase;

        public TabContestants()
        {
            InitializeComponent();
        }

        public TabContestants(int _myeveid)
        {
            InitializeComponent();
            _tabeventid = _myeveid;
            RefreshDataGrids();

        }

        private void dgEventContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cnameavail.Content = "";
            if (dgEventContestants.SelectedIndex == -1)
                return;
            if ((cmbjudgeoptions.SelectedIndex == 0) || (cmbjudgeoptions.SelectedIndex == 2))
            {
                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                txbcfname.Text = dataRow[0].ToString();
                txbclname.Text = dataRow[1].ToString();
                txbclname.IsEnabled = true;
                txbcfname.IsEnabled = true;
                
            }
             //Get PersonID
             //Check kung may Picture nung PersonID
             //Kung wala, use Default generic Picture
             //Kung merong Picture , Use Picture
                

        }

        private void btncontestantconfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                #region edit           
                if (cnameavail.Content.ToString() == "Contestant Name Taken already")
                {
                    System.Windows.MessageBox.Show("Contestant already taken..");
                    return;
                }

                if (dgEventContestants.SelectedIndex == -1)
                    return;
                

                dataRow = (DataRowView)dgEventContestants.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                string _personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspUpdateContestant",
                    "@fname", txbcfname.Text,
                    "@lname", txbclname.Text,
                    "@perid", _personid);

                if (!(clsDatabase.HasError))
                {
                    cnameavail.Content = "";
                    System.Windows.MessageBox.Show("Save Success");
                }
                RefreshDataGrids();
                //Save ID picture
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                #region add
                if (cnameavail.Content.ToString() == "Contestant Name Taken already")
                {
                    System.Windows.MessageBox.Show("Contestant already taken..");
                    return;
                }

              
                dataRow = (DataRowView)dgEventContestants.SelectedItem;

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspAddPersonToContestant",
                    "@fname", txbcfname.Text,
                    "@lname", txbclname.Text,
                    "@_eventid", _tabeventid);

                if (!(clsDatabase.HasError))
                {
                    System.Windows.MessageBox.Show("Contestant Creation Success");
                    dgEventContestants.IsEnabled = true;
                    cmbjudgeoptions.SelectedIndex = -1;
                    RefreshDataGrids();
                    cnameavail.Content = "";
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                #region delete from event

                if (dgEventContestants.SelectedIndex == -1)
                    return;
                dataRow = (DataRowView)dgEventContestants.SelectedItem;

                String s = "Confirm Removing Contestant from Event \n\nFirstname: \t" + dataRow[0].ToString() + "\n\nLastname: \t" + dataRow[1].ToString();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("KFspRemoveContsestantFromEvent",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                    cmbjudgeoptions.SelectedIndex = -1;
                    dgEventContestants.IsEnabled = true;
                    RefreshDataGrids();
                    dgEventContestants.IsEnabled = false;
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                #region delete from database

                if (dgEventContestants.SelectedIndex == -1)
                    return;
                dataRow = (DataRowView)dgEventContestants.SelectedItem;

                String s = "Confirm Removing Contestant from Database?? \n\nFirst Name: \t" + dataRow[0].ToString() + "\nLast Name: \t" + dataRow[1].ToString();

                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Warning", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("KFspRemoveContsestantFromDatabase",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                    if (!(clsDatabase.HasError))
                    {
                        System.Windows.MessageBox.Show("Deletion Successful");
                        cmbjudgeoptions.SelectedIndex = -1;
                        dgEventContestants.IsEnabled = true;
                        RefreshDataGrids();
                        dgEventContestants.IsEnabled = false;
                    }
                }
                #endregion
            }
            cmbcontestantoptions_SelectionChanged();

        }

        public void RefreshDataGrids()
        {
            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewNotEventContestants", "@_eventid", _tabeventid);
            dgAllContestants.ItemsSource = clsDatabase.Data.DefaultView;

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewEventContestants", "@_eventid", _tabeventid);
            dgEventContestants.ItemsSource = clsDatabase.Data.DefaultView;

        }

        private void txbcfname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IDatabase clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspCheckconnameavailability",
                 "@fname", txbcfname.Text,
                 "@lname", txbclname.Text);

            if (clsDatabase.Data.Rows.Count == 0)
                cnameavail.Content = "Contestant Name Available";
            else
                cnameavail.Content = "Contestant Name Taken already";

        }

        public void passingidvalue(int x)
        {
            _tabeventid = x;
        }

        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {
            if ((dgAllContestants.SelectedIndex == -1) || (dgAllContestants.SelectedItem == null))
                return;

            DataRowView dataRow = (DataRowView)dgAllContestants.SelectedItem;
            clsDatabase.ExecuteStoredProc("KFspAddExistingPersontoContestant",
                "@fname", dataRow[0].ToString(),
                "@lname", dataRow[1].ToString(),
                "@_eventid", _tabeventid);

            RefreshDataGrids();

        }

        private void cmbcontestantoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbcontestantoptions_SelectionChanged();
        }
        private void cmbcontestantoptions_SelectionChanged()
        {
            txbclname.Text = txbcfname.Text = "";
            cnameavail.Content = "";
            dgEventContestants.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Edit";
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                txbclname.IsEnabled = true;
                txbcfname.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Add";
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else
            {
                cmbjudgeoptions.SelectedIndex = -1;
                btnjudgeconfirm.Visibility = Visibility.Hidden;
            }
        }
        
        private void dgAllContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAllContestants.SelectedIndex > -1)
                btnaddexisting.IsEnabled = true;
            else
                btnaddexisting.IsEnabled = false;
        }

        private void btneditpic_Click(object sender, RoutedEventArgs e)
        {
            //open file explorer for picture
            //select a picture
            //copy picture and place in github folder/images
            //save as PersonID
            //then update Picture Control
        }
    }
}
