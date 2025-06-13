using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null) { Instance = this; }
    }

    // visuals
    private Dictionary<string, RectTransform> dynamicTransforms = new Dictionary<string, RectTransform>();
    private Dictionary<string, Image> dynamicImages = new Dictionary<string, Image>();
    private Dictionary<string, TextMeshProUGUI> dynamicTextMeshes = new Dictionary<string, TextMeshProUGUI>();
    // interactables
    private Dictionary<string, Button> dynamicButtons = new Dictionary<string, Button>();
    private Dictionary<string, TMP_InputField> dynamicInputFields = new Dictionary<string, TMP_InputField>();
    // custom
    private Dictionary<string, BaseUIController> dynamicUIControllers = new Dictionary<string, BaseUIController>();

    // generic information
    private Type[] implementedTypes =
    {
        typeof(RectTransform),
        typeof(Image),
        typeof(TextMeshProUGUI),

        typeof(Button),
        typeof(TMP_InputField),
    };

    public void AddReference<T>(string id, T type)
    {
        if (!implementedTypes.Contains(typeof(T)))
        {
            Debug.LogWarning($"Generic type {typeof(T).ToString()} not implemented! Ignoring interface reference creation");
            return;
        }

        if (typeof(T) == typeof(RectTransform))
        {
            if (!dynamicTransforms.ContainsKey(id))
            {
                dynamicTransforms.Add(id, type as RectTransform);
            }
        }
        else if (typeof(T) == typeof(Image))
        {
            if (!dynamicTransforms.ContainsKey(id))
            {
                dynamicImages.Add(id, type as Image);
            }
        }
        else if (typeof(T) == typeof(TextMeshProUGUI))
        {
            if (!dynamicTextMeshes.ContainsKey(id))
            {
                dynamicTextMeshes.Add(id, type as TextMeshProUGUI);
            }
        }
        else if (typeof(T) == typeof(Button))
        {
            if (!dynamicTransforms.ContainsKey(id))
            {
                dynamicButtons.Add(id, type as Button);
            }
        }
        else if (typeof(T) == typeof(TMP_InputField))
        {
            if (!dynamicInputFields.ContainsKey(id))
            {
                dynamicInputFields.Add(id, type as TMP_InputField);
            }
        }
        else if (typeof(T) == typeof(BaseUIController))
        {
            if (!dynamicUIControllers.ContainsKey(id))
            {
                dynamicUIControllers.Add(id, type as BaseUIController);
            }
        }
    }
    public void RemoveReference<T>(string id, T type)
    {
        if (!implementedTypes.Contains(typeof(T)))
        {
            Debug.LogWarning($"Generic type {typeof(T).ToString()} not implemented! Ignoring interface reference creation");
            return;
        }

        if (typeof(T) == typeof(RectTransform))
        {
            if (dynamicTransforms.ContainsKey(id))
            {
                dynamicTransforms.Remove(id);
            }
        }
        else if (typeof(T) == typeof(Image))
        {
            if (dynamicTransforms.ContainsKey(id))
            {
                dynamicImages.Remove(id);
            }
        }
        else if (typeof(T) == typeof(TextMeshProUGUI))
        {
            if (dynamicTextMeshes.ContainsKey(id))
            {
                dynamicTextMeshes.Remove(id);
            }
        }
        else if (typeof(T) == typeof(Button))
        {
            if (dynamicTransforms.ContainsKey(id))
            {
                dynamicButtons.Remove(id);
            }
        }
        else if (typeof(T) == typeof(TMP_InputField))
        {
            if (dynamicInputFields.ContainsKey(id))
            {
                dynamicInputFields.Remove(id);
            }
        }
        else if (typeof(T) == typeof(BaseUIController))
        {
            if (dynamicUIControllers.ContainsKey(id))
            {
                dynamicUIControllers.Remove(id);
            }
        }
    }

    // textmesh helpers
    public bool SetText(string id, string text)
    {
        if (dynamicTextMeshes.ContainsKey(id))
        {
            dynamicTextMeshes[id].text = text;
            return true;
        }
        return false;
    }
}
