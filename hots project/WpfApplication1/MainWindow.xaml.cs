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
using Microsoft.Win32;
using System.Net;
using System.IO;
using System.Web.Helpers;
using System.Collections;
using System.Reflection;

//using System.Windows.Forms;


namespace WpfApplication1
{

    public partial class PickerWindow : Window
    {
        TextBlock RelationsBox;
        //      Relations ctr;
        //      Relations syn;
        //      Relations ctd;
        dynamic Relations_;
        List List_;
        Canvas myCanvas;
        TextBox[] tbs;
        ListBox Picker;
        Button Ready;
        int turn = 0; 
        int[] allies, enemies, bans, flow,tbx,
            flowblue = { 10, 12, 0, 5, 6, 1, 2, 13, 11, 7, 8, 3, 4, 9 } ,
            flowred = { 12, 10, 5, 0, 1, 6, 7, 11, 13, 2, 3, 8, 9, 4 };
        

        public PickerWindow( dynamic Details,dynamic Relations)
        {
            allies = new int[5];
            enemies = new int[5];
            bans = new int[4];
            tbs = new TextBox[14];
            myCanvas = new Canvas();
            this.Content = myCanvas;

            Relations syn = new Relations(Relations_,"synergy");
            Relations ctr = new Relations(Relations_,"counter");
            Relations ctd = new Relations(Relations_,"countered");

            int top = 0, left = 0;

            for(int i=0; i<14; i++)
            {
                tbs[i] = new TextBox();
                if (i == 0) //blue
                {
                    top = 120;
                    left = 50;
                }
                else if (i == 5)//red
                {
                    top = 120;
                    left = 900;
                }
                else if (i == 10)//bans
                {
                    top = 20;
                    left = 520; 
                }
                Canvas.SetTop(tbs[i], top);
                Canvas.SetLeft(tbs[i], left);
                top = top + 30;

                tbs[i].Width = 80;
                myCanvas.Children.Add(tbs[i]);
            }
            initPicker(Details);
            flow = flowblue;
            initButton();
            Show();
        }

        public void initPicker(dynamic heroes)
        {
            // Constrieste Listbox Eroi
            Picker = new ListBox();
            Picker.Width = 150;
            Picker.Height = 350;
            Picker.Margin = new Thickness(30, 30, 30, 10);
            Picker.HorizontalAlignment = HorizontalAlignment.Left;

            Picker.FlowDirection = FlowDirection.LeftToRight;

            Canvas.SetTop(Picker, 50);
            Canvas.SetLeft(Picker, 150);

            foreach (dynamic hero in heroes)
            {
                Picker.Items.Add(hero.name);
            }
            myCanvas.Children.Add(Picker);
        }
        public void initButton()
        {
            // Construieste Buton Ready
            Ready = new Button();
            Ready.Content = "Ready";
            Ready.Width = 150;
            Ready.Height = 28;
            Ready.Margin = new Thickness(30, 0, 30, 30);
            Ready.HorizontalAlignment = HorizontalAlignment.Left;
            Ready.Visibility = Visibility.Visible;

            Canvas.SetTop(Ready, Picker.Height + 70);
            Canvas.SetLeft(Ready, 150);

            // Attach click event
            Ready.Click += (se, ev) =>
            {
                tbs[flow[turn]].Text = Picker.SelectedItem.ToString();
                tbx[flow[turn]] = List_.getRank(Picker.SelectedItem.ToString());
                turn = turn + 1;
            };
            myCanvas.Children.Add(Ready);
        }     
        public void Predict()
        {

        }
    }



    public partial class RelationsWindow : Window
    {
        TextBlock RelationsBox;
//      Relations ctr;
//      Relations syn;
//      Relations ctd;
        dynamic Relations_;
        Button CounterButton, CounteredButton, SynergyButton;
        Canvas myCanvas;

