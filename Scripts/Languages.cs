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
                new Keyword("if ", Colors.Red),
                new Keyword("while ", Colors.Red),
                new Keyword("for ", Colors.Red),
                new Keyword("def ", Colors.Red),
                new Keyword("()", Colors.Red),
                new Keyword(":", Colors.Red),
                new Keyword("from ", Colors.Red),
                new Keyword("import ", Colors.Red),
                new Keyword("as ", Colors.Red),
                new Keyword("True", Colors.Red),
                new Keyword("False", Colors.Red)
            };

            formatView.commonErrors = new List<commonError>() {
                new commonError(";"),
                new commonError("true"),
                new commonError("false"),
                new commonError("while ("),
                new commonError("if ("),
                new commonError("for (")
            };

            formatView.stringStarter = "\"";
            formatView.stringStarterAlt = "'";
            
            formatView.multiLineStringStarter = "\"\"\"";

            formatView.commentStarter = "#";
        }
    }
}