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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace UniversalVoting.JudgeTabs
{
    /// <summary>
    /// Interaction logic for TabVoting.xaml
    /// </summary>
    public partial class TabVoting : UserControl
    {
        IDatabase _clsDb;
        int _eventjudgejID, _contestantID;

        ObservableCollection<Criteria> criteria = new ObservableCollection<Criteria>();

        public TabVoting()
        {
            InitializeComponent();
        }

        public TabVoting(int eventjudgeID, int contestantID)
        {
            InitializeComponent();
            imgHere.DataContext = this;
            _eventjudgejID = eventjudgeID;
            _contestantID = contestantID;
            _contestantID = 8;
            LoadContestant();
            LoadCriteria();
        }

        private void LoadContestant()
        {
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("MCspViewContestant", "@ContestantID", _contestantID);
            if (_clsDb.Data.Rows.Count > 0)
            {
                lblContestantName.Content = _clsDb.Data.Rows[0].Field<int>(0).ToString() + ". " + _clsDb.Data.Rows[0].Field<string>(1) + " " +_clsDb.Data.Rows[0].Field<string>(2);
                ContestantImage = ProfilePic(_clsDb.Data.Rows[0].Field<int>(3));
            }
        }

        private void LoadCriteria()
        {
            criteria.Add(new Criteria() { Name = "Questions and Answer" });
            criteria.Add(new Criteria() { Name = "Bikini" });
            criteria.Add(new Criteria() { Name = "Formal" });
            dtgrdCriteria.ItemsSource = criteria;
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

        public string ContestantImage { get; set; }
    }
    

    #region Criteria Class

    public class Criteria : INotifyPropertyChanged
    {
        private string name;

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
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    #endregion
}
