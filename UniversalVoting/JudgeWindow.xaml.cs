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
        ObservableCollection<Contestant> _contestants;

        #endregion

        public JudgeWindow()
        {
            InitializeComponent();
            LoadEvents();
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewJudges", "@JudgeID", _judgeid.ToString());
            txtJudge.Text = "Hello " + _clsDb.Data.Rows[0].Field<string>(0) + " " +_clsDb.Data.Rows[0].Field<string>(1) + "!!";
        }

        public JudgeWindow(int JID)
        {
            InitializeComponent();
            _judgeid = JID;
            LoadEvents();
        }

        private void cbxEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadResultTab();
            LoadContestants();
        }

        private void lstContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadVotingTab();
        }

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
            _contestants = new ObservableCollection<Contestant>();
            if (cbxEvent.SelectedIndex != -1)
            {
                _clsDb.ExecuteStoredProc("MCspViewContestants", "@EventName", cbxEvent.SelectedValue.ToString());
                string name = "";
                bool status = false;
                foreach (DataRow contestants in _clsDb.Data.Rows)
                {
                    name = contestants.Field<int>(0).ToString() + ". " + contestants.Field<string>(1) + " " + contestants.Field<string>(2);
                    //Insert status of rating process
                    _contestants.Add(new Contestant() { Name = name, IsChecked = status, Avatar = ProfilePic(contestants.Field<int>(3)) });
                }
                lstContestants.ItemsSource = _contestants;
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

        private void LoadVotingTab()
        {
            UserVotingTab.Children.Clear();
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewJudgeEvent", "@EventName", cbxEvent.SelectedValue.ToString(), "@JID", _judgeid.ToString());
            if (_clsDb.Data.Rows.Count > 0)
            {
                if (lstContestants.SelectedIndex != -1)
                {
                    TabVoting votingtab = new TabVoting(_clsDb.Data.Rows[0].Field<int>(0), 2);
                    UserVotingTab.Children.Add(votingtab);
                }
            }
        }

        #endregion

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
    }

    #region Contestant Class

    public class Contestant : INotifyPropertyChanged
    {
        private string name;
        private bool status;
        private string avatar;

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public bool IsChecked
        {
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public string Status
        {
            get
            {
                if (status)
                    return "../Images/iconCheck.png";
                else
                    return "../Images/iconCircle.png";
            }
        }

        public string Avatar
        {
            get { return this.avatar; }
            set
            {
                if (this.avatar != value)
                {
                    this.avatar = value;
                    this.NotifyPropertyChanged("Avatar");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    #endregion

}
