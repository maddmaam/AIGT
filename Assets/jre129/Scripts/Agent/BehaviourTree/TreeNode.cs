using System.Collections.Generic;
using UnityEngine.UIElements;

namespace jre129.Scripts.Agent.BehaviourTree
{
    public enum TreeNodeState
    {
        Running,
        Success,
        Failure
    }
    
    public class TreeNode
    {
        #region Public Fields
        public TreeNode Parent;
        #endregion

        #region Protected Fields
        protected TreeNodeState State;
        protected List<TreeNode> Children = new List<TreeNode>();
        #endregion

        #region Private Fields
        private Dictionary<string, object> _data = new Dictionary<string, object>();
        #endregion

        public TreeNode()
        {
            Parent = null;
        }

        public TreeNode(List<TreeNode> children)
        {
            foreach (TreeNode treeNode in children)
            {
                _Attach(treeNode);
            }
        }

        private void _Attach(TreeNode treeNode)
        {
            treeNode.Parent = this;
            Children.Add(treeNode);
        }

        public virtual TreeNodeState RunNode() => TreeNodeState.Failure;

        public virtual TreeNodeState RunPhysics() => TreeNodeState.Failure;

        public void SetData(string dataKey, object value)
        {
            TreeNode nodeParent = Parent;
            TreeNode currentNode = this;
            while (nodeParent != null)
            {
                currentNode = nodeParent;
                nodeParent = nodeParent.Parent;
            }

            currentNode._data[dataKey] = value;
        }

        public object GetData(string dataKey)
        {
            object value = null;
            if (_data.TryGetValue(dataKey, out value))
            {
                return value;
            }

            TreeNode treeNode = Parent;
            while (treeNode != null)
            {
                value = treeNode.GetData(dataKey);
                if (value != null)
                {
                    return value;
                }

                treeNode = treeNode.Parent;
            }

            return null;
        } 
        
        public bool ClearData(string dataKey)
        {
            bool removedData = false;
            if (_data.ContainsKey(dataKey))
            {
                removedData = _data.Remove(dataKey);
                return removedData;
            }

            TreeNode treeNode = Parent;
            while (treeNode != null)
            {
                removedData = treeNode.ClearData(dataKey);
                if (removedData)
                {
                    return removedData;
                }

                treeNode = treeNode.Parent;
            }

            return false;
        }

        public bool TryGetData(string dataKey, out object outputObject)
        {
            outputObject = GetData(dataKey);
            return outputObject != null;
        }
    }
}