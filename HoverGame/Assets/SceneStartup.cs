using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This Script is where the ship components are decided upon in the main menu
/// </summary>
/// 
public class SceneStartup : MonoBehaviour
{


    public TextMeshProUGUI frameText;
    public TextMeshProUGUI fLText;
    public TextMeshProUGUI fRText;
    public TextMeshProUGUI bLText;
    public TextMeshProUGUI bRText;
    public TextMeshProUGUI bL1Text;
    public TextMeshProUGUI bR1Text;
    public TextMeshProUGUI extraFrontText;
    public TextMeshProUGUI extraLeftText;
    public TextMeshProUGUI extraRightText;

    public GameObject engine;
    public GameObject jetEngine;
    public GameObject aireon;
    public GameObject lightFrame;
    public GameObject mediumFrame;
    public GameObject heavyFrame;
    public GameObject fuelTank;
    public GameObject boostGulp;


    public Transform framePosition;
    public Transform fLPosition;
    public Transform fRPosition;
    public Transform bLPosition;
    public Transform bRPosition;
    public Transform bL1Position;
    public Transform bR1Position;
    public Transform extraFrontPosition;
    public Transform extraLeftPosition;
    public Transform extraRightPosition;

    Ship_Passport shipPassport;

    public GameObject backLeft1Option;
    public GameObject backRight1Option;
    public GameObject backLeft1Label;
    public GameObject backRight1Label;

    
  



    public Dictionary<ComponentSlotType, ComponentSlot> componentSlots = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitializeComponentSlots();

        GenerateShip();

