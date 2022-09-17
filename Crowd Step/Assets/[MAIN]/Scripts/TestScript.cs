using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


namespace CrowdStep
{
    public class TestScript : MonoBehaviour
    {

        [SerializeField]
        private Rigidbody target;

        public Transform follower;

        public bool following;

        Tweener tweener;

        private void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Follow();
            }*/
            if (Input.GetKey(KeyCode.Mouse0))
            {
                target.velocity = Vector3.up * 6f;
            }
        }


        void Follow()
        {
            if (following) return;
            following = true;

            tweener = follower.DOMoveY(target.position.y, 0.35f).
                OnComplete(() => following = false)
                .SetDelay(0.25f)
                .OnUpdate(()=>
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        tweener.ChangeEndValue(target.position.y);
                    }
                });
        }
    }
}
