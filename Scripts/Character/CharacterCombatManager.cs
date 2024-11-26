using SG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

namespace SG
{
    // Quản lý các đòn tấn công của nhân vật, bao gồm việc xác định loại tấn công hiện tại
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character; // Lưu trữ thông tin về nhân vật

        [Header("Last Attack Animation Performed")]
        // Lưu tên của animation tấn công cuối cùng đã được thực hiện.
        // Biến này giúp theo dõi để thực hiện logic combo dựa trên đòn tấn công trước đó.
        public string lastAttackAnimationPerformed;


        [Header("Attack Target")]
        public CharacterManager currentTarget; // Nhân vật mục tiêu hiện tại

        [Header("Attack Type")]
        public AttackType currentAttackType; // Biến lưu trữ loại tấn công hiện tại (ví dụ: LightAttack01)

        [Header("Lock On Transform")]
        public Transform lockOnTransform; // Biến lưu trữ vị trí lock-on

        // Phương thức Awake sẽ được gọi khi script này khởi tạo
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>(); // Lấy thông tin CharacterManager từ GameObject
        }

        // Phương thức này đặt mục tiêu tấn công cho nhân vật
        public virtual void SetTarget(CharacterManager newTarget)
        {
            if (character.IsOwner) // Kiểm tra xem nhân vật có phải là của người chơi điều khiển không
            {
                if (newTarget != null) // Nếu có mục tiêu mới
                {
                    currentTarget = newTarget; // Cập nhật mục tiêu hiện tại

                    // Cập nhật ID mục tiêu mạng
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else
                {
                    currentTarget = null; // Nếu không có mục tiêu, đặt mục tiêu thành null
                }
            }
        }
    }
}



