using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;


namespace FootballHedge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<BasicBrokerData> Bookmakers = new List<BasicBrokerData>();
        public List<FData> FootData = new List<FData>();
        public List<string> SuspendedMatches = new List<string>();
        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer Mailtimer = new System.Windows.Threading.DispatcherTimer();
        public List<LeagueState> Leagues = new List<LeagueState>();

        ParsedData PD;

        public MainWindow()
        {
            InitializeComponent();

            timer1.Interval = new TimeSpan(0, 0, 2);
            timer1.Tick += timer1_Tick;

            PD = new ParsedData(this);

            XMLData.LoadSettings(this);

            XMLData xd = new XMLData();
            xd.Load(FootData,Bookmakers);

            UpdateControls();
            WriteBreakToFile.ClearFile();
            WriteDataTotable("All");
        }


        public void DefaultSettings()
        {
            Leagues.Clear();
            Bookmakers.Clear();
            SuspendedMatches.Clear();

            Bookmakers.Add(new BasicBrokerData() { Name = "Matchbook", Type = BrokerType.LAY, Book = new PMatchbook(this) });
            Bookmakers.Add(new BasicBrokerData() { Name = "Bwin", Type = BrokerType.BACK, Book = new PBwin() });
            Bookmakers.Add(new BasicBrokerData() { Name = "Marathonbet", Type = BrokerType.BACK, Book = new PMarathonbet() });
            //Bookmakers.Add(new BasicBrokerData() { Name = "Sbobet", Type = BrokerType.BACK, Book = new PSbobet() });
            //Bookmakers.Add(new BasicBrokerData() { Name = "SmartLive", Type=BrokerType.BACK, Book= new PSmartLive()});
            Bookmakers.Add(new BasicBrokerData() { Name = "TonyBet", Type = BrokerType.BACK, Book = new PTonyBet() });
           // Bookmakers.Add(new BasicBrokerData() { Name = "WilliamHill", Type = BrokerType.BACK, Book = new PWilliamHill() });
            Bookmakers.Add(new BasicBrokerData() { Name = "DafaBet", Type = BrokerType.HANDICAP, Book = new PDafaBet() });
            Bookmakers.Add(new BasicBrokerData() { Name = "Pinna", Type = BrokerType.BACK, Book = new PPina() });
            Bookmakers.Add(new BasicBrokerData() { Name = "TlcBet", Type = BrokerType.BACK, Book = new P12bet() });







            Leagues.Add(new LeagueState() { Name = "UEFA CHAMPIONS LEAGUE" , State = false });
            Leagues.Add(new LeagueState() { Name = "UEFA EUROPA LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND PREMIER LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND CHAMPIONSHIP",  State = false });
            Leagues.Add(new LeagueState() { Name = "SPAIN PRIMERA DIVISION", State = false });
            Leagues.Add(new LeagueState() { Name = "GERMANY BUNDESLIGA", State = false });
            Leagues.Add(new LeagueState() { Name = "ITALY SERIE A", State = false });
            Leagues.Add(new LeagueState() { Name = "FRANCE LIGUE 1", State = false });
            Leagues.Add(new LeagueState() { Name = "NETHERLANDS EREDIVISIE", State = false });
            Leagues.Add(new LeagueState() { Name = "SCOTLAND PREMIERSHIP", State = false });
            Leagues.Add(new LeagueState() { Name = "PORTUGAL PRIMEIRA LIGA", State = false });
            Leagues.Add(new LeagueState() { Name = "GERMANY GERMAN CUP", State = true });
            Leagues.Add(new LeagueState() { Name = "FRANCE COUPE DE LA LIGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "SPAIN COPA DEL REY", State = false });
            Leagues.Add(new LeagueState() { Name = "ITALY ITALIAN CUP", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND CAPITAL ONE CUP", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND FA CUP", State = false });
            Leagues.Add(new LeagueState() { Name = "FRANCE FRENCH CUP", State = false });

            Leagues.Add(new LeagueState() { Name = "ARGENTINA PRIMERA DIVISION", State = false });
            Leagues.Add(new LeagueState() { Name = "AUSTRALIA A LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "BELGIUM PRO LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "BRAZIL BRAZILIAN SERIE A", State = false });
            Leagues.Add(new LeagueState() { Name = "BRAZIL SERIE A", State = false });
            Leagues.Add(new LeagueState() { Name = "COPA SUDAMERICANA", State = false });
            Leagues.Add(new LeagueState() { Name = "DENMARK SUPERLIGA", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND LEAGUE 1", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND LEAGUE 2", State = false });
            Leagues.Add(new LeagueState() { Name = "ENGLAND JOHNSTONES PAINT TROPHY", State = false });
            Leagues.Add(new LeagueState() { Name = "EURO 2016", State = false });
            Leagues.Add(new LeagueState() { Name = "FRANCE LIGUE 2", State = false });
            Leagues.Add(new LeagueState() { Name = "GERMANY BUNDESLIGA 2", State = false });
            Leagues.Add(new LeagueState() { Name = "INTERNATIONAL FRIENDLIES", State = false });
            Leagues.Add(new LeagueState() { Name = "IRELAND AIRTRICITY PREMIER LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "ITALY SERIE B", State = false });
            Leagues.Add(new LeagueState() { Name = "JAPAN J LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "NORWAY FIRST DIVISION", State = false });
            Leagues.Add(new LeagueState() { Name = "NORWAY TIPPELIGAEN", State = false });
            Leagues.Add(new LeagueState() { Name = "SCOTLAND SCOTTISH CUP", State = false });
            Leagues.Add(new LeagueState() { Name = "SCOTLAND SCOTTISH LEAGUE CUP", State = false });
            Leagues.Add(new LeagueState() { Name = "SPAIN SEGUNDA DIVISION", State = false });
            Leagues.Add(new LeagueState() { Name = "SWEDEN ALLSVENSKAN", State = false });
            Leagues.Add(new LeagueState() { Name = "SWITZERLAND SUPER LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "SWITZERLAND SWISS SUPER LEAGUE", State = false });
            Leagues.Add(new LeagueState() { Name = "TURKEY SUPER LIG", State = false });
            Leagues.Add(new LeagueState() { Name = "USA MLS", State = false });
            Leagues.Add(new LeagueState() { Name = "WORLD CUP QUALIFIERS", State = false });


        }
        public void UpdateDataBase(List<ParsedDataFromPage> PD, int key)
        {
            if (PD.Count == 0) return;
            BrokerType type = Bookmakers[key].Type;
            DataBase DB = new DataBase(this);

            switch (type)
            {
                case BrokerType.LAY: DB.UpLay(FootData, PD, key); break;
                case BrokerType.BACK: DB.UpBack(FootData, PD, key); break;
                case BrokerType.HANDICAP: DB.UpHand(FootData, PD, key); break;
                default: return;
            }
            if (DB.changecolor) this.Dispatcher.Invoke((Action) (() => { BackgroundGrid.Background = new SolidColorBrush(Colors.Salmon); }));
        }


        public void UpdateHedgeDataInListview()
        {
            Task.Factory.StartNew(() =>
                {
                    int index = 0;
                    bool Today = false;
                    bool ViewAll = false;
                    Dispatcher.Invoke((Action) (() => 
                    {
                        BackgroundGrid.Background = SystemColors.ActiveBorderBrush;
                        TabCont.SelectedIndex = 0;
                        LWHedge.Items.Clear();
                        if(ComboboxActiveBookies.SelectedIndex != 0) index = ComboboxActiveBookies.SelectedIndex;
                        Today = CheckBoxToday.IsChecked.Value;
                        ViewAll = CheckBoxViewAll.IsChecked.Value;

                    }));
                

                    List<LayData> datalist = new List<LayData>();

                    CalcParameters cp = new CalcParameters();
                    foreach (BasicBrokerData book in Bookmakers) { if (book.InUse || ViewAll) cp.bookielimit += "|" + book.Name + "|";  }

                    if (cp.bookielimit == null) { System.Windows.Forms.MessageBox.Show("Select Bookies !!"); return; }
                    cp.limit = FootballHedge.Properties.Settings.Default.plLimit;

                    cp.preferedbookie = ComboboxActiveBookies.Items[index].ToString();
                    cp.today = Today;
                    CalcHedge(ref datalist, cp);

                   if (datalist.Count == 0) return;

                    datalist.Sort();
                    datalist.Reverse();


                    string[] d;
                    StringBuilder bufstr = new StringBuilder();
                    foreach (LayData item in datalist)
                    {
                        bufstr.Clear();
                        ListViewHedgeData lview = new ListViewHedgeData();
                        lview.Match = item.Match;
                        lview.Team = item.Selection;
                        lview.Back = item.Back.ToString();
                        lview.Lay = item.Lay.ToString();
                        lview.PL = item.Profit.ToString();
                        lview.Size = item.Size.ToString();
                        lview.FullLeague = item.League;
                        d = item.League.Split(' ');
                        foreach (string str in d) { bufstr.Append(str.Substring(0, 1)); }
                        lview.League = bufstr.ToString();
                        lview.Bookie = item.Bookie;
                        lview.Time = item.Time;
                        lview.LastUpdate = item.LastUpdate;

                        Dispatcher.Invoke((Action)(() => LWHedge.Items.Add(lview)));

                    }
                    
                    Dispatcher.Invoke((Action)(() =>  LWHedge.ScrollIntoView(LWHedge.Items[0]) ));
                 });
        }

        public void CalcHedge(ref List<LayData> datalist, CalcParameters cpar)
        {
            DataBase DB = new DataBase(this);
            DB.Calchedge(ref datalist, FootData, cpar);
        }


        public void SendMultiEventMail()
        {
            if (!Mail.CheckIfEmailsAllowed()) { MessageBox.Show("Email sending is disabled"); return; }
            List<LayData> datalist = new List<LayData>();


            CalcParameters cp = new CalcParameters();
            foreach (BasicBrokerData book in Bookmakers) { if (book.EMailing) cp.bookielimit += "|" + book.Name + "|"; }
            if (cp.bookielimit == null) { System.Windows.Forms.MessageBox.Show("Select Bookies for emails !!"); return; }
            cp.limit = FootballHedge.Properties.Settings.Default.MailplLimit;

            cp.preferedbookie = "All";
            if (FootballHedge.Properties.Settings.Default.MailOnlyToday) cp.today = true;
            else cp.today = false;
            CalcHedge(ref datalist, cp);

            datalist.Sort();
            datalist.Reverse();

            StringBuilder body = new StringBuilder();
            body.Append("<html><body><table>");
            foreach (LayData item in datalist)
            {
                body.Append("<tr><td>" + item.Match + "</td><td>[" + item.Selection + "]</td><td>" + item.Back + " / " + Math.Round(item.Lay, 2) + "</td><td>(" + item.Bookie + ")</td><td>  " + item.LastUpdate + "</td></tr>");
            }
            body.Append("</table></body></html>");
            Mail.SendMail("Options", body.ToString());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FootballHedge.Properties.Settings.Default.MailAllow = false;
            FootballHedge.Properties.Settings.Default.Save();
            XMLData.SaveSettings(this);
            Environment.Exit(0);
        }



        private void Button_Click_Calc3WayDutch(object sender, RoutedEventArgs e) { Calc3WayDutch(); }

        private void Calc3WayDutch()
        {
            Task.Factory.StartNew(() =>
                {
                    Dispatcher.Invoke((Action)(() => LW3Way.Items.Clear()));
                    
                    List<ListViewData3WayDutch> datalist = new List<ListViewData3WayDutch>();

                    DataBase DB = new DataBase(this);
                    DB.Calc3WayDutch(datalist, FootData);


                    datalist.Sort();
                    datalist.Reverse();

                    foreach (ListViewData3WayDutch item in datalist)
                    {
                        
                        Dispatcher.Invoke((Action)(() => LW3Way.Items.Add(item)));
                    }
                });

        }
        private void Button_Click_SendMail(object sender, RoutedEventArgs e)
        {
            if (!Mail.CheckIfEmailsAllowed()) { MessageBox.Show("Email sending is disabled"); return; }

            Task.Factory.StartNew(() =>
                {
                    List<ListViewData3WayDutch> datalist = new List<ListViewData3WayDutch>();
                    DataBase DB = new DataBase(this);
                    DB.Calc3WayDutch(datalist, FootData);

                    if (datalist.Count == 0) return;

                    StringBuilder body = new StringBuilder();
                    body.Append("<html><body><table>");
                    foreach (ListViewData3WayDutch item in datalist)
                    {
                        body.Append("<tr><td>" + item.Match + "</td><td>(" + item.Bookie1 + ")</td><td>" + item.X1.ToString() + "</td><td>(" + item.Bookie0 + ")</td><td>" + item.X0.ToString() + "</td><td>(" + item.Bookie2 + ")</td><td>" + item.X2.ToString() + "</td><td>" + item.Profit.ToString() + "</td></tr>");

                    }
                    body.Append("</table></body></html>");
                    Mail.SendMail("3Way Ducth", body.ToString());
               });



            

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ClickPlaceHolder(object sender, RoutedEventArgs e)
        {
            ComboBets cb = new ComboBets();
            cb.FindOptimal();
        }







































    }

}
