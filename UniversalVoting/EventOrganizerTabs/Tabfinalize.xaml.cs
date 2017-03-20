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

namespace UniversalVoting.EventOrganizerTabs
{
    /// <summary>
    /// Interaction logic for Tabfinalize.xaml
    /// </summary>
    public partial class Tabfinalize : System.Windows.Controls.UserControl
    {
        IDatabase clsDatabase;
        int _tabeventid;
        int weightsum;
        Window kepyas;
        public Tabfinalize(int _myeveid,Window t)
        {
            InitializeComponent();
            _tabeventid = _myeveid;
            RefreshDataGrids();
            kepyas = t;
        }

        public void RefreshDataGrids()
        {

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("spViewEventJudges", "@_eventid", _tabeventid);
            dgEventJudges.ItemsSource = clsDatabase.Data.DefaultView;

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewEventContestants", "@_eventid", _tabeventid);
            dgEventContestants.ItemsSource = clsDatabase.Data.DefaultView;

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("[spViewEventCriteria]", "@_eventid", _tabeventid);
            dgEventCriteria.ItemsSource = clsDatabase.Data.DefaultView;
            weightsum = 0;
            foreach (DataRow x in clsDatabase.Data.Rows)
                weightsum += int.Parse((x.ItemArray[1].ToString()));
            ctotalweight.Content = "Criterias for This Event   Total Weight: "+ weightsum.ToString();

        }

        //Verify if Event is to be finalized
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //check if there is any judge/contestant/criteria

            if(dgEventCriteria.HasItems==false)
            {
                System.Windows.MessageBox.Show("There is no Criteria, Please add");
                return;
            }
            if (dgEventJudges.HasItems == false)
            {
                System.Windows.MessageBox.Show("There is no Judges, Please add");
                return;
            }
            if (dgEventContestants.HasItems == false)
            {
                System.Windows.MessageBox.Show("There is no Contestants, Please add");
                return;
            }

            //Check if Total weight is 100 na
            if (weightsum < 100)
            {
                System.Windows.MessageBox.Show("Total Criteria Weight is not 100, Please Adjust.");
                return;
            }
            //Check if all contestants have pictuers

            clsDatabase = new Database();
            clsDatabase.ExecuteStoredProc("KFspViewEventContestants", "@_eventid", _tabeventid);
            dgEventContestants.ItemsSource = clsDatabase.Data.DefaultView;

            foreach (DataRow x in clsDatabase.Data.Rows)
            {
                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                IDatabase clsDatabase2 = new Database();
                clsDatabase2.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", x[0].ToString(),
                    "@lname", x[1].ToString(),
                    "@_eventid", _tabeventid);
                string personidcheck = clsDatabase2.Data.Rows[0].ItemArray.GetValue(0).ToString();
                if (!(checkpictureexists(int.Parse(personidcheck))))
                {
                    System.Windows.MessageBox.Show(x[0].ToString() + " " + x[1].ToString() + " Does not yet have pictures, Please Add one");
                    return;
                }
            }


            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Finalize this event?", "Warning.", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                //for each judge, for each contestant, for each criteria, place 0
                IDatabase judgedb = new Database();
                judgedb.ExecuteStoredProc("spViewEventJudges", "@_eventid", _tabeventid);

                foreach (DataRowView judges in judgedb.Data.DefaultView)
                { int ctr = 1;
                    IDatabase condb = new Database();
                    condb.ExecuteStoredProc("KFspViewEventContestants", "@_eventid", _tabeventid);
                    foreach (DataRowView cons in condb.Data.DefaultView)
                    {
                        IDatabase addconnum = new Database();
                        addconnum.ExecuteStoredProc("KFspAddConNum",
                            "@fname", cons[0].ToString(),
                            "@lname", cons[1].ToString(),
                            "@_eventid", _tabeventid,
                            "@connum", ctr);

                        ctr++;

                        IDatabase critdb = new Database();
                        critdb.ExecuteStoredProc("[spViewEventCriteria]", "@_eventid", _tabeventid);
                        foreach (DataRowView crits in critdb.Data.DefaultView)
                        {
                            IDatabase addscore = new Database();
                            addscore.ExecuteStoredProc("[KFspPopulateScores]",
                                "@confname", cons[0].ToString(),
                                "@conlname", cons[1].ToString(),
                                "@judgefname", judges[0].ToString(),
                                "@judgelname", judges[1].ToString(),
                                "@judgeuname", judges[2].ToString(),
                                "@judgepass", judges[3].ToString(),
                                "@eventid", _tabeventid,
                                "@critname", crits[0].ToString()
                                );
                        }

                    }

                }

                System.Windows.MessageBox.Show("Event has been Finalized, Will now go to the Login Window...");
                kepyas.Close();
            }

        }

        private void dgEventContestants_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            checkpicture();
        }

        #region for pictures
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

            try
            {
                DataRowView dataRow = (DataRowView)dgEventContestants.SelectedItem;
                clsDatabase = new Database();
                clsDatabase.ExecuteStoredProc("KFspCheckConExistance",
                    "@fname", dataRow[0].ToString(),
                    "@lname", dataRow[1].ToString(),
                    "@_eventid", _tabeventid);

                string personid = clsDatabase.Data.Rows[0].ItemArray.GetValue(0).ToString();         //DITO ILALAGAY PERSON ID

                string imagetpathstr;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                if ((openFileDialog.FileName == null) || (openFileDialog.FileName == ""))
                    throw new Exception();

                imagetpathstr = openFileDialog.FileName;               //gets file location na gusto gamitin
                moveimagefromsource(imagetpathstr, personid);         //paglipat nung file from outside to solution folder
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

        private bool checkpictureexists(int personid)
        {
            
           string imagepath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/Images";        //ito yung folder na kukunin yung image

            if (File.Exists(imagepath + "/personpic" + personid + ".jpg"))        //verifies kung may pic na talaga
                return true;
            else
                return false;
        }


        public void moveimagefromsource(string src, string personid)
        {


            string des;
            des = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Images";    //checks muna kung meron na Directory nung Images

            if (!Directory.Exists(src))
                Directory.CreateDirectory(des);                 //checking part

            des += "\\personpic" + personid + ".jpg";

            if (!File.Exists(des))                              //checking part kung existing na yung pic
                File.Create(des).Close();                       //create a null .jpg file

            File.Copy(src, des, true);                          //then copy from other folder to solution folder

        }
        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RefreshDataGrids();
        }
    }
}
