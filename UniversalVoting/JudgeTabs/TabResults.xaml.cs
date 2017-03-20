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

            public string name { get; set; }
            public double score { get; set; }
            public string gender { get; set; }
            public double scorebar { get; set; }

            public Contestant(string _name, double _score, string _gender)
            {
                name = _name;
                score = _score;
                scorebar = _score;
                if (_gender.ToLower() == "male")
                    gender = "/images/defaultpicmale.jpg";
                else
                    gender = "/images/defaultpicfemale.jpg";
            }

        }

        #endregion

        #region Attributes

        public ObservableCollection<Contestant> _contestants { get; set; }
        IDatabase _clsDb;
        int _eventID = 0;
        DataTable _contestantsDT;
        DataTable _scoresDT;

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

            _clsDb.ExecuteStoredProc("MCspViewContestantsEvent", "@EventID", _eventID.ToString());
            if (_clsDb.Data.Rows.Count > 0)
                _contestantsDT = _clsDb.Data;

            foreach(DataRow cont in _contestantsDT.Rows)
            {
                string fullname = cont.Field<string>(1);
                double total = 0;
                int CID = cont.Field<int>(0);
                _clsDb.ExecuteStoredProc("MCspViewScoreWeight", "@ContestantID", CID.ToString());
                if (_clsDb.Data.Rows.Count > 0)
                    _scoresDT = _clsDb.Data;
                foreach(DataRow sco in _scoresDT.Rows)
                {
                    double score = sco.Field<double>(0)/10;
                    double weight = sco.Field<double>(1);
                    total += score * weight;
                }
                _contestants.Add(new Contestant(fullname, total, "male"));
            }

            //_criteria.Add(new Criteria() { Name = c.Field<string>(0), Score = Convert.ToInt32(_clsDb.Data.Rows[0].Field<double>(0)), Weight = c.Field<double>(1) });
            //_contestants = new ObservableCollection<Contestant>()
            //{
            //   new Contestant("Kyle",100,"male"),
            //   new Contestant("Marucz",70,"femaie"),
            //   new Contestant("Mark",69,"male"),
            //   new Contestant("Abel",70,"male"),
            //   new Contestant("Jim",50,"male"),
            //   new Contestant("Mike",70,"femaie"),
            //   new Contestant("Keanu",100,"male"),
            //   new Contestant("Betlog",70,"male"),
            //};
            myic.ItemsSource = _contestants;
        }
    }
}

