using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ComponentPanelController : MonoBehaviour
{

    public Image componentIcon_Display;
    public TextMeshProUGUI componentName_Display;
    public TextMeshProUGUI componentCost_Display;

    private Sprite componentImage;
    private string componentName;
    private int? componentCost;

    public Sprite defaultIconImage;
    private string defaultComponentName = "-";
    private string defaultComponentCost = "-";

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildComponent(Sprite icon, string name, int? cost = null)
    {
        if(componentImage != null)
        {
            componentImage = icon;
        }
        else
        {
            componentImage = defaultIconImage;
        }
        
        if(!String.IsNullOrEmpty(name))
        {
            componentName = name;
        }
        
        if(cost != null)
        {
            componentCost = cost;
        }
        

        UpdateDisplay();

    }

    void UpdateDisplay()
    {
        
        componentIcon_Display.sprite = componentImage;
        componentName_Display.text = componentName;
        componentCost_Display.text = "$" + componentCost.ToString();
    }
}
