using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    /*public static Matrix4x4 isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));

    public static Vector3 ToIsometric(this Vector3 position) => isoMatrix.MultiplyPoint3x4(position);*/

    public static int GetHitDirection(Vector3 hitDirection)
    {
        float angle = Vector3.SignedAngle(Vector3.forward, hitDirection, Vector3.up);
        int dirAngle = (int)(Mathf.Round(angle / 45) * 45f);

        dirAngle = dirAngle < 0 ? 360 + dirAngle : dirAngle;

        return dirAngle;
    }

    public static Vector3 GetHitDirection(Entity.Direction main, Entity.Direction other)
    {
        int angle = (int)other % 360;
        angle = (int)main - angle;

        return AngleToVector3(angle);
    }

    public static bool IsFrontalHit(Entity.Direction main, Entity.Direction other)
    {
        Vector3 angle = GetHitDirection(main, other);

        if (angle == Vector3.back)
            return true;
        else
            return false;
    }

    public static void GetDirection()
    {

    }

    public static Vector3 AngleToVector3(int angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
