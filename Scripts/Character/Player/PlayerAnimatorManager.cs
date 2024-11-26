using SG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace SG
{
    public class PlayerAnimatorManager : CharacterAnimatorManager
    {
        // Tham chiếu đến PlayerManager, để có thể truy cập các thuộc tính và phương thức của nó
        PlayerManager player;

        protected override void Awake()
        {
            // Gọi phương thức Awake từ lớp cơ sở để thực hiện các khởi tạo cần thiết
            base.Awake();
            // Lấy tham chiếu đến PlayerManager từ component
            player = GetComponent<PlayerManager>();
        }
        // Phương thức này được gọi bởi Unity mỗi khi Animator cập nhật vị trí của nhân vật dựa trên root motion.
        private void OnAnimatorMove()
        {
            // Kiểm tra xem root motion có được áp dụng không
            if (applyRootMotion)
            {
                // Lấy độ dịch chuyển delta từ animator
                Vector3 velocity = player.animator.deltaPosition;

                // Di chuyển nhân vật dựa trên độ dịch chuyển từ animator
                player.characterController.Move(velocity);

                // Cập nhật hướng nhân vật dựa trên độ quay delta từ animator
                player.transform.rotation *= player.animator.deltaRotation;
            }
        }



        // Kích hoạt khả năng thực hiện combo dựa trên tay đang sử dụng.
        // Nếu người chơi đang sử dụng tay phải, cho phép combo với vũ khí tay chính.
        public override void EnableCanDoComBo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)  // Kiểm tra xem người chơi có đang dùng tay phải không.
            {
                // Cho phép combo với vũ khí tay chính (main hand).
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {
                // Hiện tại chưa có logic cho trường hợp tay trái.
            }
        }

        // Vô hiệu hóa khả năng thực hiện combo với vũ khí tay chính.
        public override void DisableCanDoComBo()
        {
            // Tắt khả năng combo với vũ khí tay chính.
            player.playerCombatManager.canComboWithMainHandWeapon = false;
        }

    }
}

