# Advanced HCI: VR and Haptics

## Overview
This Unity project demonstrates how to integrate haptic feedback into virtual reality (VR) environments using the Meta XR Haptics SDK. It serves as a practical guide for designing and implementing multisensory experiences with vibrotactile, auditory, and visual feedback.

## Features
- **Vibrotactile Feedback**: Add realistic haptic responses to VR interactions.
- **Multisensory Integration**: Combine visual, auditory, and tactile feedback for enhanced immersion.
- **Customizable Interactions**: Easily map VR actions to haptic events.
- **Ready-to-Use Assets**: Preconfigured Unity assets, scripts, and haptic feedback examples.

---

## Getting Started
### Prerequisites
1. **Unity**: Version `6.6000.0.25f1` or higher.
2. **Meta Quest**: Quest 2 or higher.
3. **Meta Haptics Studio**: For creating `.haptic` files.
4. **Meta XR SDK**: Latest version from the Unity Asset Store.
5. **DOTween**: Animation support (optional but recommended).

### Installation Steps
1. **Install Unity XR Interaction Toolkit**:
    - Open Unity and navigate to `Window > Package Manager`.
    - Set the package source to `Unity Registry`.
    - Search for `XR Interaction Toolkit` and install the latest version.

2. **Add Meta XR All-in-One SDK**:
    - Download the SDK from the Unity Asset Store.
    - Add it to your project via `Assets > Add to My Assets`.

3. **Configure XR Plugin Management**:
    - Go to `Edit > Project Settings > XR Plugin Management`.
    - Enable the `Oculus XR Provider`.

4. **Meta Haptics Studio Setup**:
    - Import an audio file into Meta Haptics Studio.
    - Adjust parameters, set markers, and export as a `.haptic` file.

5. **Scene Setup**:
    - Remove the default camera from your Unity scene.
    - Right-click in the hierarchy and select `Interaction SDK > Add OVR Interaction Rig`.
    - Add your 3D models and assign interactions using the `XR Interaction Toolkit`.

---

## Project Structure
### Core Components
- **Scripts**:
    - `FeedbackController.cs`: Handles haptic feedback and input mapping.
    - `SimpleDrillController.cs`: Example script for multisensory interaction.
    - `GameManager.cs`: Mini-game management script.

- **Assets**:
    - 3D models: Preconfigured objects for VR interaction.
    - `.haptic` files: Haptic feedback configurations.
    - Audio files: Sounds for spatialized audio feedback.

- **Plugins**:
    - Meta XR SDK: Provides haptic and spatial audio support.
    - DOTween: Supports smooth animation of objects.

### Key Scripts
#### `FeedbackController.cs`
A script for triggering haptic feedback based on user interaction.
- **Haptic Feedback**:
    - Triggers `.haptic` files using `HapticClipPlayer`.
    - Adjustable parameters: amplitude, frequency, duration.
- **Integration**:
    - Maps VR controller inputs to feedback events.

#### `SimpleDrillController.cs`
Example script for implementing multisensory feedback in a virtual power drill.
- **Features**:
    - Auditory and haptic feedback for different contact states (e.g., wood vs. no contact).
    - Drill rotation animation using DOTween.
    - Interaction events handled via `OnTriggerEnter` and `OnTriggerExit`.

#### Mini-Game
A sample mini-game where users drill as many holes as possible in 60 seconds.
- **GameManager.cs**: Manages score, time, and game flow.

---

## Creating Your Own Multisensory Object
### Design Goals
- Identify sensations (e.g., vibration, texture).
- Map VR actions to specific feedback events.
- Combine visual, auditory, and tactile elements for a cohesive experience.

### Step-by-Step Guide
1. **Choose a Scenario**:
    - Examples: Gaming, education, medical training, or social VR.

2. **Define Vibrotactile Goals**:
    - Determine vibration types (e.g., pulses, continuous patterns).
    - Specify intensity and frequency to represent virtual interactions.

3. **Map Interactions**:
    - Link actions (e.g., grabbing, collisions) to haptic triggers.

4. **Integrate Haptics**:
    - Use Meta Haptics Studio to create `.haptic` files.
    - Import the `.haptic` files into Unity and assign them to interactions.

5. **Test and Iterate**:
    - Assess realism, comfort, and usability.
    - Use questionnaires or user feedback to refine the experience.

---

## Example: Power Drill
### Features
- **Visual**: Drill rotation and model animation.
- **Auditory**: Drill sounds for different materials.
- **Haptic**: Vibration feedback based on material contact.

### Implementation Highlights
- **Audio**: Add spatialized sound using the Meta XR Audio SDK.
- **Haptics**: Configure `.haptic` files for different states (e.g., no contact, wood contact).
- **Script**: Use `SimpleDrillController.cs` for interaction logic.

---

## References
- [Meta XR SDK Documentation](https://developer.meta.com/docs/)
- [Unity XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest)
- [Meta Haptics Studio Guide](https://developer.meta.com/tools/haptics-studio)

---

## Contribution
Feel free to fork this project, add your own multisensory objects, and create pull requests. Feedback and suggestions are always welcome!

---

## License
This project is licensed under the MIT License. See the LICENSE file for details.

