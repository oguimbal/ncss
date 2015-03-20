using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace NCss
{



    // ================================================================================================
    // ========================================= PARSER ===============================================
    // ================================================================================================


    namespace Parsers
    {
        internal class RuleParser : Parser<Rule>
        {
            internal override Rule DoParse()
            {
                // ============ MUST NEVER RETURN NULL ======================


                var lst = new List<Selector>();
                Rule rule;
                //var w = GetWord();
                switch (CurrentChar)
                {
                    case '@': // media query & other
                    {
                        // non cumulative
                        rule = new DirectiveRule
                        {
                            Selector = Parse<DirectiveSelector>(),
                        };
                        break;
                    }
                    case '{':
                        // meaningless block
                        AddError(ErrorCode.UnexpectedToken, "{");
                        rule = new OrphanBlockRule();
                        break;
                    case '}':
                        // unexpected block end
                        Index++; // will throw a block mismatch exception, and resume parsing after it.
                        throw new ParsingException("Unexpected token '}'"); // will never be thrown
                    default: // classic selector
                        rule = new ClassRule
                        {
                            Selector = Parse<Selector>() ?? Parse<InvalidSelector>(),
                        };
                        break;
                }

                if (End)
                {
                    AddError(ErrorCode.UnexpectedEnd, "end of rule");
                    return new ClassRule();
                }

                if (rule.ExpectedBodyType == Rule.BodyType.None)
                {
                    if (CurrentChar != ';')
                        AddError(ErrorCode.ExpectingToken, ";");
                    else
                        Index++;
                    return rule;
                }

                if (CurrentChar != '{')
                {
                    if (CurrentChar == ';')
                        Index++;
                    AddError(ErrorCode.ExpectingBody, "This rule requires a body");
                    return rule;
                }

                if (rule.ExpectedBodyType == Rule.BodyType.Properties)
                    rule.Properties = ParseBlock<Property>(true);
                else
                    rule.ChildRules = ParseBlock<Rule>(true);

                return rule;
            }
        }
    }
}