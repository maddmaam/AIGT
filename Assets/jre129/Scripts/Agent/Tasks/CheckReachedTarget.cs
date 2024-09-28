using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.Pathfinding;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class CheckReachedTarget : TreeNode
    {
        #region Constant Fields

        private const float ArrivalDistance = 2f;

        #endregion
        
        #region Private Fields

        private GameObject _target;
        private Transform _agentTransform;
        private bool _reachedEndPath;

        #endregion

        public CheckReachedTarget(GameObject target, Transform agentTransform)
        {
            _target = target;
            _agentTransform = agentTransform;
        }

        public override TreeNodeState RunPhysics()
        {
            if (!TryGetData("_arrived", out var outputObject))
                State = TreeNodeState.Failure;
            _reachedEndPath = outputObject as bool? ?? false;
            if (Vector3.Distance(_agentTransform.position, _target.transform.position) < ArrivalDistance && _reachedEndPath)
            {
                State = TreeNodeState.Success;
                return State;
            }

            State = TreeNodeState.Failure;
            return State;
        }
    }
}