        public RelationsWindow(dynamic Details, dynamic Relations)
        {
//          ctr = new Relations(Relations, "counters");
//          syn = new Relations(Relations, "synregy");
//          ctd = new Relations(Relations, "countered");
            Relations_ = Relations;

            myCanvas = new Canvas();
            initMatrix();
            this.Content = myCanvas;

            // int x = 0;

            CounterButton = new Button();
            Canvas.SetTop(CounterButton, 20);
            Canvas.SetLeft(CounterButton, 20);
            CounterButton.Content = "CounterMap";
            myCanvas.Children.Add(CounterButton);
            CounterButton.Click += new RoutedEventHandler(RelationsButtonClick);

            SynergyButton = new Button();
            Canvas.SetTop(SynergyButton, 20);
            Canvas.SetLeft(SynergyButton, 240);
            SynergyButton.Content = "SynergyMap";
            myCanvas.Children.Add(SynergyButton);
            SynergyButton.Click += new RoutedEventHandler(RelationsButtonClick);

            CounteredButton = new Button();
            Canvas.SetTop(CounteredButton, 20);
            Canvas.SetLeft(CounteredButton, 130);
            CounteredButton.Content = "CounteredMap";
            myCanvas.Children.Add(CounteredButton);
            CounteredButton.Click += new RoutedEventHandler(RelationsButtonClick);
        }


        public void initMatrix()
        {
            RelationsBox = new TextBlock();
            RelationsBox.Width = 750;
            RelationsBox.Height = 400;
            RelationsBox.Margin = new Thickness(30, 30, 30, 10);
            RelationsBox.HorizontalAlignment = HorizontalAlignment.Left;
            RelationsBox.FlowDirection = FlowDirection.LeftToRight;
            Canvas.SetTop(RelationsBox, 30);
            Canvas.SetLeft(RelationsBox, 30);

            myCanvas.Children.Add(RelationsBox);

            this.Show();
        }
        

        public void RelationsButtonClick(Object sender, EventArgs e)
        {
            string rlsType = "";

            switch (sender.ToString())
            {
                case "System.Windows.Controls.Button: CounterMap" : rlsType = "counters"; break;
                case "System.Windows.Controls.Button: SynergyMap": rlsType = "synergy"; break;
                case "System.Windows.Controls.Button: CounteredMap": rlsType = "countered"; break;
            }

            Relations rls = new Relations(Relations_, rlsType);
            string m = ""; 

            for(int i=0; i<58; i++)
            {
                for(int j=0; j<58; j++)
                {
                    m += rls.matrix[i, j] + " ";
                }
                m += System.Environment.NewLine;
            }
            RelationsBox.Text = m;
        }

    }



    public partial class DetailsWindow : Window
    {
        ListBox HeroesList;
        Button Details;
        Image Portrait;
        TextBlock DetailsBlock;

        dynamic Heroes;
        dynamic Relations;

        public void initList(dynamic heroes)
        {
            // Constrieste Listbox Eroi
            HeroesList = new ListBox();
            HeroesList.Width = 150;
            HeroesList.Height = 350;
            HeroesList.Margin = new Thickness(30, 30, 30, 10);
            HeroesList.HorizontalAlignment = HorizontalAlignment.Left;

            HeroesList.FlowDirection = FlowDirection.LeftToRight;

            Canvas.SetTop(HeroesList, 30);
            Canvas.SetLeft(HeroesList, 30);

            foreach (dynamic hero in heroes)
            {
                HeroesList.Items.Add(hero.name);
            }
        }


        public void initText()
        {
            // Construieste Textbox Detalii
            DetailsBlock.Name = "heroDetailsTextBlock";
            DetailsBlock.HorizontalAlignment = HorizontalAlignment.Left;
            DetailsBlock.Width = 500;
            DetailsBlock.Height = 300;
            DetailsBlock.Margin = new Thickness(0, 30, 30, 10);

            DetailsBlock.FlowDirection = FlowDirection.LeftToRight;
            DetailsBlock.Visibility = Visibility.Visible;

            Canvas.SetTop(DetailsBlock, 180);
            Canvas.SetLeft(DetailsBlock, HeroesList.Width + 150);
        }


        public void initButton()
        {
            // Construieste Buton Detalii
            Details = new Button();
            Details.Content = "Detalii";
            Details.Width = 150;
            Details.Height = 28;
            Details.Margin = new Thickness(30, 0, 30, 30);
            Details.HorizontalAlignment = HorizontalAlignment.Left;
            Details.Visibility = Visibility.Visible;

            Canvas.SetTop(Details, HeroesList.Height + 70);
            Canvas.SetLeft(Details, 30);

            // Attach click event
            Details.Click += (se, ev) =>
            {
                int hero = HeroesList.SelectedIndex;
                dynamic relHero = null;

                foreach (var rH in Relations)
                {
                    foreach (KeyValuePair<string, dynamic> kvp in rH)
                    {
                        if (kvp.Key == Heroes[hero].name)
                        {
                            relHero = kvp.Value;
                        }
                    }
                }

                displayDetails(Heroes[hero], relHero);
                displayPortrait(Heroes[hero]);
            };
        }


