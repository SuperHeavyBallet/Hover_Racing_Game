using UnityEngine;

public class ShipMeshSelector : MonoBehaviour
{
    public GameObject lightShipMesh;
    public GameObject medShipMesh;
    public GameObject heavyShipMesh;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShipMeshSelect(string shipClass)
    {
        switch(shipClass)
        {
            case "Light":
                lightShipMesh.SetActive(true);
                medShipMesh.SetActive(false);
                heavyShipMesh.SetActive(false);
                break;
            case "Medium":
                lightShipMesh.SetActive(false);
                medShipMesh.SetActive(true);
                heavyShipMesh.SetActive(false);
                break;
            case "Heavy":
                lightShipMesh.SetActive(false);
                medShipMesh.SetActive(false);
                heavyShipMesh.SetActive(true);
                break;
            default:
                lightShipMesh.SetActive(false);
                medShipMesh.SetActive(true);
                heavyShipMesh.SetActive(false);
                break;
        }
    }
}
