using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SmallyGames.Menus
{
    [RequireComponent(typeof(BoxCollider))]
    public class TrainingTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("Collider to detect character"), BoxGroup("Training Data")]
        private BoxCollider triggerCol;

        [SerializeField, BoxGroup("Training Data")]
        private string triggerInput;

        [SerializeField, BoxGroup("Training Data")]
        private string triggerdetectTag;

        [SerializeField, BoxGroup("Training Data")]
        private GameObject trainingView;

        [SerializeField, BoxGroup("Training Data"), OnValueChanged("ResetSaveData")]
        private bool resetSaveData = false;

        [SerializeField, ReadOnly, BoxGroup("Training Data")]
        private bool inTrigger;

        [SerializeField, ReadOnly, BoxGroup("Save Data")]
        private string Training_Key = "Training_Key";

        [ShowInInspector, BoxGroup("Save Data"), ReadOnly]
        public bool HasTrained
        {
            get
            {
                if (PlayerPrefs.HasKey(Training_Key))
                {
                    int val = PlayerPrefs.GetInt(Training_Key);

                    return  (val == 1) ? true : false;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                PlayerPrefs.SetInt(Training_Key, (value) ? 1 : 0);
            }
        }


        [SerializeField, BoxGroup("Editor Data")]
        private Color triggerColor = Color.yellow;



        private void Start()
        {
            if (HasTrained)
            {
                trainingView.SetActive(false);
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (HasTrained) return;
            if (SimpleInput.GetButtonDown(triggerInput) && inTrigger)
            {
                //Debug.Log("BOW BOW");
                trainingView.SetActive(false);
                Time.timeScale = 1;
                HasTrained = true;
                gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HasTrained) return;
            if (other.CompareTag(triggerdetectTag))
            {
                inTrigger = true;
                trainingView.SetActive(true);
                Time.timeScale = 0;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = triggerColor;
            if (triggerCol != null)
            {
                Gizmos.DrawCube(transform.position + triggerCol.center, triggerCol.size);
            }
        }

        private void ResetSaveData()
        {
            HasTrained = false;
            resetSaveData = false;
        }
#endif
    }
}
