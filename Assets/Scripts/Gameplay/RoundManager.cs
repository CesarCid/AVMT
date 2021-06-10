using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AVMT.Gameplay
{
    public class RoundManager : MonoBehaviour
    {
        private static RoundManager rm;
        public static RoundManager Instance => rm;

        private RoundState currentState = new RoundState(true);
        public RoundState CurrentState => currentState;

        private Coroutine completeStateCoroutine = null;

        private int round = 0;
        public int CurrentRound => round;

        [SerializeField]
        private GameplayTimer timer;
        public float RemainingTime => timer.RemainingTime;

        public static int RoundTimeLimit = 120;

        private int points = 0;
        public int CurrentPoints => points;

        [SerializeField]
        private GamePiecesController piecesController;
        public GamePiecesController PiecesController => piecesController;

        [SerializeField]
        private int initialRoundPointsTarget = 100;
        [SerializeField]
        private int roundPointsTargetIncreaseAmount = 20;

        [SerializeField]
        private int roundPointsTarget { get => initialRoundPointsTarget + (roundPointsTargetIncreaseAmount * round); }
        public int CurrentRoundPointsTarget => roundPointsTarget;

        private static int MatchThreeBasePoints = 5;
        private static int MatchThreeExtraPoints = 2;

        public Action<int> onRoundStarted;
        public Action<bool> onRoundFinished;
        public Action onPointsAdded;

        private void OnValidate()
        {
            if (piecesController != null) 
            {
                return;
            }

            piecesController = FindObjectOfType<GamePiecesController>();
            timer = FindObjectOfType<GameplayTimer>();
        }

        private void Awake()
        {
            if (rm == null)
            {
                rm = this;
            }
        }

        private void Start()
        {
            piecesController.onMatchesFound += OnMatchesFound;
            timer.onTimerEnd += OnTimerEnd;

            ChangeState(new RoundState(RoundState.State.RoundSetup, RoundSetup, OnRoundSetupCompleted));
        }

        private void ChangeState (RoundState state, bool forced = false)
        {
            if (currentState.Equals(state) && currentState.Completed == false && forced == false)
            {
                return;
            }

            currentState = state;
            state.OnStateSet.Invoke();
        }
        private void CompleteCurrentState(float delay = 0f)
        {
            completeStateCoroutine = StartCoroutine(CompleteCurrentStateDelayed(delay));
        }

        private IEnumerator CompleteCurrentStateDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (CheckRoundCompletion() == false)
            {
                currentState.Completed = true;
            }           

            completeStateCoroutine = null;
        }

        #region Completion
        private bool CheckRoundCompletion()
        {
            bool anyCompletion = false;

            if (CheckTimeCompletion())
            {
                EndRound(false);
                anyCompletion = true;
            }

            if (CheckPointsCompletion()) 
            {
                EndRound(true);
                anyCompletion = true;
            }           

            return anyCompletion;
        }

        private bool CheckPointsCompletion()
        {
            return points >= roundPointsTarget;
        }
        private bool CheckTimeCompletion()
        {
            return RemainingTime <= 0;
        }
        
        private void OnTimerEnd() 
        {
            if (completeStateCoroutine != null)
            {
                StopCoroutine(completeStateCoroutine);
            }

            CheckRoundCompletion();
        }

        private void EndRound(bool success)
        {
            piecesController.SetInputsAllowed(false);
            onRoundFinished?.Invoke(success);

            if (success)
            {
                NextRound();
            }
        }

        private void NextRound()
        {
            round++;
            points = 0;
            ChangeState(new RoundState(RoundState.State.RoundSetup, RoundSetup, OnRoundSetupCompleted), true);
        }
        #endregion

        #region Points
        private void OnMatchesFound(List<int> matchLenghts)
        {
            int points = 0;
            foreach(int matchLenght in matchLenghts)
            {
                points += MatchThreeBasePoints;                
                points += (matchLenght - 3) * MatchThreeExtraPoints;
            }

            AddPoints(points);
        }

        private void AddPoints(int amount)
        {
            points += amount;
            onPointsAdded?.Invoke();
        }
        #endregion

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
                onRoundStarted?.Invoke(round);
                nextState = new RoundState(RoundState.State.EvaluateRemainingMatches, EvaluateRemainingMatches, OnEvaluateRemainingMatchesCompleted);
            }
            else
            {
                nextState = new RoundState(RoundState.State.RoundSetup, RoundSetup, OnRoundSetupCompleted);
            }
            ChangeState(nextState);
        }
        #endregion

        #region EvaluateRemainingMatches

        private bool anyRemainingMatches = false;
        private void EvaluateRemainingMatches()
        {
            anyRemainingMatches = piecesController.UpdateAvailableMoves();
            CompleteCurrentState();
        }

        private void OnEvaluateRemainingMatchesCompleted()
        {
            RoundState nextState;
            if (anyRemainingMatches)
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
            piecesController.onPiecesSwitched += OnAfterPiecesSwitched;
        }

        private void OnAfterPiecesSwitched(BoardSlot from, BoardSlot to)
        {
            if (currentState.state == RoundState.State.AwaitInput)
            {
                CompleteCurrentState();
            }
        }
        private void OnAwaitInputCompleted() 
        {
            piecesController.SetInputsAllowed(false);
            piecesController.onPiecesSwitched -= OnAfterPiecesSwitched;

            ChangeState(new RoundState(RoundState.State.ResolveMatches, ResolveMatches, OnResolveMatchesCompleted));
        }
        #endregion

        #region ResolveMatches

        private bool anyMatchesResolved = false;
        private void ResolveMatches() 
        {
            anyMatchesResolved = piecesController.ResolveMatches();
            CompleteCurrentState();            
        }

        private void OnResolveMatchesCompleted() 
        {
            RoundState nextState;
            if (anyMatchesResolved)
            {                
                nextState = new RoundState(RoundState.State.FillEmptySlots, FillEmptySlots, OnFillEmptySlotsCompleted);
            }
            else
            {
                nextState = new RoundState(RoundState.State.EvaluateRemainingMatches, EvaluateRemainingMatches, OnEvaluateRemainingMatchesCompleted);                
            }
            ChangeState(nextState);
        }

        #endregion

        #region FillEmptySlots

        private void FillEmptySlots()
        {
            piecesController.onEmptySpacesFilled += OnEmptySpacesFilled;
            piecesController.FillEmptySpaces();
        }
   
        private void OnEmptySpacesFilled() 
        {
            piecesController.onEmptySpacesFilled -= OnEmptySpacesFilled;
            CompleteCurrentState();
        }

        private void OnFillEmptySlotsCompleted()
        {
            ChangeState(new RoundState(RoundState.State.ResolveMatches, ResolveMatches, OnResolveMatchesCompleted));
        }

        #endregion
    }

    [Serializable]
    public struct RoundState
    {
        public enum State { RoundSetup, EvaluateRemainingMatches, AwaitInput, ResolveMatches, FillEmptySlots, RoundEnd}

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
