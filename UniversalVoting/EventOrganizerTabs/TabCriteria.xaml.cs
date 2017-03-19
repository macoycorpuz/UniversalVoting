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
    /// Interaction logic for TabCritera.xaml
    /// </summary>
    public partial class TabCriteria : System.Windows.Controls.UserControl
    {
        private int weightsum;
        private int _tabeventid;
        IDatabase clsDatabase;

        public TabCriteria(int _myeveid)
        {
            InitializeComponent();
            _tabeventid = _myeveid;
            RefreshDataGrids();

        }

        private void cmbcriteriaoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbcriteriaoptions_SelectionChanged();
        }
        private void cmbcriteriaoptions_SelectionChanged()
        {
            txbcweight.Text = txbcname.Text = "";
            cnameavail.Content = "";
            dgEventCriteria.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txbcweight.IsEnabled = false;
                txbcname.IsEnabled = false;
                dgEventCriteria.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Edit";
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                txbcweight.IsEnabled = true;
                txbcname.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Add";
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                txbcweight.IsEnabled = false;
                txbcname.IsEnabled = false;
                dgEventCriteria.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                txbcweight.IsEnabled = false;
                txbcname.IsEnabled = false;
                dgEventCriteria.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else
            {
                cmbjudgeoptions.SelectedIndex = -1;
                btnjudgeconfirm.Visibility = Visibility.Hidden;
            }
        }

        private void dgEventCriteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cnameavail.Content = "";
            if (dgEventCriteria.SelectedIndex == -1)
                return;
            if ((cmbjudgeoptions.SelectedIndex == 0) || (cmbjudgeoptions.SelectedIndex == 2))
            {
                DataRowView dataRow = (DataRowView)dgEventCriteria.SelectedItem;
                txbcname.Text = dataRow[0].ToString();
                txbcweight.Text = dataRow[1].ToString();
                txbcweight.IsEnabled = true;
                txbcname.IsEnabled = true;


            }

        }

        private void btncriteriaconfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow;
         
            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                #region edit           
                if (cnameavail.Content.ToString() == "Criteria Taken already")
                {
                    System.Windows.MessageBox.Show("Criteria already taken..");
                    return;
                }

                if (dgEventCriteria.SelectedIndex == -1)
                    return;

                if (int.Parse(txbcweight.Text.ToString()) + weightsum -
            int.Parse(clsDatabase.Data.Rows[dgEventCriteria.SelectedIndex].ItemArray[1].ToString()) > 100)
                {
                    System.Windows.MessageBox.Show("Weight cannot be Added");
                    return;
                }

                dataRow = (DataRowView)dgEventCriteria.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("spCheckCExistance",
                    "@cname", dataRow[0].ToString(),
                    "@_eventid", _tabeventid);

                string _criteriaid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("spUpdateEventCriteria",
                    "@cname", txbcname.Text,
                    "@weight", txbcweight.Text,
                    "@_eventid", _tabeventid,
                    "@critid",_criteriaid);

                if (!(clsDatabase.HasError))
                {
                    cnameavail.Content = "";
                    System.Windows.MessageBox.Show("Save Success");
                }
                RefreshDataGrids();
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                #region add
                if (cnameavail.Content.ToString() == "Criteria Taken already")
                {
                    System.Windows.MessageBox.Show("Criteria already exists..");
                    return;
                }

                if ((int.Parse(txbcweight.Text.ToString()) + weightsum > 100) || (weightsum == 100))
                {
                    System.Windows.MessageBox.Show("Weight cannot be Added");
                    return;
                }


                dataRow = (DataRowView)dgEventCriteria.SelectedItem;

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("spAddCriteriaToEventCriteria", 
                    "@cname", txbcname.Text,
                    "@weight", txbcweight.Text,
                    "@_eventid",_tabeventid);

                if (!(clsDatabase.HasError))
                {
                    System.Windows.MessageBox.Show("Criteria Creation Success");
                    dgEventCriteria.IsEnabled = true;
                    cmbjudgeoptions.SelectedIndex = -1;
                    RefreshDataGrids();
                    cnameavail.Content = "";
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {  
                #region delete from event

                if (dgEventCriteria.SelectedIndex == -1)
                    return;
                dataRow = (DataRowView)dgEventCriteria.SelectedItem;

                String s = "Confirm Removing Criteria from Event \n\nCriteria: \t" + dataRow[0].ToString() + "\nWeight: \t" + dataRow[1].ToString();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("spRemoveCriteriaFromEventCriteria",
                    "@cname", txbcname.Text,
                    "@_eventid", _tabeventid);

                    cmbjudgeoptions.SelectedIndex = -1;
                    dgEventCriteria.IsEnabled = true;
                    RefreshDataGrids();
                    dgEventCriteria.IsEnabled = false;
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                #region delete from database

                if (dgEventCriteria.SelectedIndex == -1)
                    return;
                dataRow = (DataRowView)dgEventCriteria.SelectedItem;

                String s = "Confirm Removing Criteria from Database?? \n\nCriteria: \t" + dataRow[0].ToString() +"\nWeight: \t" + dataRow[1].ToString();

                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Warning", MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("spRemoveCriteriaFromDatabase",
                    "@cname", dataRow[0].ToString(),
                    "@_eventid", _tabeventid);

                    if (!(clsDatabase.HasError))
                    {
                        System.Windows.MessageBox.Show("Deletion Successful");
                        cmbjudgeoptions.SelectedIndex = -1;
                        dgEventCriteria.IsEnabled = true;
                        RefreshDataGrids();
                        dgEventCriteria.IsEnabled = false;
                    }
                }
                #endregion
            }
            cmbcriteriaoptions_SelectionChanged();

        }
        
        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {
            if ((dgAllCriteria.SelectedIndex == -1) || (dgAllCriteria.SelectedItem == null))
                return;

            if((int.Parse(txbcweightadd.Text.ToString())+ weightsum>100)||(weightsum==100))
            {
                System.Windows.MessageBox.Show("Weight cannot be Added");
                return;
            }

            DataRowView dataRow = (DataRowView)dgAllCriteria.SelectedItem;
            clsDatabase.ExecuteStoredProc("spAddOldCriteriaToEventCriteria",
                "@cname", dataRow[0].ToString(),
                "@weight", txbcweightadd.Text,
                "@_eventid", _tabeventid);
            txbcweightadd.Text = "";
            RefreshDataGrids();

        }
        
        public void RefreshDataGrids()
        {
            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spViewNotEventCriteria", "@_eventid", _tabeventid);
            dgAllCriteria.ItemsSource = clsDatabase.Data.DefaultView;

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("[spViewEventCriteria]", "@_eventid", _tabeventid);
            dgEventCriteria.ItemsSource = clsDatabase.Data.DefaultView;
            weightsum = 0;
            foreach (DataRow x in clsDatabase.Data.Rows)
                weightsum += int.Parse((x.ItemArray[1].ToString()));
            ctotalweight.Content = weightsum.ToString();
        }

        public void passingidvalue(int x)
        {
            _tabeventid = x;
        }

        private void txbcname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IDatabase clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spCheckcnameavailability",
                 "@cname", txbcname.Text);
            if (clsDatabase.Data.Rows.Count == 0)
                cnameavail.Content = "Criteria Name Available";
            else
                cnameavail.Content = "Criteria Name Taken already";

        }
        
        private void dgAllCriteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAllCriteria.SelectedIndex > -1)
                txbcweightadd.IsEnabled = true;
            else
                txbcweightadd.IsEnabled = false;
        }
    }
}
