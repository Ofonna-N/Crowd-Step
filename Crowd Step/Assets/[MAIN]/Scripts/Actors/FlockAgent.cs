using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace CrowdStep
{
    public class FlockAgent : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rb;
        public Rigidbody Rb => rb;

        [SerializeField]
        private Animator anim;
        public Animator Anim => anim;

        [SerializeField]
        private SkinnedMeshRenderer meshRenderer;

        [SerializeField]
        private Color agentColor = Color.cyan;

        private MaterialPropertyBlock block;
        private int colorId;

        [ReadOnly]
        public bool CanHandleMove { get; set; }


        public void Init()
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            SetMaterialColor();
        }

        public void Ascend(float speed)
        {
            rb.velocity = Vector3.up * speed;
        }

        public void Move(Vector3 velocity, float moveSpeed)
        {
            Vector3 newPos = transform.localPosition + velocity;
            newPos.y = transform.localPosition.y;

            transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, moveSpeed * Time.deltaTime);

        }

        private void SetMaterialColor()
        {
            block = new MaterialPropertyBlock();
            colorId = Shader.PropertyToID("_BaseColor");
            block.SetColor(colorId, agentColor);
            meshRenderer.SetPropertyBlock(block);
        }


        public void OnStep()
        {
            Invoke("DeactivateAgent", 5f);
        }

        public void DeactivateAgent()
        {
            gameObject.SetActive(false);
        }
    }
}
