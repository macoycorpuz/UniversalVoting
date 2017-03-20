using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UniversalVoting;

namespace UniversalVoting.EventOrganizerTabs
{
    /// <summary>
    /// Interaction logic for TabContestants.xaml
    /// </summary>
    public partial class TabContestants : System.Windows.Controls.UserControl
    {
        private int _tabeventid;
        IDatabase clsDatabase;

        public TabContestants()
        {
            InitializeComponent();
        }

        public TabContestants(int _myeveid)
        {
            InitializeComponent();
            _tabeventid = _myeveid;
            RefreshDataGrids();

        }

        private void dgEventContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cnameavail.Content = "";
            if (dgEventContestants.SelectedIndex == -1)
                return;
            if ((cmbjudgeoptions.SelectedIndex == 0) || (cmbjudgeoptions.SelectedIndex == 2))
            {
                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                txbcfname.Text = dataRow[0].ToString();
                txbclname.Text = dataRow[1].ToString();
                txbclname.IsEnabled = true;
                txbcfname.IsEnabled = true;
                btneditpic.Visibility = Visibility.Visible;
            }
            if(cmbjudgeoptions.SelectedIndex==1)
            {

                btneditpic.Visibility = Visibility.Hidden;
            }
            checkpicture();


        }

        private void btncontestantconfirm_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRow;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                #region edit      
                if (dgEventContestants.SelectedIndex == -1)
                    return;

                if (!checkfields())
                {
                    System.Windows.MessageBox.Show("One or More fields is not filled up.");
                    return;
                }


                if (cnameavail.Content.ToString() == "Contestant Name Taken already")
                {
                    System.Windows.MessageBox.Show("Contestant already taken..");
                    return;
                }



