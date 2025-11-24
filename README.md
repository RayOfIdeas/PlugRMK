# PlugRMK

A Unity package containing editor tools and runtime utilities designed to enhance development productivity and streamline common Unity workflows.

## 🚀 Features

### Editor Tools
- **Box Collider Toolbox** - Advanced BoxCollider editing with visual handles and chain creation
- **Component Utilities** - Duplicate components, save ScriptableObjects as dirty
- **Asset Database Utilities** - Simplified asset loading and management
- **Scene Selection Overlay** - Quick scene switching toolbar overlay
- **Custom Property Drawers** - Enhanced inspector controls with lock and disable attributes
- **Simple Folder Icons** - Custom folder icons in the Project window
- **Hierarchy Extensions** - Custom styling for hierarchy items
- **Vector3 Context Menu** - Quick set Vector3 values to zero/one from context menu

### Runtime Utilities

#### Generic Utilities
- **String Utility** - Comprehensive string manipulation methods (split, extract, replace, encode/decode)
- **Math Utility** - Vector operations, angle calculations, intersection detection, time formatting
- **List Utility** - List manipulation and utility methods
- **Dictionary Utility** - Dictionary helper methods
- **Number Display Utility** - Number formatting and display helpers
- **Struct Utility** - Struct manipulation utilities

#### Unity-Specific Utilities
- **Game Utilities** - Audio, Color, Component, Transform, Coroutine, and LayerMask utilities
- **HierarchyExt** - Hierarchy styling system
- **Property Attributes** - Custom attributes for inspector (`[Lock]`, `[Disable]`)

## 📦 Installation

### Via Unity Package Manager (Git URL)
1. Open Unity Package Manager
2. Click the `+` button and select "Add package from git URL"
3. Enter: `https://github.com/your-repo/PlugRMK.git`

### Via Unity Package Manager (Local)
1. Download or clone this repository
2. Open Unity Package Manager
3. Click the `+` button and select "Add package from disk"
4. Navigate to the package folder and select `package.json`

## 🛠️ Usage

### Editor Tools

#### Box Collider Toolbox
Access via `Tools > Box Col Toolbox`

**Features:**
- Visual handles for precise BoxCollider positioning
- Chain creation tool for connecting multiple colliders
- Support for custom prefabs
- ProBuilder integration (when enabled)
- Live gizmo visualization

```csharp
// The toolbox provides both Editor and Chain modes
// Editor mode: Individual collider editing with visual handles
// Chain mode: Create connected collider chains between GameObjects
```

#### Component Duplication
Right-click any component and select "Duplicate Component" to create an exact copy.

#### Property Attributes
```csharp
using PlugRMK.UnityUti;

public class ExampleScript : MonoBehaviour 
{
    [Lock]
    public string lockedField = "Cannot edit when locked";
    
    [Disable]
    public float disabledField = 42f;
}
```

### Runtime Utilities

#### String Utilities
```csharp
using PlugRMK.GenericUti;

// Replace only first occurrence
string result = "Hello world world".ReplaceFirst("world", "Unity");
// Result: "Hello Unity world"

// Extract text between tokens
string extracted = "Hello {world}!".Extract("{", "}", "default");
// Result: "world"

// Split with escape characters
var parts = "one,\"hello,world\",three".SplitEsc(",", "\"", "\"");
// Result: ["one", "\"hello,world\"", "three"]
```

#### Math Utilities
```csharp
using PlugRMK.GenericUti;

// Vector operations
Vector2 vec = 5f.NewVector2(); // Creates Vector2(5, 5)
Vector3 randomVec = MathUtility.GetRandomVector3(Vector3.one);

// Time formatting
string timeStr = 125f.SecondsToTimeString(); // "02:05"

// Angle operations
Vector2 direction = 45f.ToVector2(); // Convert angle to direction vector
float angle = Vector2.right.ToAngle(); // Convert direction to angle
```

#### Game Utilities
```csharp
// Component utilities
var component = gameObject.GetOrAddComponent<Rigidbody>();

// Transform utilities
transform.SetPosition(x: 10f); // Set only X position
transform.SetScale(y: 2f); // Set only Y scale

// Color utilities
Color newColor = Color.red.ChangeAlpha(0.5f);
```

## 🎨 Editor Features

### Simple Folder Icons
Automatically applies custom icons to folders based on their names. Place folders with specific names to get themed icons automatically.

### Hierarchy Extensions
- Custom styling for hierarchy items
- Icon support for different object types
- Color coding and visual enhancements
- Token-based naming system for automatic styling

### Scene Selection Overlay
Quick scene switching directly from the Scene view with a convenient dropdown overlay.

## 🏗️ Package Structure

```
PlugRMK/
├── Editor/                          # Editor-only tools and utilities
│   ├── AssetDatabaseUtility.cs      # Asset loading helpers
│   ├── BoxColToolboxWindow.cs       # Advanced BoxCollider editor
│   ├── DuplicateComponent.cs        # Component duplication
│   ├── SaveAsDirtySO.cs             # ScriptableObject utilities
│   ├── SceneSelectionOverlay.cs     # Scene switching overlay
│   ├── SetIconWindow.cs             # Icon management
│   ├── Vector3ContextProperties.cs  # Vector3 context menu
│   └── SimpleFolderIcon/            # Custom folder icons system
├── Runtime/
│   ├── GenericUti/                  # Generic C# utilities
│   │   ├── StringUtility.cs         # String manipulation
│   │   ├── MathUtility.cs          # Math operations
│   │   ├── ListUtility.cs          # List helpers
│   │   ├── DictionaryUtility.cs    # Dictionary helpers
│   │   └── ...
│   └── UnityUti/                   # Unity-specific utilities
│       ├── GameUtility/            # Game development utilities
│       ├── HierarchyExt/           # Hierarchy extensions
│       └── PropertyAttributes/     # Custom inspector attributes
```

## 📋 Requirements

- **Unity Version:** 6000.2 or later (should work on 2019 above too)
- **Dependencies:** None

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🔄 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history and updates.

---

## Creator(s)
- RayOfIdeas
- SimpleFolderIcon is forked from [SimpleFolderIcon](https://github.com/SeaeeesSan/SimpleFolderIcon) by SeaeeesSan
- HierarchyExt is inspired from [Colourful Hierarchy Category GameObject](https://assetstore.unity.com/packages/tools/utilities/colourful-hierarchy-category-gameobject-205934) by M STUDIO HUB
- Scene Selection Overlay and some scripts are adapted from videos by [Warped Imagination](https://www.youtube.com/@WarpedImagination)
