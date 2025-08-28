using Unity.VisualScripting;
using UnityEngine;

public class HandBlockWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var blockObj = other.gameObject.GetComponentInParent<WallBlockObj>();
        if (blockObj != null)
        {
            blockObj.colliderList.Add(other);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        var blockObj = other.gameObject.GetComponentInParent<WallBlockObj>();
        if (blockObj != null)
        {
            blockObj.Exit(other);
        }
    }

    private void OnDestroy()
    {

    }
}
