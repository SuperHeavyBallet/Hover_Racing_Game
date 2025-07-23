using UnityEngine;

public interface IEngineFireListener
{
    void OnShipEngineFiring(bool isFiring);

    void OnShipBoostFiring(bool isFiring);

    void OnShipRotateNozzle(float turnAmount);



}
