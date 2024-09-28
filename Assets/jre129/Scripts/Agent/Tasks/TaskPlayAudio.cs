using System;
using jre129.Scripts.Agent.BehaviourTree;
using UnityEngine;

namespace jre129.Scripts.Agent.Tasks
{
    public class TaskPlayAudio : TreeNode
    {
        #region Private Fields

        private AudioClip _honkSound;
        private AudioSource _audioSource;

        #endregion

        public TaskPlayAudio(AudioClip audioClip, AudioSource audioSource)
        {
            _honkSound = audioClip;
            _audioSource = audioSource;
        }

        public override TreeNodeState RunPhysics()
        {
            if (TryGetData("_collisionTag", out object outputObject))
            {
                string collisionTag = outputObject as string;
                if (string.Compare(collisionTag, "Agent", StringComparison.OrdinalIgnoreCase) == 0 && !_audioSource.isPlaying)
                {
                    _audioSource.PlayOneShot(_honkSound);
                }
            }

            State = TreeNodeState.Running;
            return State;
        }
    }
}