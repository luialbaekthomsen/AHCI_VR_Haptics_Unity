using UnityEngine;
using TMPro;
using UnityEngine.XR;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject crossPrefab;
    public Transform board;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    private int score = 0;
    private float gameTime = 60f;
    private bool gameActive = false;

    private XRNode inputSource = XRNode.RightHand;
    private InputDevice device;

    public GameObject startGame;

    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    public DrillController drillController;

    private void Start()
    {
        SetupGameUI();
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        startGame.SetActive(true);

        audioSource.loop = true;
        audioSource.clip = menuMusic;
        audioSource.Play();
    }

    private void Update()
    {
        if (!gameActive && CheckBButtonPressed())
        {
            StartGame();
        }
    }

    private bool CheckBButtonPressed()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(inputSource);
        }

        bool bButtonPressed = false;
        if (
            device.TryGetFeatureValue(CommonUsages.secondaryButton, out bButtonPressed)
            && bButtonPressed
        )
        {
            return true;
        }
        return false;
    }

    private void SetupGameUI()
    {
        score = 0;
        UpdateScoreText();
        gameActive = false;
    }

    public void StartGame()
    {
        score = 0;
        UpdateScoreText();
        gameActive = true;
        StartCoroutine(GameTimer());
        SpawnNewCross();
        startGame.SetActive(false);

        audioSource.clip = gameMusic;
        audioSource.Play();
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private IEnumerator GameTimer()
    {
        float timeRemaining = gameTime;
        while (timeRemaining > 0)
        {
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        gameActive = false;
        EndGame();
    }

    private void EndGame()
    {
        Debug.Log("Game Over! Final Score: " + score);
        startGame.GetComponentInChildren<TextMeshProUGUI>().text =
            "Try again! Final Score: " + score;
        if (timerText != null)
            timerText.text = "0";

        foreach (Transform child in board)
        {
            Destroy(child.gameObject);
        }
        startGame.SetActive(true);
    }

    public void SpawnNewCross()
    {
        if (!gameActive)
            return;

        foreach (Transform child in board)
        {
            Destroy(child.gameObject);
        }

        Collider boardRenderer = board.GetComponent<Collider>();
        float width = boardRenderer.bounds.size.x;
        float height = boardRenderer.bounds.size.y;

        Vector3 randomPosition = new Vector3(
            Random.Range(-width / 2, width / 2),
            Random.Range(-height / 2, height / 2),
            0
        );

        randomPosition = board.position + randomPosition;
        Instantiate(crossPrefab, randomPosition, Quaternion.identity, board);

        drillController.SetContactState(DrillController.ContactState.WoodContact);
    }
}
