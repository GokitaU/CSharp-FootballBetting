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

namespace FootballHedge
{
    /// <summary>
    /// Interaction logic for ComparaKoef.xaml
    /// </summary>
    public partial class ComparaKoef : Window
    {
        MainWindow MainF;
        public ComparaKoef(MainWindow MainF)
        {
            InitializeComponent();
            this.MainF = MainF;
        }
        public void Compare(string match, string selection)
        {
            if(!match.Contains("-")) { this.Close(); return; }
            LabelMatch.Content = match;
            LabelSelection.Content = selection;
            string[] d = match.Split('-');
            int id = 1;
            if (selection == "Draw") id = 0;
            if (selection == d[1].Trim()) id = 2;
            List<KoefListForSorting> KoefList = new List<KoefListForSorting>();
            foreach (FData item in MainF.FootData)
            {
                if(match == item.Team1+" - "+item.Team2)
                {
                    foreach (KeyValuePair<int, X> it in item.XKoef)
                    {
                        if (MainF.Bookmakers[it.Key].Type != BrokerType.BACK || it.Value.Koef[id] == 0) continue;
                        KoefList.Add(new KoefListForSorting() { bookie = MainF.Bookmakers[it.Key].Name, koef = it.Value.Koef[id]});
                    }


                }
            }
            KoefList.Sort();
            KoefList.Reverse();
            ItemControlCompareKoef.ItemsSource = KoefList;

            
            int count = KoefList.Count;
            this.Height += 25 * count;

            
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_ClickClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
    class KoefListForSorting : IComparable<KoefListForSorting>
    {
        public string bookie { set; get; }
        public double koef { set; get; }

        public int CompareTo(KoefListForSorting other)
        {
            return this.koef.CompareTo(other.koef);
        }
    }
}
