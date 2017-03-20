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
        ObservableCollection<Criteria> _criteria;
        DataTable _criteriaDT;
        DataTable _scoreDT;

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
            _criteria = new ObservableCollection<Criteria>();
            _criteriaDT = new DataTable();
            _clsDb = new Database();

            _clsDb.ExecuteStoredProc("[MCspViewCriteria]", "@EventJudgeID", _eventjudgejID);
            if (_clsDb.Data.Rows.Count > 0)
                _criteriaDT = _clsDb.Data;

            foreach (DataRow c in _criteriaDT.Rows)
            {
                _clsDb.ExecuteStoredProc("[MCspViewScore]", "@EventJudgeID", _eventjudgejID, "@ContestantID", _contestantID, "@EventCriteriaID", c.Field<int>(2).ToString());
                if (_clsDb.Data.Rows.Count > 0)
                    _criteria.Add(new Criteria() { Name = c.Field<string>(0), Score = Convert.ToInt32(_clsDb.Data.Rows[0].Field<double>(0)), Weight = c.Field<double>(1) });
                else
                    _criteria.Add(new Criteria() { Name = c.Field<string>(0), Score = 0, Weight = c.Field<double>(1) });
            }
            dtgrdCriteria.ItemsSource = _criteria;
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
        private double weight;
        private int score;

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

        public Grid Rating
        {
            get { return LoadRating(); }
        }

        public double Weight
        {
            get { return this.weight; }
            set
            {
                if (this.weight != value)
                {
                    this.weight = value;
                    this.NotifyPropertyChanged("Weight");
                }
            }
        }

        public int Score
        {
            get { return this.score; }
            set
            {
                if (this.score != value)
                {
                    this.score = value;
                    this.NotifyPropertyChanged("Score");
                }
            }
        }

        public Grid LoadRating()
        {
            Grid _grd = new Grid();
            RatingStar rs = new RatingStar(score);
            _grd.Children.Add(rs);   
            return _grd;
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
