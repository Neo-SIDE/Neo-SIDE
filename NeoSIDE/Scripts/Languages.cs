using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using NeoSIDE.formatting;

namespace NeoSIDE.Languages
{
    public class ScriptingLanguage
    {
        public FormatView formatView;
    }
    public class Python:ScriptingLanguage
    {
        public Python()
        {
            formatView = new FormatView() { };
            formatView.keywords = new List<Keyword>() {
                new Keyword("if", Colors.Red),
                new Keyword("while", Colors.Red),
                new Keyword("for", Colors.Red)
            };
        }
    }
}
