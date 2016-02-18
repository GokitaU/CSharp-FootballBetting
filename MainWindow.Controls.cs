using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
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
using System.Globalization;
using CalculatorLib;

namespace FootballHedge
{
    public partial class MainWindow
    {
        #region Start button and thred timer
        private void MenuItem_Start_Click(object sender, RoutedEventArgs e) { StartStopWorking(MenuItem_Start.Header.ToString()); }
        void timer1_Tick(object sender, EventArgs e)
        {

            if (PD.busy) return;
            PD.busy = true;
            Thread SParse = new Thread(PD.CheckForNewFiles);
            SParse.Start();
        }

        public void StartStopWorking(string str)
        {
            if (str == "Start")
            {
                SolidColorBrush color = new SolidColorBrush();

                MenuItem_Start.Background = new SolidColorBrush(Colors.Red);
                MenuItem_Start.Header = "Stop";
                VisibleStatus.Header = "Active";
                VisibleStatus.Background = new SolidColorBrush(Colors.Green);
                timer1.Start();

            }
            else
            {
                MenuItem_Start.Background = new SolidColorBrush(Colors.Green);
                MenuItem_Start.Header = "Start";
                VisibleStatus.Header = "Not Working";
                VisibleStatus.Background = new SolidColorBrush(Colors.Red);
                timer1.Stop();
            }
        }
        #endregion


        public void UpdateControls()
        {
            UpdateComboboxBookies();
            UpdateComboboxLeagues();
            
            
        }

        #region Updating Combo box'es
        public void UpdateComboboxLeagues()
        {
            ComboBoxEvents.SelectionChanged -= ComboBoxBookies_SelectionChanged;
            ComboBoxEvents.Items.Clear();
            ComboBoxEvents.Items.Add("All");
            ComboBoxEvents.SelectedIndex = 0;
            string buf = "";
            foreach (FData item in FootData)
            {
                if (!buf.Contains(item.League))
                {
                    ComboBoxEvents.Items.Add(item.League);
                    buf += "|" + item.League;
                }
            }
            ComboBoxEvents.SelectionChanged += ComboBoxBookies_SelectionChanged;
        }
        public void UpdateComboboxBookies()
        {
            ComboBoxBookies.SelectionChanged -= ComboBoxBookies_SelectionChanged;

            ComboboxActiveBookies.SelectionChanged -= ComboboxActiveBookies_SelectionChanged;
            ComboboxActiveBookies.Items.Clear();

            ComboBoxBookies.Items.Clear();

            ComboboxActiveBookies.Items.Add("All");


            ComboboxActiveBookies.SelectedIndex = 0;

            foreach (BasicBrokerData item in Bookmakers)
            {
               // if (item.Type == BrokerType.LAY) continue;
                if (item.InUse && item.Type != BrokerType.LAY) ComboboxActiveBookies.Items.Add(item.Name);
                ComboBoxBookies.Items.Add(item.Name);   
            }

           if (ComboBoxBookies.Items.Count > 0) ComboBoxBookies.SelectedIndex = 0;

           ComboBoxBookies.SelectionChanged += ComboBoxBookies_SelectionChanged;

           ComboboxActiveBookies.SelectionChanged += ComboboxActiveBookies_SelectionChanged;
        }

  

        #endregion

        private void AllowMails_Checked(object sender, RoutedEventArgs e)
        {
            
           if(AllowMails.IsChecked.Value)
           {
               Mailtimer.Interval = new TimeSpan(0, FootballHedge.Properties.Settings.Default.MailRepeatTimer,0);
               Mailtimer.Tick += Mailtimer_Tick;
               Mailtimer.Start();
           }
           else
           {
               Mailtimer.Stop();
           }
        }
        void Mailtimer_Tick(object sender, EventArgs e) { SendMultiEventMail(); }

