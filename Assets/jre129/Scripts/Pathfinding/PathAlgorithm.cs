using System.Collections.Generic;
using System.IO;
using jre129.Scripts.Agent;
using Priority_Queue;
using UnityEngine;

namespace jre129.Scripts.Pathfinding
{
    using Vector2 = Vector2;

    public static class PathAlgorithm
    {
        
        public static float TotalCost;

        private static readonly Vector3 Gravity = new Vector3(0, -9.81f, 0);

        // This function will return a list of nodes generated by BFS algorithm.
        // This assumes the graph is directed and unweighted meaning no costs.
        // BFS will check all nodes before returning a path
        public static List<PathNode> BFS(TerrainGraph tGraph, PathNode startPathNode, PathNode endPathNode)
        {
            // A list of nodes that denotes the path
            List<PathNode> path = new List<PathNode>();

            // Declare data structures to use
            Dictionary<PathNode, PathNode> visitedNodes = new Dictionary<PathNode, PathNode>();
            Queue<PathNode> nodesToVisit = new Queue<PathNode>();

            // START OF ALGORITHM IMPLEMENTATION

            // TODO: Queue to visit the starting node
            nodesToVisit.Enqueue(startPathNode);

            // Sets where we came from, since we start from startNode, we also came from startNode
            // The fist startNode defines the current node, the second startNode defines the source node
            // node1->node2 or visitedNodes[node2] = node1
            visitedNodes.Add(startPathNode, startPathNode);

            PathNode currentPathNode = new PathNode(0, 0);

            // Iterate through the rest of the nodes
            while (nodesToVisit.Count > 0)
            {
                // TODO: Visit the next node in the queue
                currentPathNode = nodesToVisit.Dequeue();

                if (currentPathNode == endPathNode)
                {
                    break; // Already reached the end node, exit the loop
                }

                // TODO: Check if each of the current node's neighbours have been visited
                // If the neighbour hasn't been visited before, then queue it for visitation
                // After which, set the neighbour's source node to the current node, making a connection between them
                foreach (PathNode adjacentNode in tGraph.GetNeighbours(currentPathNode))
                {
                    if (!visitedNodes.ContainsKey(adjacentNode))
                    {
                        nodesToVisit.Enqueue(adjacentNode);
                        visitedNodes.Add(adjacentNode, currentPathNode);
                    }
                }
            }

            // TODO: Construct the path, follow back the visited nodes where connections were made
            // Iterate through all visited nodes until you reach the start node working on your way back
            // Add the visited nodes to the provided path list
            while (currentPathNode != startPathNode)
            {
                // Code goes here...
                currentPathNode = visitedNodes[currentPathNode];
                path.Add(currentPathNode);
            }
            path.Reverse();
            return path;
        }

        // This function will return a list of nodes generated by Dijkstra algorithm
        // This assumes the graph is directed and weighted meaning there are costs
        public static List<PathNode> Dijkstra(TerrainGraph tGraph, PathNode startPathNode, PathNode endPathNode)
        {
            // A list of nodes that denotes the path
            List<PathNode> path = new List<PathNode>();
            TotalCost = 0; // For updating canvas info

            // Declare data structures to use
            Dictionary<PathNode, PathNode> visitedNodes = new Dictionary<PathNode, PathNode>();
            Dictionary<PathNode, float> accumulatedCost = new Dictionary<PathNode, float>();
            SimplePriorityQueue<PathNode> nodesToVisit = new SimplePriorityQueue<PathNode>();

            // START OF ALGORITHM IMPLEMENTATION

            // TODO: Queue to visit the starting node with a cost/priority of 0
            nodesToVisit.Enqueue(startPathNode, 0);

            // Sets where we came from, since we start from startNode, we also came from startNode
            // The fist startNode defines the current node, the second startNode defines the source node
            // node1->node2 or visitedNodes[node2] = node1
            // Accumulated cost for startNode is 0
            visitedNodes.Add(startPathNode, startPathNode);
            accumulatedCost.Add(startPathNode, 0);

            PathNode currentPathNode = new PathNode(0, 0);

            // Iterate through all the nodes
            while (nodesToVisit.Count > 0)
            {
                // TODO: Visit the next node in the queue
                currentPathNode = nodesToVisit.Dequeue();

                if (currentPathNode == endPathNode)
                {
                    break; // Already reached the end node, exit the loop
                }

                // Check if each of the current node's neighbours have been visited
                foreach (PathNode adjacentNode in tGraph.GetNeighbours(currentPathNode))
                {
                    if (!visitedNodes.ContainsKey(adjacentNode))
                    {
                        // TODO: If the neighbour has not been visited before, then calculate an estimated new cost by
                        // adding the accumulated cost of the current node and the next minimum cost of the current node (see TerrainGraph)
                        float newCost = 0;
                        accumulatedCost.TryGetValue(currentPathNode, out newCost);
                        newCost += tGraph.NextMinimumCost(currentPathNode);
                        
                        // Now, check if the cost of the neighbouring node has been accounted for already or
                        // if the new estimated cost is smaller than the current cost of the neighbouring node. 
                        if (!accumulatedCost.ContainsKey(adjacentNode) || newCost < accumulatedCost[adjacentNode])
                        {
                            // TODO: The accumulated cost of the neighbouring node will take the new estimated cost
                            accumulatedCost[adjacentNode] = newCost;

                            // The neighbouring node has now been processed. Now, make a connection between the adjacent and current node
                            visitedNodes[adjacentNode] = currentPathNode;

                            // Enqueue the next node to visit with its cost. The priority queue will always dequeue first the node that has the smallest cost / highest priority.
                            float priority = newCost;
                            nodesToVisit.Enqueue(adjacentNode, priority);

                            TotalCost = +newCost;
                        }
                    }
                }
            }

            // TODO: Construct the path, follow back the visited nodes where connections were made
            // Iterate through all visited nodes until you reach the start node working on your way back
            // Add the visited nodes to the provided path list
            while (currentPathNode != startPathNode)
            {
                currentPathNode = visitedNodes[currentPathNode];
                path.Add(currentPathNode);
            }
            path.Reverse();
            return path;
        }

