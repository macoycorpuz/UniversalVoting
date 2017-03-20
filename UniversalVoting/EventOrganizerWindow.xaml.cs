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
using System.Windows.Shapes;
using UniversalVoting.EventOrganizerTabs;

namespace UniversalVoting
{
    /// <summary>
    /// Interaction logic for EventOrganizerWindow.xaml
    /// </summary>
    public partial class EventOrganizerWindow : Window
    {
        private int _eventid;
        public EventOrganizerWindow(int k)
        {
            _eventid = k;

            InitializeComponent();

            TabJudges junjun = new TabJudges(_eventid);
            markpogi.Children.Add(junjun);

            TabCriteria junatahan = new TabCriteria(_eventid);
            markpanget.Children.Add(junatahan);

            TabContestants jejeboy = new TabContestants(_eventid);
            markewan.Children.Add(jejeboy);

            Tabfinalize kiki = new Tabfinalize(_eventid,this);
            markcantot.Children.Add(kiki);

            //gawa ng taga finalize na window


        }

        public int getID()
        { return _eventid; }

    
    }
}
