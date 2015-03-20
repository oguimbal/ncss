using System.Diagnostics;
using System.Text;

namespace NCss
{
    public abstract class CssBlockToken : CssToken
    {
        protected internal CssBlockToken()
        {
        }
    }


    /// <summary>
    /// Base class for all CSS items
    /// </summary>
    public abstract class CssToken
    {
        protected internal CssToken()
        {
        }

        string globalCss;
        int fromIndex;
        int toIndex;

        [DebuggerStepThrough]
        internal void SetParsingSource(string globalCss, int fromIndex, int toIndex)
        {
            this.globalCss = globalCss;
            this.fromIndex = fromIndex;
            this.toIndex = toIndex;
        }

        /// <summary>
        /// If parsed from raw CSS string, this returns the original string from which this item has been parsed.
        /// </summary>
        public string OriginalToken
        {
            get
            {
                if (globalCss == null)
                    return null;
                return globalCss.Substring(fromIndex, toIndex - fromIndex);
            }
        }

        public abstract void AppendTo(StringBuilder sb);
        public abstract bool IsValid { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }
    }
}