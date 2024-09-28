using jre129.Scripts.Pathfinding;
using UnityEngine;

namespace jre129.Scripts.HelperClasses
{
    public static class ObstacleLogic
    {
        public static void SetObstacles()
        {
            int layer;
            bool layerExists = Layers.TryCreateLayerFromName("Obstacle");
            TerrainGraph.ObstaclesOnLayer = layerExists;
            if (layerExists)
            {
                layer = LayerMask.NameToLayer("Obstacle");
            }
            else return;
            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            foreach (GameObject obstacle in obstacles)
            {
                obstacle.layer = layer;
            }
        }
    }
}