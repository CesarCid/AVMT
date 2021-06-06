using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        private int currentRound = 0;
        public int CurrentRound { get => currentRound; }

        [SerializeField]
        private GamePiecesController piecesController;


        private void OnValidate()
        {
            if (piecesController != null) 
            {
                return;
            }

            piecesController = FindObjectOfType<GamePiecesController>();
        }

        private void Start()
        {
            StartRound();
        }
        private void StartRound() 
        {
            piecesController.PopulateBoard();
            piecesController.UpdateAvailableMoves();
        }
    }
}
