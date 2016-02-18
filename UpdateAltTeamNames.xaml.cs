using System;
using System.Collections.Generic;
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
    /// Interaction logic for UpdateAltTeamNames.xaml
    /// </summary>
    public partial class UpdateAltTeamNames : Window
    {
        MainWindow MainF;
        public UpdateAltTeamNames(MainWindow MainF, string Team1, string Team2)
        {
            InitializeComponent();

            this.MainF = MainF;
            TextBoxTeam1.Text = XMLData.ReturnAltTeamName(Team1);
            TextBoxTeam2.Text = XMLData.ReturnAltTeamName(Team2);

            LabelTeamName1.Text = Team1;
            LabelTeamName2.Text = Team2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string Team1 = LabelTeamName1.Text;
            string Team2 = LabelTeamName2.Text;
            string RepPatter = FootballHedge.Properties.Settings.Default.RepPattern;
            foreach (FData item in MainF.FootData)
            {
                if(item.Team1 == Team1 && item.Team2 == Team2)
                {
                    item.Match = Regex.Replace(TextBoxTeam1.Text + "|" + TextBoxTeam2.Text, RepPatter, "").ToUpper(); 
                }
            }
            XMLData.UpdateAltTeamName(Team1, TextBoxTeam1.Text);
            XMLData.UpdateAltTeamName(Team2, TextBoxTeam2.Text);
        }

    }
}
