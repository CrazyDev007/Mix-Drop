using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Implementation of the UI service interface
/// </summary>
public class UIService : MonoBehaviour, IUIService
{
    private UIDocument _uiDocument;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null)
        {
            _uiDocument = gameObject.AddComponent<UIDocument>();
        }
    }
    
    /// <summary>
    /// Gets the root visual element of the UI
    /// </summary>
    public VisualElement RootVisualElement => _uiDocument.rootVisualElement;
    
    /// <summary>
    /// Loads a UXML asset
    /// </summary>
    /// <param name="path">The path to the UXML asset</param>
    /// <returns>The loaded VisualTreeAsset</returns>
    public VisualTreeAsset LoadUXML(string path)
    {
        return Resources.Load<VisualTreeAsset>(path);
    }
    
    /// <summary>
    /// Loads a USS asset
    /// </summary>
    /// <param name="path">The path to the USS asset</param>
    /// <returns>The loaded StyleSheet</returns>
    public StyleSheet LoadUSS(string path)
    {
        return Resources.Load<StyleSheet>(path);
    }
    
    /// <summary>
    /// Instantiates a UXML asset
    /// </summary>
    /// <param name="asset">The VisualTreeAsset to instantiate</param>
    /// <returns>The instantiated VisualElement</returns>
    public VisualElement InstantiateUXML(VisualTreeAsset asset)
    {
        return asset.Instantiate();
    }
    
    /// <summary>
    /// Adds a style sheet to a visual element
    /// </summary>
    /// <param name="element">The visual element to add the style sheet to</param>
    /// <param name="styleSheet">The style sheet to add</param>
    public void AddStyleSheet(VisualElement element, StyleSheet styleSheet)
    {
        if (element != null && styleSheet != null)
        {
            element.styleSheets.Add(styleSheet);
        }
    }
    
    /// <summary>
    /// Queries a visual element by its name
    /// </summary>
    /// <param name="parent">The parent visual element</param>
    /// <param name="name">The name of the element to query</param>
    /// <returns>The queried visual element</returns>
    public VisualElement Query(VisualElement parent, string name)
    {
        return parent?.Q(name);
    }
    
    /// <summary>
    /// Queries a visual element by its type
    /// </summary>
    /// <typeparam name="T">The type of the element to query</typeparam>
    /// <param name="parent">The parent visual element</param>
    /// <param name="name">The name of the element to query</param>
    /// <returns>The queried visual element</returns>
    public T Query<T>(VisualElement parent, string name) where T : VisualElement
    {
        return parent?.Q<T>(name);
    }
    
    /// <summary>
    /// Registers a callback for a click event
    /// </summary>
    /// <param name="element">The visual element to register the callback for</param>
    /// <param name="callback">The callback to register</param>
    public void RegisterClickCallback(VisualElement element, Action<ClickEvent> callback)
    {
        if (element != null && callback != null)
        {
            element.RegisterCallback<ClickEvent>(new EventCallback<ClickEvent>(callback));
        }
    }
    
    /// <summary>
    /// Unregisters a callback for a click event
    /// </summary>
    /// <param name="element">The visual element to unregister the callback for</param>
    /// <param name="callback">The callback to unregister</param>
    public void UnregisterClickCallback(VisualElement element, Action<ClickEvent> callback)
    {
        if (element != null && callback != null)
        {
            element.UnregisterCallback<ClickEvent>(new EventCallback<ClickEvent>(callback));
        }
    }
    
    /// <summary>
    /// Sets the text of a label
    /// </summary>
    /// <param name="label">The label to set the text for</param>
    /// <param name="text">The text to set</param>
    public void SetLabelText(Label label, string text)
    {
        if (label != null)
        {
            label.text = text;
        }
    }
    
    /// <summary>
    /// Gets the text of a label
    /// </summary>
    /// <param name="label">The label to get the text from</param>
    /// <returns>The text of the label</returns>
    public string GetLabelText(Label label)
    {
        return label?.text ?? string.Empty;
    }
    
    /// <summary>
    /// Sets the visibility of a visual element
    /// </summary>
    /// <param name="element">The visual element to set the visibility for</param>
    /// <param name="visible">Whether the element should be visible</param>
    public void SetVisibility(VisualElement element, bool visible)
    {
        if (element != null)
        {
            element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
    
    /// <summary>
    /// Gets the visibility of a visual element
    /// </summary>
    /// <param name="element">The visual element to get the visibility from</param>
    /// <returns>Whether the element is visible</returns>
    public bool GetVisibility(VisualElement element)
    {
        return element?.style.display == DisplayStyle.Flex;
    }
    
    /// <summary>
    /// Sets the enabled state of a visual element
    /// </summary>
    /// <param name="element">The visual element to set the enabled state for</param>
    /// <param name="enabled">Whether the element should be enabled</param>
    public void SetEnabled(VisualElement element, bool enabled)
    {
        if (element != null)
        {
            element.SetEnabled(enabled);
        }
    }
    
    /// <summary>
    /// Gets the enabled state of a visual element
    /// </summary>
    /// <param name="element">The visual element to get the enabled state from</param>
    /// <returns>Whether the element is enabled</returns>
    public bool GetEnabled(VisualElement element)
    {
        return element?.enabledSelf ?? false;
    }
    
    /// <summary>
    /// Sets the focus on a visual element
    /// </summary>
    /// <param name="element">The visual element to focus</param>
    public void SetFocus(VisualElement element)
    {
        if (element != null)
        {
            element.Focus();
        }
    }
    
    /// <summary>
    /// Shows a tooltip
    /// </summary>
    /// <param name="element">The visual element to show the tooltip for</param>
    /// <param name="text">The text of the tooltip</param>
    public void ShowTooltip(VisualElement element, string text)
    {
        if (element != null)
        {
            element.tooltip = text;
        }
    }
    
    /// <summary>
    /// Hides a tooltip
    /// </summary>
    /// <param name="element">The visual element to hide the tooltip for</param>
    public void HideTooltip(VisualElement element)
    {
        if (element != null)
        {
            element.tooltip = string.Empty;
        }
    }
}