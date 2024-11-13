using UnityEngine;
using UnityEngine.XR;
using Oculus.Haptics;
using DG.Tweening;
using System.Collections;

public class SimpleDrillController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip DrillSound_NoContact;
    public AudioClip DrillSound_WoodContact;

    [Header("Haptic Clips")]
    public HapticClip hapticClip_NoContact;
    public HapticClip hapticClip_WoodContact;

    private HapticClipPlayer hapticPlayerRight;

    [Header("Drill Settings")]
    public Transform drillBit;
    private bool isDrillOn = false;

    public XRNode inputSource = XRNode.RightHand;
    private AudioSource audioSource;

    private InputDevice device;
    private Tween drillBitRotationTween;
    private Coroutine checkAudioCoroutine;
    private bool audioCoroutineRunning = false;
    private ContactState currentContactState = ContactState.NoContact;

    private bool isInWoodContact = false;

    public enum ContactState
    {
        NoContact,
        WoodContact
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        device = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(inputSource);
        }

        bool triggerButtonValue;
        bool gripButtonValue;

        bool gripButtonPressed =
            device.TryGetFeatureValue(CommonUsages.gripButton, out gripButtonValue)
            && gripButtonValue;

        if (gripButtonPressed)
        {
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButtonValue))
            {
                if (triggerButtonValue && !isDrillOn)
                {
                    ToggleDrill(true);
                }
                else if (!triggerButtonValue && isDrillOn)
                {
                    ToggleDrill(false);
                }
            }
        }
        else
        {
            if (isDrillOn)
            {
                ToggleDrill(false);
            }
        }
    }

    private void ToggleDrill(bool isOn)
    {
        isDrillOn = isOn;
        audioSource.Stop();

        if (isOn)
        {
            SetContactState(isInWoodContact ? ContactState.WoodContact : ContactState.NoContact);
            SetDrillRotation(true);
        }
        else
        {
            SetDrillRotation(false);

            if (hapticPlayerRight != null)
            {
                hapticPlayerRight.Stop();
                hapticPlayerRight = null;
            }
        }
    }

    private void SetContactState(ContactState contactState)
    {
        if (!isDrillOn)
            return;

        currentContactState = contactState;
        audioSource.Stop();

        AudioClip selectedClip =
            (contactState == ContactState.WoodContact)
                ? DrillSound_WoodContact
                : DrillSound_NoContact;
        HapticClip selectedHaptic =
            (contactState == ContactState.WoodContact)
                ? hapticClip_WoodContact
                : hapticClip_NoContact;

        audioSource.clip = selectedClip;

        if (hapticPlayerRight != null)
        {
            hapticPlayerRight.Stop();
        }

        hapticPlayerRight = new HapticClipPlayer(selectedHaptic);
        audioSource.Play();
        hapticPlayerRight.Play(Controller.Right);

        if (!audioCoroutineRunning)
        {
            checkAudioCoroutine = StartCoroutine(CheckAudioAndStopTween());
        }
    }

    private void SetDrillRotation(bool startRotation)
    {
        drillBitRotationTween?.Kill();
        drillBit.localRotation = Quaternion.identity;

        if (startRotation)
        {
            float rotationDuration = currentContactState == ContactState.WoodContact ? 0.5f : 0.25f;
            drillBitRotationTween = drillBit
                .DOLocalRotate(new Vector3(0, 0, -360), rotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }
    }

    private IEnumerator CheckAudioAndStopTween()
    {
        audioCoroutineRunning = true;
        yield return new WaitWhile(() => audioSource.isPlaying);
        SetDrillRotation(false);
        audioCoroutineRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WoodPlank"))
        {
            isInWoodContact = true;

            if (isDrillOn)
            {
                SetContactState(ContactState.WoodContact);
                SetDrillRotation(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WoodPlank"))
        {
            isInWoodContact = false;

            if (isDrillOn && currentContactState == ContactState.WoodContact)
            {
                SetContactState(ContactState.NoContact);
                SetDrillRotation(true);
            }
        }
    }
}
