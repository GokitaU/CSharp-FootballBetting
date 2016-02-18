using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace FootballHedge
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private bool LeaguesChange = false;
        MainWindow MainF;
        public Settings(MainWindow MainF)
        {
            this.MainF = MainF;
            InitializeComponent();

            BookmakersSelection.ItemsSource = MainF.Bookmakers;
            SelectedLeagues.ItemsSource = MainF.Leagues;

            SuspendedMatches.ItemsSource = MainF.SuspendedMatches;
        }
        void SuspendedMatches_ClickRemoveAll(object sender, RoutedEventArgs e)
        {
            MainF.SuspendedMatches.Clear();
            SuspendedMatches.Items.Refresh();
        }

        void SuspendedMatches_ClickRemoveItem(object sender, RoutedEventArgs e)
        {

            Button bt = (Button)sender;

            MainF.SuspendedMatches.RemoveAt(MainF.SuspendedMatches.FindIndex(item => item == bt.Tag.ToString()));
            SuspendedMatches.Items.Refresh();
           
        }

        void cbLeagues_Checked(object sender, RoutedEventArgs e)
        {
          //  CheckBox cb = e.Source as CheckBox;
          //  MainF.Leagues[cb.Tag.ToString()].State = cb.IsChecked.Value;
            LeaguesChange = true;
        }

        void cbBookies_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.Source as CheckBox;

            StackPanel sp = cb.Parent as StackPanel;

            TextBox tb = sp.Children[1] as TextBox;
            tb.IsEnabled = cb.IsChecked.Value;

            int index = MainF.Bookmakers.FindIndex(item => item.Name == cb.Content.ToString());
            if (index != -1) MainF.Bookmakers[index].InUse = cb.IsChecked.Value;
            
        }

        void cbBookies_CheckAll(object sender, RoutedEventArgs e)
        {
            CheckBox cb = e.Source as CheckBox;
            /*
            foreach (KeyValuePair<string,LeagueState> item in MainF.Leagues)
            {
                item.Value.State = cb.IsChecked.Value;
            }
             * */


            foreach (LeagueState it in MainF.Leagues)
            {
                it.State = cb.IsChecked.Value;
            }
            SelectedLeagues.Items.Refresh();
            /*
            

            StackPanel sp = cb.Parent as StackPanel;

            foreach ( StackPanel item in PanelBookMakersSelect.Children)
            {
                CheckBox box = item.Children[0] as CheckBox;
                box.IsChecked = cb.IsChecked;
            }
            */

        }


        void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)e.Source;

            StackPanel sp = tb.Parent as StackPanel;

            CheckBox cb = sp.Children[0] as CheckBox;

            string str = tb.Text;

            double value;
            if (str != null && TryToParse.ParseDouble(str, out value)) 
            {
               int index = MainF.Bookmakers.FindIndex(item => item.Name == cb.Content.ToString());
               if(index != -1)  MainF.Bookmakers[index].Limit = value; 
            }
        }

        void tb_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();
            if (key.Any(char.IsDigit) || key.Contains("Decimal") || key.Contains("Period")) 
            {

                if(key.Contains("Decimal") || key.Contains("Period"))
                {
                    TextBox tb = (TextBox)e.Source;
                    string str = tb.Text;
                    if (str.Count(c => c == '.') > 0) { e.Handled = true; return; }
                }
                e.Handled = false;
                return;
            }
            e.Handled = true;  
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XMLData.SaveSettings(MainF);
            FootballHedge.Properties.Settings.Default.Save();
            if (LeaguesChange) MainF.UpdateEventsInMainDataBase();
            MainF.UpdateControls();
            IEditableCollectionView items = SelectedLeagues.Items;
            if(items.CanRemove)
            {
                items.Remove(SelectedLeagues.Items);
            }
 
            e.Cancel = false;
        }
        
        private void ButtonChangeDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult res = dialog.ShowDialog();

            if(res == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                Dir2.Content = path;
                FootballHedge.Properties.Settings.Default.Save();
            }
        }

        private void TextBoxTimer_KeyDown(object sender, KeyEventArgs e)
        {
            string key = e.Key.ToString();
            if (key.Any(char.IsDigit)) { e.Handled = false; return; }
            e.Handled = true;
        }


        private void CloseSettingsWindow_Button(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void ExpandeHeader(object sender, RoutedEventArgs e)
        {

            foreach (Expander item in StackPanelForExpanders.Children)
            {
                if (item != sender) item.IsExpanded = false;
            }
        }

        private void Click_SendEmail(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(MainF.SendMultiEventMail);
        }

        private void CheckBox_InUse(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            switch(cb.Tag.ToString())
            {
                case "1": MainF.Bookmakers.All(item => { item.InUse = cb.IsChecked.Value; return true; }); break;
                case "2": MainF.Bookmakers.All(item => { item.EMailing = cb.IsChecked.Value; return true; }); break;
                case "3": MainF.Bookmakers.All(item => { item.Dutch3Way = cb.IsChecked.Value; return true; }); break;
                default: return;
            }
            BookmakersSelection.Items.Refresh();
            
        }

    }
}
