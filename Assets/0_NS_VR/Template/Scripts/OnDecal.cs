using UnityEngine;

public class OnDecal : MonoBehaviour
{
    public System.Action _OnTrigger;
    public void TriggerEvent() {
        _OnTrigger?.Invoke();
    }

}
