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
using System.Timers;
using System.IO;
using NeoSIDE.windows;
using NeoSIDE.formatting;
using NeoSIDE.Languages;

namespace NeoSIDE
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            WindowState = WindowState.Maximized;
        }

        // show a window with the specified message, meant for debuging variables
        void showDebugMessage(string text)
        {
            string messageBoxText = text;
            string caption = "Debug Message";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        // when the cursor leaves a menu
        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Visibility = Visibility.Hidden;
        }

        // create a new object
        private void NewProject_OP(object sender, MouseButtonEventArgs e)
        {
            newProject dlg = new newProject();

            dlg.Show();

            (sender as StackPanel).Visibility = Visibility.Hidden;
        }

        // set the margin of the window border to make fullscreen work
        void setWinMargin()
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    MainGrid.Margin = new Thickness(7,7,7,7);
                    break;
                case WindowState.Normal:
                    MainGrid.Margin = new Thickness(0, 0, 0, 0);
                    break;
            }
        }

        // when the full/unfullscreen button is clicked
        private void switchScreenSizeMode_OP(object sender, MouseButtonEventArgs e) {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
            }
        }

        // highlight an object
        private async void smoothHighlight(object sender, MouseEventArgs e)
        {
            Border Sender = sender as Border;
            if (Sender.Name == "CloseButton")
            {
                TextBlock txt = Sender.Child as TextBlock;
            }

            await Task.Delay(1);
            Sender.Background.Opacity = 0.2;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.4;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.6;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.8;
            await Task.Delay(1);
            Sender.Background.Opacity = 1;
        }

        // unhighlight an object
        private async void smoothUnhighlight(object sender, MouseEventArgs e)
        {
            Border Sender = sender as Border;
            if (Sender.Name == "CloseButton")
            {
                TextBlock txt = Sender.Child as TextBlock;
            }

            await Task.Delay(1);
            Sender.Background.Opacity = 0.8;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.6;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.4;
            await Task.Delay(1);
            Sender.Background.Opacity = 0.2;
            await Task.Delay(1);
            Sender.Background.Opacity = 0;
        }

        // unhighlight an object
        private void Unhighlight(object sender, MouseEventArgs e)
        {
            Border Sender = sender as Border;
            Sender.Background.Opacity = 0;
        }
        // highlight an object
        private void Highlight(object sender, MouseEventArgs e)
        {
            Border Sender = sender as Border;
            Sender.Background.Opacity = 1;
        }

        // close the window
        private void Close_OP(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // minimize the window
        private void Minimize_OP(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                switch (WindowStyle)
                {
                    case WindowStyle.None:
                        WindowStyle = WindowStyle.SingleBorderWindow;
                        break;
                    case WindowStyle.SingleBorderWindow:
                        WindowState = WindowState.Maximized;
                        WindowStyle = WindowStyle.None;
                        break;
                }
            }
        }
    }
}