using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonPeople
{
    public class Playanimation : MonoBehaviour
    {
        public string anim;
        public bool delayed;
        public bool happy;
        public bool sad;
        public bool angry;
        public bool amazed;
        public bool disgust;
        public bool numb;
        public bool glared;
        public bool irritated;
        public const int DEFAULT_LAYER = 0;
        public const int HAPPY_LAYER = 1;
        public const int SAD_LAYER = 2;
        public const int ANGRY_LAYER = 3;
        public const int AMAZED_LAYER = 4;
        public const int DISGUST_LAYER = 5;
        public const int NUMB_LAYER = 6;
        public const int GLARE_LAYER = 7;
        public const int IRRITATED_LAYER = 8;
        public const int BLINK_LAYER = 9;
        public const int AFRAID_LAYER = 10;
        public const int MOUSE_OPEN_LAYER = 11;
        public const int AFRAID_CRY_LAYER = 12;

        string _defaultAnim = "";

        public string DefaultAnim => _defaultAnim;

        public Animator animator;

        void Start()
        {
            _defaultAnim = anim;
            animator = GetComponent<Animator>();
            // GetComponent<Animator>().Play(anim);
            if (happy) animator.SetLayerWeight(HAPPY_LAYER, 1f);
            else if (sad) animator.SetLayerWeight(SAD_LAYER, 1f);
            else if (angry) animator.SetLayerWeight(ANGRY_LAYER, 1f);
            else if (amazed) animator.SetLayerWeight(AMAZED_LAYER, 1f);
            else if (disgust) animator.SetLayerWeight(DISGUST_LAYER, 1f);
            else if (numb) animator.SetLayerWeight(NUMB_LAYER, 1f);
            else if (glared) animator.SetLayerWeight(GLARE_LAYER, 1f);
            else if (irritated) animator.SetLayerWeight(IRRITATED_LAYER, 1f);
            if (delayed)
            {
                StartCoroutine("playanim", anim);
            }
        }

        IEnumerator playanim(string anim)
        {
            GetComponent<Animator>().speed = 0.65f;
            yield return new WaitForSeconds(Random.Range(0f, 2f));
            GetComponent<Animator>().speed = 1f;
            animator.Play(anim);
        }

        public void PlayTheAnimation(string newanim)
        {
            if(animator == null) {
                animator = GetComponent<Animator>();
            }
            anim = newanim;
            animator.speed = 1f;
            animator.Play(anim,0,0);
        }

        public void SetLayerWeight(int layer, float weight, bool multiple = false)
        {
            if(!multiple) {
                for(int i = 1; i < 12; i++){
                    if(i == BLINK_LAYER) continue;
                    animator.SetLayerWeight(i, 0f);
                }
            }
            animator.SetLayerWeight(layer, weight);
        }
    }
}