        public void OpenWindow_LayDutch(bool state, double koef1 = 2, double koef2 = 2)
        {
            DefautlVariables DVar = new DefautlVariables();
            DVar.State = state;
            DVar.Stake = FootballHedge.Properties.Settings.Default.DefaultStake;
            DVar.Opacity = FootballHedge.Properties.Settings.Default.Transparency;
            DVar.ComBf = 6.5;
            DVar.ComBook = 1;
            DVar.BGBrush1 = new SolidColorBrush(Colors.LightGray);
            DVar.BGBrush2 = new SolidColorBrush(Colors.LightBlue);
            DVar.koef1 = koef1;
            DVar.koef2 = koef2;
            LD win = new LD(DVar);
            win.Show();
        }
        public void OpenWindow_3Way()
        {
            DefautlVariables DVar = new DefautlVariables();
            DVar.Stake = FootballHedge.Properties.Settings.Default.DefaultStake;
            DVar.Opacity = FootballHedge.Properties.Settings.Default.Transparency;
            DVar.ComBf = 6.5;
            DVar.ComBook = 1;
            DVar.BGBrush1 = new SolidColorBrush(Colors.LightGray);
            DVar.BGBrush2 = new SolidColorBrush(Colors.LightBlue);
            _3Way win = new _3Way(DVar);
            win.Show();
        }
        public void OpenWindow_FreeBet(bool state, double koef1 = 2, double koef2 = 2)
        {
            DefautlVariables DVar = new DefautlVariables();
            DVar.State = true;
            DVar.Stake = FootballHedge.Properties.Settings.Default.DefaultStake;
            DVar.Opacity = FootballHedge.Properties.Settings.Default.Transparency;
            DVar.ComBf = 6.5;
            DVar.ComBook = 1;
            DVar.BGBrush1 = new SolidColorBrush(Colors.LightGray);
            DVar.BGBrush2 = new SolidColorBrush(Colors.LightBlue);

            FreeBet win = new FreeBet(DVar);
            win.Show();
        }

        #region Update Events Combobox
        public delegate void DCheckAndUpdateEventsCombobox(string value);

        public void CheckAndUpdateEventsCombobox(string value)
        {
            this.Dispatcher.Invoke(new DCheckAndUpdateEventsCombobox(CMUpdate), value);
        }
        public void CMUpdate(string value)
        {
            foreach (var item in ComboBoxEvents.Items)
            {
                if (item.ToString() == value) return;

            }
            ComboBoxEvents.Items.Add(value);
        }

        #endregion

        #region Show Last parsed bookie
        public delegate void DTestLabel(string value);
        public void LastBrokerTestLabel(string value) {  this.Dispatcher.Invoke(new DTestLabel(this.UpdateTestLabel), new object[] { value }); }
        public void UpdateTestLabel(string value) { LabelLastBookie.Text = value; }

        #endregion

        #region MenuItems Click
        private void MenuItem_Options(object sender, RoutedEventArgs e)
        {
            Settings set = new Settings(this);
            set.Show();
        }
        private void MenuItem_OpenFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void MenuItem_Save(object sender, RoutedEventArgs e)
        {
            XMLData xd = new XMLData();
            xd.Save(this, FootData);
        }
        private void MenuItem_Load(object sender, RoutedEventArgs e)
        {
            XMLData xd = new XMLData();
            xd.Load(FootData,Bookmakers);
        }
        private void MenuItem_ClearAll(object sender, RoutedEventArgs e) { ClearAllData(); }

        private void MenuItem_SaveSettings(object sender, RoutedEventArgs e)
        {
            XMLData.SaveSettings(this);
        }
        private void MenuItem_LoadSettings(object sender, RoutedEventArgs e)
        {
            XMLData.LoadSettings(this);
        }

        private void MenuItem_CalcDutch(object sender, RoutedEventArgs e) { OpenWindow_LayDutch(false); }
        private void MenuItem_CalcLay(object sender, RoutedEventArgs e)  { OpenWindow_LayDutch(true); }
        private void MenuItem_Calc3Way(object sender, RoutedEventArgs e) { OpenWindow_3Way(); }
        private void MenuItem_FreeBet(object sender, RoutedEventArgs e) { OpenWindow_FreeBet(true);}


