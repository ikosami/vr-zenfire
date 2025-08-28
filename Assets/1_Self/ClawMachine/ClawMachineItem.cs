using System.Collections;
using UnityEngine;

public class ClawMachineItem : MonoBehaviour
{
    public Transform grabPoint;
    public Transform grabPointL;
    public string handPause = "GrabTeacup";
    public bool hasPose = true;

    public void DestroyItem(GameObject itemDestoryEffect)
    {
        // Generate a random delay between 0 and 0.5 seconds.
        float delay = UnityEngine.Random.Range(0f, 0.5f);

        // Use a coroutine to delay the destruction.
        StartCoroutine(DestroyItemDelayed(itemDestoryEffect, delay));
    }

    private IEnumerator DestroyItemDelayed(GameObject itemDestoryEffect, float delay)
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(delay);

        // Instantiate the destruction effect at the item's position.
        if (itemDestoryEffect != null) // Check if the effect is assigned. Important!
        {
            Instantiate(itemDestoryEffect, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("DestroyItem effect is not assigned!  Please assign the itemDestoryEffect prefab in the Inspector.");
        }


        // Destroy the item itself.
        Destroy(gameObject);
    }
}