        DisplayComponentMeshes();    
    }

    private void Start()
    {
        shipPassport = GameObject.Find("ShipPassport").GetComponent<Ship_Passport>();  
    }

    public void GenerateShip()
    {
        SetComponentSlot(ComponentSlotType.Frame, ComponentName.mediumFrame);
        TestFrameType();
        SetComponentSlot(ComponentSlotType.FrontLeft, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.FrontRight, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackLeft, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackRight, ComponentName.engine);
        SetComponentSlot(ComponentSlotType.BackLeft1, ComponentName.empty);
        SetComponentSlot(ComponentSlotType.BackRight1, ComponentName.empty);
        SetComponentSlot(ComponentSlotType.ExtraFront, ComponentName.boostGulp);
        SetComponentSlot(ComponentSlotType.ExtraLeft, ComponentName.fuelTank);
        SetComponentSlot(ComponentSlotType.ExtraRight, ComponentName.fuelTank);
    }

    void DisplayComponentMeshes()
    {
        foreach (var pair in componentSlots)
        {
            var selectedKey = pair.Value.selectedComponentKey;

            if (selectedKey == ComponentName.empty) continue;

            if (pair.Value.components.TryGetValue(selectedKey, out GameObject prefab))
            {
                GameObject newComponent = Instantiate(prefab, pair.Value.position);
                newComponent.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateComponent_Frame(ComponentName newComponentName)
    {
        if (componentSlots.TryGetValue(ComponentSlotType.Frame, out var frameSlot))
        {
            if (frameSlot.selectedComponentKey != newComponentName)
            {
                foreach (Transform child in frameSlot.position)
                {
                    Destroy(child.gameObject);
                }

                SetComponentSlot(ComponentSlotType.Frame, newComponentName);

                DisplayComponentMeshes();
            }
        }

        TestFrameType();
    }

    ComponentName GetFrameType(int val)
    {
        ComponentName replacementComponent = ComponentName.empty;

        switch(val)
        {
            case 0: replacementComponent = ComponentName.lightFrame; break;
            case 1: replacementComponent = ComponentName.mediumFrame; break;
            case 2: replacementComponent = ComponentName.heavyFrame; break;
            default: replacementComponent = ComponentName.empty; break;
        }

        return replacementComponent;
    }

    ComponentName GetComponentName(int val)
    {


        ComponentName replacementComponent = ComponentName.empty;

        switch (val)
        {
            case 0: replacementComponent = ComponentName.engine; break;
            case 1: replacementComponent = ComponentName.jetEngine; break;
            case 2: replacementComponent = ComponentName.aireon; break;
            case 3: replacementComponent = ComponentName.fuelTank; break;
            case 4: replacementComponent = ComponentName.boostGulp; break;
            default: replacementComponent = ComponentName.empty; break;
        }

        return replacementComponent;
    }

 

    void UpdateComponent(ComponentSlotType slotType, ComponentName replacementComponent)
    {
        if (componentSlots.TryGetValue(slotType, out var slotPosition))
        {
            if (slotPosition.selectedComponentKey != replacementComponent)
            {
                foreach (Transform child in slotPosition.position)
                {
                    Destroy(child.gameObject);
                }

                SetComponentSlot(slotType, replacementComponent);
                DisplayComponentMeshes();

                if(slotType == ComponentSlotType.Frame)
                {
                    TestFrameType();
                }
            }
        }

    }

    public void UpdateComponentSlot_FRAME(int val)
    {
        ComponentName replacementComponent = GetFrameType(val);
       // Debug.Log("Frame selection changed to: " + replacementComponent); // <—
        UpdateComponent(ComponentSlotType.Frame, replacementComponent);
        
    }
    public void UpdateComponentSlot_FL(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.FrontLeft, replacementComponent);
    }

    public void UpdateComponentSlot_FR(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.FrontRight, replacementComponent);
    }

    public void UpdateComponentSlot_BL(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackLeft, replacementComponent);
    }

    public void UpdateComponentSlot_BR(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackRight, replacementComponent);
    }

    public void UpdateComponentSlot_BL1(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackLeft1, replacementComponent);
    }

    public void UpdateComponentSlot_BR1(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.BackRight1, replacementComponent);
    }

    public void UpdateComponentSlot_ExtraFront(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.ExtraFront, replacementComponent);
    }

    public void UpdateComponentSlot_ExtraLeft(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.ExtraLeft, replacementComponent);
    }

    public void UpdateComponentSlot_ExtraRight(int val)
    {
        ComponentName replacementComponent = GetComponentName(val);

        UpdateComponent(ComponentSlotType.ExtraRight, replacementComponent);
    }

    void InitializeComponentSlots()
    {
        componentSlots[ComponentSlotType.Frame] = new ComponentSlot
        {
            label = frameText,
            position = framePosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.lightFrame, lightFrame },
                { ComponentName.mediumFrame, mediumFrame },
                { ComponentName.heavyFrame , heavyFrame },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.FrontLeft] = new ComponentSlot
        {
            label = fLText,
            position = fLPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.FrontRight] = new ComponentSlot
        {
            label = fRText,
            position = fRPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.BackLeft] = new ComponentSlot
        {
            label = bLText,
            position = bLPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.BackRight] = new ComponentSlot
        {
            label = bRText,
            position = bRPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.BackLeft1] = new ComponentSlot
        {
            label = bL1Text,
            position = bL1Position,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.BackRight1] = new ComponentSlot
        {
            label = bR1Text,
            position = bR1Position,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.ExtraFront] = new ComponentSlot
        {
            label = extraFrontText,
            position = extraFrontPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.fuelTank, fuelTank },
                { ComponentName.boostGulp, boostGulp },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.ExtraLeft] = new ComponentSlot
        {
            label = extraLeftText,
            position = extraLeftPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.fuelTank, fuelTank },
                { ComponentName.empty , null }
            }
        };

        componentSlots[ComponentSlotType.ExtraRight] = new ComponentSlot
        {
            label = extraRightText,
            position = extraRightPosition,
            components = new Dictionary<ComponentName, GameObject>
            {
                { ComponentName.engine, engine },
                { ComponentName.jetEngine, jetEngine },
                { ComponentName.aireon , aireon },
                { ComponentName.fuelTank, fuelTank },
                { ComponentName.empty , null }
            }
        };

    }




    void SetComponentSlot(ComponentSlotType slot, ComponentName componentKey)
    {
        if (!componentSlots.TryGetValue(slot, out var slotData)) return;

        if (slotData.label != null)
            slotData.label.text = componentKey.ToString();
        slotData.selectedComponentKey = componentKey;

        foreach (var kvp in slotData.components)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetActive(kvp.Key == componentKey);
            }
        }

    }

    


    public void SetShipLoadout()
    {
        var shipLoadout = new Dictionary<ComponentSlotType, ComponentName>();

        foreach (var pair in componentSlots)
        {
            shipLoadout[pair.Key] = pair.Value.selectedComponentKey;
        }
        Debug.Log("SET LOADOUT:");
        Debug.Log(shipLoadout.Count);

        shipPassport.ReceiveShipLoadout(shipLoadout);

        
    }

    public void TestFrameType()
    {
        bool isHeavyFrame = false;
        foreach (var pair in componentSlots)
        {
            if (pair.Key == ComponentSlotType.Frame)
            {
                if (pair.Value.selectedComponentKey == ComponentName.heavyFrame)
                {
                    isHeavyFrame = true;
                    break;
                }
            }
        }

        if (!isHeavyFrame)
        {
            DisableOptionsForExtraSlots();
        }
        else
        {
            EnableOptionsForExtraSlots();
        }
    }


    void DisableOptionsForExtraSlots()
    {
        //Debug.Log("Disabling extras. backLeft1Option is " + backLeft1Option);
        backLeft1Option.gameObject.SetActive(false);
        backLeft1Label.gameObject.SetActive(false);
        bL1Text.gameObject.SetActive(false);
        bR1Text.gameObject.SetActive(false);
        backRight1Option.gameObject.SetActive(false);
        backRight1Label.gameObject.SetActive(false);
    }

    void EnableOptionsForExtraSlots()
    {
        //Debug.Log("Enabling extras. backLeft1Option is " + backLeft1Option);
        backLeft1Option.gameObject.SetActive(true);
        backLeft1Label.gameObject.SetActive(true);
        bL1Text.gameObject.SetActive(true);
        bR1Text.gameObject.SetActive(true);
        backRight1Option.gameObject.SetActive(true);
        backRight1Label.gameObject.SetActive(true);
    }

    public void LoadScene(string sceneName)
    {
        SetShipLoadout();
        StartCoroutine(LoadSceneDelayed(sceneName));
    }

    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return 0.1f; // one frame delay
        SceneManager.LoadScene(sceneName);
    }
}
