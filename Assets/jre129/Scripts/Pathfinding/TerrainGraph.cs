using System.Collections.Generic;
using UnityEngine;

namespace jre129.Scripts.Pathfinding
{
    public class TerrainGraph
    {
        private const float AgentHeight = 3f;
        
        private static bool _obstaclesOnLayer;
        public static bool ObstaclesOnLayer
        {
            get => _obstaclesOnLayer;
            set => _obstaclesOnLayer = value;
        }

        private static readonly Vector3 TopLeftOffset = new Vector3(-0.375f, 0, 0.125f);
        private static readonly Vector3 BottomLeftOffset = new Vector3(-0.125f, 0, -0.375f);
        private static readonly Vector3 TopRightOffset = new Vector3(0.125f, 0f, 0.375f);
        private static readonly Vector3 BottomRightOffset = new Vector3(0.375f, 0f, -0.125f);
        

        private int _obstacleMask;
        
        private TerrainData _tData;
        private int _tWidth;
        private int _tLength;
        private float _gridOffset = 0.5f;

        public PathNode[,] grid;
        public float[,,] cost; 

        public Vector2 TerrainSize;
        private float _maxHeightDifference = 0.5f;

        public TerrainGraph()
        {
            // Get reference of the active terrain on the scene
            _tData = Terrain.activeTerrain.terrainData;

            // Create a representation of the graph using the terrain size
            // Taking the x (width) and z (length) values only
            // Grid offset points to the center of the node cell
            
            if (_obstaclesOnLayer)
            {
                _obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
            }

            Physics.queriesHitBackfaces = true;
            Physics.queriesHitTriggers = false;
            
            _tWidth = Mathf.FloorToInt(_tData.size.x);
            _tLength = Mathf.FloorToInt(_tData.size.z);
            grid = new PathNode[_tWidth, _tLength];
            cost = new float[_tWidth, _tLength, 8]; // cost towards each 8 direction from current node

            TerrainSize = new Vector2(_tWidth, _tLength);

            // Populate the grid with nodes
            for (int x = 0; x < _tWidth; x++)
            {
                for (int z = 0; z < _tLength; z++)
                {
                    PathNode newNode = new PathNode(x, z);
                    newNode.NodeHeight = Terrain.activeTerrain.SampleHeight(new Vector3(x + _gridOffset, 0, z + _gridOffset));
                    newNode.IsWalkable = IsPassable(newNode) && !HasHole(newNode);
                    
                    grid[x, z] = newNode;
                }
            }

            // Store costs (height difference between nodes) of the terrain in cost array
            for (int x = 0; x < _tWidth; x++)
            {
                for (int z = 0; z < _tLength; z++)
                {
                    if (x > 0 && z > 0 && x < _tWidth - 1 && z < _tLength - 1)
                    {
                        // TODO: Set the proper edge/connection cost for each of the 8 directions
                        cost[x, z, 0] = Mathf.Abs(grid[x, z].NodeHeight - grid[x-1,z].NodeHeight); // west of current node  
                        cost[x, z, 1] = Mathf.Abs(grid[x, z].NodeHeight - grid[x-1,z+1].NodeHeight); // north-west of current node  
                        cost[x, z, 2] = Mathf.Abs(grid[x, z].NodeHeight - grid[x,z+1].NodeHeight); // north of current node  
                        cost[x, z, 3] = Mathf.Abs(grid[x, z].NodeHeight - grid[x+1,z+1].NodeHeight); // north-east of current node  
                        cost[x, z, 4] = Mathf.Abs(grid[x, z].NodeHeight - grid[x+1,z].NodeHeight); // east of current node  
                        cost[x, z, 5] = Mathf.Abs(grid[x, z].NodeHeight - grid[x+1,z-1].NodeHeight); // south-east of current node  
                        cost[x, z, 6] = Mathf.Abs(grid[x, z].NodeHeight - grid[x,z-1].NodeHeight); // south of current node  
                        cost[x, z, 7] = Mathf.Abs(grid[x, z].NodeHeight - grid[x-1,z-1].NodeHeight); // south-west of current node  
                    }
                }
            }


        }

