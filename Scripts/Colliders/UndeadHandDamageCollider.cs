using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp UndeadHandDamageCollider kế thừa từ DamageCollider, đại diện cho collider gây sát thương của tay nhân vật undead
    public class UndeadHandDamageCollider : DamageCollider
    {
        // Biến undeadCharacter lưu trữ đối tượng AICharacterManager của nhân vật undead
        [SerializeField] AICharacterManager undeadCharacter;

        // Phương thức Awake được gọi khi đối tượng này được khởi tạo
        protected override void Awake()
        {
            // Gọi phương thức Awake của lớp cơ sở (DamageCollider) để thực hiện các thiết lập mặc định
            base.Awake();

            // Lấy thành phần Collider của đối tượng hiện tại và gán cho damageCollider
            damageCollider = GetComponent<Collider>();

            // Tìm đối tượng AICharacterManager trong đối tượng cha và gán cho undeadCharacter
            undeadCharacter = GetComponentInParent<AICharacterManager>();
        }

        // Phương thức này được gọi khi đối tượng bị sát thương
        protected override void DamageTarget(CharacterManager damageTarget)
        {
            // Kiểm tra nếu damageTarget (mục tiêu bị sát thương) đã nhận sát thương từ trước, không gây thêm sát thương
            if (charactersDamaged.Contains(damageTarget))
                return;

            // Thêm damageTarget vào danh sách để ghi nhận rằng nhân vật đã bị sát thương
            charactersDamaged.Add(damageTarget);

            // Tạo một hiệu ứng sát thương mới từ mẫu trong WorldCharacterEffectsManager
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

            // Gán các giá trị sát thương cho damageEffect
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.contactPoint = contactPoint;

            // Tính toán góc tấn công giữa nhân vật undead và mục tiêu bị tấn công
            damageEffect.angleHitFrom = Vector3.SignedAngle(
                undeadCharacter.transform.forward,  // Hướng của nhân vật undead
                damageTarget.transform.forward,     // Hướng của mục tiêu
                Vector3.up                          // Trục dọc để tính toán góc theo mặt phẳng ngang
            );

            // Option 1: Kiểm tra nếu undeadCharacter là chủ sở hữu (IsOwner là true)
            /*if (undeadCharacter.IsOwner) // Kiểm tra nếu undeadCharacter là chủ sở hữu
            {
                // Thông báo sát thương tới server khi undeadCharacter là chủ sở hữu
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,     // ID của nhân vật bị sát thương
                    undeadCharacter.NetworkObjectId,  // ID của nhân vật gây sát thương
                    damageEffect.physicalDamage,      // Sát thương vật lý
                    damageEffect.magicDamage,         // Sát thương phép thuật
                    damageEffect.fireDamage,          // Sát thương lửa
                    damageEffect.holyDamage,          // Sát thương thánh
                    damageEffect.poiseDamage,         // Sát thương poise (nếu có)
                    damageEffect.angleHitFrom,        // Góc tấn công
                    damageEffect.contactPoint.x,      // Tọa độ X của vị trí va chạm
                    damageEffect.contactPoint.y,      // Tọa độ Y của vị trí va chạm
                    damageEffect.contactPoint.z       // Tọa độ Z của vị trí va chạm
                );
            }*/

            // Option 2: Kiểm tra nếu damageTarget là chủ sở hữu (IsOwner là true)
            if (damageTarget.IsOwner) // Kiểm tra nếu damageTarget là chủ sở hữu
            {
                // Thông báo sát thương tới server khi damageTarget là chủ sở hữu
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,     // ID của nhân vật bị sát thương
                    undeadCharacter.NetworkObjectId,  // ID của nhân vật gây sát thương
                    damageEffect.physicalDamage,      // Sát thương vật lý
                    damageEffect.magicDamage,         // Sát thương phép thuật
                    damageEffect.fireDamage,          // Sát thương lửa
                    damageEffect.holyDamage,          // Sát thương thánh
                    damageEffect.poiseDamage,         // Sát thương poise (nếu có)
                    damageEffect.angleHitFrom,        // Góc tấn công
                    damageEffect.contactPoint.x,      // Tọa độ X của vị trí va chạm
                    damageEffect.contactPoint.y,      // Tọa độ Y của vị trí va chạm
                    damageEffect.contactPoint.z       // Tọa độ Z của vị trí va chạm
                );
            }
        }
    }
}


