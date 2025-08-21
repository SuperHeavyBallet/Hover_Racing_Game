using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentCatalogue_Wrapper : MonoBehaviour
{

    [SerializeField] private ComponentCatalogue _componentCatalogue;

    private Dictionary<string, ComponentDefinition> _byId;

   
    public ComponentDefinition Get_ComponentBy_ID(string componentID)
    {
        EnsureBuilt();
        return _componentCatalogue.GetById(componentID);
    }

    public IReadOnlyList<ComponentDefinition> Get_ComponentBy_Category(ComponentCategory componentCategory)
    {
        EnsureBuilt();
        return _componentCatalogue.GetByCategory(componentCategory);
    }

    public string GET_EmptyComponentID_AsString()
    {
        return _componentCatalogue.GET_EmptyComponentID_AsString();
    }

    public string GET_FrameID_AsString(string frameWeight)
    {
        return _componentCatalogue.GET_FrameID_AsString(frameWeight);
    }


    public void EnsureBuilt()
    {
        _componentCatalogue.EnsureBuilt();
    }

  
}
