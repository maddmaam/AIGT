using System;
using UnityEngine;

namespace jre129.Scripts.Agent.BehaviourTree
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        #region Private Fields

        private TreeNode _root;

        #endregion

        protected void Start()
        {
            _root = SetupTree();
        }

        protected virtual void Update()
        {
            _root?.RunNode(); // Null Propagation
        }

        protected virtual void FixedUpdate()
        {
            _root?.RunPhysics();
        }

        protected abstract TreeNode SetupTree();
    }
}