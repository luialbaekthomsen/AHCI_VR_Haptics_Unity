using UnityEngine;
using UnityEngine.XR;
using Oculus.Haptics;
using DG.Tweening;
using System.Collections;

public class DrillController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip DrillSound_NoContact;
    public AudioClip DrillSound_WoodContact;
    public AudioClip DrillSound_CrossContact;

    [Header("Haptic Clips")]
    public HapticClip hapticClip_NoContact;
    public HapticClip hapticClip_WoodContact;
    public HapticClip hapticClip_CrossContact;

    private HapticClipPlayer _hapticPlayerRight;
    private AudioSource audioSource;

    [Header("Drill Settings")]
    public Transform drillBit;
    public bool isDrillOn = false;
    private XRNode inputSource = XRNode.RightHand;
    private InputDevice device;

    private Tween drillBitRotationTween;
    private Coroutine checkAudioCoroutine;
    private bool audioCoroutineRunning = false;
    private Coroutine crossCollisionCoroutine = null;
    private ContactState currentContactState = ContactState.NoContact;

    public GameManager gameManager;
    public bool crossContact = false;

    public enum ContactState
    {
        NoContact,
        WoodContact,
        CrossContact,
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
            SetContactState(currentContactState);
            StartDrillRotation();
            Debug.Log("Drill ON");
        }
        else
        {
            StopDrillBitRotation();

            if (_hapticPlayerRight != null)
            {
                _hapticPlayerRight.Stop();
                _hapticPlayerRight = null;
            }

            Debug.Log("Drill OFF");
        }
    }

    public void SetContactState(ContactState contactState)
    {
        if (!isDrillOn)
            return;

        currentContactState = contactState;
        audioSource.Stop();

        AudioClip selectedClip = contactState switch
        {
            ContactState.WoodContact => DrillSound_WoodContact,
            ContactState.CrossContact => DrillSound_CrossContact,
            _ => DrillSound_NoContact,
        };

        HapticClip selectedHaptic = contactState switch
        {
            ContactState.WoodContact => hapticClip_WoodContact,
            ContactState.CrossContact => hapticClip_CrossContact,
            _ => hapticClip_NoContact,
        };

        audioSource.clip = selectedClip;
        _hapticPlayerRight?.Stop();
        _hapticPlayerRight = new HapticClipPlayer(selectedHaptic);
        audioSource.Play();
        _hapticPlayerRight.Play(Controller.Right);

        if (!audioCoroutineRunning)
        {
            checkAudioCoroutine = StartCoroutine(CheckAudioAndStopTween());
        }
    }

    private void StartDrillRotation()
    {
        drillBitRotationTween?.Kill();
        drillBit.localRotation = Quaternion.identity;

        float rotationDuration = currentContactState switch
        {
            ContactState.WoodContact => 0.5f,
            ContactState.CrossContact => 0.75f,
            _ => 0.25f,
        };

        drillBitRotationTween = drillBit
            .DOLocalRotate(new Vector3(0, 0, -360), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopDrillBitRotation()
    {
        drillBitRotationTween?.Kill();
        drillBit.localRotation = Quaternion.identity;
    }

    private IEnumerator CheckAudioAndStopTween()
    {
        audioCoroutineRunning = true;
        yield return new WaitWhile(() => audioSource.isPlaying);
        StopDrillBitRotation();
        audioCoroutineRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDrillOn)
        {
            if (other.CompareTag("Cross"))
            {
                SetContactState(ContactState.CrossContact);
                StartDrillRotation();

                if (gameManager != null && gameManager.IsGameActive())
                {
                    crossCollisionCoroutine ??= StartCoroutine(
                        CrossCollisionTimer(other.gameObject)
                    );
                }
            }
            else if (
                other.CompareTag("WoodPlank") && currentContactState != ContactState.CrossContact
            )
            {
                SetContactState(ContactState.WoodContact);
                StartDrillRotation();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDrillOn)
        {
            if (other.CompareTag("WoodPlank") && currentContactState == ContactState.WoodContact)
            {
                SetContactState(ContactState.NoContact);
                StartDrillRotation();
            }
            else if (other.CompareTag("Cross") && currentContactState == ContactState.CrossContact)
            {
                SetContactState(ContactState.NoContact);
                StartDrillRotation();

                if (crossCollisionCoroutine != null)
                {
                    StopCoroutine(crossCollisionCoroutine);
                    crossCollisionCoroutine = null;
                }
            }
        }
    }

    private IEnumerator CrossCollisionTimer(GameObject cross)
    {
        float collisionTime = 0f;
        while (collisionTime < 2f)
        {
            yield return null;
            collisionTime += Time.deltaTime;
        }

        if (gameManager != null)
        {
            gameManager.AddScore(10);
            gameManager.SpawnNewCross();
        }

        Destroy(cross);
        crossCollisionCoroutine = null;
    }
}
