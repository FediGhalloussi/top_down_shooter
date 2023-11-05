using UnityEngine;

using Data;

public class Gun : MonoBehaviour
{
    protected BaseAvatar baseAvatar;
    
    [SerializeField] private AvatarType targetType;

    [SerializeField]
    private string weaponName;

    [SerializeField]
    private float rateOfFire;

    [SerializeField]
    private float bulletSpeed;

    [SerializeField]
    private float bulletDamage;

    [SerializeField]
    private BulletType bulletType;

    [SerializeField]
    private bool drawCanonsGizmo;

    [SerializeField]
    private CanonDescription[] canonDescriptions;

    private float lastFireTime;

    public float RateOfFire
    {
        get => this.rateOfFire;
        private set => this.rateOfFire = value;
    }

    public float BulletSpeed => this.bulletSpeed;

    public float BulletDamage
    {
        get => this.bulletDamage;
        private set => this.bulletDamage = value;
    }

    public string WeaponName => this.weaponName;

    public BulletType BulletType => this.bulletType;

    public virtual bool IsFiring
    {
        get
        {
            if (this.RateOfFire > 0f)
            {
                float durationBetweenTwoBullets = 1f / this.RateOfFire;

                if (Time.time < this.lastFireTime + durationBetweenTwoBullets)
                {
                    // The bullet gun is in cooldown.
                    return true;
                }
            }
            return false;
        }
    }

    protected float GameObjectAngle => -this.transform.eulerAngles.y * Mathf.Deg2Rad;

    public virtual bool TryToFire()
    {
        if (this.CanFire())
        {
            this.Fire();
            return true;
        }
        
        return false;
    }

    protected virtual bool CanFire()
    {
        if (!this.enabled)
        {
            return false;
        }

        // if (this.BaseAvatar != null && !this.BaseAvatar.CanFire())
        // {
        //     return false;
        // }

        if (this.RateOfFire <= 0f)
        {
            // The avatar has a nul rate of fire, The bullet gun can't fire.
            return false;
        }

        if (this.IsFiring)
        {
            // The bullet gun is in cooldown, it can't fire.
            return false;
        }

        // We don't want this anymore because of the rule of energy restoring.
        ////if (this.baseAvatar.Energy < this.EnergyConsumedPerBullet)
        ////{
        ////    // Not enough energy to fire a bullet.
        ////    return false;
        ////}

        return true;
    }

    protected Vector3 GetBulletSpawnPosition(int canonIndex)
    {
        Vector3 worldOffset = transform.localToWorldMatrix * canonDescriptions[canonIndex].BulletSpawnOffsetPosition;
        return transform.position + worldOffset;
    }

    protected float GetBulletSpawnAngle(int canonIndex)
    {
        return this.GameObjectAngle + (this.canonDescriptions[canonIndex].BulletSpawnOffsetAngle * Mathf.Deg2Rad);
    }

    protected virtual void Fire()
    {
        this.lastFireTime = Time.time;

        for (int index = 0; index < this.canonDescriptions.Length; index++)
        {
            float bulletSpawnAngle = this.GetBulletSpawnAngle(index);
            Vector3 direction = new Vector3(Mathf.Cos(bulletSpawnAngle),0, Mathf.Sin(bulletSpawnAngle));

            //Debug.Log("Bullet spawn angle: " + bulletSpawnAngle);
            // Fire a bullet !
            Bullet bullet = BulletsFactory.GetBullet(this.GetBulletSpawnPosition(index), this.BulletType);
            bullet.Initialize(direction, this.BulletSpeed, this.BulletDamage, targetType);
        }
    }

    protected virtual void Start()
    {
        this.baseAvatar = this.GetComponent<BaseAvatar>();
        if (this.baseAvatar == null)
        {
            Debug.LogWarning(string.Format("Can't retrieve a base avatar on the gameobject {0}.", this.gameObject.name));
        }
    }

    protected void OnDrawGizmos()
    {
        if (this.drawCanonsGizmo)
        {
            for (int index = 0; index < this.canonDescriptions.Length; index++)
            {
                float bulletSpawnAngle = this.GetBulletSpawnAngle(index);
                Vector3 speed = new Vector3(this.BulletSpeed * Mathf.Cos(bulletSpawnAngle),0, this.BulletSpeed * Mathf.Sin(bulletSpawnAngle));

                Debug.DrawLine(this.GetBulletSpawnPosition(index), this.GetBulletSpawnPosition(index) + speed, Color.red);
            }
        }
    }
    
    public void Upgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Damage:
                this.BulletDamage += 1;
                break;
            case UpgradeType.FireRate:
                this.RateOfFire += 1;
                break;
            default:
                Debug.LogError("Unknown upgrade type " + upgradeType);
                break;
        }
    }
}