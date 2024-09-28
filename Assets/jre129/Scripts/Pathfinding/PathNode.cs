using System;
using UnityEngine;

namespace jre129.Scripts.Pathfinding
{
    public class PathNode : IEquatable<PathNode>
    {
        private const double Tolerance = 0.1;
        
        public Vector2Int NodePosition;
        public float NodeHeight;
        public float Priority;
        private bool _isWalkable;
        
        public bool IsWalkable { get=>_isWalkable; set=>_isWalkable=value; }

        public PathNode(int x, int z)
        {
            NodePosition = new Vector2Int(x, z);
        }

        public PathNode(int x, int z, bool isWalkable)
        {
            _isWalkable = isWalkable;
            NodePosition = new Vector2Int(x, z);
        }

        public bool Equals(PathNode other)
        {
            if (other is null)
                return false;
            return this.NodePosition == other.NodePosition && Math.Abs(this.NodeHeight - other.NodeHeight) < Tolerance;
        }

        public override bool Equals(object obj) => Equals(obj as PathNode);
        public override int GetHashCode() => NodePosition.GetHashCode();
    }
}


