using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

namespace CrowdStep
{
    public class StepReward : MonoBehaviour
    {
        [SerializeField]
        private float rewardMultiplier = 1;
        public float RewardMultiplier
        {
            get
            {
                onEnterReward.PlayFeedbacks();
                return rewardMultiplier;
            }
        }

        [SerializeField]
        private MMFeedbacks onEnterReward;
    }
}
