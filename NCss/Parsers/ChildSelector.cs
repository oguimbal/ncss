using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{
    public class ChildSelector : Selector
    {
        public enum ChildType
        {
            FirstPredecesorOf = (int) '+',
            PredecesorOf = (int) '~',
            DirectChild = (int) '>',
            AnyChild = (int) ' ',
        }

        public ChildType Type { get; set; }

        public Selector Child { get; set; }
        public Selector Parent { get; set; }

        internal override void AppendTo(StringBuilder sb)
        {
            if (Parent != null)
                Parent.AppendTo(sb);
            sb.Append((char) Type);
            if (Child != null)
                Child.AppendTo(sb);
        }


        public override bool IsValid
        {
            get
            {
                if (Parent != null && !Parent.IsValid)
                    return false;
                if (Child != null && !Child.IsValid)
                    return false;
                return Parent != null || Child != null;
            }
        }
    }



    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================



    namespace Parsers
    {

        internal class ChildSelectorParser : Parser<ChildSelector>
        {
            internal override ChildSelector DoParse()
            {
                var isDef = Enum.IsDefined(typeof (ChildSelector.ChildType), (int) CurrentChar);
                if (!isDef && !WasWhitespace)
                    return null;
                var type = isDef ? (ChildSelector.ChildType) CurrentChar : ChildSelector.ChildType.AnyChild;

                if (isDef)
                    Index++; // skip '>'
                else
                    ResetWasWhitespace();

                if (End)
                {
                    AddError(ErrorCode.UnexpectedEnd, "selector");
                    return null;
                }

                var child = Parse<Selector>();
                if (child == null)
                {
                    AddError(ErrorCode.ExpectingToken, "valid child selector");
                    return null;
                }
                return new ChildSelector
                {
                    Type = type,
                    Child = child,
                };
            }

        }
    }
}