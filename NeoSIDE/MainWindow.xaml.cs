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

namespace NeoSIDE.formatting
{
    public class Keyword
    {
        public string Text = "";
        public Color TextColor;

        public Keyword(string text, Color textColor)
        {
            Text = text;
            TextColor = textColor;
        }
    }
}

namespace NeoSIDE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        formatting.Keyword[] keywords =
        {
            new formatting.Keyword ("I", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("no idea", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("text formatter", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("in realtime", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("my IDE", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("pretty cool", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("I think is cool", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("customise the formatting", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("wanted to", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("I could", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("green", Color.FromRgb(0, 255, 0)),
            new formatting.Keyword ("current code", Color.FromRgb(255, 0, 0)),
            new formatting.Keyword ("the image", Color.FromRgb(255, 0, 0)),
        };

        int currentLine = 1;
        int cursorPlacement = 0;

        bool canHitEnter = true;

        StackPanel currentCursor;
        StackPanel currentCursorParent;

        bool rightPanelOpen = false;
        bool leftPanelOpen = true;

        bool textBoxUsable = true;

        public MainWindow()
        {
            InitializeComponent();
            currentCursor = Cursor;
            currentCursorParent = Cursor.Parent as StackPanel;

            WindowState = WindowState.Maximized;

            formatText();

            MainWorkspace.Cursor = Cursors.IBeam;
            // anotationPencil.
        }

        void addLine()
        {
            currentLine += 1;

            Grid newLine = new Grid();
            newLine.Height = 20;

            Grid lineNumber = new Grid();
            lineNumber.Width = 70;
            lineNumber.HorizontalAlignment = HorizontalAlignment.Left;

            TextBlock lineNumberText = new TextBlock();
            lineNumberText.Text = (currentLine).ToString();
            lineNumberText.Foreground = new SolidColorBrush(Color.FromRgb(0, 103, 150));
            lineNumberText.FontSize = 20;
            lineNumberText.HorizontalAlignment = HorizontalAlignment.Center;
            lineNumberText.TextAlignment = TextAlignment.Center;
            lineNumberText.Margin = new Thickness(0, -5, 0, 0);

            lineNumber.Children.Add(lineNumberText);

            StackPanel lineText = new StackPanel();
            lineText.Margin = new Thickness(70, 0, 0, 0);
            lineText.Orientation = Orientation.Horizontal;

            //lineText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

            newLine.Children.Add(lineNumber);
            newLine.Children.Add(lineText);

            ScriptEditor.Children.Add(newLine);
            cursorPlacement = 0;
            currentCursorParent.Children.Remove(currentCursor);

            createCursor();
        }

        void createCursor()
        {
            StackPanel cursor = new StackPanel();
            cursor.Width = 2;
            cursor.Background = new SolidColorBrush(Colors.White);

            ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(cursor);

            currentCursorParent.Children.Remove(currentCursor);

            cursor.Name = "Cursor";

            currentCursor = cursor;
            currentCursorParent = cursor.Parent as StackPanel;
        }

        void addText(string character)
        {
            string line = "";

            UIElementCollection lineUI = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children;
            foreach (UIElement charUI in lineUI)
            {
                if (charUI is TextBlock)
                {
                    line += (charUI as TextBlock).Text;
                }
            }

            string part1 = line.Substring(0, cursorPlacement);
            string part2 = line.Substring(cursorPlacement, line.Length - cursorPlacement);

            line = part1 + character + part2;

            cursorPlacement += 1;

            ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Clear();

            foreach (char lineCharacter in (part1 + character))
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }

            createCursor();

            foreach (char lineCharacter in part2)
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }
        }

        void refreshLine()
        {
            string line = "";

            UIElementCollection lineUI = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children;
            foreach (UIElement charUI in lineUI)
            {
                if (charUI is TextBlock)
                {
                    line += (charUI as TextBlock).Text;
                }
            }

            string part1 = line.Substring(0, cursorPlacement);
            string part2 = line.Substring(cursorPlacement, line.Length - cursorPlacement);

            ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Clear();

            foreach (char lineCharacter in part1)
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }

            createCursor();

            foreach (char lineCharacter in part2)
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }
        }

        void checkForAndTypechar(System.Windows.Input.Key key, System.Windows.Input.Key checkKey, string lowCharacter, string upCharacter)
        {
            if (key == checkKey && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift) && !Console.CapsLock)) { addText(lowCharacter); }
            if (key == checkKey && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || Console.CapsLock)) { addText(upCharacter); }
        }
        void checkForAndTypeSpecialchar(System.Windows.Input.Key key, System.Windows.Input.Key checkKey, string lowCharacter, string upCharacter)
        {
            if (key == checkKey && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))) { addText(lowCharacter); }
            if (key == checkKey && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) { addText(upCharacter); }
        }

        void backspace()
        {
            if (cursorPlacement == 0 && currentLine != 1)
            {
                ScriptEditor.Children.Remove(ScriptEditor.Children[currentLine - 1]);
                currentLine -= 1;
                cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                refreshLine();
            }
            else if (cursorPlacement == 0 && currentLine == 1) { 

            }
            else
            {
                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Remove(
                    ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children[cursorPlacement - 1]
                );
                cursorPlacement -= 1;
            }
        }

        void saveFile()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "python script (.py)|*.py|C file (.c)|*.c|c++ file (.cpp)|*.cpp|c# file (.cs)|*.cs|html document (.html)|*.html|css stylesheet (.css)|*.css|javascript script (.js)|*.js|xml data structure (.xml)|*.xml|text file (.txt)|*.txt|"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;

                FileStream stream = new FileStream(filename, FileMode.Create);
                StreamWriter sw = new StreamWriter(stream);

                foreach (Grid elem in ScriptEditor.Children)
                {
                    string line = "";
                    foreach (UIElement charach in (elem.Children[1] as StackPanel).Children)
                    {
                        if (charach is TextBlock)
                        {
                            TextBlock UsedCharach = charach as TextBlock;
                            line += UsedCharach.Text;
                        }
                    }

                    sw.WriteLine(line);
                }

                sw.Close();
                stream.Close();
            }
        }

        string readLineText(StackPanel line)
        {
            string lineText = "";

            foreach (UIElement tempTextBlock in line.Children)
            {
                TextBlock textBlock;

                if (tempTextBlock is TextBlock)
                {
                    textBlock = tempTextBlock as TextBlock;
                    lineText += textBlock.Text;
                }
            }

            return lineText;
        }

        async void formatText()
        {
            await Task.Delay(10);

            foreach (formatting.Keyword keyword in keywords)
            {
                foreach (Grid tempLine in ScriptEditor.Children)
                {
                    StackPanel Line = tempLine.Children[1] as StackPanel;
                    string lineText = readLineText(Line);

                    // showDebugMessage("line text is '" + lineText + "'");

                    int currentStart = 0;
                    string checkingKeyword = keyword.Text;

                    while ((currentStart + checkingKeyword.Length) <= lineText.Length)
                    {

                        // showDebugMessage("while loop pass");

                        string checkingText = lineText.Substring(currentStart, checkingKeyword.Length);
                        // showDebugMessage(checkingText);

                        if (checkingText == checkingKeyword)
                        {
                            // showDebugMessage("matched");
                            foreach (UIElement tempTextBlock in Line.Children)
                            {
                                TextBlock textBlock;

                                if (tempTextBlock is TextBlock)
                                {
                                    textBlock = tempTextBlock as TextBlock;
                                    int index = (textBlock.Parent as StackPanel).Children.IndexOf(textBlock) + 1;

                                    // showDebugMessage("index: " + index.ToString());
                                    // showDebugMessage("currentStart: " + currentStart.ToString());
                                    // showDebugMessage("other variable: " + (currentStart + checkingKeyword.Length).ToString());

                                    // showDebugMessage("bool 1: " + (index <= currentStart).ToString());
                                    // showDebugMessage("bool 2: " + (currentStart + checkingKeyword.Length).ToString());


                                    if (index >= currentStart && index <= (currentStart + checkingKeyword.Length))
                                    {
                                        // showDebugMessage("formatted");

                                        textBlock.Foreground = new SolidColorBrush(keyword.TextColor);
                                    }
                                }
                            }
                        }
                        currentStart += 1;
                    }
                }
            }
        }

        private void ScrollViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                rightPanelToggle();
            }
            if (e.Key == Key.T && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                leftPanelToggle();
            }

            if (textBoxUsable)
            {
                formatText();

                if (e.Key == Key.Enter && canHitEnter) { addLine(); canHitEnter = false; }

                checkForAndTypechar(e.Key, Key.A, "a", "A");
                checkForAndTypechar(e.Key, Key.B, "b", "B");
                checkForAndTypechar(e.Key, Key.C, "c", "C");
                checkForAndTypechar(e.Key, Key.D, "d", "D");
                checkForAndTypechar(e.Key, Key.E, "e", "E");
                checkForAndTypechar(e.Key, Key.F, "f", "F");
                checkForAndTypechar(e.Key, Key.G, "g", "G");
                checkForAndTypechar(e.Key, Key.H, "h", "H");
                checkForAndTypechar(e.Key, Key.I, "i", "I");
                checkForAndTypechar(e.Key, Key.J, "j", "J");
                checkForAndTypechar(e.Key, Key.K, "k", "K");
                checkForAndTypechar(e.Key, Key.L, "l", "L");
                checkForAndTypechar(e.Key, Key.M, "m", "M");
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    checkForAndTypechar(e.Key, Key.N, "n", "N");
                }
                checkForAndTypechar(e.Key, Key.O, "o", "O");
                checkForAndTypechar(e.Key, Key.P, "p", "P");
                checkForAndTypechar(e.Key, Key.Q, "q", "Q");
                checkForAndTypechar(e.Key, Key.R, "r", "R");
                checkForAndTypechar(e.Key, Key.S, "s", "S");
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    checkForAndTypechar(e.Key, Key.T, "t", "T");
                }
                checkForAndTypechar(e.Key, Key.U, "u", "U");
                checkForAndTypechar(e.Key, Key.V, "v", "V");
                checkForAndTypechar(e.Key, Key.W, "w", "W");
                checkForAndTypechar(e.Key, Key.X, "x", "X");
                checkForAndTypechar(e.Key, Key.Y, "y", "Y");
                checkForAndTypechar(e.Key, Key.Z, "z", "Z");

                checkForAndTypechar(e.Key, Key.Space, " ", " ");

                checkForAndTypeSpecialchar(e.Key, Key.D1, "1", "!");
                checkForAndTypeSpecialchar(e.Key, Key.D2, "2", "@");
                checkForAndTypeSpecialchar(e.Key, Key.D3, "3", "#");
                checkForAndTypeSpecialchar(e.Key, Key.D4, "4", "$");
                checkForAndTypeSpecialchar(e.Key, Key.D5, "5", "%");
                checkForAndTypeSpecialchar(e.Key, Key.D6, "6", "^");
                checkForAndTypeSpecialchar(e.Key, Key.D7, "7", "&");
                checkForAndTypeSpecialchar(e.Key, Key.D8, "8", "*");
                checkForAndTypeSpecialchar(e.Key, Key.D9, "9", "(");
                checkForAndTypeSpecialchar(e.Key, Key.D0, "0", ")");

                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");
                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");
                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");
                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");
                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");
                checkForAndTypeSpecialchar(e.Key, Key.Tab, " ", " ");

                checkForAndTypeSpecialchar(e.Key, Key.OemTilde, "`", "~");
                checkForAndTypeSpecialchar(e.Key, Key.OemPipe, "\\", "|");
                checkForAndTypeSpecialchar(e.Key, Key.OemMinus, "-", "_");
                checkForAndTypeSpecialchar(e.Key, Key.OemPlus, "=", "+");
                checkForAndTypeSpecialchar(e.Key, Key.OemOpenBrackets, "[", "{");
                checkForAndTypeSpecialchar(e.Key, Key.OemCloseBrackets, "]", "}");
                checkForAndTypeSpecialchar(e.Key, Key.OemSemicolon, ";", ":");
                checkForAndTypeSpecialchar(e.Key, Key.OemQuotes, "'", '"'.ToString());
                checkForAndTypeSpecialchar(e.Key, Key.OemComma, ",", "<");
                checkForAndTypeSpecialchar(e.Key, Key.OemPeriod, ".", ">");
                checkForAndTypeSpecialchar(e.Key, Key.OemQuestion, "/", "?");
                checkForAndTypeSpecialchar(e.Key, Key.OemBackslash, "\\", "|");

                switch (e.Key)
                {
                    case Key.Left:
                        cursorLeft();
                        break;
                    case Key.Right:
                        cursorRight();
                        break;
                    case Key.Up:
                        cursorUp();
                        break;
                    case Key.Down:
                        cursorDown();
                        break;
                }

                if (e.Key == Key.Back)
                {
                    backspace();
                }
            }
        }

        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            canHitEnter = true;
        }

        async void rightPanelToggle()
        {
            if (rightPanelOpen == true)
            {
                rightPanelOpen = false;

                rightPanel.Margin = new Thickness(0, 2, -33, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -67, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -93, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -134, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -179, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -205, 2);
            }
            else if (rightPanelOpen == false)
            {
                rightPanel.Margin = new Thickness(0, 2, -179, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -134, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -93, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -67, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, -33, 2);
                await Task.Delay(1);
                rightPanel.Margin = new Thickness(0, 2, 2, 2);


                rightPanelOpen = true;
            }
        }
        async void leftPanelToggle()
        {
            if (leftPanelOpen == true)
            {
                leftPanelOpen = false;

                leftPanel.Margin = new Thickness(5, 5, 0, 0);
                await Task.Delay(1);
                leftPanel.Margin = new Thickness(-30, 5, 0, 0);
                await Task.Delay(1);
                leftPanel.Margin = new Thickness(-65, 5, 0, 0);
            }
            else if (leftPanelOpen == false)
            {
                leftPanelOpen = true;

                leftPanel.Margin = new Thickness(-65, 5, 0, 0);
                await Task.Delay(1);
                leftPanel.Margin = new Thickness(-30, 5, 0, 0);
                await Task.Delay(1);
                leftPanel.Margin = new Thickness(5, 5, 0, 0);
            }
        }

        private void MainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            rightPanel.MaxHeight = MainWorkspace.ActualHeight - 20;

            Rect scriptEditorClipRect = new Rect(new Size());

            scriptEditorClipRect.Width = MainWorkspace.ActualWidth - 6;
            scriptEditorClipRect.Height = MainWorkspace.ActualWidth - 6;

            ScriptEditorClip.Rect = scriptEditorClipRect;
        }

        void showDebugMessage(string text)
        {
            string messageBoxText = text;
            string caption = "Debug Message";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void cursorLeft()
        {
            if (cursorPlacement != 0)
            {
                cursorPlacement -= 1;
                refreshLine();
            } else if (cursorPlacement == 0)
            {

            }
        }
        private void cursorRight()
        {
            if (!(((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count - 1 == (cursorPlacement)))
            {
                cursorPlacement += 1;

                refreshLine();
            }
        }
        private void cursorUp()
        {
            if (currentLine != 1)
            {
                currentLine -= 1;

                if (((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count < cursorPlacement)
                {
                    cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                }

                refreshLine();
            }
        }
        private void cursorDown()
        {
            if (currentLine != ScriptEditor.Children.Count)
            {
                currentLine += 1;

                if (((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count < cursorPlacement)
                {
                    cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                }

                refreshLine();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            saveFile();
        }

        private void CharacterSelect(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                cursorPlacement = ((sender as TextBlock).Parent as StackPanel).Children.IndexOf(sender as TextBlock);
                currentLine = ((((sender as TextBlock).Parent as StackPanel).Parent as Grid).Parent as StackPanel).Children.IndexOf(((sender as TextBlock).Parent as StackPanel).Parent as Grid) + 1;

                refreshLine();
            } else
            {
                StackPanel Sender = sender as StackPanel;
                currentLine = ((Sender.Parent as Grid).Parent as StackPanel).Children.IndexOf((Sender.Parent as Grid)) + 1;
                cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
            
                if (cursorPlacement > ((Sender.Parent as Grid).Parent as StackPanel).Children.Count)
                {
                    cursorPlacement = ((Sender.Parent as Grid).Parent as StackPanel).Children.Count;
                }

                refreshLine();
            }
        }

        private void topMenu_mouseOver(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                (sender as Border).Background = new SolidColorBrush(Color.FromRgb(41, 41, 41));
            }
            if (sender is StackPanel)
            {
                (sender as StackPanel).Background = new SolidColorBrush(Color.FromArgb(177, 48, 155, 255));
            }
        }
        private void topMenu_mouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                (sender as Border).Background = new SolidColorBrush(Color.FromRgb(21, 21, 21));
            }
            if (sender is StackPanel)
            {
                (sender as StackPanel).Background = new SolidColorBrush(Color.FromArgb(0, 21, 21, 21));
            }
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as StackPanel).Visibility = Visibility.Hidden;
        }

        private void FileMenuOpen(object sender, MouseButtonEventArgs e)
        {
            FileMenu.Visibility = Visibility.Visible;
        }

        private void NewProjectOP(object sender, MouseButtonEventArgs e)
        {
            newProject dlg = new newProject();

            dlg.Show();

            (sender as StackPanel).Visibility = Visibility.Hidden;
        }
    }
}
