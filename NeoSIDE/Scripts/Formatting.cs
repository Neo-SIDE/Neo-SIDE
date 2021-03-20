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
    public class FormatView
    {
        public string stringStarter = "\"";
        public string stringStarterAlt = "'";
        public string multiLineStringStarter;

        public string commentStarter;
        public string multiLineCommentStarter;

        public List<Keyword> keywords = new List<Keyword> {};
    }

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