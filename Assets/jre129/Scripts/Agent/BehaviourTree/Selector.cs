using System.Collections.Generic;

namespace jre129.Scripts.Agent.BehaviourTree
{
    public class Selector : TreeNode
    {
        public Selector() : base() {}
        public Selector(List<TreeNode> treeNode) : base(treeNode) {}
        
        public override TreeNodeState RunNode()
        {
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunNode())
                {
                    case TreeNodeState.Failure:
                        continue;
                    case TreeNodeState.Success:
                        State = TreeNodeState.Success;
                        return State;
                    case TreeNodeState.Running:
                        State = TreeNodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }

            State = TreeNodeState.Failure;
            return State;
        }

        public override TreeNodeState RunPhysics()
        {
            foreach (TreeNode treeNode in Children)
            {
                switch (treeNode.RunPhysics())
                {
                    case TreeNodeState.Failure:
                        continue;
                    case TreeNodeState.Success:
                        State = TreeNodeState.Success;
                        return State;
                    case TreeNodeState.Running:
                        State = TreeNodeState.Running;
                        return State;
                    default:
                        continue;
                }
            }

            State = TreeNodeState.Failure;
            return State;
        }
    }
}