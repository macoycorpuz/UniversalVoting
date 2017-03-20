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
            public int score { get; set; }
            public string gender { get; set; }
            public int scorebar { get; set; }

            public Contestant(string _name, int _score, string _gender)
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

        public ObservableCollection<Contestant> contestants { get; set; }

        public TabResults()
        {
            InitializeComponent();
            contestants = new ObservableCollection<Contestant>()
            {
               new Contestant("Kyle",100,"male"),
               new Contestant("Marucz",70,"femaie"),
               new Contestant("Mark",69,"male"),
               new Contestant("Abel",70,"male"),
               new Contestant("Jim",50,"male"),
               new Contestant("Mike",70,"femaie"),
               new Contestant("Keanu",100,"male"),
               new Contestant("Betlog",70,"male"),
            };
            myic.ItemsSource = contestants;
        }
    }
}

