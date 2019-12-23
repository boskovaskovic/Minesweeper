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
using System.Windows.Navigation;
using model;

namespace Minolovac
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        public Welcome()
        {
            InitializeComponent();
            
           
                

            

        }

        private void onClick(object sender, RoutedEventArgs args) {


            Button b = sender as Button;
            MainWindow main=null;
            if (b.Name.Equals("EASY"))
            {

                main = new MainWindow(Difficility.EASY);
            }
            else if (b.Name.Equals("INTERMEDIATE"))
            {
                main = new MainWindow(Difficility.INTERMEDIATE);

            }
            else if (b.Name.Equals("HARD"))
            {
                main = new MainWindow(Difficility.HARD);

            }
            else {

                MessageBox.Show("An error occured!");
            
            }



            this.Close();
            main.Show();







        }

       

    }
}
