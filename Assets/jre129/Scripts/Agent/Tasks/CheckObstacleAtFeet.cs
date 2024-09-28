using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.HelperClasses;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class CheckObstacleAtFeet : TreeNode
    {
        #region Constant Fields

        private const float StraightRayCastDist = 2f;

        #endregion
        
        #region Private Fields

        private Rigidbody _agentBody;
        private Transform _agentTransform;
        private int _obstacleLayerMask = -1;
        private Vector3 _lookTarget;

        #endregion

        public CheckObstacleAtFeet(Transform agentTransform, Rigidbody agentRigidBody, Vector3 initialLookTarget)
        {
            _agentTransform = agentTransform;
            _agentBody = agentRigidBody;
            _lookTarget = initialLookTarget;
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            if (obstacleLayer != -1)
            {
                _obstacleLayerMask = 1 << obstacleLayer;
            }
        }

        public override TreeNodeState RunPhysics()
        {
            if (TryGetData("_targetNode", out var targetDataOutput))
            {
                _lookTarget = targetDataOutput as Vector3? ?? default;
            }

            bool obstacleDetected = ObstacleAtFeet();
            State = obstacleDetected ? TreeNodeState.Success : TreeNodeState.Failure;
            return State;
        }

        private bool ObstacleAtFeet()
        {
            bool objectAtFeet = false;
            RaycastHit[] hitResults = new RaycastHit[5];
            RaycastHit closestHit;
            Vector3 rayOrigin = _agentTransform.position;
            rayOrigin.y -= 0.75f;
            _lookTarget.y = rayOrigin.y;
            Vector3 direction = _lookTarget - rayOrigin;
            int numHitResults;
            if (_obstacleLayerMask != -1)
            {
                numHitResults = Physics.RaycastNonAlloc(rayOrigin, direction, hitResults, StraightRayCastDist,
                    _obstacleLayerMask);
                Debug.DrawLine(rayOrigin, rayOrigin + direction.normalized * StraightRayCastDist, Color.blue);
            }
            else
            {
                numHitResults = Physics.RaycastNonAlloc(rayOrigin, direction, hitResults, StraightRayCastDist);
                Debug.DrawLine(rayOrigin, rayOrigin + direction.normalized * StraightRayCastDist, Color.blue);
            }
            
            if (numHitResults > 0)
            {
                closestHit = RayCastHelper.GetClosestHit(hitResults);
                if (closestHit.collider.CompareTag("Obstacle"))
                    objectAtFeet = true;
            }

            return objectAtFeet;
        }
    }
}