        public void initPortrait()
        {
            Portrait.Width = 100;
            Portrait.Height = 100;
            
            Canvas.SetTop(Portrait, 60);
            Canvas.SetLeft(Portrait, HeroesList.Width + 150);
        }


        public DetailsWindow(dynamic Details, dynamic Relations)
        {

            this.Portrait = new Image();
            this.DetailsBlock = new TextBlock();

            Canvas thisCanvas = new Canvas();

            Heroes = Details;
            this.Relations = Relations;

            initList(Details);
            thisCanvas.Children.Add(HeroesList); // ListBox-ul

            initButton();
            thisCanvas.Children.Add(this.Details); // Buton 'Detalii'

            initPortrait();
            thisCanvas.Children.Add(Portrait); // Imagine avatar

            initText();
            thisCanvas.Children.Add(DetailsBlock); // TextBlock

            this.Content = thisCanvas;
            this.Show();
        }


        // Gets hero avatar (from online source)
        private string getPortraitPath(string heroName)
        {
            string path = "";

            WebRequest reqPortraitHTML = WebRequest.Create("http://heroesofthestorm.wikia.com/wiki/Portrait");
            WebResponse rspPortraitHTML = reqPortraitHTML.GetResponse();

            Stream dataStrPortrait = rspPortraitHTML.GetResponseStream();
            StreamReader readerPortrait = new StreamReader(dataStrPortrait);

            string PortraitsHTML = readerPortrait.ReadToEnd();

            int pos = PortraitsHTML.IndexOf(heroName + "Portrait.png");
            if (pos == -1)
            {
                pos = PortraitsHTML.IndexOf(heroName + "_portrait.png");
            }

            int end = PortraitsHTML.IndexOf('"', pos);
            int start = PortraitsHTML.LastIndexOf('"', pos);
            path = PortraitsHTML.Substring(start + 1, end - start);

            return path;
        }


        public void displayPortrait(dynamic details)
        {
            string portraitPath = getPortraitPath(details.name);
            Portrait.Source = new BitmapImage(new Uri(portraitPath));
        }


        public void displayDetails(dynamic details, dynamic relations)
        {

            // Din 'relations'
            string[] P = new[] { "  ", "  ", "  ", "  ", "  " };
            foreach (KeyValuePair<string, dynamic> kvr in relations.counters)
            {
                P[0] += kvr.Value + ", ";
            }
            P[0] = P[0].Substring(0, P[0].Length - 2);

            foreach (KeyValuePair<string, dynamic> kvr in relations.synergy)
            {
                P[1] += kvr.Value + ", ";
            }
            P[1] = P[1].Substring(0, P[1].Length - 2);

            foreach (KeyValuePair<string, dynamic> kvr in relations.countered)
            {
                P[2] += kvr.Value + ", ";
            }
            P[2] = P[2].Substring(0, P[2].Length - 2);

            foreach (KeyValuePair<string, dynamic> kvr in relations.strongermaps)
            {
                P[3] += kvr.Value + ", ";
            }
            P[3] = P[3].Substring(0, P[3].Length - 2);

            foreach (KeyValuePair<string, dynamic> kvr in relations.weakermaps)
            {
                P[4] += kvr.Value + ", ";
            }
            P[4] = P[4].Substring(0, P[4].Length - 2);


            // Din 'details'
            DetailsBlock.Text = // DetailsBlock.Text +
                "Name: " + details.name + System.Environment.NewLine +
                "Title: " + details.title + System.Environment.NewLine +
                "Description: " + details.description + System.Environment.NewLine +
                "Role: " + details.role + System.Environment.NewLine +
                "Type: " + details.type + System.Environment.NewLine +
                "Gender: " + details.gender + System.Environment.NewLine +
                "Franchise: " + details.franchise + System.Environment.NewLine +
                "Difficulty: " + details.difficulty + System.Environment.NewLine +
                "Release date: " + details.releaseDate + System.Environment.NewLine + System.Environment.NewLine +

                "Counters: " + P[0] + System.Environment.NewLine +
                "Synergy: " + P[1] + System.Environment.NewLine +
                "Countered: " + P[2] + System.Environment.NewLine +
                "Strongermaps: " + P[3] + System.Environment.NewLine +
                "Weakermaps: " + P[4];
        }
    }


    class List
    {
        string[] ranks = new string [58];
        public dynamic list;
        public int lenght;

