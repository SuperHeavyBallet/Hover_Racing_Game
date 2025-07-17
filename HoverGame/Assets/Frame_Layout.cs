using UnityEngine;

public class Frame_Layout : MonoBehaviour
{

    public Transform frameEnginePosition;
    public Transform frontLeftPosition;
   
    public Transform frontRightPosition;
    public Transform backRightPosition;
    public Transform backLeftPosition;

    public Transform backLeft1Position;
    public Transform backRight1Position;


    public FrameSize frameSize = FrameSize.medium;

    public enum FrameSize
    {
        light,
        medium,
        heavy
    }

    public Transform GetFrontLeftPosition()
    {
        return frontLeftPosition;
    }

    public Transform GetFrontRightPosition()
    {
        return frontRightPosition;
    }

    public Transform GetBackLeftPosition()
    {
        return backLeftPosition;
    }

    public Transform GetBackRightPosition()
    {
        return backRightPosition;
    }

    public Transform GetBackLeft1Position()
    {
        return backLeft1Position;
    }

    public Transform GetBackRight1Position()
    {
        return backRight1Position;
    }

    public Transform GetFrameEnginePosition()
    {
        return frameEnginePosition;
    }

    public string GetFrameSize()
    {
        return frameSize.ToString();
    }
}
