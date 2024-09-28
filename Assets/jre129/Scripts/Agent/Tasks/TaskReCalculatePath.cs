using System;
using System.Collections.Generic;
using jre129.Scripts.Agent.BehaviourTree;
using jre129.Scripts.Pathfinding;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskReCalculatePath : TreeNode
    {
        #region Public Properties

        private Algorithm RecalculationPathAlgorithm
        {
            get => _currentAlgorithm;
            set => _currentAlgorithm = value;
        }

        #endregion
        
        #region Private Fields

        private Transform _agentTransform;
        private List<PathNode> _path;
        private Algorithm _currentAlgorithm = Algorithm.AStarDiagonal;

        #endregion

        public TaskReCalculatePath(Transform transform)
        {
            _agentTransform = transform;
        }

        public override TreeNodeState RunPhysics()
        {
            Vector3 position = _agentTransform.position;
            PathNode startPosition = new PathNode((int)position.x, (int)position.z);
            if (GetData("_endNode") is not PathNode endNode)
                throw new NullReferenceException("endNode was Null. Have you set it in the roots data storage?");

            if (GetData("_graph") is not TerrainGraph terrainGraph)
                throw new NullReferenceException("terrainGraph was Null. Have you set it in the roots data storage?");

            _path = _currentAlgorithm switch
            {
                Algorithm.BreadthFirstSearch => PathAlgorithm.BFS(terrainGraph, startPosition, endNode),
                Algorithm.Dijkstra => PathAlgorithm.Dijkstra(terrainGraph, startPosition, endNode),
                Algorithm.AStar => PathAlgorithm.AStar(terrainGraph, startPosition, endNode, Algorithm.AStar),
                Algorithm.AStarEuclidean => PathAlgorithm.AStar(terrainGraph, startPosition, endNode, Algorithm.AStarEuclidean),
                Algorithm.AStarManhattan => PathAlgorithm.AStar(terrainGraph, startPosition, endNode, Algorithm.AStarManhattan),
                Algorithm.AStarDiagonal => PathAlgorithm.AStar(terrainGraph, startPosition, endNode, Algorithm.AStarDiagonal),
                Algorithm.AStarOctile => PathAlgorithm.AStar(terrainGraph, startPosition, endNode, Algorithm.AStarOctile),
                _ => throw new ArgumentOutOfRangeException(nameof(_currentAlgorithm),"Invalid Algorithm used for Recalculation of path")
            };
            SetData("_path", _path);
            State = TreeNodeState.Running;
            return State;
        }
    }
}