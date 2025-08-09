using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Component Catalogue")]
public class ComponentCatalogue : ScriptableObject
{
    public List<ComponentDefinition> components = new();


    //Built at runtime
    private Dictionary<string, ComponentDefinition> byId;
    private Dictionary<ComponentCategory, List<ComponentDefinition>> byCategory;

    public void BuildLookups()
    {
        byId = new();
        byCategory = new()
        {
            { ComponentCategory.Frame, new() },
            { ComponentCategory.Engine, new() },
            { ComponentCategory.Extra, new() },
            { ComponentCategory.ExtraTop, new() },
        };

        foreach (var component in components)
        {
            if(component == null || string.IsNullOrWhiteSpace(component.id)) continue;

            byId[component.id] = component;
            byCategory[component.category].Add(component);
        }
    }

    public ComponentDefinition GetById(string id)
    {
        if(byId != null && byId.TryGetValue(id, out var component)) return component;
        else return null;
    }

    public IReadOnlyList<ComponentDefinition> GetByCategory(ComponentCategory cat)
    {
        if(byCategory != null)
        {
            return byCategory[cat];
        }
        else
        {
            return System.Array.Empty<ComponentDefinition>();
        }
    }
}
