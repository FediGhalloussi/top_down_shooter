using Microlight.MicroBar;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO USE UIDOCUMENT INSTEAD
public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI roundText;
    public GameObject upgradePanel;
    public GameObject gameOverPanel;
    public Button choice1Button;
    public Button choice2Button;
    public MicroBar healthBar;
    public GameObject loadingScreen;
    
    //singleton
    public static UIManager Instance
    {
        get;
        private set;
    }
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        // Assurez-vous que les objets Text sont correctement référencés dans l'Inspector.
        if (scoreText == null || roundText == null || upgradePanel == null || choice1Button == null || choice2Button == null)
        {
            Debug.LogError("Veuillez assigner les objets Text dans l'Inspector.");
        }
        
        choice1Button.onClick.AddListener(() => GameManager.Instance.UpgradePlayer(UpgradeType.FireRate));
        choice2Button.onClick.AddListener(() => GameManager.Instance.UpgradePlayer(UpgradeType.Damage));
        
        //hide loading screen after 2 seconds
        Invoke(nameof(HideLoadingScreen), 3f);
        
    }

    private void FixedUpdate()
    {
        UpdateScore(GameManager.Instance.Score);
        UpdateRound(GameManager.Instance.Round);
    }

    public void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    public void UpdateRound(int newRound)
    {
        roundText.text = "Round: " + newRound;
    }
    
    public void ShowUpgradePanel()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
    
    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }
    
    public void HideLoadingScreen()
    {
        loadingScreen.SetActive(false);
    }
    
}