                dataRow = (DataRowView)dgEventContestants.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                string _personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspUpdateContestant",
                    "@fname", txbcfname.Text,
                    "@lname", txbclname.Text,
                    "@perid", _personid);

                if (!(clsDatabase.HasError))
                {
                    cnameavail.Content = "";
                    System.Windows.MessageBox.Show("Save Success");
                }
                RefreshDataGrids();
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                #region add
                if (cnameavail.Content.ToString() == "Contestant Name Taken already")
                {
                    System.Windows.MessageBox.Show("Contestant already taken..");
                    return;
                }

                if (!checkfields())
                {
                    System.Windows.MessageBox.Show("One or More fields is not filled up.");
                    return;
                }


                //dataRow = (DataRowView)dgEventContestants.SelectedItem;

                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspAddPersonToContestant",
                    "@fname", txbcfname.Text,
                    "@lname", txbclname.Text,
                    "@_eventid", _tabeventid);

                if (!(clsDatabase.HasError))
                {
                    System.Windows.MessageBox.Show("Contestant Creation Success");
                    dgEventContestants.IsEnabled = true;
                    cmbjudgeoptions.SelectedIndex = -1;
                    RefreshDataGrids();
                    cnameavail.Content = "";
                }
                #endregion
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                #region delete from event

                if (dgEventContestants.SelectedIndex == -1)
                    return;

                dataRow = (DataRowView)dgEventContestants.SelectedItem;

                String s = "Confirm Removing Contestant from Event \n\nFirstname: \t" + dataRow[0].ToString() + "\n\nLastname: \t" + dataRow[1].ToString();
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(s, "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    clsDatabase = new Database();
                    clsDatabase.ExecuteStoredProc("KFspRemoveContsestantFromEvent",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                    cmbjudgeoptions.SelectedIndex = -1;
                    dgEventContestants.IsEnabled = true;
                    RefreshDataGrids();
                    dgEventContestants.IsEnabled = false;
                }
                #endregion
            }

            else
            cmbcontestantoptions_SelectionChanged();
            disableall();
        }

        public void RefreshDataGrids()
        {
            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewNotEventContestants", "@_eventid", _tabeventid);
            dgAllContestants.ItemsSource = clsDatabase.Data.DefaultView;

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewEventContestants", "@_eventid", _tabeventid);
            dgEventContestants.ItemsSource = clsDatabase.Data.DefaultView;

        }

        private void txbcfname_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IDatabase clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspCheckconnameavailability",
                 "@fname", txbcfname.Text,
                 "@lname", txbclname.Text);

            if (clsDatabase.Data.Rows.Count == 0)
                cnameavail.Content = "Contestant Name Available";
            else
                cnameavail.Content = "Contestant Name Taken already";

        }

        public void passingidvalue(int x)
        {
            _tabeventid = x;
        }

        private void btnaddexisting_Click(object sender, RoutedEventArgs e)
        {
            if ((dgAllContestants.SelectedIndex == -1) || (dgAllContestants.SelectedItem == null))
                return;

            DataRowView dataRow = (DataRowView)dgAllContestants.SelectedItem;
            clsDatabase.ExecuteStoredProc("KFspAddExistingPersontoContestant",
                "@fname", dataRow[0].ToString(),
                "@lname", dataRow[1].ToString(),
                "@_eventid", _tabeventid);

            RefreshDataGrids();

        }

        private void cmbcontestantoptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbcontestantoptions_SelectionChanged();
        }
        private void cmbcontestantoptions_SelectionChanged()
        {
            txbclname.Text = txbcfname.Text = "";
            cnameavail.Content = "";
            dgEventContestants.IsEnabled = false;

            if (cmbjudgeoptions.SelectedIndex == 0)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Edit";
            }
            else if (cmbjudgeoptions.SelectedIndex == 1)
            {
                txbclname.IsEnabled = true;
                txbcfname.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btneditpic.Visibility = Visibility.Hidden;
                btnjudgeconfirm.Content = "Add";
            }
            else if (cmbjudgeoptions.SelectedIndex == 2)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else if (cmbjudgeoptions.SelectedIndex == 3)
            {
                txbclname.IsEnabled = false;
                txbcfname.IsEnabled = false;
                dgEventContestants.IsEnabled = true;
                btnjudgeconfirm.Visibility = Visibility.Visible;
                btnjudgeconfirm.Content = "Remove";
            }
            else
            {
                cmbjudgeoptions.SelectedIndex = -1;
                btnjudgeconfirm.Visibility = Visibility.Hidden;
                disableall();
            }
          
        }
        
        private void dgAllContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgAllContestants.SelectedIndex > -1)
                btnaddexisting.IsEnabled = true;
            else
                btnaddexisting.IsEnabled = false;
        }
        
        public static ImageSource BitmapFromUri(Uri source)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = source;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            return bitmap;
        }

        private void btneditpic_Click(object sender, RoutedEventArgs e)

        {
            if (dgEventContestants.SelectedIndex == -1)
                return;
            try
            {
                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                string personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();       

                string imagetpathstr;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JPEG Files (*.jpg)|*.jpg";
                openFileDialog.ShowDialog();
                if ((openFileDialog.FileName == null) || (openFileDialog.FileName == ""))
                    throw new Exception();

                imagetpathstr = openFileDialog.FileName;               //gets file location na gusto gamitin
                moveimagefromsource(imagetpathstr,personid);         //paglipat nung file from outside to solution folder
                imgconpic.Source = null;
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Error Occured during Image Selection");
            }
            checkpicture();
        }
        
        private void checkpicture()
        {
            string imagepath;
            if (dgEventContestants.SelectedIndex > -1)
            {

                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                string personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();         //DITO ILALAGAY PERSON ID

                 imagepath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Images";        //ito yung folder na kukunin yung image

                if (File.Exists(imagepath + "/personpic" + personid + ".jpg"))        //verifies kung may pic na talaga
                    imagepath += "/personpic" + personid + ".jpg";
                else
                    imagepath += "/defaultpicmale.jpg";                                //pag walang pic, gamitin yung default
            }
            else
            {
                 imagepath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Images/defaultpicmale.jpg";
            }
            ImageSource imageSrc = BitmapFromUri(new Uri(imagepath)).Clone();
            imgconpic.Source = imageSrc;

        }
        
        public void moveimagefromsource(string src,string personid)
        {


            string des;                  
            des = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Images";    //checks muna kung meron na Directory nung Images

            if (!Directory.Exists(src))
                Directory.CreateDirectory(des);                 //checking part

            des += "\\personpic"+personid+".jpg";              
            
            if (!File.Exists(des))                              //checking part kung existing na yung pic
                File.Create(des).Close();                       //create a null .jpg file

            File.Copy(src, des, true);                          //then copy from other folder to solution folder

        }

        private bool checkfields()
        {
            if (txbcfname.Text == "")
                return false;
            if (txbclname.Text == "")
                return false;
            return true;
        }

        private void disableall()
        {
            dgEventContestants.SelectedIndex = -1;
            dgEventContestants.IsEnabled = false;
            txbcfname.IsEnabled = false;
            txbclname.IsEnabled = false;
            btnjudgeconfirm.Visibility = Visibility.Hidden;
            cnameavail.Content = "";
            cmbjudgeoptions.SelectedIndex = -1;
            btneditpic.Visibility = Visibility.Hidden;
            checkpicture();

        }


    }
}
