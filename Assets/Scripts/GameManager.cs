using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerAvatar playerPrefab;
    
    [SerializeField] private float rateOfEnemySpawn = 0.2f;
    
    [SerializeField] private float rateOfEnemySpawnIncreaseStep = 0.02f;
    
    private double lastEnemySpawnTime;

    public static GameManager Instance { get; private set; }

    public PlayerAvatar PlayerAvatar { get; private set; }

    public GameState State { get; set; }

    public int Score { get; private set; }
    public int Round { get; private set; }

    public int BestScore { get; private set; }

    public void AddScoreGain(int gain)
    {
        this.Score += gain;
        if (this.Score > this.BestScore)
        {
            this.BestScore = this.Score;
            PlayerPrefs.SetInt("BestScore", this.BestScore);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is multiple instance of singleton GameManager");
            return;
        }

        Instance = this;
    }

    private IEnumerator Start()
    {
        this.BestScore = PlayerPrefs.GetInt("BestScore", 0);

        yield return this.StartCoroutine(this.StartNewGame());
    }

    private IEnumerator StartNewGame()
    {

        // Reset game if needed.
        if (this.PlayerAvatar != null)
        {
            Destroy(this.PlayerAvatar.gameObject);
            this.PlayerAvatar = null;
        }

        
        // Reset game state.
        this.Score = 0;
        this.Round = 1;

        // Kill all enemies before starting new level.
        EnemyAvatar[] enemies = FindObjectsOfType<EnemyAvatar>();
        foreach (EnemyAvatar enemy in enemies)
        {
            EnemyFactory.ReleaseEnemy(enemy);
        }
        // Kill all bullets before starting new level.
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            BulletsFactory.ReleaseBullet(bullet);
        }
        
        // Hide UI
        UIManager.Instance.HideUpgradePanel();
        UIManager.Instance.HideGameOverPanel();

        // Spawn the player.
        var player = Instantiate(Instance.playerPrefab.gameObject, new Vector3(0f, 1f,0f),
            Quaternion.identity);
        this.PlayerAvatar = player.GetComponent<PlayerAvatar>();
        if (this.PlayerAvatar == null)
        {
            Debug.LogError("Can't retrieve the PlayerAvatar script.");
        }
        
        this.State = GameState.Playing;

        yield break;
    }
    
    public void CleanUp()
    {
        // Kill all enemies before starting new level.
        EnemyAvatar[] enemies = FindObjectsOfType<EnemyAvatar>();
        foreach (EnemyAvatar enemy in enemies)
        {
            EnemyFactory.ReleaseEnemy(enemy);
        }
        // Kill all bullets before starting new level.
        Bullet[] bullets = FindObjectsOfType<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            BulletsFactory.ReleaseBullet(bullet);
        }
    }

    private void Update()
    {
        switch (this.State)
        {
            case GameState.Initializing:
                break;

            case GameState.Playing:
                CheckRoundUpdate();
                this.RandomSpawn();
                break;

            case GameState.Dead:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Start a new game.
                    this.State = GameState.Initializing;

                    this.StartCoroutine(this.StartNewGame());
                }

                break;
        }
    }

    public void GameOver()
    {
        Instance.State = GameState.Dead;
        Instance.CleanUp();
        Instance.ShowGameOverMenu();
    }
    private void CheckRoundUpdate()
    {
        if (Score >= CalculateScoreRequiredForNextRound(Round))
        {
            // Increase the round and adjust spawn rate
            Round++;
            UpdateSpawnRate();
            ShowUpgradeMenu();
        }
    }
    
    int CalculateScoreRequiredForNextRound(int roundNumber)
    {
        int baseScore = 100;
        float growthFactor = 1.5f; 
        return (int)(baseScore * Mathf.Pow(growthFactor, roundNumber));
    }


    private void UpdateSpawnRate()
    {
        // todo better calcul for better difficulty curve
        rateOfEnemySpawn += rateOfEnemySpawnIncreaseStep;
    }

    private void RandomSpawn()
    {
        if (this.rateOfEnemySpawn <= 0f)
        {
            return;
        }

        float durationBetweenTwoEnemySpawn = 1f / this.rateOfEnemySpawn;

        if (Time.time < this.lastEnemySpawnTime + durationBetweenTwoEnemySpawn)
        {
            // The bullet gun is in cooldown, it can't fire.
            return;
        }
        // Randomly select an enemy to spawn.
        EnemyFactory.GetEnemy(CalculateSpawnPosition(), Quaternion.Euler(0f, 0f, 0f), EnemyFactory.Instance.GetRandomEnemyType());
        this.lastEnemySpawnTime = Time.time;

        // Up the difficulty.
        //this.rateOfEnemySpawn += this.rateOfEnemySpawn > this.maximumRateOfEnemySpawn ? 0f : this.rateOfEnemySpawnIncreaseStep;
    }


    private Vector3 CalculateSpawnPosition()
    {
        // Calculate the spawn position
        /*float terrainWidth = spawnTerrain.GetComponent<MeshRenderer>().bounds.size.x;
        float terrainHeight = spawnTerrain.GetComponent<MeshRenderer>().bounds.size.z;
        float terrainX = spawnTerrain.transform.position.x;
        float terrainZ = spawnTerrain.transform.position.z;*/
        float terrainWidth = 75f;
        float terrainHeight = 75f;
        float terrainX = 0f;
        float terrainZ = 0f;
        float spawnX = Random.Range(terrainX - terrainWidth / 2,
            terrainX + terrainWidth / 2);
        float spawnZ = Random.Range(terrainZ - terrainHeight / 2,
            terrainZ + terrainHeight / 2);

        //todo : make it so that the enemy spawn where the camera is not pointing !!!
        // Return the spawn position todo dont put 1f
        return new Vector3(spawnX, 1f, spawnZ);
    }

    private void ShowUpgradeMenu()
    {
        // todo
        UIManager.Instance.ShowUpgradePanel();
    }
    private void ShowGameOverMenu()
    {
        // todo
        UIManager.Instance.ShowGameOverPanel();
    }

    public void UpgradePlayer(UpgradeType upgradeType)
    {
        PlayerAvatar.Upgrade(upgradeType);
        UIManager.Instance.HideUpgradePanel();
    }
}