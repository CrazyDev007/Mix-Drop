# Abstraction Services

This directory contains interfaces and implementations for abstracting Unity-specific functionality, making it easier to test components and replace implementations if needed.

## Overview

The abstraction services provide a layer of indirection between your game code and Unity-specific APIs. This allows you to:

1. Write more testable code by mocking Unity APIs
2. Replace Unity implementations with custom ones if needed
3. Decouple your game logic from specific Unity APIs

## Services

### IUnityService

Provides abstraction for core Unity functionality:

- Time management (Time, DeltaTime, TimeScale)
- GameObject operations (Instantiate, Destroy, Find)
- Scene management (LoadScene, LoadSceneAsync)
- Application control (Quit)
- Logging (Log, LogWarning, LogError)

### IUIService

Provides abstraction for UI functionality using Unity's UI Toolkit:

- Loading UXML and USS assets
- Creating and manipulating visual elements
- Handling UI events (clicks, etc.)
- Managing UI state (visibility, enabled state)

### IAudioService

Provides abstraction for audio functionality:

- Playing sound effects and background music
- Managing audio volume and muting
- Loading audio clips synchronously and asynchronously

### IInputService

Provides abstraction for input functionality:

- Handling keyboard, mouse, and touch input
- Managing input actions and callbacks
- Enabling/disabling input processing

## Usage

### Getting Services

Services are registered with the dependency injection container and can be accessed through the ServiceLocator:

```csharp
// Get a service
var unityService = ServiceLocator.Current.Resolve<IUnityService>();
var uiService = ServiceLocator.Current.Resolve<IUIService>();
var audioService = ServiceLocator.Current.Resolve<IAudioService>();
var inputService = ServiceLocator.Current.Resolve<IInputService>();
```

### Using Services

Once you have a reference to a service, you can use its methods:

```csharp
// Using UnityService
unityService.Instantiate(prefab);
unityService.LoadScene("GameScene");
unityService.Log("Hello, world!");

// Using UIService
var button = uiService.Query<Button>(rootElement, "MyButton");
uiService.RegisterClickCallback(button, OnButtonClicked);

// Using AudioService
audioService.PlaySoundEffect(clickSound);
audioService.PlayMusic(backgroundMusic);
audioService.SetSoundEffectVolume(0.5f);

// Using InputService
inputService.RegisterInputActionCallback("Jump", OnJumpAction);
bool isJumpPressed = inputService.IsButtonPressed("Jump");
```

### Example Component

See `Assets/Scripts/Examples/AbstractionServicesExample.cs` for a complete example of how to use the abstraction services in a component.

## Custom Implementations

You can provide custom implementations of the service interfaces by:

1. Creating a class that implements the interface
2. Setting the custom implementation on the ServiceManager:

```csharp
// Create a custom implementation
var customUnityService = new CustomUnityService();

// Set it on the ServiceManager
ServiceManager.Instance.SetUnityService(customUnityService);
```

## Testing

The abstraction services make it easier to test your components by allowing you to mock Unity APIs:

```csharp
// Create a mock service
var mockUnityService = new MockUnityService();

// Set up the mock to return specific values
mockUnityService.Time.Returns(10.0f);

// Inject the mock into your component
component.UnityService = mockUnityService;

// Test your component
component.Update();
```

## ServiceManager

The `ServiceManager` class provides a centralized point for managing all abstraction services. It:

- Ensures services are initialized as singletons
- Provides easy access to all services
- Allows custom implementations to be set
- Registers services with the dependency injection container

The ServiceManager is automatically created and initialized when the DependencyInjectionBootstrap runs.