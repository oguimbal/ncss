using System;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{

    public class AttributeCondition : Selector
    {
        public enum Type
        {
            HavingAttribute,
            Equals,
            ContainsWord = '~',
            StartsWithDashSeparated = '|',
            StartsWith = '^',
            EndsWith = '$',
            Contains = '*',
        }

        public override void AppendTo(StringBuilder sb)
        {
            sb.Append('[');
            if (!string.IsNullOrWhiteSpace(InvalidCondition))
            {
                sb.Append(InvalidCondition);
                sb.Append(']');
                return;
            }

            if (!string.IsNullOrWhiteSpace(Attribute))
                sb.Append(Attribute);
            switch (ConditionType)
            {
                case Type.HavingAttribute:
                    sb.Append(']');
                    return;
                case Type.Equals:
                    break;
                default:
                    sb.Append((char) ConditionType);
                    break;
            }
            sb.Append('=');
            if (!String.IsNullOrWhiteSpace(Value))
                sb.Append(Value);
            sb.Append(']');
        }

        public override bool IsValid
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(InvalidCondition))
                    return false;
                return !string.IsNullOrWhiteSpace(Attribute) && Enum.IsDefined(typeof (Type), ConditionType);
            }
        }

        public string InvalidCondition { get; set; }
        public string Attribute { get; set; }
        public Type ConditionType { get; set; }
        public string Value { get; set; }
    }




    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================



    namespace Parsers
    {

        internal class AttributeConditionParser : Parser<AttributeCondition>
        {
            internal override AttributeCondition DoParse()
            {
                // http://www.w3schools.com/cssref/css_selectors.asp
                if (CurrentChar != '[')
                    return null;
                Index++; // skip '['

                var attr = PickName();
                if (attr == null)
                {
                    AddError(ErrorCode.ExpectingToken, "condition");
                    var cond = SkipTillEnd();
                    return new AttributeCondition
                    {
                        InvalidCondition = cond,
                    };
                }

                if (End || CurrentChar == ']')
                {
                    if (End)
                        AddError(ErrorCode.UnexpectedEnd, "]");
                    else
                        Index++; // skip ']'
                    return new AttributeCondition
                    {
                        Attribute = attr,
                        ConditionType = AttributeCondition.Type.HavingAttribute,
                    };
                }


                string sep;
                AttributeCondition.Type type;
                switch (CurrentChar)
                {
                    case '=':
                        type = AttributeCondition.Type.Equals;
                        sep = "=";
                        Index++; // skip the '='
                        break;
                    case '~':
                    case '|':
                    case '^':
                    case '$':
                    case '*':
                        if (NextChar != '=')
                        {
                            AddError(ErrorCode.ExpectingToken, "=");
                            return new AttributeCondition
                            {
                                InvalidCondition = attr + SkipTillEnd(),
                            };
                        }
                        type = (AttributeCondition.Type) CurrentChar;
                        sep = CurrentChar + "=";
                        Index += 2; // skip the token
                        break;
                    default:
                        AddError(ErrorCode.UnexpectedToken, CurrentChar.ToString());
                        return new AttributeCondition
                        {
                            InvalidCondition = attr + SkipTillEnd(),
                        };
                }

                var val = PickString();
                if (val == null)
                {
                    var ret = SkipTillEnd();
                    AddError(ErrorCode.ExpectingValue, ret);
                    return new AttributeCondition
                    {
                        InvalidCondition = attr + sep + SkipTillEnd()
                    };
                }

                if (End)
                {
                    AddError(ErrorCode.ExpectingToken, "]");
                }
                else if (CurrentChar != ']')
                {
                    AddError(ErrorCode.ExpectingToken, "]");
                    return new AttributeCondition
                    {
                        InvalidCondition = attr + sep + val + SkipTillEnd()
                    };
                }

                Index++; // skip ']'

                return new AttributeCondition
                {
                    Attribute = attr,
                    Value = val,
                    ConditionType = type,
                };
            }

            string SkipTillEnd()
            {
                var ret = SkipUntil(';', '{', '}', ']');
                if (!End && CurrentChar == ']')
                    Index++; // skip ']'
                return ret;
            }
        }
    }
}