        private void MenuItem_UpdateEvents(object sender, RoutedEventArgs e) { UpdateEventsInMainDataBase(); UpdateComboboxLeagues(); }
        private void MenuItem_RemoveLiveMatches(object sender, RoutedEventArgs e) { RemoveLiveMatches(); }
        public void UpdateEventsInMainDataBase()
        {
            for (int i = FootData.Count - 1; i >= 0; i--)
            {
                if (!Leagues[Leagues.FindIndex(j => j.Name == FootData[i].League)].State) FootData.RemoveAt(i);
            }
        }

        #endregion
        #region ListView Hedge Context Menu Items
        private void MenuItem_Click_Refresh(object sender, RoutedEventArgs e) { UpdateHedgeDataInListview(); }

        private void MenuItem_Click_CalcLay(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = (ListViewHedgeData)LWHedge.SelectedItems[0];
            double koef1 = 2;
            double koef2 = 2;
            TryToParse.ParseDouble(lw.Back, out koef1);
            TryToParse.ParseDouble(lw.Lay, out koef2);
            OpenWindow_LayDutch(true, koef1, koef2);
        }
        private void MenuItem_Click_CalcDutch(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = (ListViewHedgeData)LWHedge.SelectedItems[0];
            double koef1 = 2;
            double koef2 = 2;
            TryToParse.ParseDouble(lw.Back, out koef1);
            TryToParse.ParseDouble(lw.Lay, out koef2);
            OpenWindow_LayDutch(false, koef1, koef2);

        }
        private void MenuItem_Click_CalcFreeBet(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = (ListViewHedgeData)LWHedge.SelectedItems[0];
            double koef1 = 2;
            double koef2 = 2;
            TryToParse.ParseDouble(lw.Back, out koef1);
            TryToParse.ParseDouble(lw.Lay, out koef2);
            OpenWindow_FreeBet(true, koef1, koef2);
        }
        private void MenuItem_Click_ClearItem(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = LWHedge.SelectedItems[0] as ListViewHedgeData;

            for (int i = FootData.Count - 1; i >= 0; i--)
            {
                FData item = FootData[i];

                if (lw.Match == item.Team1 + " - " + item.Team2)
                {
                    foreach (KeyValuePair<int, X> it in item.XKoef)
                    {
                        if( Bookmakers[it.Key].Name == lw.Bookie)
                        {
                            it.Value.Koef[0] = 1;
                            it.Value.Koef[1] = 1;
                            it.Value.Koef[2] = 1;
                            break;
                        }
                        }
                    break;
                }
            }
            UpdateHedgeDataInListview();
        }
        private void MenuItem_Click_Removeitem(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = LWHedge.SelectedItems[0] as ListViewHedgeData;
            for (int i = FootData.Count - 1; i >= 0; i--)
            {
                FData item = FootData[i];

                if (lw.Match == item.Team1 + " - " + item.Team2)
                {
                    FootData.RemoveAt(i);
                }
            }
            UpdateHedgeDataInListview();
        }
        private void MenuItem_Click_Compare(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            if (LWHedge.Items.Count == 0 || LWHedge.SelectedItems.Count == 0) return;
            ComparaKoef CK = new ComparaKoef(this);
            CK.Show();
            ListViewHedgeData lw = LWHedge.SelectedItems[0] as ListViewHedgeData;

            CK.Compare(lw.Match, lw.Team);
        }
        private void MenuItem_Click_SuspendItem(object sender, RoutedEventArgs e)
        {
            if (LWHedge.Items.Count < 1) return;
            ListViewHedgeData lw = LWHedge.SelectedItems[0] as ListViewHedgeData;
            SuspendedMatches.Add(lw.Match);
            for (int i = FootData.Count - 1; i >= 0; i--)
            {
                FData item = FootData[i];

                if (lw.Match == item.Team1 + " - " + item.Team2)
                {
                    FootData.RemoveAt(i);
                }
            }
            UpdateHedgeDataInListview();
        }
            #endregion
        #region ListViewData context menu items


