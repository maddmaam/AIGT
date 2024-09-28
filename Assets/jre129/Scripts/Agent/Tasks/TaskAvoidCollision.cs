using jre129.Scripts.Agent.BehaviourTree;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskAvoidCollision : TreeNode
    {
        #region Constant Fields

        private const float MaxLinearVelocity = 10f;

        #endregion
        
        #region Private Fields

        private Rigidbody _agentBody;
        private Transform _agentTransform;

        #endregion

        public TaskAvoidCollision(Rigidbody rigidBody, Transform transform)
        {
            _agentBody = rigidBody;
            _agentTransform = transform;
        }

        public override TreeNodeState RunPhysics()
        {   
            Vector3 deltaVelocity = (Vector3)GetData("deltaVelocity");
            _agentBody.velocity += deltaVelocity;
            _agentBody.velocity = Vector3.ClampMagnitude(_agentBody.velocity * MaxLinearVelocity, MaxLinearVelocity);
            _agentBody.angularVelocity = Vector3.zero;
            SetData("avoiding", true);
            State= TreeNodeState.Running;
            return State;
        }
    }
}