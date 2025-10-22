using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interface for UI-specific Unity services that need to be abstracted
/// </summary>
public interface IUIService
{
    /// <summary>
    /// Gets the root visual element of the UI
    /// </summary>
    VisualElement RootVisualElement { get; }
    
    /// <summary>
    /// Loads a UXML asset
    /// </summary>
    /// <param name="path">The path to the UXML asset</param>
    /// <returns>The loaded VisualTreeAsset</returns>
    VisualTreeAsset LoadUXML(string path);
    
    /// <summary>
    /// Loads a USS asset
    /// </summary>
    /// <param name="path">The path to the USS asset</param>
    /// <returns>The loaded StyleSheet</returns>
    StyleSheet LoadUSS(string path);
    
    /// <summary>
    /// Instantiates a UXML asset
    /// </summary>
    /// <param name="asset">The VisualTreeAsset to instantiate</param>
    /// <returns>The instantiated VisualElement</returns>
    VisualElement InstantiateUXML(VisualTreeAsset asset);
    
    /// <summary>
    /// Adds a style sheet to a visual element
    /// </summary>
    /// <param name="element">The visual element to add the style sheet to</param>
    /// <param name="styleSheet">The style sheet to add</param>
    void AddStyleSheet(VisualElement element, StyleSheet styleSheet);
    
    /// <summary>
    /// Queries a visual element by its name
    /// </summary>
    /// <param name="parent">The parent visual element</param>
    /// <param name="name">The name of the element to query</param>
    /// <returns>The queried visual element</returns>
    VisualElement Query(VisualElement parent, string name);
    
    /// <summary>
    /// Queries a visual element by its type
    /// </summary>
    /// <typeparam name="T">The type of the element to query</typeparam>
    /// <param name="parent">The parent visual element</param>
    /// <param name="name">The name of the element to query</param>
    /// <returns>The queried visual element</returns>
    T Query<T>(VisualElement parent, string name) where T : VisualElement;
    
    /// <summary>
    /// Registers a callback for a click event
    /// </summary>
    /// <param name="element">The visual element to register the callback for</param>
    /// <param name="callback">The callback to register</param>
    void RegisterClickCallback(VisualElement element, Action<ClickEvent> callback);
    
    /// <summary>
    /// Unregisters a callback for a click event
    /// </summary>
    /// <param name="element">The visual element to unregister the callback for</param>
    /// <param name="callback">The callback to unregister</param>
    void UnregisterClickCallback(VisualElement element, Action<ClickEvent> callback);
    
    /// <summary>
    /// Sets the text of a label
    /// </summary>
    /// <param name="label">The label to set the text for</param>
    /// <param name="text">The text to set</param>
    void SetLabelText(Label label, string text);
    
    /// <summary>
    /// Gets the text of a label
    /// </summary>
    /// <param name="label">The label to get the text from</param>
    /// <returns>The text of the label</returns>
    string GetLabelText(Label label);
    
    /// <summary>
    /// Sets the visibility of a visual element
    /// </summary>
    /// <param name="element">The visual element to set the visibility for</param>
    /// <param name="visible">Whether the element should be visible</param>
    void SetVisibility(VisualElement element, bool visible);
    
    /// <summary>
    /// Gets the visibility of a visual element
    /// </summary>
    /// <param name="element">The visual element to get the visibility from</param>
    /// <returns>Whether the element is visible</returns>
    bool GetVisibility(VisualElement element);
    
    /// <summary>
    /// Sets the enabled state of a visual element
    /// </summary>
    /// <param name="element">The visual element to set the enabled state for</param>
    /// <param name="enabled">Whether the element should be enabled</param>
    void SetEnabled(VisualElement element, bool enabled);
    
    /// <summary>
    /// Gets the enabled state of a visual element
    /// </summary>
    /// <param name="element">The visual element to get the enabled state from</param>
    /// <returns>Whether the element is enabled</returns>
    bool GetEnabled(VisualElement element);
    
    /// <summary>
    /// Sets the focus on a visual element
    /// </summary>
    /// <param name="element">The visual element to focus</param>
    void SetFocus(VisualElement element);
    
    /// <summary>
    /// Shows a tooltip
    /// </summary>
    /// <param name="element">The visual element to show the tooltip for</param>
    /// <param name="text">The text of the tooltip</param>
    void ShowTooltip(VisualElement element, string text);
    
    /// <summary>
    /// Hides a tooltip
    /// </summary>
    /// <param name="element">The visual element to hide the tooltip for</param>
    void HideTooltip(VisualElement element);
}