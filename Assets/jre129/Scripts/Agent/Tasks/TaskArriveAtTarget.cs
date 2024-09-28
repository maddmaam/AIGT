using jre129.Scripts.Agent.BehaviourTree;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskArriveAtTarget : TreeNode
    {
        #region Private Fields

        private Transform _agentTransform;
        private Rigidbody _agentRigidBody;
        private GameObject _target;

        #endregion

        public TaskArriveAtTarget(Transform agentTransform, Rigidbody agentRigidBody, GameObject target)
        {
            _agentTransform = agentTransform;
            _agentRigidBody = agentRigidBody;
            _target = target;
        }

        public override TreeNodeState RunPhysics()
        {
            float speed = Mathf.Lerp(_agentRigidBody.velocity.magnitude, 0,  Time.fixedDeltaTime * 100f);
            Vector3 targetPosition = _target.transform.position;
            Vector3 position = _agentTransform.position;
            targetPosition.y = position.y;
            Vector3 direction = targetPosition - position;
            _agentRigidBody.velocity = direction.normalized * speed;
            State = TreeNodeState.Running;
            return State;
        }
    }
}