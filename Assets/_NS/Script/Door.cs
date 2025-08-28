using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Animator doorAnimator;
    public void Open()
    {
        doorAnimator.Play("Open");
    }
}
