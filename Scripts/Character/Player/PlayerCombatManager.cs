using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SG
{
    // PlayerCombatManager kế thừa từ CharacterCombatManager để quản lý chiến đấu của người chơi
    public class PlayerCombatManager : CharacterCombatManager
    {
        // Tham chiếu đến PlayerManager để quản lý các trạng thái của người chơi
        PlayerManager player;

        // Vũ khí hiện tại mà người chơi đang sử dụng
        public WeaponItem currentWeaponBeingUsed;

        [Header("Flags")]
        // Cho phép combo với vũ khí tay chính (true nếu có thể thực hiện combo).
        public bool canComboWithMainHandWeapon = false;
        // Cho phép combo với vũ khí tay phụ (true nếu có thể thực hiện combo).
        public bool canComboWithOffHandWeapon = false;



        // Override phương thức Awake để thực hiện các hành động khởi tạo cho PlayerCombatManager
        protected override void Awake()
        {
            // Gọi hàm Awake từ lớp cha (CharacterCombatManager) để đảm bảo các khởi tạo của lớp cha được thực thi
            base.Awake();

            // Lấy tham chiếu đến PlayerManager từ thành phần của người chơi
            player = GetComponent<PlayerManager>();
        }


        // Hàm thực hiện một hành động dựa trên vũ khí (ví dụ: tấn công, phòng thủ, né tránh,...)
        // weaponAction: hành động dựa trên vũ khí (chứa logic của hành động cụ thể)
        // weaponPerformingAction: vũ khí hiện tại mà người chơi đang sử dụng để thực hiện hành động
        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            // Kiểm tra nếu người chơi là chủ sở hữu (thường dùng trong các game mạng nhiều người chơi để đảm bảo đúng chủ sở hữu điều khiển nhân vật)
            if (player.IsOwner)
            {
                // Gọi phương thức AttemptToPerformAction từ đối tượng WeaponItemAction.
                // Truyền vào tham chiếu đến người chơi và vũ khí đang được sử dụng để thực hiện hành động.
                // Phương thức này sẽ thực hiện hành động dựa trên logic trong WeaponItemAction.
                weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

                // Gửi yêu cầu lên server thông qua Remote Procedure Call (RPC) để thông báo về hành động vũ khí.
                // Truyền vào ID của client, ID của hành động, và ID của vật phẩm (vũ khí) để đồng bộ giữa các client trong game.
                player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
            }
        }

        // Hàm tính toán và trừ điểm stamina dựa trên loại tấn công được thực hiện
        public virtual void DrainStaminaBaseOnAttack()
        {
            // Nếu người chơi không phải chủ sở hữu (client không phải là người điều khiển nhân vật), thì không làm gì
            if (!player.IsOwner)
                return;
            // Nếu không có vũ khí đang được sử dụng, thì không trừ stamina
            if (currentWeaponBeingUsed == null)
                return;
            // Biến lưu trữ lượng stamina sẽ bị trừ, ban đầu là 0
            float staminaDeducted = 0;

            // Kiểm tra loại tấn công hiện tại và tính toán lượng stamina tiêu hao tương ứng
            switch (currentAttackType)
            {
                case AttackType.LightAttack01:
                    // Nếu là tấn công nhẹ (LightAttack01), lượng stamina bị trừ sẽ bằng baseStaminaCost của vũ khí 
                    // nhân với hệ số lightAttackStaminaCostMultiplier (điều chỉnh mức tiêu hao stamina cho tấn công nhẹ)
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;

                default:
                    // Trường hợp không xác định, không làm gì thêm
                    break;
            }

            // In ra giá trị stamina bị trừ để kiểm tra (chỉ dành cho mục đích debug)
            //Debug.Log("Lượng thể lực bị trừ: " + staminaDeducted);

            // Trừ điểm stamina của nhân vật dựa trên giá trị đã tính toán
            // player.playerNetworkManager.currentStamina.Value là biến lưu trữ lượng stamina hiện tại của nhân vật
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted); // Làm tròn số và trừ đi
        }

        //Phương thức này thiết lập một mục tiêu mới cho nhân vật và điều chỉnh chiều cao camera nếu người chơi là chủ sở hữu của nhân vật.
        public override void SetTarget(CharacterManager newTarget)
        {
            // Gọi phương thức SetTarget từ lớp cơ sở để thiết lập mục tiêu mới
            base.SetTarget(newTarget);

            // Kiểm tra xem người chơi hiện tại có phải là chủ sở hữu của nhân vật không
            if (player.IsOwner)
            {
                // Nếu là chủ sở hữu, gọi phương thức SetLockCameraHeight để điều chỉnh chiều cao camera
                PlayerCamera.instance.SetLockCameraHeight();
            }
        }

    }
}
