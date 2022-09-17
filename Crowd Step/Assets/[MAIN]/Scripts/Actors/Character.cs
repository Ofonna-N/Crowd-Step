using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using TMPro;

namespace CrowdStep
{
    public class Character : MonoBehaviour
    {
        [SerializeField, Tooltip("Character rigidbody for handling physics")]
        private Rigidbody rb;
        public Rigidbody Rb => rb;

        [SerializeField, Tooltip("Character Animator")]
        private Animator anim;
        public Animator Anim => anim;

        [SerializeField, Tooltip("number of agents in the flock")]
        private TextMeshProUGUI flockCount_Text;
        public TextMeshProUGUI FlockCount_Text => flockCount_Text;

        [SerializeField, Tooltip("character mesh renderer with material")]
        private SkinnedMeshRenderer meshRenderer;

        [SerializeField, Tooltip("character color, distinct from the flock")]
        private Color characterColor = Color.cyan;

        [SerializeField, Tooltip("Feedback when diamond is collected")]
        private MMFeedbacks collectDiamond_fb;

        [SerializeField, Tooltip("feedback when player lands on floor")]
        private MMFeedbacks charDead_FB;

        [SerializeField, Tooltip("feedback when player lands on floor")]
        private MMFeedbacks land_fb;
        
        [SerializeField, Tooltip("feedback when character looses or hits obstacle")]
        private MMFeedbacks obstacleHit_Fb;

        //material block responsible for changing color of items with same material
        private MaterialPropertyBlock block;
        //hashing from material property Id to int
        private int colorId;

        [ReadOnly, PropertyTooltip("Variable used only if this is an ai agent")]
        public bool CanHandleMove { get; set; }


        [ReadOnly, PropertyTooltip("Variable used only if this is an ai agent")]
        public bool CharacterDead { get; set; }

        //reward multiplier on enter step trigger
        public float GameOverRewardMultiplier { get; set; }

        //action event when character lands on floor
        public System.Action OnCharacterLand;

        //action event when character lands on step
        public System.Action OnStepLand;

        //action event when character hits obstacle
        public System.Action OnObstacleHit;

        /// <summary>
        /// character init function
        /// - setting material block color
        /// </summary>
        public void Init()
        {
            SetMaterialColor();
        }

        /// <summary>
        /// ascend function adding upward velocity to character
        /// </summary>
        /// <param name="speed"></param>
        public void Ascend(float speed)
        {
            rb.velocity = Vector3.up * speed;
        }


        /// <summary>
        /// responsible for all collisions on character
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(StaticStrings.Floor_Tag))
            {
                //Debug.Log(collision.relativeVelocity.y);
                if (CharacterDead) charDead_FB.PlayFeedbacks(transform.position);
                if (!CanHandleMove) return;
                anim.SetBool(StaticStrings.Move_AnimParam, true);
                if (collision.relativeVelocity.y > 24)
                {
                    land_fb.PlayFeedbacks(transform.position);
                    GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
                    OnCharacterLand();
                }
            }

            if (collision.gameObject.CompareTag(StaticStrings.FinishStep_Tag))
            {
                GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);
                OnStepLand();
            }

            if (collision.gameObject.CompareTag(StaticStrings.Obstacle_Tag))
            {
                obstacleHit_Fb.PlayFeedbacks();
                OnObstacleHit();
            }
        }


        /// <summary>
        /// responsible for all triggers on character
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(StaticStrings.Diamond_Tag))
            {
                other.transform.gameObject.SetActive(false);
                collectDiamond_fb.PlayFeedbacks(other.transform.position);
                GameManager.instance.RewardCoinsManager.
                    AnimateCoin(GameManager.instance.MainCamera.WorldToScreenPoint(other.transform.position));
            }

            if (other.gameObject.CompareTag(StaticStrings.FinishStep_Tag))
            {
                GameOverRewardMultiplier = other.GetComponent<StepReward>().RewardMultiplier;
                OnStepLand();
            }
        }


        /// <summary>
        /// sets material color with property block
        /// </summary>
        private void SetMaterialColor()
        {
            block = new MaterialPropertyBlock();
            colorId = Shader.PropertyToID("_BaseColor");
            block.SetColor(colorId, characterColor);
            meshRenderer.SetPropertyBlock(block);
        }
    }
}
