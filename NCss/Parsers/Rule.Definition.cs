using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace NCss
{
    public abstract class Rule : CssBlockToken
    {
        internal Rule() { }

        public List<Property> Properties { get; set; }

        internal abstract BodyType ExpectedBodyType { get; }
        public List<Rule> ChildRules { get; set; }

        internal enum BodyType
        {
            None,
            Properties,
            ChildRules,
        }
    }

    internal class OrphanBlockRule : Rule
    {
        internal override void AppendTo(StringBuilder sb)
        {
        }
        
        public override bool IsValid
        {
            get { return false; }
        }

        internal override BodyType ExpectedBodyType
        {
            get { return BodyType.Properties; }
        }
    }

    internal class ClassRule : Rule
    {
        public override void AppendToWithOptions(StringBuilder sb, CssRestitution option)
        {
            if (Selector != null)
                Selector.AppendToWithOptions(sb, option);
            sb.Append('{');
            if (Properties != null)
            {
                foreach (var p in Properties)
                    p.AppendToWithOptions(sb, option);
            }
            sb.Append('}');
        }


        internal override void AppendTo(StringBuilder sb)
        {
            AppendToWithOptions(sb, CssRestitution.OnlyWhatYouUnderstood);
        }

        public override bool IsValid
        {
            get
            {
                if (Selector == null)
                    return false;
                if (!Selector.IsValid)
                    return false;
                if (Properties == null || Properties.Count == 0)
                    return true;
                var ret = Properties.All(p => p.IsValid);
                return ret;
            }
        }

        public Selector Selector { get; set; }

        internal override BodyType ExpectedBodyType
        {
            get { return BodyType.Properties; }
        }
    }

    public class DirectiveRule : Rule
    {

        internal override void AppendTo(StringBuilder sb)
        {
            if (Selector != null)
                Selector.AppendTo(sb);
            switch (ExpectedBodyType)
            {
                case BodyType.None:
                    sb.Append(';');
                    break;
                case BodyType.Properties:
                    sb.Append('{');
                    if (Properties != null)
                    {
                        foreach (var p in Properties)
                            p.AppendTo(sb);
                    }
                    sb.Append('}');
                    break;
                case BodyType.ChildRules:
                    sb.Append('{');
                    if (ChildRules != null)
                    {
                        foreach (var p in ChildRules)
                            p.AppendTo(sb);
                    }
                    sb.Append('}');
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool IsValid
        {
            get
            {
                if (Selector == null)
                    return false;
                if (!Selector.IsValid)
                    return false;
                switch (ExpectedBodyType)
                {
                    case BodyType.None:
                        return true;
                    case BodyType.Properties:
                        if (Properties == null || Properties.Count == 0)
                            return true;
                        return Properties.All(p => p.IsValid);
                    case BodyType.ChildRules:
                        if (ChildRules == null || ChildRules.Count == 0)
                            return true;
                        return ChildRules.All(x => x.IsValid);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public DirectiveSelector Selector { get; set; }

        internal override BodyType ExpectedBodyType
        {
            get { return Selector == null ? BodyType.None : Selector.ExpectedBodyType; }
        }
    }
    public class NotParsableBlockRule : Rule
    {

        internal override void AppendTo(StringBuilder sb)
        {
            // do nothing
        }

        public override bool IsValid
        {
            get { return false; }
        }

        internal override BodyType ExpectedBodyType
        {
            get { return BodyType.Properties; }
        }
    }
}