using System.Collections.Generic;
using System.Linq;
using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.Pathfinding;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class CheckCurrentlyOnPath : TreeNode
    {
        #region Private Fields

        private List<PathNode> _path;
        private Transform _agentTransform;

        #endregion

        public CheckCurrentlyOnPath(Transform transform, List<PathNode> initialPath)
        {
            _agentTransform = transform;
            _path = initialPath;
        }

        public override TreeNodeState RunPhysics()
        {
            if (GetData("_path") is List<PathNode> currentPath && !_path.SequenceEqual(currentPath))
            {
                _path = currentPath;
            }
            Vector3 position = _agentTransform.position;
            Vector2Int currentPosition = new Vector2Int((int)position.x, (int)position.z);
            PathNode currentPositionPathNode = new PathNode(currentPosition.x, currentPosition.y);
            if (_path.Contains(currentPositionPathNode) || GetData("deltaVelocity") == null)
            {
                State = TreeNodeState.Failure;
                return State;
            }
            State = TreeNodeState.Success;
            return State;
        }
    }
}