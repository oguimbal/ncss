using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace NCss
{
    /// <summary>
    /// A simple css value, or a function call
    /// </summary>
    public class CssSimpleValue : CssValue
    {
        public string Value { get; set; }

        /// <summary>
        /// Provided if that's a function call
        /// </summary>
        public List<CssValue> Arguments { get; set; }

        public override void AppendTo(StringBuilder sb)
        {
            if (HasParenthesis)
                sb.Append('(');
            if (!string.IsNullOrWhiteSpace(Value))
                sb.Append(Value);
            if (Arguments != null)
            {
                sb.Append('(');
                bool appendSpace = false;
                foreach (var a in Arguments)
                {
                    if (appendSpace)
                        sb.Append(' ');
                    a.AppendTo(sb);
                    appendSpace = !a.HasComma;
                }
                    
                sb.Append(')');
            }
            if (HasComma)
                sb.Append(',');
            if (HasParenthesis)
                sb.Append(')');
        }

        public override bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Value))
                    return false;
                if (Arguments != null)
                {
                    // check that it is a function name
                    if (Value.StartsWith("progid:"))
                    {
                        if (!Regex.IsMatch(Value, @"^progid:[a-zA-Z_\-][a-zA-Z0-9_\-\.]*$"))
                            return false;
                    }
                    else if (!Regex.IsMatch(Value, @"^[a-zA-Z_\-][a-zA-Z0-9_\-]*$"))
                        return false;
                    return Arguments.All(a => a.IsValid);
                }

                if (Value[0] == '#')
                {
                    // is valid color ?
                    return Regex.IsMatch(Value, @"^#[a-fA-F0-9]{3,6}$");
                }

                // is valid string ?
                if (Value[0] == '\'')
                    return Value.Length > 1 && Value[Value.Length - 1] == '\'';
                if (Value[0] == '"')
                    return Value.Length > 1 && Value[Value.Length - 1] == '"';

                return true;
            }
        }
    }



    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================


    namespace Parsers
    {
        internal class CssSimpleValueParser : Parser<CssSimpleValue>
        {
            /// <summary>
            /// A "value" is a simple value, or a function call. It MAY end with a comma.
            /// </summary>
            internal override CssSimpleValue DoParse()
            {
                var value = PickValue();
                if (value == null)
                    return null;
                if (value == "progid" && !End && CurrentChar == ':')
                {
                    // ie is a shit
                    var fun = SkipUntil('(', ';', '}');
                    value = value + fun;
                }

                if (End)
                    return new CssSimpleValue {Value = value,};

                List<CssValue> argument = null;

                #region We've got a function call !

                if (CurrentChar == '(')
                {
                    Index++;
                    if (value.ToLower() == "url")
                    {
                        // url function is a bit special, since it can host data that may have ';' inside.
                        // it always contains raw values without line breaks, though.. let's not parse it's argument and read it from raw.
                        argument = new List<CssValue>
                        {
                            new CssSimpleValue
                            {
                                Value = SkipUntil(')', '\r', '\n', '}')
                            }
                        };
                    }
                    else
                    {
                        // Parse function arguments
                        argument = new List<CssValue>();
                        while (!End)
                        {
                            var na = Parse<CssValue>();
                            if (na == null)
                            {
                                AddError(ErrorCode.ExpectingToken, "argument");
                                break;
                            }
                            argument.Add(na);

                            if (!End && CurrentChar == ')')
                                break;
                        }
                    }

                    if (End)
                        AddError(ErrorCode.UnexpectedEnd, ")");
                    else if (CurrentChar != ')')
                        AddError(ErrorCode.ExpectingToken, ")");
                    else
                        Index++;
                }

                #endregion

                // is this value ending with a comma ?
                bool hasComma = !End && CurrentChar == ',';
                if (hasComma)
                    Index++;


                return new CssSimpleValue
                {
                    Value = value,
                    HasComma = hasComma,
                    Arguments = argument,
                };
            }


        }

    }
}