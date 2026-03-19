using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldWeapon : MonoBehaviour
{
    private List<Weapons.Weapon> weapons = new();
    private SpriteRenderer spriteRenderer;
    private Transform child;

    private void Awake()
    {
        foreach (Weapons.Weapon weapon in transform.parent.GetComponents<Weapons.Weapon>())
        {
            if (weapon is not Weapons.Turret && weapon is not Weapons.WeaponArmor) weapons.Add(weapon);
        }

        spriteRenderer = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        child = spriteRenderer.transform;
    }

    private void Update()
    {
        foreach (Weapons.Weapon weapon in weapons)
        {
            if (weapon.enabled)
            {
                int index = weapon.poolManager.weaponIndex;
                Vector2 targetOffset = weapon.offset[index];
                Sprite targetSprite = weapon.weapons[index];
                bool isFlip = weapon.isFlip[index];

                if ((Vector2)child.transform.localPosition != targetOffset)
                    child.transform.localPosition = targetOffset;

                if (spriteRenderer.sprite != targetSprite)
                {
                    spriteRenderer.sprite = targetSprite;
                    spriteRenderer.flipX = isFlip;
                }
            }
        }
    }
}
