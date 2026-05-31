# Puppy
Cute little puppy AR app.

## What is included

- `Assets/Scripts/DogARManager.cs`: AR Foundation script to place a procedural dog model on detected planes.
- `Packages/manifest.json`: Unity package manifest with AR Foundation dependencies.
- `ProjectSettings/ProjectVersion.txt`: Recommended Unity editor version.
- `.gitignore`: Unity ignore rules.

## Setup

1. Open this folder in Unity 2022.3 or newer.
2. Import AR Foundation and the appropriate platform plugin in the Package Manager:
   - `AR Foundation`
   - `ARCore XR Plugin` for Android
   - `ARKit XR Plugin` for iOS
3. Create a new scene and add:
   - `AR Session`
   - `AR Session Origin`
   - `AR Camera`
   - `ARRaycastManager` on `AR Session Origin`
   - `ARPlaneManager` on `AR Session Origin`
   - `DogARManager` on `AR Session Origin`
4. Optionally assign a dog prefab to `dogPrefab`. If you leave it empty, the script creates a simple procedural dog.

## Usage

- Run the scene on an AR-capable device.
- Tap a detected horizontal surface to place the dog.

## Notes

- This repository is a lightweight AR prototype. Open in Unity, then use the Unity Editor to configure the AR scene and build settings.

