using System.Collections.Generic;
using UnityEngine;

namespace jre129.Scripts.Traps
{
    public class BombingLogic : MonoBehaviour
    {
        [SerializeField] List<GameObject> bombs;
        private int _currentBomb = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Agent"))
            {
                if (_currentBomb > bombs.Count)
                {
                    _currentBomb = 0;
                    bombs[_currentBomb].transform.position = transform.position;
                    return;
                }
                bombs[_currentBomb++].SetActive(true);
            }
        }
    }
}
