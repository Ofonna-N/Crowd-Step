using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

namespace CrowdStep
{
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private Transform finishLine;
        public Transform FinishLine => finishLine;

        [SerializeField]
        private SplineComputer spline;
        public SplineComputer Spline => spline;

        public void AboveStep()
        {
            GameManager.instance.AboveStep = true;
        }
    }
}
