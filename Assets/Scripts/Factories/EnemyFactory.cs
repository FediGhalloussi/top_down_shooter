using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    private readonly Dictionary<EnemyType, Queue<EnemyAvatar>> availableEnemiesByType =
        new Dictionary<EnemyType, Queue<EnemyAvatar>>();

    [System.Serializable]
    public class EnemyPrefab
    {
        public EnemyType enemyType;
        public EnemyAvatar prefab;
        public int preinstantiateCount;
    }

    [SerializeField] private List<EnemyPrefab> enemyPrefabs = new List<EnemyPrefab>();

    public static EnemyFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is multiple instance of singleton EnemyFactory");
            return;
        }

        Instance = this;

        foreach (EnemyPrefab enemyPrefab in enemyPrefabs)
        {
            availableEnemiesByType.Add(enemyPrefab.enemyType, new Queue<EnemyAvatar>());
            PreinstantiateEnemies(enemyPrefab.prefab, enemyPrefab.preinstantiateCount);
        }
    }

    public static EnemyAvatar GetEnemy(Vector3 position, Quaternion rotation, EnemyType enemyType)
    {
        if (!Instance.availableEnemiesByType.ContainsKey(enemyType))
        {
            Instance.availableEnemiesByType.Add(enemyType, new Queue<EnemyAvatar>());
        }

        Queue<EnemyAvatar> availableEnemies = Instance.availableEnemiesByType[enemyType];

        EnemyAvatar enemy = null;
        if (availableEnemies.Count > 0)
        {
            enemy = availableEnemies.Dequeue();
        }

        if (enemy == null)
        {
            // Instantiate a new enemy.
            enemy = InstantiateEnemy(enemyType, position, rotation);
        }

        enemy.Reset();
        enemy.Position = position;
        enemy.gameObject.SetActive(true);

        return enemy;
    }

    public static void ReleaseEnemy(EnemyAvatar enemyAvatar)
    {
        Queue<EnemyAvatar> availableEnemies = Instance.availableEnemiesByType[enemyAvatar.EnemyType];
        enemyAvatar.gameObject.SetActive(false);
        availableEnemies.Enqueue(enemyAvatar);
    }

    private void PreinstantiateEnemies(EnemyAvatar enemyPrefab, int numberOfEnemiesToPreinstantiate)
    {
        EnemyType enemyType = enemyPrefab.EnemyType;
        Queue<EnemyAvatar> enemies = availableEnemiesByType[enemyType];
        for (int index = 0; index < numberOfEnemiesToPreinstantiate; index++)
        {
            EnemyAvatar enemy = InstantiateEnemy(enemyType, Vector3.zero, Quaternion.identity);
            if (enemy == null)
            {
                Debug.LogError($"Failed to instantiate {numberOfEnemiesToPreinstantiate} enemies.");
                break;
            }

            enemies.Enqueue(enemy);
        }
    }

    private static EnemyAvatar InstantiateEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        EnemyPrefab enemyPrefab = Instance.enemyPrefabs.Find(e => e.enemyType == enemyType);

        if (enemyPrefab != null)
        {
            GameObject gameObject = Instantiate(enemyPrefab.prefab.gameObject, position, rotation);
            gameObject.transform.parent = Instance.transform;
            EnemyAvatar enemy = gameObject.GetComponent<EnemyAvatar>();
            return enemy;
        }
        else
        {
            Debug.LogError("EnemyPrefab not found for type: " + enemyType);
            return null;
        }
    }

    public EnemyType GetRandomEnemyType()
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)].enemyType;
    }
}