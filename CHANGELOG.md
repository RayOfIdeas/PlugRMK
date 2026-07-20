# PlugRMK

## [1.1.0] - 2026-07-20
### Added
- `EnumEditorAttribute` and `EnumEditorWindow` for visually editing, renaming and reordering enum members from the inspector
- `EnumDiffWindow` for previewing and applying enum member renames/removals across referencing assets and scripts
- `DropdownSRAttribute` and `DropdownSRPropertyDrawer` for picking the concrete type of a `[SerializeReference]` field from a dropdown
- `ListElementNameAttribute` for customizing element labels of lists/arrays in the inspector
- `DropdownIntStringAttribute` and `IntStringList` ScriptableObject for dropdown fields backed by a reusable int-string list

### Changed
- `HierarchyExt` now targets `EditorApplication.hierarchyWindowItemByEntityIdOnGUI` on Unity 6000.5+ while falling back to the legacy `hierarchyWindowItemOnGUI` API (via `#if UNITY_6000_5_OR_NEWER`) so 6000.2-6000.4 projects keep working
- `DropdownSRPropertyDrawer` now supports IMGUI in addition to UI Toolkit
- Reorganized `PropertyAttributes` folder structure for consistency
- Several runtime/editor utilities (`CreateScriptTemplate`, `SceneSelectionOverlay`, `NumberDisplayUtility`, `IconDictionaryCreator`, `EditorIconsPath`, `ScriptIconsPath`) switched to string interpolation

### Fixed
- `HierarchyExt` instanceID to EntityId conversion
- `PlugRMK.UnityUti.EditorUti.asmdef` missing Editor folder inclusion
- `IconDictionaryCreator` null check and path handling
- Superfluous line in `EnumEditorWindow`

## [1.0.0] - 2025-11-24
### Added
- All the basic initial features of PlugRMK (see [README](README.md))