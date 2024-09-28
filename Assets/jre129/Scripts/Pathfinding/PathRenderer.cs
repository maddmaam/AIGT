using System;
using System.Collections.Generic;
using System.Globalization;
using jre129.Scripts.Agent;
using TMPro;
using UnityEngine;

namespace jre129.Scripts.Pathfinding
{
    public class PathRenderer : MonoBehaviour
    {
        public Transform start;
        public Transform end;
        public LineRenderer lr;

        public Algorithm algorithm = Algorithm.BreadthFirstSearch;

        [Header("Info Refs")]
        public TextMeshProUGUI startPosText;
        public TextMeshProUGUI endPosText;
        public TextMeshProUGUI accumulatedCostText;
        public TextMeshProUGUI nodeNumText;


        public TerrainGraph Graph;
        private const float GridOffset = 0.5f;
        public List<PathNode> Path = new List<PathNode>();

        private void Update()
        {
            RenderPath();
            // UpdateInfo();
        }

        private void RenderPath()
        {
            // Start node position
            Vector3 position = start.position;
            int startX = (int)position.x;
            int startZ = (int)position.z;

            PathNode startPathNode = Graph.grid[startX, startZ];

            // End node position
            Vector3 position1 = end.position;
            int endX = (int)position1.x;
            int endZ = (int)position1.z;

            PathNode endPathNode = Graph.grid[endX, endZ];

            // Get list of nodes making the path based on selected algorithm

            // Create the line using an array of vertices based on the nodes in the path
            // Grid offset points to the center of the node cell
            Vector3[] lineVertices = new Vector3[Path.Count];
            int index = 0;

            foreach (PathNode n in Path)
            {
                float x = n.NodePosition.x + GridOffset;
                float y = n.NodeHeight + GridOffset;
                float z = n.NodePosition.y + GridOffset;

                lineVertices[index++] = new Vector3(x, y, z);
            }

            // Render the path
            lr.positionCount = Path.Count;
            lr.SetPositions(lineVertices);
        }

        // private void UpdateInfo()
        // {
        //     startPosText.text = "(" + Mathf.FloorToInt(start.transform.position.x) + ", " + Mathf.FloorToInt(start.transform.position.z) + ")";
        //     endPosText.text = "(" + Mathf.FloorToInt(end.transform.position.x) + ", " + Mathf.FloorToInt(end.transform.position.z) + ")";
        //     accumulatedCostText.text = PathAlgorithm.TotalCost.ToString(CultureInfo.CurrentCulture);
        //     nodeNumText.text = Path.Count.ToString();
        // }
    }

}
