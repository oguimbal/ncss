using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{
    public class Property : CssBlockToken
    {
        public string Name { get; set; }
        public List<CssValue> Values { get; set; }

        public override void AppendTo(StringBuilder sb)
        {
            if (HasStar)
                sb.Append('*');
            if (!string.IsNullOrWhiteSpace(Name))
            {
                sb.Append(Name).Append(":");

                if (Values != null)
                {
                    bool notFirst = false;
                    foreach (var v in Values)
                    {
                        if (notFirst)
                            sb.Append(' ');
                        notFirst = true;
                        v.AppendTo(sb);
                    }
                }
            }

            if (Important)
            {
                if (string.IsNullOrWhiteSpace(BangHackName))
                    sb.Append(" !important");
                else
                    sb.Append('!').Append(BangHackName);
            }

            if (HasSlash9)
                sb.Append("\\9");
            if (HasSlash0)
                sb.Append("\\0/");
            sb.Append(';');
        }

        public override bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    return false;
                if (Values == null || Values.Count == 0)
                    return false;
                if (HasSlash0 && HasSlash9)
                    return false;
                return Values.All(x => x.IsValid) && !Values.Last().HasComma;
            }
        }

        //http://stackoverflow.com/questions/814219/how-does-one-target-ie7-and-ie8-with-valid-css
        /// <summary>
        /// IE hack: Target only IE7 and below
        /// </summary>
        public bool HasStar { get; set; }
        /// <summary>
        /// IE hack: Target only IE8 and below using "property:value\9;"
        /// </summary>
        public bool HasSlash9 { get; set; }
        /// <summary>
        /// IE hack: Target IE8 only, using "property:value\0/;"
        /// </summary>
        public bool HasSlash0 { get; set; }


        public bool Important { get; set; }

        /// <summary>
        /// Only parsed successuflly by IE7 and below : "property:value!ie7;"
        /// </summary>
        public string BangHackName { get; set; }
    }

    public class NotParsableProperty : Property
    {
        public string InvalidPropertyContent { get; private set; }

        public NotParsableProperty(string invalid)
        {
            InvalidPropertyContent = invalid;
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
        internal class PropertyParser : Parser<Property>
        {
            internal override Property DoParse()
            {
                // =============== MUST NEVER RETURN NULL ==================

                if (CurrentChar == ';')
                {
                    Index++;
                    return new Property(); // empty property
                }

                //http://stackoverflow.com/a/1667560/919514
                bool starHack = CurrentChar == '*';
                if (starHack)
                    Index++;

                var name = PickName();
                if (name == null)
                    return new NotParsableProperty(SkipTillEnd());

                if (End)
                    return new NotParsableProperty(name);

                if (CurrentChar != ':')
                    return new NotParsableProperty(name + SkipTillEnd());

                Index++;

                CssValue v;
                var values = new List<CssValue>();
                while ((v = Parse<CssValue>()) != null)
                    values.Add(v);

                bool important = false;
                string bangHackName = null;
                if (!End && CurrentChar == '!')
                {
                    Index++;
                    var imp = PickName();
                    if (imp == null)
                        AddError(ErrorCode.ExpectingToken, "important");
                    else
                        important = true;

                    if(imp != "important")
                        bangHackName = imp;
                }

                bool hasSlash0 = false;
                var hasSlash9 = !End && CurrentChar == '\\' && NextChar == '9';
                if (hasSlash9)
                    Index += 2;
                else
                {
                    hasSlash0 = !End && CurrentChar == '\\' && NextChar == '0';
                    if (hasSlash0)
                    {
                        Index += 2;
                        if (End || CurrentChar != '/')
                            AddError(ErrorCode.ExpectingToken, "/");
                        else
                            Index++;
                    }
                }


                if (!End)
                {
                    if (CurrentChar != ';' && CurrentChar != '}')
                        AddError(ErrorCode.ExpectingToken, ";");
                    else if (CurrentChar == ';')
                        Index++;
                }

                // todo: parse values
                return new Property
                {
                    Name = name,
                    Values = values,
                    HasStar = starHack,
                    HasSlash9 = hasSlash9,
                    HasSlash0 = hasSlash0,
                    BangHackName = bangHackName,
                    Important = important,
                };
            }


            string SkipTillEnd()
            {
                var ret = SkipUntil(';', '}');
                if (!End && CurrentChar == ';')
                    Index++; // skip ';'
                return ret;
            }
        }
    }
}