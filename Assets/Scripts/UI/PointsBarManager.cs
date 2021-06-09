using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AVMT.Gameplay
{
    public class PointsBarManager : MonoBehaviour
    {
        [SerializeField]
        private Text pointsText;

        [SerializeField]
        private Image pointsBar;

        private RoundManager roundManager;

        private void Start()
        {
            roundManager = RoundManager.Instance;

            roundManager.onPointsAdded += UpdatePointsBar;
        }

        public void UpdatePointsBar()
        {
            pointsText.text = roundManager.CurrentPoints.ToString();

            float pointsPct = (float) roundManager.CurrentPoints / roundManager.CurrentRoundPointsTarget;

            float width = Mathf.Min(50 + (250 * pointsPct), 300);

            pointsBar.rectTransform.sizeDelta = new Vector2(width, pointsBar.rectTransform.sizeDelta.y);
        }
    }
}
