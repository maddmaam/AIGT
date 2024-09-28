using System.Collections.Generic;
using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.HelperClasses;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class CheckObstacleInRange : TreeNode
    {
        #region Constants

        private const float MaxLinearVelocity = 10f;
        private const int MaxNumRayCasts = 5;
        private const int MaxNumNonAllocRayCasts = 10;
        private const float StraightRayCastDist = 2f;
        private const float AngleRayCastDist = 1f;
        
        #endregion
        
        #region Private Fields

        private Transform _agentTransform;
        private List<Ray> _rays;
        private float _angle = 90;
        private Vector3 _lookTarget;
        
        #endregion

        public CheckObstacleInRange(Transform transform, Vector3 lookTarget)
        {
            _agentTransform = transform;
            _rays = new List<Ray>(MaxNumNonAllocRayCasts);
            InitializeRays(ref _rays);
            _lookTarget = lookTarget;
            _lookTarget.y = 0;
        }

        private void InitializeRays(ref List<Ray> raysRef)
        {
            for (int rayIndex = 0; rayIndex < MaxNumNonAllocRayCasts; rayIndex++)
            {
                Ray newRay = new Ray(_agentTransform.position, _agentTransform.forward);
                raysRef.Add(newRay);
            }
        }

        public override TreeNodeState RunPhysics()
        {   // TODO: Improve This Algorithm when there are multiple collidable objects
            RaycastHit[] hitResults = new RaycastHit[5];
            int totalHitResults = 0;
            Vector3 deltaVelocity = Vector3.zero;
            object targetDataOutput;
            if (TryGetData("_targetNode", out targetDataOutput))
            {
                _lookTarget = targetDataOutput as Vector3? ?? default;
                _lookTarget.y = 0;
            }
            for (int rayIndex = 1; rayIndex < _rays.Count; ++rayIndex)
            {
                Quaternion rotation = _agentTransform.rotation;
                float angle = (rayIndex / (float)MaxNumNonAllocRayCasts - 1) * _angle * 2 - _angle;
                Quaternion deltaRotation = Quaternion.AngleAxis(
                    angle, _agentTransform.up);
                Vector3 direction = rotation * deltaRotation * -(_lookTarget - _agentTransform.position);
                direction.y = 0;

                float distance = Mathf.Abs(Mathf.Cos((360-Mathf.Abs(angle)) * Mathf.Deg2Rad) * StraightRayCastDist) 
                                 + Mathf.Abs(Mathf.Sin((360 - Mathf.Abs(angle)) * Mathf.Deg2Rad) * AngleRayCastDist);
                
                Ray currentRay = _rays[rayIndex];
                currentRay.origin = _agentTransform.position;
                currentRay.direction = direction;
                
                Debug.DrawLine(currentRay.origin, currentRay.direction.normalized * distance + currentRay.origin, Color.blue);
                int numHitResults = Physics.RaycastNonAlloc(currentRay, hitResults, distance);
                if (numHitResults > 0)
                {
                    RaycastHit closestHit = RayCastHelper.GetClosestHit(hitResults);
                    if (!(closestHit.collider.CompareTag("Obstacle") || closestHit.collider.CompareTag("Agent")))
                    {
                        continue;
                    }
                    if (closestHit.collider.CompareTag("Agent")) SetData("_collisionTag", "Agent");
                    deltaVelocity -= direction * ((StraightRayCastDist / MaxNumNonAllocRayCasts + AngleRayCastDist) * MaxLinearVelocity);
                }
                else
                {
                    deltaVelocity += (StraightRayCastDist / MaxNumNonAllocRayCasts + AngleRayCastDist) * MaxLinearVelocity * direction;
                }

                totalHitResults += numHitResults;
            }
            SetData("deltaVelocity", deltaVelocity);
            State =  totalHitResults > 0 ? TreeNodeState.Success : TreeNodeState.Failure;
            if (State == TreeNodeState.Failure)
            {
                ClearData("deltaVelocity");
            }
            return State;
        }
    }
}