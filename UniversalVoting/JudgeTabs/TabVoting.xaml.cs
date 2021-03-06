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
        #region Attributes

        IDatabase _clsDb;
        int _eventjudgejID, _contestantID;
        ObservableCollection<Criteria> _criteria;
        DataTable _criteriaDT;
        DataTable _scoreDT;
        int _rate;
        int EventCriteriaID;
        #endregion

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

        private void dtgrdCriteria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                object item = dtgrdCriteria.SelectedItem;
                string name = (dtgrdCriteria.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                _clsDb.ExecuteStoredProc("[MCspGetEventCriteriaID]", "@Name", name, "@EventJudgeID", _eventjudgejID);
                EventCriteriaID = _clsDb.Data.Rows[0].Field<int>(0);
                _clsDb = new Database();
                _clsDb.ExecuteStoredProc("[MCspViewScore]", "@EventJudgeID", _eventjudgejID, "@ContestantID", _contestantID, "@EventCriteriaID", EventCriteriaID.ToString());
                _rate = Convert.ToInt32(_clsDb.Data.Rows[0].ItemArray.GetValue(0).ToString());
                lblCriteria.Content = name;
                lblCriteria.Visibility = Visibility.Visible;
                txtRate.Visibility = Visibility.Visible;
                btnRefresh.Visibility = Visibility.Visible;
                txtRate.Text = _rate.ToString();
                //pnlRateHere.Children.Clear();
                //RatingStar rs = new RatingStar(_rate, false, _eventjudgejID, _contestantID, EventCriteriaID);
                //Label lblCriteria = new Label();
                //lblCriteria.VerticalAlignment = VerticalAlignment.Center;
                //lblCriteria.Margin = new Thickness(10);
                //lblCriteria.FontFamily = new FontFamily("../Fonts/Helvetica.otf#Helvetica");
                //lblCriteria.FontSize = 21;
                //lblCriteria.Content = name;
                //pnlRateHere.Children.Add(lblCriteria);
                //pnlRateHere.Children.Add(rs);
            }
            catch 
            {
            }

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _clsDb = new Database();
            int temp;
            if (int.TryParse(txtRate.Text, out temp))
            {
                int r = Convert.ToInt32(txtRate.Text);
                if (r > 0 && r <= 10)
                    _clsDb.ExecuteStoredProc("[MCspUpdateScore]", "@EventJudgeID", _eventjudgejID.ToString(), "@ContestantID", _contestantID.ToString(), "@EventCriteriaID", EventCriteriaID.ToString(), "@Score", txtRate.Text.ToString());
                else
                    MessageBox.Show("Invalid Rating!");
            }
            else
                MessageBox.Show("Invalid Rating!");
            LoadCriteria();
        }

        private string ProfilePic(int personID)
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string image = "personpic" + personID.ToString() + ".jpg";
            string strDirectory = System.IO.Path.Combine(dir, "Images", image);
            if (File.Exists(strDirectory))
                return strDirectory;
            else
                return "../Images/iconAvatar.jpg";
        }

        public string ContestantImage { get; set; }

        #region Load Items

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

            _clsDb.ExecuteStoredProc("[MCspViewCriteria]", "@EventJudgeID", _eventjudgejID.ToString());
            if (_clsDb.Data.Rows.Count > 0)
                _criteriaDT = _clsDb.Data;

            foreach (DataRow c in _criteriaDT.Rows)
            {
                _clsDb.ExecuteStoredProc("[MCspViewScore]", "@EventJudgeID", _eventjudgejID.ToString(), "@ContestantID", _contestantID.ToString(), "@EventCriteriaID", c.Field<int>(2).ToString());
                if (_clsDb.Data.Rows.Count > 0)
                    _criteria.Add(new Criteria() { Name = c.Field<string>(0), Score = Convert.ToInt32(_clsDb.Data.Rows[0].Field<double>(0)), Weight = c.Field<double>(1) });
                else
                    _criteria.Add(new Criteria() { Name = c.Field<string>(0), Score = 0, Weight = c.Field<double>(1) });
            }

            dtgrdCriteria.ItemsSource = _criteria;
        }

        #endregion
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
            RatingStar rs = new RatingStar(score, true, 0, 0, 0);
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
