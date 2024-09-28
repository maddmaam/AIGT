using System.Collections.Generic;

namespace jre129.Scripts.Agent.BehaviourTree
{
    public class Sequence : TreeNode
    {
        public Sequence() : base() {}
        public Sequence(List<TreeNode> treeNode) : base(treeNode) {}
        
        public override TreeNodeState RunNode()
        {
            bool childExecuting = false;
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunNode())
                {
                    case TreeNodeState.Failure:
                        State = TreeNodeState.Failure;
                        return State;
                    case TreeNodeState.Success:
                        continue;
                    case TreeNodeState.Running:
                        childExecuting = true;
                        continue;
                    default:
                        State = TreeNodeState.Success;
                        return State;
                }
            }
            
            State = childExecuting ? TreeNodeState.Running : TreeNodeState.Success;
            return State;
        }

        public override TreeNodeState RunPhysics()
        {
            bool childExecuting = false;
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunPhysics())
                {
                    case TreeNodeState.Failure:
                        State = TreeNodeState.Failure;
                        return State;
                    case TreeNodeState.Success:
                        continue;
                    case TreeNodeState.Running:
                        childExecuting = true;
                        continue;
                    default:
                        State = TreeNodeState.Success;
                        return State;
                }
            }
            
            State = childExecuting ? TreeNodeState.Running : TreeNodeState.Success;
            return State;
        }
    }
}