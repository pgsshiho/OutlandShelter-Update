using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Weapons
{
    public class Turret : Weapon
    {
        [SerializeField] private GameObject[] exampleImage;
        [SerializeField] private TextMeshProUGUI turretCount;

        public override void Attack()
        {
            Build(Input.mousePosition);
        }

        public GameObject Build(Vector3 position)
        {
            GameObject temp = poolManager.Pool.Get();

            temp.transform.position = new Vector3(position.x, position.y);

            WorkingManager.turretCounts[poolManager.weaponIndex]--;

            return temp;
        }

        protected override void Play()
        {
        }

        GameObject temp;
        ConstructionTurret conTurret;

        protected override void Update()
        {
            if (!ItemOwnManager.ownWeapon[kind][poolManager.weaponIndex])
            {
                enabled = false;
            }

            if (WorkingManager.turretCounts[poolManager.weaponIndex] > 0)
            {
                if (temp == null)
                {
                    temp = Instantiate(exampleImage[poolManager.weaponIndex]);
                    if (temp.TryGetComponent(out conTurret))
                    {
                        conTurret.turretIndex = poolManager.weaponIndex;
                    }
                }
                else if (conTurret.turretIndex != poolManager.weaponIndex) 
                {
                    Destroy(temp);
                    temp = Instantiate(exampleImage[poolManager.weaponIndex]);
                    if (temp.TryGetComponent(out conTurret))
                    {
                        conTurret.turretIndex = poolManager.weaponIndex;
                    }
                }
            }

            turretCount.text = WorkingManager.turretCounts[poolManager.weaponIndex].ToString();
        }

        private void OnEnable()
        {
            turretCount.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            Destroy(temp);

            turretCount.gameObject.SetActive(false);
        }
    }
}
