using UnityEngine;

namespace jre129.Scripts.HelperClasses
{
    public static class RayCastHelper
    {
        public static RaycastHit GetClosestHit(RaycastHit[] hitResults)
        {
            float closestCollider = float.MaxValue;
            int closestHitIndex = 0;

            for (int hitIndex = 0; hitIndex < hitResults.Length; hitIndex++)
            {
                Collider collider = hitResults[hitIndex].collider;
                if (hitResults[hitIndex].distance < closestCollider && collider is not null)
                {
                    closestHitIndex = hitIndex;
                    closestCollider = hitResults[hitIndex].distance;
                }
            }

            return hitResults[closestHitIndex];
        }
    }
}