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

namespace UniversalVoting
{
    /// <summary>
    /// Interaction logic for EventOrganizerWindow.xaml
    /// </summary>
    public partial class EventOrganizerWindow : Window
    {

        private int _eventid;
        public EventOrganizerWindow()
        {
            InitializeComponent();
        }

        public EventOrganizerWindow(int event_id)
        {
            InitializeComponent();
            _eventid = event_id;
        }

        //using the Database
        /*
            IDatabase clsDatabase;                                  //initialize the instance
            clsDatabase.ExecuteAdapterQuery(tb1.Text.ToString());   //pass the query
            dg1.ItemsSource = clsDatabase.Data.DefaultView;         //retrieve the data
         
         
         */
    }
}
