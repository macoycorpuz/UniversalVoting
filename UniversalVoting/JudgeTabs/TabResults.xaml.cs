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
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace UniversalVoting.JudgeTabs
{
    /// <summary>
    /// Interaction logic for TabResults.xaml
    /// </summary>
    public partial class TabResults : UserControl
    {
        
        #region Contestant Class

        public class Contestant
        {
            int _eventID = 0;
            bool IsResultsFinalize = true;

            private string ProfilePic(int personID)
            {
                string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string image = "personpic" + personID.ToString() + ".jpg";
                string strDirectory = System.IO.Path.Combine(dir, "Images", image);
                // x = 0;

                IDatabase _clsDb = new Database();
                _clsDb.ExecuteStoredProc("MCspViewOfficialResults", "@EventID", _eventID);
                if (_clsDb.Data.Rows.Count > 0)
                    foreach(DataRow scores in _clsDb.Data.Rows)
                    {
                        //if (Convert.ToDouble(_clsDb.Data.Rows[x].ItemArray.GetValue(0).ToString()) > 0)
                        //    IsResultsFinalize = false;
                        //x++;
                        if (scores.Field<double>(0) == 0)
                            IsResultsFinalize = false;
                    }

                if (File.Exists(strDirectory) && IsResultsFinalize)
                    return strDirectory;
                else
                    return "../Images/iconAvatar.jpg";
            }
            public string name { get; set; }
            public double score { get; set; }
            public string gender { get; set; }
            public double scorebar { get; set; }

            public Contestant(string _name, double _score, int eventID, int personID)
            {
                _eventID = eventID;
                score = _score;
                scorebar = _score;
                gender = ProfilePic(personID);
                if (IsResultsFinalize)
                    name = _name;
                else
                    name = "Anonymous";
            }
        }

        #endregion

        #region Attributes

        public ObservableCollection<Contestant> _contestants { get; set; }
        IDatabase _clsDb;
        int _eventID = 0;
        DataTable _contestantsDT;
        DataTable _scoresDT;
        DataTable _judgesDT;

        #endregion

        public TabResults(int eventID)
        {
            InitializeComponent();
            _eventID = eventID;
            LoadResults();
        }

        private void LoadResults()
        {
            _contestants = new ObservableCollection<Contestant>();
            _clsDb = new Database();
            _contestantsDT = new DataTable();
            _scoresDT = new DataTable();
            _judgesDT = new DataTable();

            _clsDb.ExecuteStoredProc("MCspViewContestantsEvent", "@EventID", _eventID.ToString());
            if (_clsDb.Data.Rows.Count > 0)
                _contestantsDT = _clsDb.Data;

            foreach(DataRow cont in _contestantsDT.Rows)
            {
                string fullname = cont.Field<string>(1);
                double final = 0;
                double total = 0;
                int judgectr = 0;

                _clsDb.ExecuteCommand("SELECT EventJudgesID FROM EventJudges WHERE EventID = " + _eventID);
                int CID = cont.Field<int>(0);
                if (_clsDb.Data.Rows.Count > 0)
                    _judgesDT = _clsDb.Data;
                foreach (DataRow judge in _judgesDT.Rows)
                {
                    judgectr++;
                    _clsDb.ExecuteStoredProc("MCspViewScoreWeight", "@ContestantID", CID.ToString(), "@EventJudgesID", judge.Field<int>(0));
                    if (_clsDb.Data.Rows.Count > 0)
                        _scoresDT = _clsDb.Data;
                    foreach (DataRow sco in _scoresDT.Rows)
                    {
                        double score = sco.Field<double>(0) / 10;
                        double weight = sco.Field<double>(1);
                        total += score * weight;
                    }
                }
                final += total / judgectr;
                _clsDb.ExecuteCommand("UPDATE Contestant SET TotalScore = " + final.ToString() + "WHERE ContestantID = " + CID.ToString());
                _contestants.Add(new Contestant(fullname, final, _eventID, cont.Field<int>(2)));
            }
            myic.ItemsSource = _contestants;
        }
    }
}

