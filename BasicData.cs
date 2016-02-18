using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FootballHedge
{
    public enum BrokerType
    {
        BACK, LAY, HANDICAP
    }
   
    public class BasicBrokerData
    {
        public string Name { set; get; }
        public BrokerType Type;
        public double Limit { set; get; }
        public ParseBookies Book;
        public bool InUse { set; get; }
        public bool EMailing { set; get; }
        public bool Dutch3Way { set; get; }
        public double PLlimit { set; get; }


        public BasicBrokerData()
        {
            Limit = 1;
            PLlimit = FootballHedge.Properties.Settings.Default.MailplLimit;
            InUse = false;
            EMailing = false;
            Dutch3Way = false;
        }
    }
    public class FData
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Match { get; set; }
        public string League { get; set; }
        public double[] SizeX = new double[3];
        public double[] Lay = new double[3];
        public DateTime time { get; set; }
        public Dictionary<int,X> XKoef = new Dictionary<int,X>();
        public FData(int key)
        {
            XKoef.Add(key, new X());
        }
        public FData() { }
        public void AddNewItenToDictionary(int key)
        {
            XKoef.Add(key,new X());
        }

    }
    public class X
    {
        public double[] Koef;
        public string LastUpdate { get; set; }
        public X()
        {
            Koef = new double[3];
        }

    }
    public class LayData : IComparable<LayData>
    {
        public string Match { get; set; }
        public string Selection { get; set; }
        public double Lay { get; set; }
        public double Back { get; set; }
        public double Profit { get; set; }
        public double Size { get; set; }
        public string Bookie { get; set; }
        public string League { get; set; }
        public string Time { get; set; }
        public string LastUpdate { get; set; }
        public int CompareTo(LayData other)
        {
            return this.Profit.CompareTo(other.Profit);
        }
    }


    class SortByTime : IComparer<LayData>
    {
        public int Compare(LayData x, LayData y)
        {
            return string.Compare(x.LastUpdate, y.LastUpdate);
        }

    }


    public class ManageOpenWindows
    {
        List<Window> WinControls = new List<Window>();

        public void Add(Window wnd)
        {
            WinControls.Add(wnd);
        }
        public void CloseAll()
        {
            for (int i = WinControls.Count-1; i >= 0 ; i--)
            {
                WinControls[i].Close();
                WinControls.RemoveAt(i);
               
                /*
                 * if (Application.OpenForms.OfType<UpdateWindow>().Count() == 1)
    Application.OpenForms.OfType<UpdateWindow>().First().Close();

UpdateWindow frm = new UpdateWindow()
frm.Show();
                 * */

            }
        }
    }

    public class LeagueState
    {
        public string Name { set; get; }
        public bool State { set; get; }
    }
    public class CalcParameters
    {
        public double limit { get; set; }
        public string bookielimit { get; set; }
        public bool today { get; set; }
        public string preferedbookie { get; set; }


    }
    public class ParsedDataFromPage
    {
        public string time { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public double[] X = new double[3];
        public double[] Size = new double[3];
        public double[] Lay = new double[3];
        public string League { get; set; }
        public bool delete = false;
    }
    public class ListViewHedgeData //Adding item to listview hedge
    {
        public string Match { set; get; }
        public string Team { set; get; }
        public string Lay { set; get; }
        public string Back { set; get; }
        public string PL { set; get; }
        public string Size { set; get; }
        public string League { set; get; }
        public string FullLeague { get; set; }
        public string Bookie { set; get; }
        public string Time { set; get; }
        public string LastUpdate { set; get; }
        public Color Color { set; get;  }
    }

    public class ListViewData // adding items to listview data
    {
        public string Team1 { set; get; }
        public string Team2 { set; get; }
        public string X1 { set; get; }
        public string X0 { set; get; }
        public string X2 { set; get; }
        public string League { set; get; }
        public string FullLeague { set; get; }
        public string Time { set; get; }
        public string LastUpdate { set; get; }
        public string Match { set; get; }
    }
    public class ListViewData3WayDutch : IComparable<ListViewData3WayDutch>// adding items to listview 3way dutch
    {
        public string Match { set; get; }
        public string Bookie1 { set; get; }
        public double X1 { set; get; }
        public string Bookie0 { set; get; }
        public double X0 { set; get; }
        public string Bookie2 { set; get; }
        public double X2 { set; get; }
        public double Profit { set; get; }
        public string League { set; get; }

        public int CompareTo(ListViewData3WayDutch other)
        {
            return this.Profit.CompareTo(other.Profit);
        }
    }


    [ValueConversion(typeof(object), typeof(int))]
    public class CompareProfitValues : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            double number = (double)System.Convert.ChangeType(value, typeof(double));

            if (number < 0.99) return -1;


            if (number >= 0.99 && number < 1) return 0;
                

            if (number >= 1 && number < 1.02) return 1;

            return 2;
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack not supported");
        }

    }

   




}