        // This function will return a list of nodes generated by A* algorithm
        // This assumes the graph is directed and weighted meaning there are costs
        public static List<PathNode> AStar(TerrainGraph tGraph, PathNode startPathNode, PathNode endPathNode, Algorithm mode)
        {
            // A list of nodes that denotes the path
            List<PathNode> path = new List<PathNode>();
            TotalCost = 0; // For updating canvas info

            // Declare data structures to use
            Dictionary<PathNode, PathNode> visitedNodes = new Dictionary<PathNode, PathNode>();
            Dictionary<PathNode, float> accumulatedCost = new Dictionary<PathNode, float>();
            SimplePriorityQueue<PathNode> nodesToVisit = new SimplePriorityQueue<PathNode>();

            // START OF ALGORITHM IMPLEMENTATION

            // TODO: Queue to visit the starting node with a cost/priority of 0
            nodesToVisit.Enqueue(startPathNode, 0);

            // Sets where we came from, since we start from startNode, we also came from startNode
            // The fist startNode defines the current node, the second startNode defines the source node
            // node1->node2 or visitedNodes[node2] = node1
            // Accumulated cost for startNode is 0
            visitedNodes.Add(startPathNode, startPathNode);
            accumulatedCost.Add(startPathNode, 0);

            PathNode currentPathNode = new PathNode(0, 0);

            // Iterate through all the nodes
            while (nodesToVisit.Count > 0)
            {
                // TODO: Visit the next node in the queue
                currentPathNode = nodesToVisit.Dequeue();

                if (currentPathNode == endPathNode)
                {
                    break; // Already reached the end node, exit the loop
                }

                // Check if each of the current node's neighbours have been visited
                foreach (PathNode adjacentNode in tGraph.GetNeighbours(currentPathNode))
                {
                    if (!visitedNodes.ContainsKey(adjacentNode))
                    {
                        // TODO: If the neighbour has not been visited before, then calculate an estimated new cost by
                        // adding the accumulated cost of the current node and the next minimum cost of the current node (see TerrainGraph)
                        float newCost = 0;
                        accumulatedCost.TryGetValue(currentPathNode, out newCost);
                        newCost += tGraph.NextMinimumCost(currentPathNode);

                        // Now, check if the cost of the neighbouring node has been accounted for already or
                        // if the new estimated cost is smaller than the current cost of the neighbouring node. 
                        if (!accumulatedCost.ContainsKey(adjacentNode) || newCost < accumulatedCost[adjacentNode])
                        {
                            // TODO: The accumulated cost of the neighbouring node will take the new estimated cost
                            accumulatedCost[adjacentNode] = newCost;

                            // The neighbouring node has now been processed. Now, make a connection between the adjacent and current node
                            visitedNodes[adjacentNode] = currentPathNode;

                            // Calculate the heuristic value depending on the selected A* heuristic
                            float heuristicValue = mode switch
                            {
                                Algorithm.AStar => // Standard
                                    HeuristicStandard(adjacentNode, endPathNode),
                                Algorithm.AStarEuclidean => // Euclidian
                                    HeuristicEuclidian(adjacentNode, endPathNode),
                                Algorithm.AStarManhattan => // Manhattan
                                    HeuristicManhattan(adjacentNode, endPathNode),
                                Algorithm.AStarDiagonal => // Diagonal
                                    HeuristicDiagonal(adjacentNode, endPathNode),
                                Algorithm.AStarOctile => // Octile
                                    HeuristicOctile(adjacentNode, endPathNode),
                                Algorithm.AStarCustom => HeuristicCustom(adjacentNode, currentPathNode, tGraph, endPathNode),
                                _ => 0
                            };

                            // Add the calculated heuristic value to the new estimated cost
                            // Enqueue the next node to visit with its cost. The priority queue will always dequeue first the node that has the smallest cost / highest priority.
                            float priority = newCost + heuristicValue;
                            nodesToVisit.Enqueue(adjacentNode, priority);

                            TotalCost += newCost;
                        }
                    }
                }

            }

            // TODO: Construct the path, follow back the visited nodes where connections were made
            // Iterate through all visited nodes until your reach the start node working on your way back
            // Add the visited nodes to the provided path list
            while (currentPathNode != startPathNode)
            {
                currentPathNode = visitedNodes[currentPathNode];
                path.Add(currentPathNode);
            }
            path.Reverse();
            return path;
        }

