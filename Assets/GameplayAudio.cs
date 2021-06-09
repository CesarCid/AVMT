using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class GameplayAudio : MonoBehaviour
    {
        private GamePiecesController piecesController;

        [SerializeField]
        private AudioClip switchedAudio;
        [SerializeField]
        private AudioClip selectedAudio;
        [SerializeField]
        private AudioClip clearedAudio;

        private void Start()
        {
            piecesController = RoundManager.Instance.PiecesController;

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

        private void PlayCleared()
        {
            AudioSource.PlayClipAtPoint(clearedAudio, Vector3.zero);
        }
    }
}
