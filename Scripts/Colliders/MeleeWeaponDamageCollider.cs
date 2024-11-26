using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp MeleeWeaponDamageCollider kế thừa từ lớp DamageCollider, dùng để xác định việc gây sát thương cho vũ khí cận chiến
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        // Biến này lưu trữ tham chiếu đến nhân vật đang gây sát thương
        public CharacterManager characterCausingDamage;

        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier; // Hệ số điều chỉnh cho đòn tấn công nhẹ 01.
        public float light_Attack_02_Modifier; // Hệ số điều chỉnh cho đòn tấn công nhẹ 02.
        public float heavy_Attack_01_Modifier; // Hệ số điều chỉnh cho đòn tấn công nặng 01.
        public float heavy_Attack_02_Modifier; // Hệ số điều chỉnh cho đòn tấn công nặng 02.
        public float charge_Attack_01_Modifier; // Hệ số điều chỉnh cho đòn tấn công sạc 01.
        public float charge_Attack_02_Modifier; // Hệ số điều chỉnh cho đòn tấn công sạc 02.


        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp cha
            if (damageCollider == null)
            {
                damageCollider = GetComponent<Collider>(); // Lấy collider từ đối tượng
            }
            damageCollider.enabled = false; // Bắt đầu với collider tắt
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Kiểm tra nếu đối tượng va chạm là một nhân vật bằng cách tìm kiếm CharacterManager trong đối tượng cha
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            // Nếu tìm thấy đối tượng có CharacterManager, tức là nhân vật bị tấn công
            if (damageTarget != null)
            {
                if (damageTarget == characterCausingDamage)
                    return; // Nếu là nhân vật gây sát thương, không xử lý

                // Tính toán vị trí va chạm gần nhất với đối tượng 'other' so với vị trí của đối tượng hiện tại
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                // Gây sát thương cho mục tiêu nếu damageTarget hợp lệ
                DamageTarget(damageTarget);
            }
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            // Nếu nhân vật đã bị sát thương trước đó, không gây thêm sát thương
            if (charactersDamaged.Contains(damageTarget))
                return;

            // Thêm nhân vật vào danh sách đã bị sát thương
            charactersDamaged.Add(damageTarget);

            // Tạo một hiệu ứng sát thương (TakeDamageEffect) từ hiệu ứng toàn cục
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage; // Gán sát thương vật lý
            damageEffect.magicDamage = magicDamage; // Gán sát thương phép thuật
            damageEffect.fireDamage = fireDamage; // Gán sát thương lửa
            damageEffect.holyDamage = holyDamage; // Gán sát thương thánh
            damageEffect.contactPoint = contactPoint; // Gán vị trí va chạm


            // Tính toán góc tấn công dựa trên hướng của nhân vật gây sát thương và hướng của mục tiêu bị sát thương, sử dụng Vector3.SignedAngle.
            // Giá trị này sẽ lưu trong thuộc tính angleHitFrom của damageEffect.
            damageEffect.angleHitFrom = Vector3.SignedAngle(
                characterCausingDamage.transform.forward,  // Hướng của nhân vật gây sát thương
                damageTarget.transform.forward,            // Hướng của nhân vật bị sát thương
                Vector3.up                                 // Xác định mặt phẳng dùng để tính góc (mặt phẳng ngang)
            );

            // Kiểm tra loại đòn tấn công đang được thực hiện và áp dụng hệ số điều chỉnh tương ứng.
            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công nhẹ 01.
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công nhẹ 02.
                    ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công nặng 01.
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công nặng 02.
                    ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công sạc 01.
                    ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack02:
                    // Áp dụng hệ số điều chỉnh cho đòn tấn công sạc 02.
                    ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
                    break;
                default:
                    // Không có hành động nào cho loại tấn công không xác định.
                    break;
            }

            if (characterCausingDamage.IsOwner) // Kiểm tra nếu nhân vật gây sát thương là người sở hữu
            {
                // Thông báo tới server về sát thương gây ra
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId, // ID của nhân vật bị tấn công
                    characterCausingDamage.NetworkObjectId, // ID của nhân vật gây sát thương
                    damageEffect.physicalDamage, // Sát thương vật lý
                    damageEffect.magicDamage, // Sát thương phép thuật
                    damageEffect.fireDamage, // Sát thương lửa
                    damageEffect.holyDamage, // Sát thương thánh
                    damageEffect.poiseDamage, // Sát thương poise (nếu có)
                    damageEffect.angleHitFrom, // Góc tấn công (nếu có)
                    damageEffect.contactPoint.x, // Tọa độ X của vị trí va chạm
                    damageEffect.contactPoint.y, // Tọa độ Y của vị trí va chạm
                    damageEffect.contactPoint.z); // Tọa độ Z của vị trí va chạm
            }
        }

        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
        {
            // Áp dụng hệ số điều chỉnh cho các loại sát thương
            damage.physicalDamage *= modifier; // Điều chỉnh sát thương vật lý
            damage.magicDamage *= modifier; // Điều chỉnh sát thương phép thuật
            damage.fireDamage *= modifier; // Điều chỉnh sát thương lửa
            damage.holyDamage *= modifier; // Điều chỉnh sát thương thánh
            damage.poiseDamage *= modifier; // Điều chỉnh sát thương poise (nếu có)
        }
    }
}
