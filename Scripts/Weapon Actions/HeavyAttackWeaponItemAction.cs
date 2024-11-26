using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo lớp hành động cho đòn tấn công nặng, kế thừa từ WeaponItemAction.
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01"; // Tên animation cho đòn tấn công nặng.
        [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02"; // Tên animation cho đòn tấn công nặng.

        // Phương thức cố gắng thực hiện hành động tấn công.
        public override void AttemptToPerformAction(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            // Gọi phương thức cơ sở để thực hiện các thao tác chung.
            base.AttemptToPerformAction(playerPerformAction, weaponPerformAction);

            // Kiểm tra nếu người chơi không phải là chủ sở hữu của hành động.
            if (!playerPerformAction.IsOwner)
                return; // Kết thúc nếu không phải là chủ sở hữu.

            // Kiểm tra nếu stamina hiện tại bằng 0.
            if (playerPerformAction.playerNetworkManager.currentStamina.Value <= 0)
                return; // Kết thúc nếu không đủ stamina.

            // Kiểm tra nếu người chơi không ở trên mặt đất.
            if (!playerPerformAction.playerLocomotionManager.isGrounded)
                return; // Kết thúc nếu không ở trên mặt đất.

            // Thực hiện đòn tấn công nặng nếu tất cả điều kiện đều thỏa mãn.
            PerformHeavyAttack(playerPerformAction, weaponPerformAction);
        }

        // Phương thức thực hiện đòn tấn công nặng.
        private void PerformHeavyAttack(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            // Kiểm tra khả năng combo và trạng thái đang thực hiện hành động.
            if (playerPerformAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformAction.isPerformingAction)
            {
                // Tắt khả năng thực hiện combo tiếp theo.
                playerPerformAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // Kiểm tra đòn tấn công trước đó để quyết định đòn tiếp theo.
                if (playerPerformAction.playerCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02, heavy_Attack_02, true); // Phát đòn tấn công nặng 02.
                }
                else
                {
                    // Nếu không, thực hiện đòn tấn công nặng 01.
                    playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true); // Phát đòn tấn công nặng 01.
                }
            }
            else if (!playerPerformAction.isPerformingAction)
            {
                // Nếu nhân vật không đang thực hiện hành động nào, thực hiện đòn tấn công nặng đầu tiên (heavy_Attack_01).
                playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_Attack_01, true); // Tấn công nặng khi không có hành động nào khác.
            }
        }
    }
}

