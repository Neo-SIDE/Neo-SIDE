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

        // if formatting is enabled
        bool format = true;

        ScriptingLanguage currentLanguage = new Python();

        // create a new instance of mainwindow and call functions on start

        /// <summary>
        /// I honestly have no Idea what InitializeComponent() does
        /// 
        /// set the current cursor variable to the current cursor
        /// set the current cursor parrent variable to the current cursor's parent
        /// 
        /// Maximize the window
        /// 
        /// set the mainWorkspaces hover cursor to the I beam, commonly used in a text bos
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            currentCursor = Cursor;
            currentCursorParent = Cursor.Parent as StackPanel;

            WindowState = WindowState.Maximized;

            MainWorkspace.Cursor = Cursors.IBeam;
        }

        // add a line to the program

        /// <summary>
        /// add ome to the current line variable to place the cursor on the newly created line
        /// 
        /// # newline variable
        /// create a grid for the new line define the height of the new line
        /// set the new lines height to 20
        /// 
        /// # line number
        /// create a grid to store the number for the line
        /// give it a width of 70 to make it fit the background
        /// make it have a horizontal allignment of left so that it's on the left side
        /// 
        /// # line number text
        /// create a textblock to display the actual
        /// set the textblock's text to the current line, which is now the added one
        /// set it's foreground to white so that it is better visible
        /// set it's font size to 20 to make it bigger and more readable
        /// set it's horizontal allignment to center to center it
        /// make the text allignment center too
        /// set the margin to normal exept the top goes out a bit to elliminate unwanted padding
        /// 
        /// add the textblock to the "lineNumber" grid so that it is visible
        /// 
        /// # line text
        /// create a stackpanel called "line text" to store the line's text
        /// set it's left magin to 70 to keep it away from the number line area
        /// set it's orientation to horizontal so that characters stack horizontally instead of vertically
        /// 
        /// # add children
        /// add the line number (container) grid to the newline grid
        /// add the line's text (container) stackpanel to the new line too
        /// 
        /// # other stuff
        /// add the newline into the script editor itself (which should be a vertical stackpanel, containing all the lines)
        /// set the cursor placement to 0, because the line should be empty
        /// 
        /// create the cursor (function)
        /// </summary>

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

            foreach (Grid line in ScriptEditor.Children)
            {
                lines.Add(line);
                if (ScriptEditor.Children.IndexOf(line) == currentLine - 2) {
                    lines.Add(newLine);
                }
            }
            ScriptEditor.Children.Clear();
            foreach (Grid line in lines)
            {
                ScriptEditor.Children.Add(line);
                ((line.Children[0] as Grid).Children[0] as TextBlock).Text = (ScriptEditor.Children.IndexOf(line) + 1).ToString();
            }
            cursorPlacement = 0;

            createCursor();
        }

        // create the cursor

        /// <summary>
        /// # create the cursor
        /// create a stackpanel called "cursor" (a stackpanel because I generally use it instead of a Rectangle, i dont know why)
        /// set the cursor's width to a small but visible 2
        /// set the cursor's background to white
        /// 
        /// # add the new cursor and remove the old
        /// 
        /// add the new cursor:
        /// (
        ///     # getting the line as a grid
        ///     from the script editor's (vertical stackpanel with the lines) children
        ///     get the child with an index of the current line minus 1 (minus 1 because arrays start with 0, and not 1, and the variable starts on 1 because there is no line 0)
        ///     get it as a grid
        ///     
        ///     # get the line's stackpanel of characters
        ///     from the current line grid's children, get the seccond one (2 - 1 = 1, .chilren[1]) which should be a stackpanel of all the characters
        ///     get it as a stackpanel
        ///     
        ///     # add the cursor
        ///     add the cursor to the referenced stackpanel's children
        /// )
        /// 
        /// remove the currentcursor from it's parent
        /// 
        /// # definition
        /// set the current cursor to the newly created cursor
        /// set the current cursor's parent to it's refernced parent as a stackpanel (because it should be a stackpanel)
        /// </summary>

        void createCursor()
        {
                // create the cursor
                StackPanel cursor = new StackPanel();
                cursor.Width = 2;
                cursor.Background = new SolidColorBrush(Colors.White);

                Grid CurrentLine = ScriptEditor.Children[currentLine - 1] as Grid;
                StackPanel lineTextContainer = CurrentLine.Children[1] as StackPanel;
                lineTextContainer.Children.Add(cursor);

                currentCursorParent.Children.Remove(currentCursor);

                // definition
                currentCursor = cursor;
                currentCursorParent = cursor.Parent as StackPanel;
        }

        // add a charachter the current line

        /// <param name="character"></param>
        /// <summary>
        /// # get the current line's text
        /// create a string for the line's text
        /// 
        /// get the current line and put it into a variable
        /// 
        /// (
        ///     # get the current line
        ///     from the script editors children
        ///     get the one with the index of current line - 1 (- 1 because arrays start with 0, and not 1, and the variable starts on 1 because there is no line 0)
        ///     get it as a grid because it should be a grid
        /// 
        ///     # get the current line's text container
        ///     from the line grid
        ///     get it's seccond child
        ///     get it as a stackpanel
        ///     and set that to lineStackPanel
        /// )
        /// set the line string ot the return value of a function that reads the text from a line, the line being the newly referenced stack panel
        /// 
        /// # find measurements
        /// create a string which is every character from the line's text up until the cursor
        /// create another string which is every character from after the cursor to the end of the line's text
        /// 
        /// set the line to before the cursor + the new character + after the cursor
        /// 
        /// add 1 to the cursor placement (for obvious reasons)
        /// 
        /// 
        /// clear the current line's children to erase all the text
        /// 
        /// # Add text
        /// 
        /// for every character in part 1 including the new character
        ///     create a new textblock to be the character
        ///     give it a fontsize of 18
        ///     set it's text to the current character
        ///     set it's margin to 0, -5, 0, -2 to get rid of some stuff
        ///     set it's foreground to white
        ///     
        ///     make it so that when you left click the new character it selects that character
        ///     
        ///     add the character to the line stack panel
        /// 
        /// create the cursor
        /// 
        /// for every character in part 2
        ///     create a new textblock to be the character
        ///     give it a fontsize of 18
        ///     set it's text to the current character
        ///     set it's margin to 0, -5, 0, -2 to get rid of some stuff
        ///     set it's foreground to white
        ///     
        ///     make it so that when you left click the new character it selects that character
        ///     
        ///     add the character to the line stack panel
        /// </summary>

        void addText(string character)
        {
            // get the current line's text
            string line = "";

            StackPanel lineStackPanel = /* get the current line */(ScriptEditor.Children[currentLine - 1] as Grid)/* get the current line's text container */.Children[1] as StackPanel;
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

                ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Add(newText);
            }
        }


        // refresh line

        /// <summary>
        /// do what the last function did but a bit differently
        /// </summary>
        void refreshLine()
            {
                string line = "";

                StackPanel lineStackPanel = (ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel;
                line = readLineText(lineStackPanel);

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

            /// <summary>
            /// if the keythat is being checked for is held and neither left shift or right shift or caps lock are on type the lowercase of the character
            /// if the keythat is being checked for is held and left shift or right shift are pressed or caps lock is on type the uppercase
            /// </summary>
            /// <param name="key"></param>
            /// <param name="checkKey"></param>
            /// <param name="lowCharacter"></param>
            /// <param name="upCharacter"></param>

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

            // backspace the last character

            /// <summary>
            /// make a StackPanel to hold the current line
            /// (
            ///     from the script editor get the first line
            ///     get it as a grid
            ///     from it's children get the second
            ///     get it as a stackpanel
            ///     (return)
            /// )
            /// 
            /// if the cursor's placement is at the beggining of the line and the current line is not the first
            ///     (remove the current line and add it at the end of the previous)
            ///     
            ///     make a string to contain the current line's
            ///     
            ///     from the script editor's children, remove the current
            ///     subtract one from the current line
            ///     place the cursor at the end of the previous line, that the cursor should now be on
            ///     refresh the line
            ///     
            ///     for every character in the previous line's text
            ///        add the character to the current line
            ///     
            ///     remove the text newly added from the cursor's placement
            ///     refresh the line
            /// 
            /// if not, if the cusor is at the beggening of the first line
            ///     do nothing
            /// 
            /// if not
            ///     (basic backspace)
            ///     
            ///     remove the current character from the current line
            ///     move the cursor backwards
            ///     refresh the line
            /// </summary>

            void backspace()
            {
                StackPanel currentLineStackPanel = (ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel;

                if (cursorPlacement == 0 && currentLine != 1)
                {
                    string lineText = readLineText(currentLineStackPanel);

                    ScriptEditor.Children.Remove(ScriptEditor.Children[currentLine - 1]);
                    currentLine -= 1;
                    cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
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

            // read a lines text

            /// <summary>
            /// make a string for the value to be returned
            /// 
            /// for every object in the line's children
            ///     create a textblock
            ///     if the current object is a textblock (so it's not the cursor)
            ///         make that new textblock the current object as a textblock
            ///         add the current textblock's text to the return value
            ///         
            /// return the return value so the function acts as a variable
            /// 
            /// </summary>
            /// <param name="line"></param>
            /// <returns></returns>

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

            // format the text in the text box

            /// <summary>
            /// if the text should be formatted
            ///     wait 10 ms (0.01s)
            /// 
            ///     create a variable with a smaller name than the current languages keywords' directory
            ///     create a variable with a smaller name than the current languages errors' directory
            /// 
            ///     for every keyword in the current languages keyword
            ///         for every line in the script editor or file
            ///             create a stackpanel to hold the text objects that are the children
            ///             create a string that holds the lines text that is set to the text read from the line
            ///         
            ///             create an number to be where the scan for the keyword will currently start
            ///             create a string to be the current keyword being searched for
            ///                
            ///             while the current keyword scan is inside the span of the current line
            ///                 make a string to be the text that is being checked for the keyword  (hypothetical position of keyword, keyword length)
            ///                 
            ///                 if the text being checked for the keyword is the keyword
            ///                     for every uiElement in the current lines children
            ///                         create a textblock to hold the current character in the line
            ///                         
            ///                         if that uiElement is a textblock (not the cursor)
            ///                             make textblock the ui element as a textblock
            ///                             create an int to hold the current character's position
            ///                             
            ///                             if the characters index is less within the range of the found keyword
            ///                                 make the current character's text color the keyword's specified formatting color
            ///     
            ///     for every error in the current languages common errors
            ///         for every line in the script editor or file
            ///             create a stackpanel to hold the text objects that are the children
            ///             create a string that holds the lines text that is set to the text read from the line
            ///         
            ///             create an number to be where the scan for the keyword will currently start
            ///             create a string to be the current keyword being searched for
            ///                
            ///             while the current keyword scan is inside the span of the current line
            ///                 make a string to be the text that is being checked for the keyword  (hypothetical position of keyword, keyword length)
            ///                 
            ///                 if the text being checked for the keyword is the keyword
            ///                     for every uiElement in the current lines children
            ///                         create a textblock to hold the current character in the line
            ///                         
            ///                         if that uiElement is a textblock (not the cursor)
            ///                             make textblock the ui element as a textblock
            ///                             create an int to hold the current character's position
            ///                             
            ///                             if the characters index is less within the range of the found keyword
            ///                                 make the current errored  text highlight in red
            /// </summary> 

            async void formatText()
            {
                if (format == true)
                {
                    await Task.Delay(10);

                    List<Keyword> keywords = currentLanguage.formatView.keywords;
                    List<commonError> errors = currentLanguage.formatView.commonErrors;

                    foreach (formatting.Keyword keyword in keywords)
                    {
                        foreach (Grid tempLine in ScriptEditor.Children)
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
                    }

                    foreach (formatting.commonError error in errors)
                    {
                        foreach (Grid tempLine in ScriptEditor.Children)
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

                                                textBlock.Background = new SolidColorBrush(Colors.Red);
                                            }
                                        }
                                    }
                                }
                                currentStart += 1;
                            }
                        }
                    }

                    // strings 

                    bool inString = false;
                    foreach (Grid lineTemp in ScriptEditor.Children)
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

            // show a window with the specified message, meant for debuging variables
            void showDebugMessage(string text)
            {
                string messageBoxText = text;
                string caption = "Debug Message";
                MessageBoxButton button = MessageBoxButton.YesNoCancel;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

            // move the cursor left and refresh the line
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
            // move the cursor right and refresh the line
            private void cursorRight()
            {
                if (!(((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count - 1 == (cursorPlacement)))
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

                    if (((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count < cursorPlacement)
                    {
                        cursorPlacement = ((ScriptEditor.Children[currentLine - 1] as Grid).Children[1] as StackPanel).Children.Count;
                    }

                    refreshLine();
                }
            }

            // move the cursor down and refresh the line
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

            // select the cursor on the selected character

            /// <summary>
            /// if the object calling the function is a textblock
            ///     move the cursor the where the carrachter is
            ///     
            ///     refresh the line
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void CharacterSelect(object sender, MouseButtonEventArgs e)
            {
                if (sender is TextBlock)
                {
                    cursorPlacement = ((sender as TextBlock).Parent as StackPanel).Children.IndexOf(sender as TextBlock);
                    currentLine = ((((sender as TextBlock).Parent as StackPanel).Parent as Grid).Parent as StackPanel).Children.IndexOf(((sender as TextBlock).Parent as StackPanel).Parent as Grid) + 1;

                    refreshLine();
                }
            }

            // when the cursor leaves a menu
            private void Menu_MouseLeave(object sender, MouseEventArgs e)
            {
                (sender as StackPanel).Visibility = Visibility.Hidden;
            }

            // open the file menu
            private void FileMenuOpen_OP(object sender, MouseButtonEventArgs e)
            {
                FileMenu.Visibility = Visibility.Visible;
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
                        MainGrid.Margin = new Thickness(8, 8, 8, 8);
                        break;
                    case WindowState.Normal:
                        MainGrid.Margin = new Thickness(0, 0, 0, 0);
                        break;
                }
            }

            // modify the window when it is maximized or normalized
            private void Window_StateChanged(object sender, EventArgs e)
            {
                setWinMargin();
                if (WindowState == WindowState.Normal)
                {
                    fullUnfullscreen.Source = fullScreenSourceDummy.Source;
                }
                if (WindowState == WindowState.Maximized)
                {
                    fullUnfullscreen.Source = unfullScreenSourceDummy.Source;
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
                    txt.Foreground = new SolidColorBrush(Colors.White);
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
                    txt.Foreground = new SolidColorBrush(Color.FromRgb(0, 101, 101));
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
        }
    }
