using System;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace NCss
{

    public class InvalidSelector : Selector
    {
        public string Value { get; internal set; }


        public override void AppendTo(StringBuilder sb)
        {
            sb.Append(Value);
        }

        public override bool IsValid
        {
            get { return false; }
        }
    }



    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================


    namespace Parsers
    {
        internal class InvalidSelectorParser : Parser<InvalidSelector>
        {

            internal override InvalidSelector DoParse()
            {
                var c = CurrentChar;
                var sb = new StringBuilder();
                int braced = 0;
                while (!Regex.IsMatch(c.ToString(), @"[\.\s#+>,~;{}]") || braced > 0)
                {
                    if (c == '(')
                        braced++;
                    if (braced > 0 && c == ')')
                        braced--;
                    sb.Append(c);
                    Index++;
                    if (End)
                        break;
                    c = CurrentChar;
                }

                return new InvalidSelector
                {
                    Value = sb.ToString()
                };
            }
        }
    }
}