using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Khai báo lớp AIUndeadCombatManager kế thừa từ AICharacterCombatManager
    public class AIUndeadCombatManager : AICharacterCombatManager
    {
        // Các biến lưu trữ các collider gây sát thương cho tay phải và tay trái
        [Header("Damage Colliders")]
        [SerializeField] UndeadHandDamageCollider rightHandDamageCollider; // Collider cho tay phải
        [SerializeField] UndeadHandDamageCollider leftHandDamageCollider;  // Collider cho tay trái

        // Các biến cấu hình sát thương
        [Header("Damage")]
        [SerializeField] int baseDamage = 25; // Sát thương cơ bản
        [SerializeField] float attack01DamageModifier = 1.0f; // Hệ số sát thương cho đòn tấn công 1
        [SerializeField] float attack02DamageModifier = 1.5f; // Hệ số sát thương cho đòn tấn công 2

        // Phương thức thiết lập sát thương cho đòn tấn công 1
        public void SetAttack01Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier; // Tính sát thương cho tay phải
            leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier; // Tính sát thương cho tay trái
        }

        // Phương thức thiết lập sát thương cho đòn tấn công 2
        public void SetAttack02Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier; // Tính sát thương cho tay phải
            leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier; // Tính sát thương cho tay trái
        }

        // Phương thức mở collider sát thương của tay phải
        public void OpenRightHandDamageCollider()
        {
            // Phát âm thanh tấn công, tạo cảm giác uy hiếp khi tay phải bắt đầu đòn đánh
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();

            rightHandDamageCollider.EnableDamageCollider(); // Kích hoạt collider tay phải
        }

        // Phương thức đóng collider sát thương của tay phải
        public void CloseRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider(); // Vô hiệu hóa collider tay phải
        }

        // Phương thức mở collider sát thương của tay trái
        public void OpenLeftHandDamageCollider()
        {
            // Phát âm thanh tấn công, tạo cảm giác uy hiếp khi tay phải bắt đầu đòn đánh
            aiCharacter.characterSoundFXManager.PlayAttackGrunt();

            leftHandDamageCollider.EnableDamageCollider(); // Kích hoạt collider tay trái
        }

        // Phương thức đóng collider sát thương của tay trái
        public void CloseLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider(); // Vô hiệu hóa collider tay trái
        }
    }
}

