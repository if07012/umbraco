using System.Linq.Expressions;

namespace Voxteneo.Core.Domains.LambdaSqlBuilder.Resolver.ExpressionTree
{
    class SingleOperationNode : Node
    {
        public ExpressionType Operator { get; set; }
        public Node Child { get; set; }
    }
}
