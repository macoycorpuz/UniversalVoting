using UniversalVoting.JudgeTabs;
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
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace UniversalVoting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class JudgeWindow : Window
    {
        #region Attributes

        IDatabase _clsDb;
        int _judgeid = 1;
        //ObservableCollection<Contestant> _contestants;
        DataRowView _selectedContestant = null;
        DataTable _contestants;
        DataTable c;

        #endregion

        public JudgeWindow()
        {
            InitializeComponent();
            LoadEvents();
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewJudges", "@JudgeID", _judgeid.ToString());
            //try catch

            txtJudge.Text = "Hello " + _clsDb.Data.Rows[0].Field<string>(0) + " " +_clsDb.Data.Rows[0].Field<string>(1) + "!!";
        }

        public JudgeWindow(int JID)
        {
            InitializeComponent();
            _judgeid = JID;
            LoadEvents();
        }

        private string ProfilePic(int personID)
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string image = personID.ToString() + ".jpg";
            string strDirectory = System.IO.Path.Combine(dir, "Images", image);
            if (File.Exists(strDirectory))
                return strDirectory;
            else
                return "../Images/iconAvatar.jpg";
        }

        private string IsVoteOk(int CID)
        {
            bool status = true;
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewStatus", "@EventName", cbxEvent.SelectedValue.ToString(), "@JudgeID", _judgeid.ToString(), "@ContestantID", CID.ToString());
            foreach(DataRow Score in _clsDb.Data.Rows)
            {
                if (Score.Field<double>(0) == 0)
                    status = false;
            }

            if (status)
                return "../Images/iconCheck.png";
            else
                return "../Images/iconCircle.png";
        }

        #region Selection

        private void cbxEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserVotingTab.Children.Clear();
            LoadResultTab();
            LoadContestants();
        }

        private void lstContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LoadVotingTab();
        }

        private void ListViewItem_OnConversationClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                _selectedContestant = null;
                _selectedContestant = item.DataContext as DataRowView;
                LoadVotingTab(Convert.ToInt32(_selectedContestant.Row["ID"].ToString()));
            }
        }

        private void lstContestants_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Load Items

        private void LoadEvents()
        {
            _clsDb = new Database();
            cbxEvent.Items.Clear();
            _clsDb.ExecuteCommand("SELECT * FROM EVENT");
            foreach (DataRow events in _clsDb.Data.Rows)
            {
                cbxEvent.Items.Add(events.Field<string>(0));
            }
        }

        private void LoadContestants()
        {
            _clsDb = new Database();
            _contestants = new DataTable();
            c = new DataTable();
            if (cbxEvent.SelectedIndex != -1)
            {
                c.Columns.Add("ID");
                c.Columns.Add("Name");
                c.Columns.Add("Status");
                c.Columns.Add("Avatar");
                _clsDb.ExecuteStoredProc("MCspViewContestants", "@EventName", cbxEvent.SelectedValue.ToString());
                if (_clsDb.Data.Rows.Count > 0)
                    _contestants = _clsDb.Data;

                string name = "";
                foreach (DataRow contestants in _contestants.Rows)
                {
                    name = contestants.Field<int>(0).ToString() + ". " + contestants.Field<string>(1) + " " + contestants.Field<string>(2);
                    c.Rows.Add(contestants.Field<int>(4), name, IsVoteOk(contestants.Field<int>(4)), ProfilePic(contestants.Field<int>(3)));
                }
                lstContestants.ItemsSource = c.DefaultView;
            }
        }

        private void LoadResultTab()
        {
            //UserVotingTab.Children.Clear();
            //UserResultsTab.Children.Clear();
            //_clsDb = new Database();
            //_clsDb.ExecuteStoredProc("MCspViewJudgeEvent", "@EventName", cbxEvent.SelectedValue.ToString(), "@JID", _judgeid.ToString());
            //if (_clsDb.Data.Rows.Count > 0)
            //{
            //    TabVoting votingtab = new TabVoting(_clsDb.Data.Rows[0].Field<int>(0));
            //    UserVotingTab.Children.Add(votingtab);
            //    TabResults resultstab = new TabResults();
            //    UserResultsTab.Children.Add(resultstab);
            //}
        }

        private void LoadVotingTab(int contestantID)
        {
            UserVotingTab.Children.Clear();
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewJudgeEvent", "@EventName", cbxEvent.SelectedValue.ToString(), "@JID", _judgeid.ToString());
            if (_clsDb.Data.Rows.Count > 0)
            {
                if (lstContestants.SelectedIndex != -1)
                {
                    TabVoting votingtab = new TabVoting(_clsDb.Data.Rows[0].Field<int>(0), contestantID);
                    UserVotingTab.Children.Add(votingtab);
                }
            }
        }

        #endregion

    }
}