        // Standard A* heuristic. Calculate the distance between 2 nodes.
        private static float HeuristicStandard(PathNode a, PathNode b)
        {
            return Vector2.Distance(a.NodePosition, b.NodePosition);
        }

        // TODO: Euclidian distance. Calculate the straight-line distance between the x and y coordinates of two nodes. Assumes there are no obstacles.
        // This is calculated by:
        // 1. Getting the squared absolute distance between 2 nodes
        // 2. Calculating the square root of the sum of distance X and distance Y
        private static float HeuristicEuclidian(PathNode a, PathNode b)
        {
            float distX = Mathf.Pow(Mathf.Abs(b.NodePosition.x - a.NodePosition.x), 2);
            float distY = Mathf.Pow(Mathf.Abs(b.NodePosition.y - a.NodePosition.y), 2);
            float squareroot = Mathf.Sqrt(distX + distY);

            return squareroot;
        }
        /// <summary>
        /// Manhattan distance. Calculate the absolute distance between the x and y coordinates of two nodes.
        /// After which, get the sum of distance X and distance Y
        /// </summary>
        /// <param name="a">The current Node</param>
        /// <param name="b">The adjacent node</param>
        private static float HeuristicManhattan(PathNode a, PathNode b)
        {
            float distX = Mathf.Abs(a.NodePosition.x - b.NodePosition.x);
            float distY = Mathf.Abs(a.NodePosition.y - b.NodePosition.y);

            return distX + distY;
        }

        // TODO: Diagonal distance. Calculate the maximum absolute distances between the x and y coordinates of two nodes.
        // See Mathf.Max() function.
        private static float HeuristicDiagonal(PathNode a, PathNode b)
        {
            float distX = Mathf.Abs(b.NodePosition.x - a.NodePosition.x);
            float distY = Mathf.Abs(b.NodePosition.y - a.NodePosition.y);

            return Mathf.Max(distX, distY);
        }

        // TODO: Octile distance. Calculate the maximum and minimum absolute distances between the x and y coordinates of two nodes.
        private static float HeuristicOctile(PathNode a, PathNode b)
        {
            float distX = Mathf.Abs(a.NodePosition.x - b.NodePosition.x);
            float distY = Mathf.Abs(a.NodePosition.y - b.NodePosition.y);

            float diagonal = Mathf.Min(distX, distY); // Min
            float straight = Mathf.Max(distX, distY) - diagonal; // Max minus the diagonal movement

            return diagonal * Mathf.Sqrt(2) + straight;
        }

        private static float HeuristicCustom(PathNode adjacentNode, PathNode currentNode, TerrainGraph terrainGraph, PathNode endNode)
        {
            float heuristicCost = HeuristicManhattan(adjacentNode, endNode) * 0.6f;
            Vector3 normalAdjacentNode = terrainGraph.GetNormal(adjacentNode.NodePosition.x / 128f, adjacentNode.NodePosition.y / 128f);
            Vector3 planeVector = new Vector3(0, 1, 0);
            heuristicCost += Vector3.Angle(planeVector, normalAdjacentNode);
            heuristicCost += HeuristicOctile(currentNode, adjacentNode) * 0.4f;
            return heuristicCost;
        }
    }
}