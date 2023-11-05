using System;

public class PlayerAvatar : BaseAvatar
{
    public bool IsDead => this.Health <= 0;

    public override void Reset()
    {
        hpBar = UIManager.Instance.healthBar;
        base.Reset();
        hpBar.FadeBar(true, 0.5f);
    }
    private void Update()
    {
        if (this.IsDead)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Upgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.Health:
                this.Health += 10;
                break;
            case UpgradeType.Speed:
                this.MaxSpeed += 1;
                break;
            case UpgradeType.Damage:
                GetComponentInChildren<Gun>().Upgrade(upgradeType);
                break;
            case UpgradeType.FireRate:
                GetComponentInChildren<Gun>().Upgrade(upgradeType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null);
        }
    }
    
    public override void Die()
    {
        hpBar.UpdateHealthBar(Health);
        hpBar.FadeBar(false, 0.5f);
    }
    
    protected override void SetType()
    {
        this.Type = AvatarType.Player;
    }
    
}