        public List(dynamic list)
        {

            this.list = list;
            buildRanks();
        }

        public void buildRanks()
        {
            int i = 0,
                j = 0;

            // 'list' e array de obiecte, nu mai trebuie numarate, are 'Length' = numarul de elemente.
            for (i = 0; i < list.Length; i++)
            {
                foreach (dynamic hero in list[i])   // Aici list[i] are doar cate un obiect (erou) 
                {                                   // pentru fiecare 'i' dar e 'foreach' ca sa-i atribuie lui
                                                    // 'hero' continutul lui 'list[i]' deci e parcurs doar cate 
                                                    // o singura data (dintr-un motiv necunoscut cu '=' nu merge)

                    // Asta e manevra ca sa ia proprietatile lui 'hero' (adica 'Abathur' si 'continutul')
                    Type myType = hero.GetType();
                    IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                    // Aici itereaza cele 2 proprietati ale lui 'hero'
                    j = 0;
                    foreach (PropertyInfo prop in props)
                    {
                        if (j == 0) // daca e prima ('0') adica numele eroului i-l
                                    // atribuie lui 'ranks'
                        {
                            ranks[i] = prop.GetValue(hero, null);
                            j++;
                        }
                    }
                }
            }

            lenght = list.Length;
        }


        public int getRank(dynamic heroName)
        {    
            for(int i=0; i<this.ranks.Length; i++)
            {
                if (heroName.Trim() == ranks[i].Trim()) return i;
            }

            return -1;
        }

    }



    class Relations
    {
        List list;
        public int[,] matrix;

        public Relations(dynamic l, string reltype)
        {
            int i = 0;
            list = new List (l);
            matrix = new int[list.lenght, list.lenght];

            foreach(dynamic hero in list.list)
            {
                foreach (dynamic relations in hero)
                {
                    foreach (dynamic rels in relations.Value)
                    {
                        if (reltype == rels.Key)
                        {
                            if (relations.Value != null)
                            {
                                foreach (dynamic rel in rels.Value)
                                {
                                    if (list.getRank(rel.Value) > -1)
                                    {
                                        matrix[i, list.getRank(rel.Value)] = 1;
                                    }
                                }
                            }
                        }
                    }
                }
                i++;
            }
        }
    }


    public partial class MainWindow : Window
    {
        dynamic Details = null, Relations = null;

        public MainWindow()
        {
            InitializeComponent();

//          DetailsWindow detailsWindow;
//          dynamic Details = null, Relations = null;
            
            // Load Details
            WebRequest reqDetails = WebRequest.Create("http://heroesjson.com/heroes.json");
            try
            {
                WebResponse respDetails = reqDetails.GetResponse();
                Stream streamDetails = respDetails.GetResponseStream();
                StreamReader readerDetails = new StreamReader(streamDetails);

                string strDetails = readerDetails.ReadToEnd();
                Details = System.Web.Helpers.Json.Decode(strDetails);

                respDetails.Close();
            }
            catch(WebException err)
            {
                WebException testPoint = err;
                MessageBox.Show("Not connected!", "JSON request error");
            }

            // Load Relations
            WebRequest reqRelations = WebRequest.Create("http://sharkmedia.ro/hots/lista-hots-arr-min.json");
            try
            {
                WebResponse respRelations = reqRelations.GetResponse();
                Stream streamRelations = respRelations.GetResponseStream();
                StreamReader readerRelations = new StreamReader(streamRelations);

                string strRelations = readerRelations.ReadToEnd();
                Relations = System.Web.Helpers.Json.Decode(strRelations);

                respRelations.Close();
            }
//          catch (InvalidCastException err)
            catch (WebException err)
            {
                WebException testPoint = err;
                MessageBox.Show("Not connected!", "JSON request error");
            }
            //          finally
            //          {
            //              MessageBox.Show("O eroare oarecare, probabil lipsa unei conexiuni!", "Eroare!");
            //          }
            
//          detailsWindow = new DetailsWindow(Details, Relations);
        } // end Main Window


        // Details button
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            DetailsWindow detailsWindow;
            detailsWindow = new DetailsWindow(Details, Relations);
        }

        private void ShowPickerClick(object sender, RoutedEventArgs e)
        {
            PickerWindow PickerWindow;
            PickerWindow = new PickerWindow(Details, Relations);
        }


        // Relation button
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            RelationsWindow relationsWindow;
            relationsWindow = new RelationsWindow(Details, Relations);
        }
    }
}