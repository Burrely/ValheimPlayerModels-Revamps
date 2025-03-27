# ValheimPlayerModels - Revamps
![GitHub all releases](https://img.shields.io/github/downloads/dresklaw/ValheimPlayerModels/total)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/burrely/ValheimPlayerModels-Revamps)

A revamped edition of the CustomPlayerModels mod for Valheim, originally created by Ikeiwa and maintained by Dresklaw, introducing animator support and an animator enabled SDK!

**[Download the latest version here](https://github.com/Burrely/ValheimPlayerModels-Revamps/releases/download/1.3.1/ValheimPlayerModels.dll)**
Looking to set up your own model? Find instructions and downloads below!

![Action Menu Preview!](https://github.com/user-attachments/assets/7748596f-1da8-4377-87f2-ce597124a73e)
![preview](https://github.com/dresklaw/ValheimPlayerModels/blob/main/preview.png)

# How to install

 1. Download and install the [BepInEx package](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
 2. Drag and drop ValheimPlayerModels.dll in the "BepInEx/plugin" folder in your game install folder
 3. Launch the game
 4. Place all your avatars in the newly created "PlayerModels" folder inside your game install folder
 5. The mod will load the avatar with the same filename as your character name (you might want to restart the game)

# How to create an avatar

 1. Install [Unity 2019.4.31](https://unity.com/releases/editor/whats-new/2019.4.31)
 2. Create a new 3D Project
 3. [Download the SDK](https://github.com/Burrely/ValheimPlayerModels-Revamps/releases/download/1.3.1/ValheimPlayerModels_SDK_1.3.1.unitypackage)
 4. Import the SDK and all your avatar assets
 5. Change your avatar model rig to Humanoid
 6. Place your avatar model in the scene
 7. Add a "Valheim Avatar Descriptor" component on your avatar
 8. Click the "Auto-Setup" Button
 9. Move the different equipment previews how you like
 10. Set the material shaders to Standard, this allows you to create smoothness/metallic maps instead of being limited to a slider.
 11. Click the "Export" button in the Avatar Descriptor
 12. Export in the "PlayerModels" folder in your game install folder
