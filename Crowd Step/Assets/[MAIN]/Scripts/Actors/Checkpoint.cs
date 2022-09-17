using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;

namespace CrowdStep
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField]
        private MMFeedbacks onCheckpointCrossed_Feedback;

        [SerializeField]
        private bool finishLine;

/*        [SerializeField, ShowIf("finishLine"), Tooltip("Finish line feedbacks to activate")]
        private string[] feedbackLabels;

        private MMFeedback[] finishLineFeedbacks;*/

/*        private void Start()
        {
            if (finishLine)
            {
                finishLineFeedbacks = new MMFeedback[feedbackLabels.Length];

                for (int i = 0; i < feedbackLabels.Length; i++)
                {
                    finishLineFeedbacks[i] = onCheckpointCrossed_Feedback.Feedbacks.Find((x) => x.Label == feedbackLabels[i]);
                }
            }
        }*/

        public void OnCrossedCheckpoint()
        {
            //Debug.Log("Crossed");
            /*for (int i = 0; i < feedbackLabels.Length; i++) finishLineFeedbacks[i].Active = true;*/

            if (finishLine)
            {
                GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.Success);
            }
            else
            {
                GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
            }

            onCheckpointCrossed_Feedback?.PlayFeedbacks();
            GameManager.instance.CheckPointCrossed(finishLine);
        }
    }
}
