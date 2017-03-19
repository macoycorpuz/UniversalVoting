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


namespace UniversalVoting.JudgeTabs
{
    /// <summary>
    /// Interaction logic for TabVoting.xaml
    /// </summary>
    public partial class TabVoting : UserControl
    {
        IDatabase _clsDb;
        ObservableCollection<Contestant> contestants = new ObservableCollection<Contestant>();
        ObservableCollection<Contestant> contestantInfo = new ObservableCollection<Contestant>();
        ObservableCollection<Criteria> criteria = new ObservableCollection<Criteria>();

        public TabVoting()
        {
            InitializeComponent();
        }

        public TabVoting(int ID)
        {
            InitializeComponent();
            LoadContestants();
            LoadCriteria();
        }

        private void LoadContestants()
        {
            _clsDb = new Database();
            contestants.Add(new Contestant() { Name = "Kyle Floresta", Status = "1", Avatar = "../Images/iconAvatar.jpg" });
            contestants.Add(new Contestant() { Name = "Marcuz Corpuz", Status = "", Avatar = "../Images/iconAvatar.jpg" });
            contestants.Add(new Contestant() { Name = "Lols", Status = "1", Avatar = "../Images/iconAvatar.jpg" });
            lstContestants.ItemsSource = contestants;
        }

        private void LoadCriteria()
        {
            criteria.Add(new Criteria() { Name = "Questions and Answer" });
            criteria.Add(new Criteria() { Name = "Bikini" });
            criteria.Add(new Criteria() { Name = "Formal" });
            dtgrdCriteria.ItemsSource = criteria;
        }
    }


    #region Contestant Class

    public class Contestant : INotifyPropertyChanged
    {
        private string name;
        private string status;
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

        public string Status
        {
            get
            {
                if (this.status == "1")
                    return "../Images/iconCheck.png";
                else
                    return "../Images/iconCircle.png";
            }
            set
            {
                if (this.status != value)
                {
                    this.status = value;
                    this.NotifyPropertyChanged("Status");
                }
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
