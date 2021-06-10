using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class GameplayAudio : MonoBehaviour
    {
        [Serializable]
        public class Audio 
        {
            public AudioClip audioClip;
            public float volume;
        }

        private RoundManager roundManager;
        private GamePiecesController piecesController;

        [SerializeField]
        private AudioSource source;

        [SerializeField]
        private Audio switchedAudio;
        [SerializeField]
        private Audio selectedAudio;
        [SerializeField]
        private Audio clearedAudio;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (source == null)
            {
                source = GetComponentInChildren<AudioSource>();
            }
        }
#endif

        private void Start()
        {
            roundManager = RoundManager.Instance;
            piecesController = roundManager.PiecesController;

            roundManager.onRoundFinished += OnRoundFinished;
            piecesController.onPiecesSwitched += (slot1, slot2) => PlaySwitched();
            piecesController.onSlotSelected += (slot) => PlaySelected();            
        }

        private void PlayOneShot(Audio audio)
        {
            source.PlayOneShot(audio.audioClip, audio.volume);
        }

        private void PlaySwitched()
        {
            PlayOneShot(switchedAudio);
        }

        private void PlaySelected()
        {
            PlayOneShot(selectedAudio);            
        }

        private void OnRoundFinished(bool success)
        {
            if (success)
            {
                PlayOneShot(clearedAudio);
            }
        }
    }
}
