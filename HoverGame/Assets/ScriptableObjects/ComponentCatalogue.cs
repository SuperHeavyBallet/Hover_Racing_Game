using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Component Catalogue")]
public class ComponentCatalogue : ScriptableObject
{
    /// <summary>
    /// This catalogue will be the monolith reference for other scripts to pull from
    /// </summary>
    /// Components are assigned in the inspector - this should be the extent of adding a new component
    public List<ComponentDefinition> components = new();


    //Built at runtime
    private Dictionary<string, ComponentDefinition> byId;
    private Dictionary<ComponentCategory, List<ComponentDefinition>> byCategory;
    bool built;



    // Later set these to pull from the components themselves
    //public string EMPTY_ID;
    //public string FRAME_LIGHT_ID;
    //public string FRAME_MEDIUM_ID;
    //public string FRAME_HEAVY_ID;

    [SerializeField] private ComponentDefinition LightFrameDefinition;
    [SerializeField] private ComponentDefinition MediumFrameDefinition;
    [SerializeField] private ComponentDefinition HeavyFrameDefinition;
    [SerializeField] private ComponentDefinition EmptyComponentDefinition;

    // Self enforced Init, Ensure via other calling scripts

    void OnEnable() => built = false;

    private void Awake()
    {
        //AssignIDs(EMPTY_ID, EmptyComponentDefinition);
        //AssignIDs(FRAME_LIGHT_ID, LightFrameDefinition);
        //AssignIDs(FRAME_MEDIUM_ID, MediumFrameDefinition);
        //AssignIDs(FRAME_HEAVY_ID, HeavyFrameDefinition);
    }

    void AssignIDs(string id_String, ComponentDefinition componentDefinition)
    {
        if(componentDefinition.id != null)
        {
            id_String = componentDefinition.id;
        }
        else
        {
            Debug.LogError("Unable to assign id from: " + componentDefinition.id + " to: " +  id_String);
        }
    }

    public void EnsureBuilt()
    {
        if (built) return;
        byId = new();
        byCategory = new()
        {
            { ComponentCategory.Frame, new() },
            { ComponentCategory.Engine, new() },
            { ComponentCategory.Extra, new() },
            { ComponentCategory.ExtraTop, new() },
            { ComponentCategory.Empty, new() }
        };

        foreach (var c in components)
        {
            if (!c || string.IsNullOrWhiteSpace(c.id)) continue;
            byId[c.id] = c;
            byCategory[c.category].Add(c);
        }
        built = true;
    }

    public List<ComponentDefinition> GetListOfAllComponents()
    {
        EnsureBuilt();
        List<ComponentDefinition> componentsWithoutEmpty = new();
        foreach (var c in components)
        {
            if(c.id != EmptyComponentDefinition.id)
            {
                componentsWithoutEmpty.Add(c);
            }
        }
        return componentsWithoutEmpty;
    }


    // External Functions which query the catalogue  content
    public ComponentDefinition GetById(string id)
    {
        EnsureBuilt();

        if (byId != null)
        {
            if (byId.TryGetValue(id, out var component))
            {
                return component;
            }
        }

        return null;
    }

    public IReadOnlyList<ComponentDefinition> GetByCategory(ComponentCategory cat)
    {
        EnsureBuilt();
        if (byCategory != null)
        {
            return byCategory[cat];
        }
        else
        {
            return System.Array.Empty<ComponentDefinition>();
        }
    }

    public List<string> GetComponentList(ComponentCategory cat)
    {
        EnsureBuilt();
        List<string> componentList = new();

        foreach (ComponentDefinition componentDefinition in GetByCategory(cat))
        {
            componentList.Add(componentDefinition.id);
        }


        return componentList;
    }

    public string GET_EmptyComponentID_AsString()
    {
        return EmptyComponentDefinition.id;
    }

    public string GET_FrameID_AsString(string frameWeight)
    {
        string returnID;

        switch(frameWeight)
        {
            case "LIGHT":
                returnID = LightFrameDefinition.id; break;
            case "MEDIUM":
                returnID = MediumFrameDefinition.id; break;
            case "HEAVY":
                returnID = HeavyFrameDefinition.id; break;
            default:
                Debug.LogError("Invalid Frame Weight: " + frameWeight + " - Sent to Receive ID");
                returnID = null; break;

        }
        return returnID;
    }




}
