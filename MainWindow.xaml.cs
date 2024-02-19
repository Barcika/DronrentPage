using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DronrentPage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() //konstruktor ez is
        {
            InitializeComponent();
            mainPage = new MainPage(); // a konstruktorban tudom példányosítani
        }

        private MainPage mainPage;  //példányosítás után tudom változóban eltárolni
        private const string DronDBpath = "dron.csv"; //mentés visszatöltéshez
        private void Window_Loaded(object sender, RoutedEventArgs e)
        { //betölt az ablak, a főoldal jelenik meg /MainFrame.Content
            MainFrame.Content = mainPage;
            mainPage.BTN_NewDron.Click += BTN_NewDron_Click;
            mainPage.BTN_EditDron.Click += BTN_EditDron_Click;
            mainPage.BTN_DelDron.Click += BTN_DelDron_Click;
            //feliratkozom a három gomb Click eseményére
            mainPage.LB_DronList.MouseDoubleClick += LB_DronList_MouseDoubleClick;
           
            if (File.Exists(DronDBpath))
            {
                string[] dblines = File.ReadAllLines(DronDBpath);
                foreach (string dronline in dblines)
                {
                    Dron dron = Dron.FromCsv(dronline);
                   mainPage.LB_DronList.Items.Add(dron);
                }
            }
            else
            {
                File.Create(DronDBpath);
            }
        }

        
        private void LB_DronList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BTN_EditDron_Click(sender, e);
        }

        private void BTN_DelDron_Click(object sender, RoutedEventArgs e)
        {
            if (mainPage.LB_DronList.SelectedItem != null) //ha létezik a kijelölt elem, van ott valami
            {
                var alarmresult = MessageBox.Show("Are you sure to delete the selected item?", "Delete...", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (alarmresult == MessageBoxResult.Yes) //dob egy választ adó message boxot, aminek az eredményet egy változóba teszem
                {// a változó var típus, sok mindent elbír
                    mainPage.LB_DronList.Items.Remove(mainPage.LB_DronList.SelectedItem);
                    SaveChanges();
                }//igen válasz esetén törlöm a listboxból a kiválasztott elemet
            }
        }
        private void BTN_EditDron_Click(object sender, RoutedEventArgs e)
        { //ha az Edit gombra kattintunk, elindul ez a metódus
            if (mainPage.LB_DronList.SelectedItem != null) // ha van kiválasztott elem
            {
                var dron = (Dron)mainPage.LB_DronList.SelectedItem; //beleteszi a LB kiválasztott elemének értékeit a változóba, ráadásul típuskényszerítéssel
                var page = new DronEditorPage(); //page-be tárolom a szerkesztő felületet

                //adatátadás
                page.TB_Producer.Text = dron.Producer;
                page.TB_Type.Text = dron.Type;
                page.TB_Range.Text = Convert.ToString(dron.Range);
                page.TB_PurposeUse.Text = dron.PurposeUse;
                page.DP_TimeEntry.SelectedDate = dron.TimeEntry;

                //A gombok Click eseményeire is fel kell iratkozni
                page.BTN_Cancel.Click += DronEditorPage_BTN_Cancel_Click;
                page.BTN_Save.Click += DronEditorPage_BTN_Save_Click;
                //frissítem a LB elemeit
                mainPage.LB_DronList.Items.Refresh();
                //nézetet is átadom
                SaveChanges();
                MainFrame.Content = page;
            }
        }

        private void DronEditorPage_BTN_Save_Click(object sender, RoutedEventArgs e)
        {
            var page = (DronEditorPage)MainFrame.Content; //page-be tárolom a szerkesztő felületet, plusz típuskényszerítés is
            var dron = (Dron)mainPage.LB_DronList.SelectedItem; //beleteszi a LB kiválasztott elemének értékeit a változóba, ráadásul típuskényszerítéssel

            //adatátadás
            dron.Producer = page.TB_Producer.Text;
            dron.Type = page.TB_Type.Text;
            dron.Range = int.Parse(page.TB_Range.Text);
            dron.PurposeUse = page.TB_PurposeUse.Text;
            dron.TimeEntry = page.DP_TimeEntry.SelectedDate;

            //frissítem a LB elemeit
            mainPage.LB_DronList.Items.Refresh();
            //nézetet is viszaadom a főoldalnak /page aktuális oldal, mindig változhat, mindig példányosítom, mainPage, fixen le van mentve mint főoldal
            SaveChanges();
            MainFrame.Content = mainPage;
        }

        private void BTN_NewDron_Click(object sender, RoutedEventArgs e)
        {
            var page = new DronEditorPage(); //új szerkesztő ablak betölt
            MainFrame.Content = page; 
            page.BTN_Cancel.Click += DronEditorPage_BTN_Cancel_Click;
            page.BTN_Save.Click += DronEditorPage_BTN_Save_New_Click;
        }

        private void DronEditorPage_BTN_Save_New_Click(object sender, RoutedEventArgs e)
        {
            var page = (DronEditorPage)MainFrame.Content; //page-be tárolom a szerkesztő felületet, plusz típuskényszerítés is
            var dron = new Dron() //példányosítás - feltöltöm az új elem adataival
            {   //Az osztály egy példányába töltöm az adatokat
                Producer = page.TB_Producer.Text,
                Type = page.TB_Type.Text,
                Range = int.Parse(page.TB_Range.Text),
                PurposeUse = page.TB_PurposeUse.Text,
                TimeEntry = page.DP_TimeEntry.SelectedDate,
            };
            mainPage.LB_DronList.Items.Add(dron); //átadom a Listboxnak
            SaveChanges();
            MainFrame.Content = mainPage; //vissza a főoldalra
        }

        private void DronEditorPage_BTN_Cancel_Click(object sender, RoutedEventArgs e)
        {   //álljon vissza a MainPage-re, amit nem példányosítunk, el van tárolva ebben a mezőben
            MainFrame.Content = mainPage;
        }
        private void SaveChanges()
        {
            List<string> outputdrons = new List<string>();
            foreach (Dron item in mainPage.LB_DronList.Items)
            {
                outputdrons.Add(item.ToCsv());

            }
            File.WriteAllLines(DronDBpath, outputdrons);
        }

        

    }
}