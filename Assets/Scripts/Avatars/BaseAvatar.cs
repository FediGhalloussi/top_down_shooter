using Microlight.MicroBar;
using UnityEngine;

public abstract class BaseAvatar : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] protected MicroBar hpBar;
    [SerializeField] protected Animator animator;
    
    private static readonly int Hit = Animator.StringToHash("Hit");

    public float MaxSpeed
    {
        get => this.maxSpeed;
        protected set => this.maxSpeed = value;
    }

    public float MaxHealth
    {
        get => this.maxHealth;
        protected set => this.maxHealth = value;
    }
    
    public Vector3 Position
    {
        get => this.transform.position;
        set => this.transform.position = value;
    }
    public float Health { get; protected set; }
    public AvatarType Type { get; protected set; }

    private void Start()
    {
        this.Reset();
    }

    public virtual void Reset()
    {
        this.Health = this.MaxHealth;
        hpBar.Initialize(MaxHealth);
        SetType();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        hpBar.UpdateHealthBar(Health);
        VFXFactory.GetVFX(VFXType.Impact, transform.position);
        animator.SetTrigger(Hit);

        if (Health <= 0)
        {
            VFXFactory.GetVFX(VFXType.Explosion, transform.position);
            Die();
        }
    }

    public abstract void Die();
    protected abstract void SetType();
}