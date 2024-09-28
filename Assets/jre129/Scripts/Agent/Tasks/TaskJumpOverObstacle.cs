using jre129.Scripts.Agent.BehaviourTree;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskJumpOverObstacle : TreeNode
    {
        #region Constant Fields

        private const float MaxForce = 10f;

        #endregion
        
        #region Private Fields

        private Rigidbody _agentBody;
        private Transform _agentTransform;

        #endregion

        public TaskJumpOverObstacle(Rigidbody agentRigidBody, Transform agentTransform)
        {
            _agentBody = agentRigidBody;
            _agentTransform = agentTransform;
        }

        public override TreeNodeState RunPhysics()
        {
            Vector3 jumpForce = Vector3.up * MaxForce;
            _agentBody.AddForce(jumpForce, ForceMode.VelocityChange);
            State = TreeNodeState.Running;
            return State;
        }
    }
}