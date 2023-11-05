using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] private BulletType type;
    private Vector3 speed;
    private AvatarType targetType;
    
    public BulletType Type => this.type;

    public Vector3 Speed
    {
        get => this.speed;
        protected set => this.speed = value;
    }

    public float Damage { get; protected set; }

    public Vector3 Position
    {
        get => this.transform.position;
        set => this.transform.position = value;
    }

    public virtual void Initialize(Vector3 startDirection, float speed, float damage, AvatarType targetType)
    {
        this.Damage = damage;
        this.targetType = targetType;
    }

    protected abstract void UpdatePosition();

    private void Update()
    {
        this.UpdatePosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseAvatar avatar = other.GetComponent<BaseAvatar>();
        if (avatar == null)
        {
            BulletsFactory.ReleaseBullet(this);
            VFXFactory.GetVFX(VFXType.Impact,transform.position);
        }
        else if (avatar != null && avatar.Type == targetType)
        {        
            BulletsFactory.ReleaseBullet(this);
            //VFXFactory.GetVFX(VFXType.Explosion,transform.position);
            //this.vfx = Instantiate(BulletsFactory.Instance.GetVFX(this.Type), this.transform);
            avatar.TakeDamage(Damage);
        }
    }
}