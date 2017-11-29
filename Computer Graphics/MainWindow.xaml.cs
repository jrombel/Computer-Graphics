using System.Windows;
using System.Windows.Controls;

namespace Computer_Graphics
{
    public partial class MainWindow : Window
    {
        Page exercise1;
        Page exercise2;
        Page exercise3;
        Page exercise4;
        Page exercise5;
        Page exercise6;
        Page exercise7;
        Page exercise8;
        Page exercise9;

        public MainWindow()
        {
            InitializeComponent();

            exercise1_rb.IsChecked = true;
            exerciseNumber_Click(null, null);
        }

        private void exerciseNumber_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)exercise1_rb.IsChecked)
            {
                if (exercise1 == null)
                    exercise1 = new Exercise1();
                main_f.Content = exercise1;
            }
            else if ((bool)exercise2_rb.IsChecked)
            {
                if (exercise2 == null)
                    exercise2 = new Exercise2();
                main_f.Content = exercise2;
            }
            else if ((bool)exercise3_rb.IsChecked)
            {
                if (exercise3 == null)
                    exercise3 = new Exercise3();
                main_f.Content = exercise3;
            }
            else if ((bool)exercise4_rb.IsChecked)
            {
                if (exercise4 == null)
                    exercise4 = new Exercise4();
                main_f.Content = exercise4;
            }
            else if ((bool)exercise5_rb.IsChecked)
            {
                if (exercise5 == null)
                    exercise5 = new Exercise5();
                main_f.Content = exercise5;
            }
            else if ((bool)exercise6_rb.IsChecked)
            {
                if (exercise6 == null)
                    exercise6 = new Exercise6();
                main_f.Content = exercise6;
            }
            else if ((bool)exercise7_rb.IsChecked)
            {
                if (exercise7 == null)
                    exercise7 = new Exercise7();
                main_f.Content = exercise7;
            }
            else if ((bool)exercise8_rb.IsChecked)
            {
                if (exercise8 == null)
                    exercise8 = new Exercise8();
                main_f.Content = exercise8;
            }
            else if ((bool)exercise9_rb.IsChecked)
            {
                if (exercise9 == null)
                    exercise9 = new Exercise9();
                main_f.Content = exercise9;
            }
            main_f.NavigationService.RemoveBackEntry();
        }
    }
}