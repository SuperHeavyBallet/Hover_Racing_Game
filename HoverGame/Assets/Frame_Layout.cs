using System.Collections.Generic;
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

    public Transform extraFrontPosition;
    public Transform extraSlotLeftPosition;
    public Transform extraSlotRightPosition;


    public FrameSize frameSize = FrameSize.medium;

    public GameObject[] leftBoosters;
    public GameObject[] rightBoosters;

    List<SideBoosterController> sideBoosterControllers_Right = new List<SideBoosterController>();
    List<SideBoosterController> sideBoosterControllers_Left = new List<SideBoosterController>();


    private void Awake()
    {
       AssignBoosterControllers(leftBoosters, sideBoosterControllers_Left);
        AssignBoosterControllers(rightBoosters, sideBoosterControllers_Right);
    }

    void AssignBoosterControllers(GameObject[] boostersArray, List<SideBoosterController> sideBoosterList)
    {
        foreach (GameObject booster in boostersArray)
        {
            SideBoosterController controller = booster.GetComponent<SideBoosterController>();
            if (controller != null)
            {
                sideBoosterList.Add(controller);
            }
        }
    }

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

    public Transform GetExtraFrontPosition()
    {
        return extraFrontPosition;
    }

    public Transform GetExtraLeftPosition()
    {
        return extraSlotLeftPosition;
    }

    public Transform GetExtraRightPosition()
    {
        return extraSlotRightPosition;
    }

    public Transform GetFrameEnginePosition()
    {
        return frameEnginePosition;
    }

    public string GetFrameSize()
    {
        return frameSize.ToString();
    }

    public Dictionary<string, Transform> GetFramePositions()
    {
        Dictionary<string, Transform> newFramePositions = new Dictionary<string, Transform>();

        

        return newFramePositions;
    }

    public void FireBoosters_Left(bool shouldFire)
    {
        foreach(SideBoosterController boosterController in sideBoosterControllers_Left)
        {
            boosterController.ActivateFire(shouldFire);
        }
    }

    public void FireBoosters_Right(bool shouldFire)
    {
        foreach (SideBoosterController boosterController in sideBoosterControllers_Right)
        {
            boosterController.ActivateFire(shouldFire);
        }
    }

    public List<SideBoosterController> GetBoosters_Right()
    {
        return sideBoosterControllers_Right;
    }

    public List<SideBoosterController> GetBoosters_Left()
    {
        return sideBoosterControllers_Left;
    }
}
