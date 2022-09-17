using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using MoreMountains.Feedbacks;

namespace CrowdStep
{
    public class Gate : MonoBehaviour
    {
        [SerializeField]
        private MultiplerType multiplerType;
        

        [SerializeField]
        private TextMeshPro gateText;


        [SerializeField]
        private MMFeedbacks onGateCrossed_Feedback;


        [SerializeField]
        private int multiplier = 2;



        private void OnTriggerEnter(Collider other)
        {
            //if (StackHit) return;
            if (other.transform.CompareTag(StaticStrings.Character_Tag))
            {

                onGateCrossed_Feedback?.PlayFeedbacks();
                GameManager.instance.PlayHaptic(MoreMountains.NiceVibrations.HapticTypes.SoftImpact);

                switch (multiplerType)
                {
                    case MultiplerType.Addition:
                        //Debug.Log($"{multiplier} = multiplier");
                        GameManager.instance.PlayerController.CharacterCrossedGate(multiplier);
                        break;

                    case MultiplerType.Multiplication:
                        //Debug.Log($"{multiplier} x {GameManager.instance.PlayerController.FlockAgents.Count} = {multiplier * GameManager.instance.PlayerController.FlockAgents.Count}");
                        GameManager.instance.PlayerController.CharacterCrossedGate((multiplier * GameManager.instance.PlayerController.FlockAgents.Count) - GameManager.instance.PlayerController.FlockAgents.Count);
                        break;
                    default:
                        Debug.Log("Multiplier Error");
                        break;
                }
            }
        }

#if UNITY_EDITOR
        [Button]
        private void UpdateGates()
        {
            //Debug.Log("Populating gates");
            UnityEditor.EditorUtility.SetDirty(this);
            //UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(gates);

            switch (multiplerType)
            {
                case MultiplerType.Addition:
                    gateText.text = $"+{multiplier}";
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(gateText);
                    break;
                /*case MultiplerType.Subtraction:
                    g.GateText.text = $"-{g.Multiplier}";
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(g.GateText);
                    break;*/
                case MultiplerType.Multiplication:
                    gateText.text = $"<size=20>x</size>{multiplier}";
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(gateText);
                    break;
                default:
                    break;
            }
        }
#endif
    }

    public enum MultiplerType { Addition, /*Subtraction, */Multiplication }
}
