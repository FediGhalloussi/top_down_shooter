using UnityEngine;

public class EnemyAvatar : BaseAvatar
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private int scoreGain = 10;
    public EnemyType EnemyType => this.enemyType;

    public override void Reset()
    {
        base.Reset();
        gameObject.GetComponent<EnemyController>().target = GameManager.Instance.PlayerAvatar.transform;
    }

    public override void Die()
    {
        hpBar.UpdateHealthBar(Health);
        hpBar.FadeBar(false, 0.5f);
        GameManager.Instance.AddScoreGain(scoreGain);
        EnemyFactory.ReleaseEnemy(this);
    }
    
    protected override void SetType()
    {
        this.Type = AvatarType.Enemy;
    }
}