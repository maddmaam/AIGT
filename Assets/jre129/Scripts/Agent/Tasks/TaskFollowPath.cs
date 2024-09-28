using System.Collections.Generic;
using System.Linq;
using jre129.Scripts.Pathfinding;
using jre129.Scripts.Agent.BehaviourTree;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskFollowPath : TreeNode
    {
        #region Constants

        private const float MaxLinearVelocity = 10f;
        private const float ChangePathThreshold = 2.25f;

        #endregion
        
        #region Private Fields

        private Rigidbody _agentBody;
        private List<PathNode> _path;
        private int _pathIndex = 0;
        private PathNode _currentTargetPathNode;
        private Vector3 _currentTargetPathNodePosition;
        private Transform _agentTransform;
        private GameObject _target;
        private int _lookIndex;

        #endregion

        public TaskFollowPath(Rigidbody rigidBody, List<PathNode> agentPath, Transform agentTransform, GameObject target)
        {
            _agentBody = rigidBody;
            _path = agentPath;
            _agentTransform = agentTransform;
            _target = target;
            _currentTargetPathNode = _path[_pathIndex++];
            _currentTargetPathNodePosition = NodeToVector3(_currentTargetPathNode);
            _lookIndex = 4;
        }
        
        public override TreeNodeState RunPhysics()
        {
            if (GetData("_path") is List<PathNode> newPath && !_path.SequenceEqual(newPath))
            {
                _path = newPath;
                _pathIndex = 0;
                _currentTargetPathNode = newPath[0];
                _currentTargetPathNodePosition = NodeToVector3(_currentTargetPathNode);
            }
            
            float distance = Vector3.Distance(_currentTargetPathNodePosition, _agentTransform.position);
            if (distance < ChangePathThreshold)
            {
                Vector3 targetPosition = _target.transform.position;
                if (_pathIndex >= _path.Count && _currentTargetPathNodePosition != targetPosition)
                {
                    _currentTargetPathNodePosition = _target.transform.position;
                    _currentTargetPathNodePosition.y = _agentTransform.position.y;
                    SetData("_arrived", true);
                } 
                else if (_currentTargetPathNodePosition == targetPosition)
                {
                    return TreeNodeState.Running; // Early Return as we reached end
                }
                else
                {
                    if (_lookIndex + 5 >= _path.Count)
                    {
                        _lookIndex = _path.Count - 1;
                    }
                    else if (_lookIndex - _pathIndex <= 1)
                    {
                        _lookIndex += 5;
                    }

                    Vector3 lookTargetPos = NodeToVector3(_path[_lookIndex]);
                    SetData("_targetNode", lookTargetPos);
                    _currentTargetPathNode = _path[_pathIndex++];
                    _currentTargetPathNodePosition = NodeToVector3(_currentTargetPathNode);
                }
                
            }
            // ApplyRotationTorque();
            
            Vector3 desiredVelocity = SeekForce();
            _agentBody.velocity = desiredVelocity * MaxLinearVelocity;
            _agentBody.angularVelocity = Vector3.zero;
            return TreeNodeState.Running;
        }
        
        private Vector3 SeekForce()
        {
            Vector3 desiredVelocity = Vector3.Normalize(_currentTargetPathNodePosition - _agentTransform.position) * MaxLinearVelocity;
            Vector3 steering = desiredVelocity - _agentBody.velocity;
            steering = Vector3.ClampMagnitude(steering, MaxLinearVelocity);
            Vector3 velocity = Vector3.ClampMagnitude(_agentBody.velocity + steering, MaxLinearVelocity);
            return velocity;
        }
        
        private Vector3 NodeToVector3(PathNode gridPathNode)
        {
            return new Vector3(_currentTargetPathNode.NodePosition.x,
                _currentTargetPathNode.NodeHeight, _currentTargetPathNode.NodePosition.y);
        }
    }
}