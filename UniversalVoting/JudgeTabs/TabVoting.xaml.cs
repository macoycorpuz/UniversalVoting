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
        ObservableCollection<Contestant> contestants = new ObservableCollection<Contestant>();
        ObservableCollection<Contestant> contestantInfo = new ObservableCollection<Contestant>();
        ObservableCollection<Criteria> criteria = new ObservableCollection<Criteria>();


        public TabVoting()
        {
            InitializeComponent();
            LoadContestants();
            LoadCriteria();
            //Try lang
            //Try Commit kay KyleWork1 (add code tapos lagay karin comments para cool) hahaha
        }

        private void LoadContestants()
        {
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

        #region Rating

        //int intRate = 0;
        //int intCount = 1;
        //int Rate = 0;

        //private void LoadImages()
        //{
        //    for (int i = 1; i <= 5; i++)
        //    {
        //        Image img = new Image();
        //        img.Name = "imgRate" + i;
        //        img.Stretch = Stretch.UniformToFill;
        //        img.Height = 25;
        //        img.Width = 25;
        //        img.Source = new BitmapImage(new Uri(@"\Images\MinusRate.png", UriKind.Relative));
        //        img.MouseEnter += imgRateMinus_MouseEnter;
        //        pnlMinus.Children.Add(img);

        //        Image img1 = new Image();
        //        img1.Name = "imgRate" + i + i;
        //        img1.Stretch = Stretch.UniformToFill;
        //        img1.Height = 25;
        //        img1.Width = 25;
        //        img1.Visibility = Visibility.Hidden;
        //        img1.Source = new BitmapImage(new Uri(@"\Images\PlusRate.png", UriKind.Relative));
        //        img1.MouseEnter += imgRatePlus_MouseEnter;
        //        img1.MouseLeave += imgRatePlus_MouseLeave;
        //        img1.MouseLeftButtonUp += imgRatePlus_MouseLeftButtonUp;
        //        pnlPlus.Children.Add(img1);
        //    }
        //}

        //private void imgRateMinus_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    GetData(sender, Visibility.Visible, Visibility.Hidden);
        //}

        //private void imgRatePlus_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    GetData(sender, Visibility.Visible, Visibility.Hidden);
        //}

        //private void imgRatePlus_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    GetData(sender, Visibility.Hidden, Visibility.Visible);
        //}

        //private void gdRating_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    SetImage(Rate, Visibility.Visible, Visibility.Hidden);
        //}

        //private void GetData(object sender, Visibility imgYellowVisibility, Visibility imgGrayVisibility)
        //{
        //    GetRating(sender as Image);
        //    SetImage(intRate, imgYellowVisibility, imgGrayVisibility);
        //}

        //private void SetImage(int intRate, Visibility imgYellowVisibility, Visibility imgGrayVisibility)
        //{
        //    foreach (Image imgItem in pnlPlus.Children.OfType<Image>())
        //    {
        //        if (intCount <= intRate)
        //            imgItem.Visibility = imgYellowVisibility;
        //        else
        //            imgItem.Visibility = imgGrayVisibility;
        //        intCount++;
        //    }
        //    intCount = 1;
        //}

        //private void imgRatePlus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    GetRating(sender as Image);
        //    Rate = intRate;
        //    //lblRating.Text = intRate.ToString();
        //}

        //private void GetRating(Image Img)
        //{
        //    string strImgName = Img.Name;
        //    intRate = Convert.ToInt32(strImgName.Substring(strImgName.Length - 1, 1));
        //}

        #endregion
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
