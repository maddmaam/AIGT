using System.Data;
using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.Pathfinding;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class CheckTargetHasMoved : TreeNode
    {
        #region Private Fields

        private PathNode[,] _terrainGrid;
        private GameObject _target;
        private Vector3 _targetPreviousPosition;

        #endregion

        public CheckTargetHasMoved(PathNode[,] terrainGrid, GameObject target)
        {
            _terrainGrid = terrainGrid;
            _target = target;
            _targetPreviousPosition = target.transform.position;
        }

        public override TreeNodeState RunPhysics()
        {
            if (!TryGetData("_arrived", out var outputObject))
            {
                State = TreeNodeState.Failure;
                return State;
            }

            Vector3 currentTargetPosition = _target.transform.position;
            bool reachedTarget = outputObject as bool? ?? false;
            if (reachedTarget && currentTargetPosition != _targetPreviousPosition)
            {
                PathNode newTargetNode = _terrainGrid[(int)currentTargetPosition.x, (int)currentTargetPosition.z];
                SetData("_endNode", newTargetNode);
                _targetPreviousPosition = currentTargetPosition;
                ClearData("_arrived");
                State = TreeNodeState.Success;
            }
            else
            {
                State = TreeNodeState.Failure;
            }

            return State;
        }
    }
}