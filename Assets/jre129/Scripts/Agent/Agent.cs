using System;
using System.Collections.Generic;
using UnityEngine;
using jre129.Scripts.Pathfinding;

namespace jre129.Scripts.Agent
{
    public struct PathSegment
    {
        private PathNode _startPathNode;
        private PathNode _endPathNode;

        public PathNode EndPathNode
        {
            get => _endPathNode;
            set => _endPathNode = value;
        }

        public PathNode StartPathNode
        {
            get => _startPathNode;
            set => _startPathNode = value;
        }
        
        public PathSegment(PathNode startPathNode, PathNode endPathNode)
        {
            _startPathNode = startPathNode;
            _endPathNode = endPathNode;
        }
    }
    
    public enum Algorithm
    {
        BreadthFirstSearch,
        Dijkstra,
        AStar,
        AStarEuclidean,
        AStarManhattan,
        AStarDiagonal,
        AStarOctile,
        AStarCustom
    }
    
    [Obsolete("Replaced with AgentBT. This class will continue to function, but will not be supported nor " +
              "will it receive updates")]
    public class Agent : MonoBehaviour
    {
        // Serialized Fields:
        [SerializeField] private GameObject target;
        [SerializeField] private Algorithm pathAlgorithm;
        [SerializeField] private PathRenderer pathRenderer;
        
        // Private Fields:
        private Rigidbody _agentBody;
        private TerrainGraph _graph;
        private List<PathNode> _path;
        private List<PathSegment> _pathSegments;
        
        private int _pathIndex = 0;
        private PathNode _currentTargetPathNode;
        private Vector3 _currentTargetNodePosition;
        private int _numPathSegments;
        
        private bool _isAvoiding = false;
        private float _movementAvoidMultiplier;
        private Vector3 _desiredVelocity;
        
        // Const Fields
        private const float FinishedThreshold = 0.25f;
        private const float MaxAngularVelocity = 7f;
        private const float MaxRayCastLength = float.MaxValue;
        private const int MaxNumberRayCasts = 5;
        private const float MaxForce = 10f;
        private const float MaxTorque = 10f;
        private const float RotationThreshold = 0.5f;
        private const float ConeSensorMultiplier = 0.5f;
        private const float ConeSensorAngleDegrees = 30f;
        private const float StraightSensorMultiplier = 1f;
        private const float MaxLinearVelocity = 10f;
        private const float ChangePathThreshold = 1.75f;
        
        // Readonly Fields:
        private readonly Vector3 _sensorStartPosition = new Vector3(0, 0.2f, 0.5f);

        // TODO: Add Rotation 
        // TODO: Add Collision Avoidance
        // TODO: Add Behaviour Trees
        
        // Start is called before the first frame update
        private void Start()
        {
            if (target == null) target = GameObject.FindWithTag("Finish Point");
            Vector3 agentTransform = transform.position;
            Vector3 targetTransform = target.transform.position;
            PathNode startPathNode = new PathNode((int)agentTransform.x, (int)agentTransform.z);
            startPathNode.NodeHeight = agentTransform.y;
            _graph = new TerrainGraph();
            PathNode endPathNode = _graph.grid[(int)targetTransform.x + 1, (int)targetTransform.z + 1];
            _agentBody = GetComponent<Rigidbody>();
            _path = pathAlgorithm switch
            {
                Algorithm.BreadthFirstSearch => PathAlgorithm.BFS(_graph, startPathNode, endPathNode),
                Algorithm.Dijkstra => PathAlgorithm.Dijkstra(_graph, startPathNode, endPathNode),
                Algorithm.AStar => PathAlgorithm.AStar(_graph, startPathNode, endPathNode, Algorithm.AStar),
                Algorithm.AStarEuclidean => PathAlgorithm.AStar(_graph, startPathNode, endPathNode,
                    Algorithm.AStarEuclidean),
                Algorithm.AStarManhattan => PathAlgorithm.AStar(_graph, startPathNode, endPathNode,
                    Algorithm.AStarManhattan),
                Algorithm.AStarDiagonal => PathAlgorithm.AStar(_graph, startPathNode, endPathNode,
                    Algorithm.AStarDiagonal),
                Algorithm.AStarOctile => PathAlgorithm.AStar(_graph, startPathNode, endPathNode, Algorithm.AStarOctile),
                _ => throw new ArgumentOutOfRangeException() // Default case
            };

            _numPathSegments = SplitPath();
            _agentBody.maxLinearVelocity = MaxLinearVelocity;
            _agentBody.maxAngularVelocity = MaxAngularVelocity;
            
            _currentTargetPathNode = _path[_pathIndex++]; 
            
            _currentTargetNodePosition = new Vector3(_currentTargetPathNode.NodePosition.x,
                _currentTargetPathNode.NodeHeight, _currentTargetPathNode.NodePosition.y);
            _agentBody.velocity = (_currentTargetNodePosition - transform.position) * MaxLinearVelocity;
            _movementAvoidMultiplier = 0f;
            // _raySensors = new List<Ray>();
            // SensorInitialization();
            
            if (pathRenderer == null) return;
            pathRenderer.Path = _path;
            pathRenderer.Graph = _graph;
            
        }

