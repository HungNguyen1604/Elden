using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp WeaponManager quản lý các thông tin liên quan đến vũ khí và sát thương mà vũ khí gây ra
    public class WeaponManager : MonoBehaviour
    {
        // Biến meleeDamageCollider lưu trữ tham chiếu đến đối tượng MeleeWeaponDamageCollider để xử lý va chạm sát thương
        public MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            // Tìm và gán đối tượng MeleeWeaponDamageCollider từ các đối tượng con của vũ khí
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        // Phương thức này cài đặt sát thương cho vũ khí dựa trên nhân vật đang sử dụng vũ khí và thông tin của vũ khí đó
        public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weapon)
        {
            // Gán nhân vật đang cầm vũ khí vào biến characterCausingDamage để theo dõi nguồn gốc sát thương
            meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;

            // Gán các giá trị sát thương của vũ khí (vật lý, phép thuật, lửa, sét, thánh) từ WeaponItem
            meleeDamageCollider.physicalDamage = weapon.physicalDamage;
            meleeDamageCollider.magicDamage = weapon.magicDamage;
            meleeDamageCollider.fireDamage = weapon.fireDamage;
            meleeDamageCollider.lightningDamage = weapon.lightningDamage;
            meleeDamageCollider.holyDamage = weapon.holyDamage;

            // Gán hệ số điều chỉnh cho các đòn tấn công từ vũ khí vào collider gây sát thương.
            meleeDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công nhẹ 01 từ vũ khí
            meleeDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công nhẹ 02 từ vũ khí
            meleeDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công nặng 01 từ vũ khí
            meleeDamageCollider.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công nặng 02 từ vũ khí
            meleeDamageCollider.charge_Attack_01_Modifier = weapon.charge_Attack_01_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công sạc 01 từ vũ khí
            meleeDamageCollider.charge_Attack_02_Modifier = weapon.charge_Attack_02_Modifier; // Gán hệ số điều chỉnh cho đòn tấn công sạc 02 từ vũ khí

        }
    }
}

