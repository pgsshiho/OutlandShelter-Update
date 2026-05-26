using UnityEngine;

[CreateAssetMenu(fileName = "UsingRifleBullet", menuName = "UsingBullet/UsingRifleBullet")]
public class SO_UsingRifleBullet : SO_UsingBullet
{
    public override int UsingBullet
    {
        get => BulletManager.rifleBullet;
        set => BulletManager.rifleBullet = value;
    }
}
