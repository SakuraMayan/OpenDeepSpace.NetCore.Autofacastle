using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.NetCore.Autofacastle.AspectAttention
{
    /// <summary>
    /// 拦截节点构建
    /// </summary>
    public class InterceptNodeBuilder
    {
        private readonly LinkedList<InterceptNode> interceptNodes = new();

        /// <summary>
        /// 新增拦截器链
        /// </summary>
        /// <param name="Node"></param>
        public void Use(Func<InterceptContextDelegate, InterceptContextDelegate> Node)
        {
            var node = new InterceptNode
            {
                Node = Node
            };

            interceptNodes.AddLast(node);
        }

        /// <summary>
        /// 构建拦截器链
        /// </summary>
        /// <returns></returns>
        public InterceptContextDelegate Build()
        {
            var node = interceptNodes.Last;
            while (node != null)
            {
                node.Value.Next = GetNextFunc(node);
                node.Value.Process = node.Value.Node(node.Value.Next);
                node = node.Previous;
            }

            return interceptNodes.First.Value.Process;
        }

        /// <summary>
        /// 获取下一个
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static InterceptContextDelegate GetNextFunc(LinkedListNode<InterceptNode> node)
        {
            return node.Next == null ? ctx => Task.CompletedTask : node.Next.Value.Process;
        }
    }
}
