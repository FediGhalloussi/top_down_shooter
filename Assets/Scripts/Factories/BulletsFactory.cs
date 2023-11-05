using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletsFactory : MonoBehaviour
{
    private readonly Dictionary<BulletType, Queue<Bullet>> availableBulletsByType = new Dictionary<BulletType, Queue<Bullet>>();

    [Serializable]
    public class BulletPrefab
    {
        public BulletType bulletType;
        public Bullet prefab;
        public int preinstantiateCount;
    }

    [SerializeField] private List<BulletPrefab> bulletPrefabs = new List<BulletPrefab>();

    public static BulletsFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is multiple instance of singleton BulletsFactory");
            return;
        }

        Instance = this;

        //Debug.Log("BulletsFactory Awake" + bulletPrefabs[0].prefab);
        foreach (BulletPrefab bulletPrefab in bulletPrefabs)
        {
            availableBulletsByType.Add(bulletPrefab.bulletType, new Queue<Bullet>());
            PreinstantiateBullets(bulletPrefab.prefab, bulletPrefab.preinstantiateCount);
        }
    }

    public static Bullet GetBullet(Vector3 position, BulletType bulletType)
    {
        Queue<Bullet> availableBullets = Instance.availableBulletsByType[bulletType];

        Bullet bullet = null;
        if (availableBullets.Count > 0)
        {
            bullet = availableBullets.Dequeue();
        }

        if (bullet == null)
        {
            // Instantiate a new bullet.
            bullet = InstantiateBullet(bulletType);
        }

        bullet.Position = position;
        bullet.gameObject.SetActive(true);

        return bullet;
    }

    public static void ReleaseBullet(Bullet bullet)
    {
        Queue<Bullet> availableBullets = Instance.availableBulletsByType[bullet.Type];
        bullet.gameObject.SetActive(false);
        availableBullets.Enqueue(bullet);
    }

    private void PreinstantiateBullets(Bullet bulletPrefab, int numberOfBulletsToPreinstantiate)
    {
        BulletType bulletType = bulletPrefab.Type;
        Queue<Bullet> bullets = availableBulletsByType[bulletType];
        for (int index = 0; index < numberOfBulletsToPreinstantiate; index++)
        {
            Bullet bullet = InstantiateBullet(bulletType);
            if (bullet == null)
            {
                Debug.LogError($"Failed to instantiate {numberOfBulletsToPreinstantiate} bullets.");
                break;
            }

            bullets.Enqueue(bullet);
        }
    }

    private static Bullet InstantiateBullet(BulletType bulletType)
    {
        // Implement logic to choose the right prefab based on the bulletType.
        BulletPrefab bulletPrefab = Instance.bulletPrefabs.Find(b => b.bulletType == bulletType);

        if (bulletPrefab != null)
        {
            GameObject gameObject = Instantiate(bulletPrefab.prefab.gameObject, Instance.transform, true);
            Bullet bullet = gameObject.GetComponent<Bullet>();
            return bullet;
        }
        else
        {
            Debug.LogError("BulletPrefab not found for type: " + bulletType);
            return null;
        }
    }
}