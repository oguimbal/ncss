using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{
    public class Stylesheet : CssToken
    {
        public List<Rule> Rules { get; set; }

        public override bool IsValid
        {
            get { return Rules.All(x => x.IsValid); }
        }

        public override void AppendTo(StringBuilder sb)
        {
            if(Rules == null)
                return;
            foreach (var r in Rules.Where(x=>x!=null))
                r.AppendTo(sb);
        }
    }



    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================


    namespace Parsers
    {
        internal class StylesheetParser : Parser<Stylesheet>
        {
            internal override Stylesheet DoParse()
            {
                var sh = new Stylesheet();
                List<Rule> rules = new List<Rule>();
                while (!End)
                {
                    var ind = Index;
                    // supposted to be executed only once... But if some BlockMismatch exception happens, that's not the case.
                    // ... And we dont want to stop parsing on some crappy scenario (even if that's invalid CSS)
                    rules.AddRange(ParseBlock<Rule>(false));
                    if (!End && Index == ind)
                    {
                        throw new ParsingException("Failed to parse to the end of CSS");
                    }
                }
                sh.Rules = rules;
                return sh;
            }
        }
    }
}
