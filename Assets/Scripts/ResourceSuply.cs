using UnityEngine;

public class ResourceSuply : ResourceObject
{
    public override int Break(float amount)
    {
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                BulletManager.pistolBullet += 50;
                Notion.Log("Pistolammoget");
                break;
            case 1:
                BulletManager.rifleBullet += 70;
                Notion.Log("Rifleammoget");
                break;
            case 2:
                BulletManager.shotGunBullet += 30;
                Notion.Log("Shotgunammoget");
                break;
        }
        SoundManager.SFX.PlayOneShot(SFXReference.Instance.Supplyget, 0.5f);
        return 0;
    }
}
