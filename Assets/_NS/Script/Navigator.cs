using UnityEngine;

public class Navigator : PlayerNearActiveBase
{
    [SerializeField] GameObject arrowObj;
    protected override void Update()
    {
        base.Update();

        Vector3 navigatorPos = transform.position;
        var target = PlayerController.Instance.target;
        if (target == null) return;

        var targetPos = target.transform;



        if (isActive)
        {
            Vector3 toTarget = new Vector3(
                targetPos.position.x - navigatorPos.x,
                0,
                targetPos.position.z - navigatorPos.z
            );
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                arrowObj.transform.rotation = Quaternion.LookRotation(toTarget);
            }
        }
        else
        {
            arrowObj.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

}
