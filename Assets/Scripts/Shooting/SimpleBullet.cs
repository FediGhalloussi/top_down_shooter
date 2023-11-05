// <copyright file="SimpleBullet.cs" company="AAllard">Copyright AAllard. All rights reserved.</copyright>

using UnityEngine;

public class SimpleBullet : Bullet
{
    public override void Initialize(Vector3 startDirection, float speed, float damage, AvatarType targetType)
    {
        base.Initialize(startDirection, speed, damage, targetType);

        this.Speed = startDirection * speed;
    }

    protected override void UpdatePosition()
    {
        this.Position += this.Speed * Time.deltaTime;
    }
}
