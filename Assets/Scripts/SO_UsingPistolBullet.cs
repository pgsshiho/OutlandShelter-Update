using UnityEngine;

[CreateAssetMenu(fileName = "UsingPistolBullet", menuName = "UsingBullet/UsingPistolBullet")]
public class SO_UsingPistolBullet : SO_UsingBullet
{
    public override int UsingBullet
    {
        get => BulletManager.pistolBullet;
        set => BulletManager.pistolBullet = value;
    }
}
