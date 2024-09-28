using System;
using System.Collections.Generic;
using jre129.Scripts.Agent.Tasks;
using jre129.Scripts.HelperClasses;
using jre129.Scripts.Pathfinding;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace jre129.Scripts.Agent.BehaviourTree
{
    public class AgentBT : BehaviourTree
    {
        #region Constant Fields

        private const float MaxLinearVelocity = 10f;
        private const float MaxAngularVelocity = 7f;
        private const float MaxForce = 10f;
        private const float MaxTorque = 10f;
        private const float ArrivedThreshold = 0.1f;

        #endregion
        
        #region Serialized Fields

        [SerializeField] private GameObject target;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private Transform agentTransform;
        [SerializeField] private Algorithm pathAlgorithm;
        // [SerializeField] private PathRenderer pathRenderer;
        [SerializeField] private AudioClip honkSound;
        [SerializeField] private AudioSource audioSource;

        #endregion

        #region Private Fields

        private TerrainGraph _graph;
        private List<PathNode> _path;
        private PathNode _endNode;
        private bool _hasArrived;

        #endregion
        
        protected override TreeNode SetupTree()
        {
            ObstacleLogic.SetObstacles();
            if (target == null) target = GameObject.FindWithTag("goal");
            _path = CreatePath();
            SetupRigidBody();
            // TreeNode root = new TaskFollowPath(rigidBody, _path, agentTransform, target);
            Vector3 initialLookTarget =
                new Vector3(_path[0].NodePosition.x, 0, _path[0].NodePosition.y);
            TreeNode root = new Selector(new List<TreeNode>
            {
                new Selector(new List<TreeNode>
                {
                    new Sequence(new List<TreeNode>
                    {
                        new CheckReachedTarget(target, agentTransform),
                        new TaskArriveAtTarget(agentTransform, rigidBody, target)
                    }),
                    new Sequence(new List<TreeNode>
                    {
                        new CheckTargetHasMoved(_graph.grid, target),
                        new TaskReCalculatePath(agentTransform)
                    })
                }),
                new Sequence(new List<TreeNode>
                {
                    new Sequence(new List<TreeNode>
                    {
                        new CheckObstacleInRange(agentTransform, initialLookTarget),
                        new TaskAvoidCollision(rigidBody, agentTransform)
                    }),
                    new TaskPlayAudio(honkSound, audioSource)
                }),
                new Sequence(new List<TreeNode>
                {
                    new CheckObstacleAtFeet(agentTransform, rigidBody, initialLookTarget),
                    new TaskJumpOverObstacle(rigidBody, agentTransform)
                }),
                new Sequence(new List<TreeNode>
                {
                    new CheckCurrentlyOnPath(agentTransform, _path),
                    new TaskReCalculatePath(agentTransform)
                }),
                new TaskFollowPath(rigidBody, _path, agentTransform, target)
            });
            
            root.SetData("_endNode", _endNode);
            root.SetData("_graph", _graph);
            // pathRenderer.Path = _path;
            // pathRenderer.Graph = _graph;
            return root;
        }

        private List<PathNode> CreatePath()
        {
            Vector3 targetTransform = target.transform.position;
            Vector3 position = agentTransform.position;
            _graph = new TerrainGraph();
            PathNode startPathNode = _graph.grid[(int)position.x+1, (int)(position.z+1)];
            _endNode = _graph.grid[(int)targetTransform.x + 1, (int)targetTransform.z + 1];
            
            return pathAlgorithm switch
            {
                Algorithm.BreadthFirstSearch => PathAlgorithm.BFS(_graph, startPathNode, _endNode),
                Algorithm.Dijkstra => PathAlgorithm.Dijkstra(_graph, startPathNode, _endNode),
                Algorithm.AStar => PathAlgorithm.AStar(_graph, startPathNode, _endNode, Algorithm.AStar),
                Algorithm.AStarEuclidean => PathAlgorithm.AStar(_graph, startPathNode, _endNode,
                    Algorithm.AStarEuclidean),
                Algorithm.AStarManhattan => PathAlgorithm.AStar(_graph, startPathNode, _endNode,
                    Algorithm.AStarManhattan),
                Algorithm.AStarDiagonal => PathAlgorithm.AStar(_graph, startPathNode, _endNode,
                    Algorithm.AStarDiagonal),
                Algorithm.AStarOctile => PathAlgorithm.AStar(_graph, startPathNode, _endNode, Algorithm.AStarOctile),
                Algorithm.AStarCustom => PathAlgorithm.AStar(_graph, startPathNode, _endNode, Algorithm.AStarCustom),
                _ => throw new ArgumentOutOfRangeException(nameof(pathAlgorithm), "Algorithm is not a valid value!") // Default case
            };
            
        }

        private void SetupRigidBody()
        {
            rigidBody.maxLinearVelocity = MaxLinearVelocity;
            rigidBody.maxAngularVelocity = MaxAngularVelocity;
        }

        protected override void FixedUpdate()
        {
            Vector3 agentPosition = agentTransform.position;
            Vector3 targetPosition = target.transform.position;
            float xDistance = Mathf.Abs(agentPosition.x - targetPosition.x);
            float zDistance = Mathf.Abs(agentPosition.z - targetPosition.z);
            if ((xDistance < ArrivedThreshold && zDistance < ArrivedThreshold) || _hasArrived)
            {
                if (!_hasArrived)
                {
                    rigidBody.velocity = Vector3.zero;
                    _hasArrived = true;
                }
                return;
            }
            base.FixedUpdate();
        }

        protected override void Update()
        {
            Vector3 agentPosition = agentTransform.position;
            Vector3 targetPosition = target.transform.position;
            float xDistance = Mathf.Abs(agentPosition.x - targetPosition.x);
            float zDistance = Mathf.Abs(agentPosition.z - targetPosition.z);
            if ((xDistance < ArrivedThreshold && zDistance < ArrivedThreshold) || _hasArrived)
            {   // Check if we have reached the target - if we have return
                if (!_hasArrived)
                {
                    rigidBody.velocity = Vector3.zero;
                    _hasArrived = true;
                }
                return;
            }
            base.Update();
        }
    }
}