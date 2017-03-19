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

namespace UniversalVoting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class JudgeWindow : Window
    {
        #region Attributes
        IDatabase _clsDb;
        int _judgeid = 2;
        List<string> _eventnames = new List<string>();
        #endregion

        public JudgeWindow()
        {
            InitializeComponent();
            LoadEvents();
        }

        public JudgeWindow(int JID)
        {
            InitializeComponent();
            LoadEvents();
            _judgeid = JID;
        }

        private void LoadEvents()
        {
            _clsDb = new Database();
            cbxEvent.Items.Clear();
            _eventnames.Clear();
            _clsDb.ExecuteCommand("SELECT * FROM EVENT");
            foreach (DataRow events in _clsDb.Data.Rows)
            {
                cbxEvent.Items.Add(events.Field<string>(0));
                _eventnames.Add(events.Field<string>(0));
            }
        }

        private void cbxEvent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _clsDb = new Database();
            _clsDb.ExecuteStoredProc("spViewJudgeEvent", "@EventName", cbxEvent.SelectedValue.ToString(), "@JID", _judgeid.ToString());
            if (_clsDb.Data.Rows.Count > 0)
            {
                TabVoting votingtab = new TabVoting(_clsDb.Data.Rows[0].Field<int>(0));
                UserVotingTab.Children.Add(votingtab);
                TabResults resultstab = new TabResults();
                UserResultsTab.Children.Add(resultstab);
            }
        }
    }

}
