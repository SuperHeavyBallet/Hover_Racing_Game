using UnityEngine;
[System.Serializable]
public struct GhostFrame
{
    //Position
    public float x, y, z;

    //Rotation
    public float rotX,rotY,rotZ,rotW;

    public GhostFrame(Vector3 position, Quaternion rotation)
    {
        x = position.x;
        y = position.y;
        z = position.z;

        rotX = rotation.x;
        rotY = rotation.y;
        rotZ = rotation.z;
        rotW = rotation.w;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);

    public Quaternion ToQuaternion() => new Quaternion(rotX, rotY, rotZ, rotW);

}
