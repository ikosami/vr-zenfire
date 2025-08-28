using UnityEngine;

public class PhysicsHelper : MonoBehaviour
{
    public static readonly int LAYER_NPC = LayerMask.NameToLayer("NPC");
    public static readonly int LAYER_PLAYER = LayerMask.NameToLayer("Player");
    public static readonly int LAYER_PLAYER_CAR = LayerMask.NameToLayer("PlayerCar");
    public static readonly int LAYER_OBSTACLE = LayerMask.NameToLayer("Default");

}
