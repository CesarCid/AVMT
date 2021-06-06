using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AVMT.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        private int currentRound = 0;
        public int CurrentRound => currentRound;

        private RoundState currentState = new RoundState(true);
        public RoundState CurrentState => currentState;

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
            ChangeState(new RoundState(RoundState.State.RoundSetup, RoundSetup, OnRoundSetupCompleted));
        }
        private void ChangeState (RoundState state)
        {
            if (currentState.Equals(state) && currentState.Completed == false)
            {
                return;
            }

            currentState = state;
            state.OnStateSet.Invoke();
        }
        private void CompleteCurrentState()
        {
            currentState.Completed = true;
        }

        #region RoundSetup
        private void RoundSetup()
        {
            piecesController.PopulateBoard();
            CompleteCurrentState();
        }

        private void OnRoundSetupCompleted()
        {
            RoundState nextState;
            if (piecesController.UpdateAvailableMoves())
            {
                nextState = new RoundState(RoundState.State.AwaitInput, AwaitInput, OnAwaitInputCompleted);
            }
            else
            {
                nextState = new RoundState(RoundState.State.RoundSetup, RoundSetup, OnRoundSetupCompleted);
            }
            ChangeState(nextState);
        }
        #endregion

        #region AwaitInput
        private void AwaitInput()
        {
            piecesController.SetInputsAllowed(true);
            piecesController.OnPieceMoved += OnPieceMoved;
        }

        private void OnPieceMoved(GamePiece piece, BoardSlot slot)
        {
            CompleteCurrentState();
        }
        private void OnAwaitInputCompleted() 
        {
            piecesController.SetInputsAllowed(false);
            piecesController.OnPieceMoved -= OnPieceMoved;

            ChangeState(new RoundState(RoundState.State.ResolveMatches, ResolveMatches, OnResolveMatchesCompleted));
        }
        #endregion

        #region ResolveMatches
        private void ResolveMatches() { }
        private void OnResolveMatchesCompleted() { }

        #endregion
    }

    public struct RoundState
    {
        public enum State { RoundSetup, EvaluateRemainingMatches, AwaitInput, ResolveMatches, FillEmptySlots }

        public State state;

        private bool completed;
        public bool Completed
        {
            get => completed;
            set
            {
                bool previouslyCompleted = completed;

                completed = value;

                if (previouslyCompleted == false && value)
                {
                    OnStateCompleted?.Invoke();
                }                
            }
        }

        public Action OnStateSet;
        public Action OnStateCompleted;

        public RoundState(State state, Action onSet, Action onComplete)
        {
            this.state = state;
            completed = false;
            OnStateSet = onSet;
            OnStateCompleted = onComplete;
        }

        public RoundState(bool completed)
        {
            state = default;
            this.completed = completed;
            OnStateSet = null;
            OnStateCompleted = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is RoundState == false)
            {
                return false;
            }

            return ((RoundState)obj).state == state;
        }
    }
}
