using UnityEngine;


public class FixPosition : MonoBehaviour
{

    public bool X = false;
    public bool Y = false;
    public bool Z = false;

    public Vector3 Position = new Vector3(0, 0, 0);

    void Start()
    {
    }


    void FixedUpdate()
    {
        Fix();
    }

    void Fix()
    {
        var pos = transform.position;
        if(X) pos.x = Position.x;
        if(Y) pos.y = Position.y;
        if(Z) pos.z = Position.z;
        transform.position = pos;
    }
}