        private bool IsPassable(PathNode currentNode)
        {
            Vector3 nodePosition = new Vector3(currentNode.NodePosition.x, currentNode.NodeHeight,
                currentNode.NodePosition.y);
            Vector3 raycastTopLeft = nodePosition + TopLeftOffset;
            Vector3 raycastBottomLeft = nodePosition + BottomLeftOffset;
            Vector3 raycastTopRight = nodePosition + TopRightOffset;
            Vector3 raycastBottomRight = nodePosition + BottomRightOffset;
            Vector3 topDownVector = new Vector3(0, 30, 0);
            bool hitObjectBelow;
            bool hitObjectAbove;
            RaycastHit hit;
            if (_obstaclesOnLayer)
            {   // Return if any of the four 2x2 RGSS Raycasts hit an obstacle
                hitObjectBelow = (Physics.Raycast(raycastTopLeft, Vector3.up, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastBottomLeft, Vector3.up, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastTopRight, Vector3.up, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastBottomRight, Vector3.up, out hit, 30f, _obstacleMask));
                hitObjectAbove = Physics.Raycast(raycastTopLeft + topDownVector, Vector3.down, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastBottomLeft + topDownVector, Vector3.down, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastTopRight + topDownVector, Vector3.down, out hit, 30f, _obstacleMask) ||
                    Physics.Raycast(raycastBottomRight + topDownVector, Vector3.down, out hit, 30f, _obstacleMask);
            }
            else
            {   // Return if any of the four 2x2 RGSS Raycasts hit an object tagged obstacle
                hitObjectBelow = (Physics.Raycast(raycastTopLeft, Vector3.up, out hit, 30f) ||
                    Physics.Raycast(raycastBottomLeft, Vector3.up, out hit, 30f) ||
                    Physics.Raycast(raycastTopRight, Vector3.up, out hit, 30f) ||
                    Physics.Raycast(raycastBottomRight, Vector3.up, out hit, 30f))
                            && hit.collider.CompareTag("Obstacle");
                hitObjectAbove = (Physics.Raycast(raycastTopLeft + topDownVector, Vector3.down, out hit, 30f) ||
                    Physics.Raycast(raycastBottomLeft + topDownVector, Vector3.down, out hit, 30f) ||
                    Physics.Raycast(raycastTopRight + topDownVector, Vector3.down, out hit, 30f) ||
                    Physics.Raycast(raycastBottomRight + topDownVector, Vector3.down, out hit, 30f))
                            && hit.collider.CompareTag("Obstacle");
            }

            if (hitObjectBelow && hit.distance > AgentHeight && !HasRigidBody(hit))
            {
                hitObjectBelow = false;
                hitObjectAbove = false;
            }
            return !(hitObjectBelow || hitObjectAbove);
        }

        private bool HasRigidBody(RaycastHit hitResult)
        {
            Rigidbody[] rigidBodies;
            rigidBodies = hitResult.collider.GetComponentsInChildren<Rigidbody>(true);
            return rigidBodies.Length > 0;
        }

        private bool HasHole(PathNode currentNode)
        {
            Vector3 nodePosition = new Vector3(currentNode.NodePosition.x, currentNode.NodeHeight + 1,
                currentNode.NodePosition.y);
            return !Physics.Raycast(nodePosition, Vector3.down, 30f);
        }

        public List<PathNode> GetNeighbours(PathNode n)
        {
            List<PathNode> neighbours = new List<PathNode>();

            // TODO: Take all the nodes from all cardinal and ordinal directions
            // Assume current node is at Vector2(0,0)

            Vector2[] directions =
            {
                new Vector2(-1, 0), // west
                new Vector2(-1, 1), // north-west
                new Vector2(0, 1),  // north
                new Vector2(1, 1),  // north-east
                new Vector2(1, 0),  // east
                new Vector2(1, -1), // south-east
                new Vector2(0, -1), // south
                new Vector2(-1, -1) // south-west
            };

            // Find all nodes via the 8 directions
            foreach (Vector2 dir in directions)
            {
                Vector2 v = new Vector2(dir.x, dir.y) + new Vector2(n.NodePosition.x, n.NodePosition.y);

                // Check if the neighbouring node actually exist in the terrain
                // y here is actually the z
                bool doExist = (v.x >= 0 && v.x < _tWidth && v.y >= 0 && v.y < _tLength);
                
                
                bool passable = false;
                if (doExist)
                {
                    passable = grid[(int)v.x, (int)v.y].IsWalkable && Mathf.Abs(grid[(int)v.x, (int)v.y].NodeHeight - n.NodeHeight) < _maxHeightDifference;
                }

                if (doExist && passable)
                {
                    neighbours.Add(grid[(int)v.x, (int)v.y]);
                }
            }

            return neighbours;
        }

        // This function searches and returns the least cost among all the 8 neighbors of a node
        public float NextMinimumCost(PathNode n)
        {
            float minCost = float.PositiveInfinity; // dummy value
            
            for (int index = 0; index < 8; index++)
            {
                // TODO: Search for minimum vertical cost of
                if (minCost > cost[n.NodePosition.x, n.NodePosition.y, index]) 
                {
                    minCost = cost[n.NodePosition.x, n.NodePosition.y, index];
                }
            }

            // Since graph is a tile grid, horizontal cost is 1
            return minCost + 1;
        }

        public Vector3 GetNormal(float x, float y)
        {
            return _tData.GetInterpolatedNormal(x, y);
        }
    }
}
