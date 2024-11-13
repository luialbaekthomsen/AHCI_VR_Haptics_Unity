// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.

using UnityEngine;
using Oculus.Haptics;
using System;

// This scene is a minimal integration example, meant to run on device (f.e. Meta Quest 2, Meta Quest Pro).
// It showcases how events, like button presses, can be hooked up to haptic feedback; and how we can use other input, like
// a controller's thumbstick movements, to modulate haptic effects.
// We gain access to the Haptics SDK's features through an API by importing Oculus.Haptics (see above).
public class HapticsSdkPlaySample : MonoBehaviour
{
    // The haptic clips are assignable in the Unity editor.
    // For this example, we are using the two demo clips found in Assets/Haptics.
    // Haptic clips can be designed in Haptics Studio (https://developer.oculus.com/experimental/exp-haptics-studio)
    public HapticClip clip;
    HapticClipPlayer _playerLeft;
    HapticClipPlayer _playerRight;

    protected virtual void Start()
    {
        // We create two haptic clip players for each hand.
        _playerLeft = new HapticClipPlayer(clip);
        _playerRight = new HapticClipPlayer(clip);

    }

    // This helper function allows us to identify the controller we are currently playing back on.
    // We use this further down for logging purposes.
    String GetControllerName(OVRInput.Controller controller)
    {
        if (controller == OVRInput.Controller.LTouch)
        {
            return "left controller";
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            return "right controller";
        }

        return "unknown controller";
    }

    // This section provides a series of interactions that showcase the playback and modulation capabilities of the
    // Haptics SDK.
    void HandleControllerInput(OVRInput.Controller controller, HapticClipPlayer clipPlayer, Controller hand)
    {
        string controllerName = GetControllerName(controller);

        try
        {
            // Play first clip with default priority using the index trigger
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                clipPlayer.Play(hand);
                Debug.Log("Should feel vibration from clipPlayer1 on " + controllerName + ".");
            }

            // Stop first clip when releasing the index trigger
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, controller))
            {
                clipPlayer.Stop();
                Debug.Log("Vibration from clipPlayer1 on " + controllerName + " should stop.");
            }


            // Loop first clip using the B/Y-button
            if (OVRInput.GetDown(OVRInput.Button.Two, controller))
            {
                clipPlayer.isLooping = !clipPlayer.isLooping;
                Debug.Log(String.Format("Looping should be {0} on " + controllerName + ".", clipPlayer.isLooping));
            }

            // Modulate the amplitude and frequency of the first clip using the thumbstick
            // - Moving left/right modulates the frequency shift
            // - Moving up/down modulates the amplitude
            if (controller == OVRInput.Controller.LTouch)
            {
                clipPlayer.amplitude = Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y, 0.0f, 1.0f);
                clipPlayer.frequencyShift = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).x;
            }
            else if (controller == OVRInput.Controller.RTouch)
            {
                clipPlayer.amplitude = Mathf.Clamp(1.0f + OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y, 0.0f, 1.0f);
                clipPlayer.frequencyShift = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).x;
            }
        }

        // If any exceptions occur, we catch and log them here.
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // We poll for controller interactions on every frame using the Update() loop
    protected virtual void Update()
    {
        HandleControllerInput(OVRInput.Controller.LTouch, _playerLeft, Controller.Left);
        HandleControllerInput(OVRInput.Controller.RTouch, _playerRight, Controller.Right);
    }

    protected virtual void OnDestroy()
    {
        _playerLeft?.Dispose();
        _playerRight?.Dispose();
    }

    // Upon exiting the application (or when playmode is stopped) we release the haptic clip players and uninitialize (dispose) the SDK.
    protected virtual void OnApplicationQuit()
    {
        Haptics.Instance.Dispose();
    }
}
