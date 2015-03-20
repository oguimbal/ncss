using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{
    public abstract class Selector : CssToken
    {
    }

    public class MultiConditionSelector : Selector
    {
        public List<Selector> Conditions { get; set; }

        public override void AppendTo(StringBuilder sb)
        {
            foreach (var c in Conditions)
                c.AppendTo(sb);
        }

        public override bool IsValid
        {
            get { return Conditions.All(x => x.IsValid); }
        }
    }
    public class SelectorList : Selector
    {
        public List<Selector> Selectors { get; set; }

        public override void AppendTo(StringBuilder sb)
        {
            bool notFirst=false;
            foreach (var c in Selectors)
            {
                if (notFirst)
                    sb.Append(',');
                notFirst = true;
                c.AppendTo(sb);
            }
        }

        public override bool IsValid
        {
            get { return Selectors.All(x => x.IsValid); }
        }
    }





    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================


    namespace Parsers
    {
        internal class SelectorParser : Parser<Selector>
        {
            internal override Selector DoParse()
            {
                if (End)
                    return null;
                var lst = new SelectorList
                {
                    Selectors = new List<Selector>(),
                };

                bool expectingNext = false;
                while (true)
                {
                    bool end = false;
                    switch (CurrentChar)
                    {

                        default:
                            var sel = DoParseOne();
                            if (sel == null)
                            {
                                // that's a warning
                                end = true;
                            }
                            else
                                lst.Selectors.Add(sel);
                            break;
                        case ';':
                        case '{':
                        case '}':
                        case ')':
                            // stoop
                            end = true;
                            break;
                    }

                    if (expectingNext && end)
                    {
                        // we've got a token after a ','
                        if (!End)
                            AddError(ErrorCode.UnexpectedToken, CurrentChar.ToString());
                    }

                    expectingNext = !End && CurrentChar == ',';
                    if (expectingNext)
                        Index++;

                    if (End || !expectingNext)
                        end = true;

                    if (end)
                    {
                        if (lst.Selectors.Count == 0)
                            return null;
                        if (lst.Selectors.Count == 1)
                            return lst.Selectors[0];
                        return lst;
                    }
                }
            }

            Selector DoParseOne()
            {
                if (End)
                    return null;

                // todo : check the difference between   "X::selector" and "X ::selector"  or "X :hover" and "X:hover" or "X [condition]" and "X[condition]" and fix accordingly
                //    => it has been assumed that they are the same... but i think i'm wrong

                var lst = new MultiConditionSelector
                {
                    Conditions = new List<Selector>(),
                };
                ResetWasWhitespace();

                while (true)
                {
                    ChildSelector chainWith = null;
                    bool end = false;
                    var pi = Index;


                    switch (CurrentChar)
                    {
                        // ============ group list ==============
                        // todo selector condtions
                        case '[':
                            lst.Conditions.Add(Parse<AttributeCondition>() ?? (Selector) Parse<InvalidSelector>());
                            break;
                        default:
                            if (WasWhitespace)
                            {
                                // aws a space before... that's still a chain
                                chainWith = Parse<ChildSelector>();
                                if (chainWith == null)
                                    throw new ParsingException("Expecting child selector"); // should never happen
                                break;
                            }
                            var sel = Parse<SimpleSelector>() ?? (Selector) Parse<InvalidSelector>();
                            lst.Conditions.Add(sel);
                            break;
                        // ========= separators ================
                        case '>':
                        case '~':
                        case '+':
                            chainWith = Parse<ChildSelector>();
                            if (chainWith == null)
                                throw new ParsingException("Expecting child selector"); // should never happen
                            break;
                        case ',':
                        case ';':
                        case '{':
                        case '}':
                        case ')':
                            // stooop ! (warn if ';')
                            end = true;
                            break;
                    }

                    // safety check
                    if (!end && pi == Index)
                    {
                        AddError(ErrorCode.ExpectingToken, "valid selector");
                        end = true;
                    }

                    if (chainWith != null)
                    {
                        if (lst.Conditions.Count > 0)
                        {
                            if (lst.Conditions.Count == 1)
                                chainWith.Parent = lst.Conditions[0];
                            else
                                chainWith.Parent = lst;
                        }
                        return chainWith;
                    }

                    if (End)
                        end = true;

                    if (end)
                    {
                        if (lst.Conditions.Count == 0)
                            return null;
                        if (lst.Conditions.Count == 1)
                            return lst.Conditions[0];
                        return lst;
                    }
                }
            }
        }
    }
}