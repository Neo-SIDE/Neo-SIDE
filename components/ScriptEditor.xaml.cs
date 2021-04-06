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
using NeoSIDE.formatting;
using NeoSIDE.Languages;

namespace NeoSIDE.components
{
    public partial class ScriptEditor : UserControl
    {
        // where the cursor is placed

        int currentLine = 1; // what line is selected
        int cursorPlacement = 0; // where in the line the cursor is

        // if you can hit enter and make a new line
        bool canHitEnter = true;

        StackPanel currentCursor; // the current cursor object
        StackPanel currentCursorParent; // the current cursor's parent to remove the cursor from it's parent to make a new one

        // if the right panel is open
        bool rightPanelOpen = true;

        // if you can use the textbox
        bool textBoxUsable = true;

        public ScriptingLanguage currentLanguage = new Python();


        public ScriptEditor()
        {
            InitializeComponent();

            currentCursor = Cursor;
            currentCursorParent = Cursor.Parent as StackPanel;

            MainWorkspace.Cursor = Cursors.IBeam;
        }

        // move the cursor left and refresh the line
        private void cursorLeft()
        {
            if (cursorPlacement != 0)
            {
                cursorPlacement -= 1;
                refreshLine();
            }
            else if (cursorPlacement == 0)
            {

            }
        }
        // move the cursor right and refresh the line
        private void cursorRight()
        {
            if (!(((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count - 1 == (cursorPlacement)))
            {
                cursorPlacement += 1;

                refreshLine();
            }
        }
        // move the cursor up and refresh the line
        private void cursorUp()
        {
            if (currentLine != 1)
            {
                currentLine -= 1;

                if (((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count < cursorPlacement)
                {
                    cursorPlacement = ((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                }

                refreshLine();
            }
        }

        // move the cursor down and refresh the line
        private void cursorDown()
        {
            if (currentLine != scriptEditor.Children.Count)
            {
                currentLine += 1;

                if (((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count < cursorPlacement)
                {
                    cursorPlacement = ((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                }

                refreshLine();
            }
        }

        private void CharacterSelect(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock)
            {
                cursorPlacement = ((sender as TextBlock).Parent as StackPanel).Children.IndexOf(sender as TextBlock);
                currentLine = ((((sender as TextBlock).Parent as StackPanel).Parent as Grid).Parent as StackPanel).Children.IndexOf(((sender as TextBlock).Parent as StackPanel).Parent as Grid) + 1;

                refreshLine();
            }
        }

        void addLine()
        {
            currentLine += 1;

            // newline variable
            Grid newLine = new Grid();
            newLine.Height = 20;

            // line number
            Grid lineNumber = new Grid();
            lineNumber.Width = 70;
            lineNumber.HorizontalAlignment = HorizontalAlignment.Left;

            // line number text
            TextBlock lineNumberText = new TextBlock();
            lineNumberText.Text = (currentLine).ToString();
            lineNumberText.Foreground = new SolidColorBrush(Colors.White);
            lineNumberText.FontSize = 20;
            lineNumberText.HorizontalAlignment = HorizontalAlignment.Center;
            lineNumberText.TextAlignment = TextAlignment.Center;
            lineNumberText.Margin = new Thickness(0, -5, 0, 0);

            lineNumber.Children.Add(lineNumberText);

            // line text
            StackPanel lineText = new StackPanel();
            lineText.Margin = new Thickness(70, 0, 0, 0);
            lineText.Orientation = Orientation.Horizontal;

            // add children
            newLine.Children.Add(lineNumber);
            newLine.Children.Add(lineText);

            List<Grid> lines = new List<Grid>();

            foreach (Grid line in scriptEditor.Children)
            {
                lines.Add(line);
                if (scriptEditor.Children.IndexOf(line) == currentLine - 2)
                {
                    lines.Add(newLine);
                }
            }
            scriptEditor.Children.Clear();
            foreach (Grid line in lines)
            {
                scriptEditor.Children.Add(line);
                ((line.Children[0] as Grid).Children[0] as TextBlock).Text = (scriptEditor.Children.IndexOf(line) + 1).ToString();
            }
            cursorPlacement = 0;

            createCursor();
        }

        void createCursor()
        {
            // create the cursor
            StackPanel cursor = new StackPanel();
            cursor.Width = 2;
            cursor.Background = new SolidColorBrush(Colors.White);

            Grid CurrentLine = scriptEditor.Children[currentLine - 1] as Grid;
            StackPanel lineTextContainer = CurrentLine.Children[1] as StackPanel;
            lineTextContainer.Children.Add(cursor);

            currentCursorParent.Children.Remove(currentCursor);

            // definition
            currentCursor = cursor;
            currentCursorParent = cursor.Parent as StackPanel;
        }


        void addText(string character)
        {
            // get the current line's text
            string line = "";

            StackPanel lineStackPanel = /* get the current line */(scriptEditor.Children[currentLine - 1] as Grid)/* get the current line's text container */.Children[1] as StackPanel;
            line = readLineText(lineStackPanel);

            // find measurements
            string part1 = line.Substring(0, cursorPlacement);
            string part2 = line.Substring(cursorPlacement, line.Length - cursorPlacement);

            line = part1 + character + part2;

            cursorPlacement += 1;


            lineStackPanel.Children.Clear();

            // add text

            foreach (char lineCharacter in (part1 + character))
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseLeftButtonDown += new MouseButtonEventHandler(CharacterSelect);

                lineStackPanel.Children.Add(newText);
            }

            createCursor();

            foreach (char lineCharacter in part2)
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseLeftButtonDown += new MouseButtonEventHandler(CharacterSelect);

                ((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }

            formatText();
        }
        void refreshLine()
        {
            string line = "";

            StackPanel lineStackPanel = (scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel;
            line = readLineText(lineStackPanel);

            string part1 = line.Substring(0, cursorPlacement);
            string part2 = line.Substring(cursorPlacement, line.Length - cursorPlacement);

            ((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Clear();

            foreach (char lineCharacter in part1)
            {
                TextBlock newText = new TextBlock();
                newText.FontSize = 18;
                newText.Text = lineCharacter.ToString();
                newText.Margin = new Thickness(0, -5, 0, -2);
                newText.Foreground = new SolidColorBrush(Colors.White);

                newText.MouseDown += new MouseButtonEventHandler(CharacterSelect);

                lineStackPanel.Children.Add(newText);
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

                lineStackPanel.Children.Add(newText);
            }
        }

        void checkForAndTypechar(System.Windows.Input.Key key, System.Windows.Input.Key checkKey, string lowCharacter, string upCharacter)
        {
            if (key == checkKey && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift) && !Console.CapsLock)) { addText(lowCharacter); }
            if (key == checkKey && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || Console.CapsLock)) { addText(upCharacter); }
        }
        // same function but caps lock is ignored
        void checkForAndTypeSpecialchar(System.Windows.Input.Key key, System.Windows.Input.Key checkKey, string lowCharacter, string upCharacter)
        {
            if (key == checkKey && (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))) { addText(lowCharacter); }
            if (key == checkKey && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))) { addText(upCharacter); }
        }

        void backspace()
        {
            StackPanel currentLineStackPanel = (scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel;

            if (cursorPlacement == 0 && currentLine != 1)
            {
                string lineText = readLineText(currentLineStackPanel);

                scriptEditor.Children.Remove(scriptEditor.Children[currentLine - 1]);
                currentLine -= 1;
                cursorPlacement = ((scriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                refreshLine();

                foreach (Char charac in lineText)
                {
                    addText(charac.ToString());
                }
                cursorPlacement -= lineText.Length;
                refreshLine();
            }
            else if (cursorPlacement == 0 && currentLine == 1)
            {

            }
            else
            {
                currentLineStackPanel.Children.Remove(
                    currentLineStackPanel.Children[cursorPlacement - 1]
                );
                cursorPlacement -= 1;
                refreshLine();
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


        // ------------------------------------------------------------- Formatting ----------------------------------------------------
        // this entire section is just to format the text
        // also don't move anything it will cause huge huge performance issues

        async void formatText()
        {
            // --------------------------------------- basic keyword formatting ---------------------------------------
            // this will format keywords
            List<Keyword> keywords = currentLanguage.formatView.keywords;

            foreach (formatting.Keyword keyword in keywords)
            {
                foreach (Grid tempLine in scriptEditor.Children)
                {
                    StackPanel Line = tempLine.Children[1] as StackPanel;
                    string lineText = readLineText(Line);

                    int currentStart = 0;
                    string checkingKeyword = keyword.Text;

                    while ((currentStart + checkingKeyword.Length) <= lineText.Length)
                    {
                        string checkingText = lineText.Substring(currentStart, checkingKeyword.Length);

                        if (checkingText == checkingKeyword)
                        {
                            foreach (UIElement tempTextBlock in Line.Children)
                            {
                                TextBlock textBlock;

                                if (tempTextBlock is TextBlock)
                                {
                                    textBlock = tempTextBlock as TextBlock;
                                    int index = (textBlock.Parent as StackPanel).Children.IndexOf(textBlock) + 1;

                                    if (index >= currentStart && index <= (currentStart + checkingKeyword.Length))
                                    {

                                        textBlock.Foreground = new SolidColorBrush(keyword.TextColor);
                                    }
                                }
                            }
                        }
                        currentStart += 1;
                    }
                }

                // ----------------------------------------------- common errors --------------------------------------
                // the text editor will point out very common errors as if they're keywords

                List<commonError> errors = currentLanguage.formatView.commonErrors;

                foreach (formatting.commonError error in errors)
                {
                    foreach (Grid tempLine in scriptEditor.Children)
                    {
                        StackPanel Line = tempLine.Children[1] as StackPanel;
                        string lineText = readLineText(Line);

                        int currentStart = 0;
                        string checkingKeyword = error.signifier;

                        while ((currentStart + checkingKeyword.Length) <= lineText.Length)
                        {
                            string checkingText = lineText.Substring(currentStart, checkingKeyword.Length);

                            if (checkingText == checkingKeyword)
                            {
                                foreach (UIElement tempTextBlock in Line.Children)
                                {
                                    TextBlock textBlock;

                                    if (tempTextBlock is TextBlock)
                                    {
                                        textBlock = tempTextBlock as TextBlock;
                                        int index = (textBlock.Parent as StackPanel).Children.IndexOf(textBlock) + 1;

                                        if (index >= currentStart && index <= (currentStart + checkingKeyword.Length))
                                        {
                                            textBlock.Foreground = new SolidColorBrush(Colors.White);
                                            textBlock.Background = new SolidColorBrush(Colors.Red);
                                        }
                                    }
                                }
                            }
                            currentStart += 1;
                        }
                    }
                }

                // ---------------------------------------------------- strings  ----------------------------------------------------------
                // this will format strings
                // everything in between quotations will be marked as a string
                // and colored to their respective assigned color

                bool inString = false;
                foreach (Grid lineTemp in scriptEditor.Children)
                {
                    StackPanel line = lineTemp.Children[1] as StackPanel;

                    foreach (UIElement tempTXT in line.Children)
                    {
                        if (tempTXT is TextBlock)
                        {
                            string txt = (tempTXT as TextBlock).Text;
                            TextBlock textb = tempTXT as TextBlock;

                            if (txt == "\"")
                            {
                                switch (inString)
                                {
                                    case false:
                                        inString = true;
                                        break;
                                    case true:
                                        inString = false;
                                        break;
                                }
                                textb.Foreground = new SolidColorBrush(Colors.Orange);
                            }

                            if (inString)
                            {
                                textb.Foreground = new SolidColorBrush(Colors.Orange);
                            }
                        }
                    }
                }

                /* =====================================] comments [===================================================
                
                comments like this one will turn whatever color they are assigned to
                this color will probably vary between languages but who knows*/


                /* ------------- single line comments ------------
                 * 
                single line comments, like this one */

                bool inComment = false;
                foreach (Grid lineTemp in scriptEditor.Children)
                {
                    inComment = false;

                    StackPanel line = lineTemp.Children[1] as StackPanel;

                    foreach (UIElement tempTXT in line.Children)
                    {
                        if (tempTXT is TextBlock)
                        {
                            string txt = (tempTXT as TextBlock).Text;
                            TextBlock textb = tempTXT as TextBlock;

                            if (txt == "#")
                            {
                                inComment = true;
                                textb.Background = new SolidColorBrush(Colors.Transparent);
                            }

                            if (inComment)
                            {
                                textb.Foreground = new SolidColorBrush(Colors.Gray);
                                textb.Background = new SolidColorBrush(Colors.Transparent);
                            }
                        }
                    }
                }
            }
        }

        // check for key input
        private void ScrollViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.N && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                rightPanelToggle();
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

                // move cursor with arrow keys
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

        // make the enter key usable
        private void MainGrid_KeyUp(object sender, KeyEventArgs e)
        {
            canHitEnter = true;
        }

        // animate the right panel open or closed
        async void rightPanelToggle()
        {
            switch (rightPanelOpen)
            {
                case true:
                    rightPanelOpen = false;

                    rightPanel.Opacity = 1;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.9;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.8;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.7;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.6;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.5;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.4;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.3;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.2;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.1;
                    await Task.Delay(1);
                    rightPanel.Visibility = Visibility.Hidden;

                    break;

                case false:
                    rightPanelOpen = true;

                    rightPanel.Visibility = Visibility.Visible;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.1;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.2;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.3;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.4;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.5;
                    rightPanel.Opacity = 0.5;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.6;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.7;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.8;
                    await Task.Delay(1);
                    rightPanel.Opacity = 0.9;
                    await Task.Delay(1);
                    rightPanel.Opacity = 1;

                    break;
            }
        }
    }
}