using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterExpressions : ParameterChunk
    {
        public short Comparison;
        public List<ParameterExpression> Expressions = new();

        public FrameEvents? FrameEvents;

        public ParameterExpressions()
        {
            ChunkName = "ParameterExpressions";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Comparison = reader.ReadShort();
            bool CheckInlinedLoop = false;
            
            while (true)
            {
                ParameterExpression newExpression = new ParameterExpression();
                newExpression.ReadCCN(reader);

                if (newExpression.ObjectType == 0 && newExpression.Num == 0)
                    break;
                
                if (NebulaCore.Build >= 296 && NebulaCore.Windows && NebulaCore.Fusion >= 2.5)
                {
                    if (CheckInlinedLoop)
                    {
                        CheckInlinedLoop = false;
                        if (newExpression.ObjectType == -1 && newExpression.Num == 0)
                        {
                            int loopId = ((ExpressionInt)newExpression.Expression).Value;
                            newExpression.Num = 3;
                            newExpression.Expression = new ExpressionString()
                            {
                                Value = "NebulaLoop#" + loopId
                            };
                        }
                    }
                    else if (newExpression.ObjectType == -1 && newExpression.Num == 46) // LoopIndex(
                        CheckInlinedLoop = true;
                }
                
                newExpression.Parent = Parent;
                Expressions.Add(newExpression);
            }
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteShort(Comparison);

            foreach (ParameterExpression expression in Expressions)
                expression.WriteMFA(writer);

            writer.WriteInt(0); // Final Expression
        }

        public override string ToString()
        {
            string str = string.Empty;
            for (int i = 0; i < Expressions.Count; i++)
            {
                ParameterExpression expression = Expressions[i];
                if (expression.IsExtensionExpression)
                    str += expression.ExtensionToString(HasParameters(i));
                else
                    str += expression.ToString();
            }
            return str.Trim();
        }

        // Does the extension expression at index i take parameters? Fusion stores them right after
        // the expression token, closed by a ")" token. After an expression without parameters, only
        // an operator, "," or ")" (or the end) can follow, so any other token starts a parameter.
        // The only ambiguous case is a parameter starting with a minus sign: the resolver's
        // parameter count decides then.
        bool HasParameters(int i)
        {
            if (i + 1 >= Expressions.Count)
                return false;
            ParameterExpression next = Expressions[i + 1];
            if (next.ObjectType == -1 && (next.Num == -3 || next.Num == -2)) // "," or ")"
                return false;
            if (next.ObjectType == 0) // operator
                return next.Num == 4 && Expressions[i].ExtensionParameterCount > 0; // " - "
            return true;
        }
    }
}
