using System.Collections.Generic;
using UnityEngine;

namespace jre129.Scripts.Traps
{
    public class CrateLogic : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> crates;

        private List<Vector3> _startPositions;
        // Start is called before the first frame update
        private void Start()
        {
            _startPositions = new List<Vector3>();
            foreach (Rigidbody rb in crates)
            {
                _startPositions.Add(rb.position);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Agent"))
            {
                for (int crateIndex = 0; crateIndex < crates.Count; crateIndex++)
                {
                    crates[crateIndex].MovePosition(_startPositions[crateIndex]);
                }
            }
        }
    }
}
