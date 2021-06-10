using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class GameplayAudio : MonoBehaviour
    {
        private RoundManager roundManager;
        private GamePiecesController piecesController;

        [SerializeField]
        private AudioClip switchedAudio;
        [SerializeField]
        private AudioClip selectedAudio;
        [SerializeField]
        private AudioClip clearedAudio;

        private void Start()
        {
            roundManager = RoundManager.Instance;
            piecesController = roundManager.PiecesController;

            roundManager.onRoundFinished += OnRoundFinished;
            piecesController.onPiecesSwitched += (slot1, slot2) => PlaySwitched();
            piecesController.onSlotSelected += (slot) => PlaySelected();            
        }

        private void PlaySwitched()
        {
            AudioSource.PlayClipAtPoint(switchedAudio, Vector3.zero);
        }

        private void PlaySelected()
        {
            AudioSource.PlayClipAtPoint(selectedAudio, Vector3.zero);
        }

        private void OnRoundFinished(bool success)
        {
            if (success)
            {
                AudioSource.PlayClipAtPoint(clearedAudio, Vector3.zero);
            }
        }
    }
}