            private void LWDataContextMenu_UpdateTeams(object sender, RoutedEventArgs e)
            {
                if(LWData.SelectedItems.Count  < 1 ) return;

                ListViewData de = (ListViewData)LWData.SelectedItems[0];

                UpdateAltTeamNames up = new UpdateAltTeamNames(this,de.Team1, de.Team2);
                up.Closing += up_Closing;
                up.Show();
            
            }

            void up_Closing(object sender, CancelEventArgs e) { WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString()); }

            private void LWDataContextMenu_ClearSelected(object sender, RoutedEventArgs e)
            {
                if (LWData.SelectedItems.Count < 1) return;
            ListViewData lw = (ListViewData)LWData.SelectedItems[0];

                for (int i = FootData.Count -1; i >=0  ; i--)
                {
                    FData item = FootData[i];

                    if (lw.Team1 == item.Team1 && item.Team2 == lw.Team2)
                    {
                        foreach (KeyValuePair<int, X> hi in item.XKoef)
                        {
                            hi.Value.Koef[0] = 1;
                            hi.Value.Koef[1] = 1;
                            hi.Value.Koef[2] = 1;

                        }   
                        break;
                    }
                }
                WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString());
            }
            private void LWDataContextMenu_RemoveSelected(object sender, RoutedEventArgs e)
            {
                if (LWData.SelectedItems.Count < 1) return;
                ListViewData lw = (ListViewData)LWData.SelectedItems[0];
                for (int i = FootData.Count - 1; i >= 0; i--)
                {
                    FData item = FootData[i];

                    if (lw.Team1 == item.Team1 && item.Team2 ==lw.Team2)
                    {
                        FootData.RemoveAt(i);
                    }
                }
                WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString());
            }
            private void LWDataContextMenu_Refresh(object sender, RoutedEventArgs e)
            {
                WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString());
            
            }
            private void LWDataContextMenu_ClearAll(object sender, RoutedEventArgs e) {  ClearAllData(); }

            private void LWDataContextMenu_SuspendItem(object sender, RoutedEventArgs e)
            {
                if (LWData.SelectedItems.Count < 1) return;
                ListViewData lw = (ListViewData)LWData.SelectedItems[0];
                SuspendedMatches.Add(lw.Team1 + " - " + lw.Team2);

                for (int i = FootData.Count - 1; i >= 0; i--)
                {
                    FData item = FootData[i];

                    if (lw.Team1 == item.Team1 && item.Team2 == lw.Team2)
                    {
                        FootData.RemoveAt(i);
                    }
                }
                WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString());
            }
            #endregion

        #region Write Data to listview data
        public void WriteDataTotable(string str)
        {
            LWData.Items.Clear();
            if (FootData.Count < 1 || ComboBoxBookies.SelectedIndex < 0) return;

            int i = Bookmakers.FindIndex(item => ComboBoxBookies.Items[ComboBoxBookies.SelectedIndex].ToString().ToUpper().Contains(item.Name.ToUpper()));

            string[] d;
            StringBuilder bufstr = new StringBuilder();
            for (int id = FootData.Count - 1; id >= 0; id--)
            {
                bufstr.Clear();
                FData item = FootData[id];
                if (str != "All" && item.League != str) continue;
                bool active = FootData[id].XKoef.ContainsKey(i);
                ListViewData lw = new ListViewData();
                lw.Team1 = item.Team1;
                lw.Team2 = item.Team2;

                if(active)
                {
                    lw.X1 = String.Format("{0} / {1}", item.XKoef[i].Koef[1], item.Lay[1]);
                    lw.X0 = String.Format("{0} / {1}", item.XKoef[i].Koef[0], item.Lay[0]);
                    lw.X2 = String.Format("{0} / {1}", item.XKoef[i].Koef[2], item.Lay[2]);
                    lw.LastUpdate = item.XKoef[i].LastUpdate;
                }
                else
                {
                    lw.X1 = String.Format("{0}", item.Lay[1]);
                    lw.X0 = String.Format("{0}", item.Lay[0]);
                    lw.X2 = String.Format("{0}", item.Lay[2]);
                }
                lw.FullLeague = item.League;
                d = item.League.Split(' ');
                foreach (string st in d) { bufstr.Append(st.Substring(0, 1)); }
                lw.League = bufstr.ToString();


                lw.Time = ChangeTimeToString(item.time);
               // lw.LastUpdate = item.LastUpdate[i];
                
                lw.Match = item.Match;

                LWData.Items.Add(lw);
            }
       
        }


        #endregion

        #region Manage open  app windows

        #endregion

        private void LabelError_MouseRightButtonDown(object sender, MouseButtonEventArgs e) { LabelError.Text = "..."; }


        public void ClearAllData()
        {
            XMLData.DeleteFiles();
            FootData.Clear();
            LWData.Items.Clear();
            ComboBoxEvents.Items.Clear();
            ComboBoxEvents.Items.Add("All");
            ComboBoxEvents.SelectedIndex = 0;
            SuspendedMatches.Clear();
        }

        private void CalcHedge_ButtonClick(object sender, RoutedEventArgs e) { UpdateHedgeDataInListview(); }


        private void ComboBoxBookies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxEvents.SelectedIndex < 0) return;
            WriteDataTotable(ComboBoxEvents.Items[ComboBoxEvents.SelectedIndex].ToString());
        }

        void ComboboxActiveBookies_SelectionChanged(object sender, SelectionChangedEventArgs e) {   UpdateHedgeDataInListview(); }
        private void CheckBoxToday_Checked(object sender, RoutedEventArgs e) { UpdateHedgeDataInListview(); }

        #region Label Updates

        public delegate void DLastMatchbookUpdate();
        public void LastMatchbookUpdate()
        {
            this.Dispatcher.Invoke(new DLastMatchbookUpdate(() => { MatchbookLastUpdate.Text = "Last MB Update: " + DateTime.Now.ToString("HH:mm"); }));
        }

        public delegate void DChangeColor();
        public void ChangeColor() { BackgroundGrid.Background = new SolidColorBrush(Colors.Salmon); }

        #region Error Label

        public void ErrorLabel(string value) { this.Dispatcher.Invoke(new DUpdateErrorLabel(this.UpdateErrorLabel), value); }

        public delegate void DUpdateErrorLabel(string value);
        public void UpdateErrorLabel(string value) { LabelError.Text = DateTime.Now.ToString("HH:mm") + " Error: " + value; }

        #endregion

        #endregion

        public void RemoveLiveMatches()
        {

            for (int i = FootData.Count-1; i >= 0; i--)
            {
                FData item = FootData[i];
                if (item.time < DateTime.Now) FootData.RemoveAt(i);
            }
            UpdateComboboxLeagues();
        }

        private void Button_Click_TopMost(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            if (this.Topmost) { this.Topmost = false; bt.Background = Brushes.Gray; }
            else { this.Topmost = true; bt.Background = Brushes.Red; }
        }

        string ChangeTimeToString(DateTime time)
        {
            if (time.Day == DateTime.Now.Day) return time.ToString("HH:mm") + " Today";
            return time.ToString("MMM-dd HH:mm");
        }
    }

    public class TryToParse
    {
        public static bool ParseDouble(string value, out double number)
        {
            number = 0;
            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out number) &&
                !double.TryParse(value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out number) &&
                !double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out number) &&
                !double.TryParse(value, NumberStyles.Float, CultureInfo.GetCultureInfo("lt-LT"), out number) )
            { return false; }

            return true;
        }
        public static bool ParseDateTime(string value, out DateTime data)
        {
            data = DateTime.MinValue;
            if (!DateTime.TryParse(value,CultureInfo.CurrentCulture, DateTimeStyles.None , out data) &&
                !DateTime.TryParse(value, CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out data) &&
                !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
            { return false; }

            return true;
        }

    }

}