        private void FixedUpdate()
        {
            float distance = Vector3.Distance(_currentTargetNodePosition, transform.position);
            if (distance < ChangePathThreshold)
            {
                if (_pathIndex >= _path.Count)
                {
                    _agentBody.velocity = Vector3.zero;
                    return;
                }
                _currentTargetPathNode = _path[_pathIndex++];
                _currentTargetNodePosition = NodeToVector3(_currentTargetPathNode);
            }
            ApplyMovement();
        }

        private Vector3 NodeToVector3(PathNode gridPathNode)
        {
            return new Vector3(_currentTargetPathNode.NodePosition.x,
                _currentTargetPathNode.NodeHeight, _currentTargetPathNode.NodePosition.y);
        }

        private Vector3 SeekForce()
        {
            Vector3 desiredVelocity = Vector3.Normalize(_currentTargetNodePosition - transform.position) * MaxLinearVelocity;
            Vector3 steering = desiredVelocity - _agentBody.velocity;
            steering = Vector3.ClampMagnitude(steering, MaxLinearVelocity);
            // steering /= _agentBody.mass;
            Vector3 velocity = Vector3.ClampMagnitude(_agentBody.velocity + steering, MaxLinearVelocity);
            return velocity;
        }

        private void ApplyMovement()
        {
            _desiredVelocity = SeekForce();
            _agentBody.velocity = _desiredVelocity * MaxLinearVelocity;
        }
        /// <summary>
        /// Attempts to split the path into segments for ray firing to check for collisions
        /// </summary>
        /// <returns>The number of segments the path is split into</returns>
        private int SplitPath()
        {
            PathNode startPathNode = _path[0];
            PathNode endPathNode;
            Vector3 startNodePosition =
                new Vector3(startPathNode.NodePosition.x, startPathNode.NodeHeight, startPathNode.NodePosition.y);
            PathNode previousPathNode = startPathNode;
            foreach (PathNode node in _path)
            {
                Vector3 previousNodePosition3D = new Vector3(previousPathNode.NodePosition.x, previousPathNode.NodeHeight, previousPathNode.NodePosition.y);
                Vector3 nodePosition3D = new Vector3(node.NodePosition.x, node.NodeHeight, node.NodePosition.y);
                if (Mathf.Abs(Vector3.SignedAngle(previousNodePosition3D, nodePosition3D, Vector3.forward)) > 7.5f ||
                    Mathf.Abs(Vector3.SignedAngle(startNodePosition, nodePosition3D, Vector3.up)) > 15f)
                {
                    PathSegment newSegment = new PathSegment(startPathNode, previousPathNode);
                    _pathSegments.Add(newSegment);
                }
                previousPathNode = node;
            }
            return _pathSegments.Count;
        }
    }
}
