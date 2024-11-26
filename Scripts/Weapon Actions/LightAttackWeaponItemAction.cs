using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo ScriptableObject cho hành động tấn công nhẹ của vũ khí.
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        // Tên của các animation tấn công nhẹ (có thể thay đổi cho từng loại vũ khí).
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";

        // Ghi đè phương thức thực hiện hành động tấn công nhẹ.
        // Phương thức này kiểm tra các điều kiện cần thiết và thực hiện tấn công.
        public override void AttemptToPerformAction(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            // Gọi phương thức cơ bản để xử lý các logic chung (nếu có).
            base.AttemptToPerformAction(playerPerformAction, weaponPerformAction);

            // Kiểm tra nếu người chơi không phải là chủ sở hữu của nhân vật, thì không thể thực hiện hành động.
            if (!playerPerformAction.IsOwner)
                return;

            // Kiểm tra nếu người chơi không đủ stamina thì không thực hiện được hành động.
            if (playerPerformAction.playerNetworkManager.currentStamina.Value <= 0)
                return;

            // Kiểm tra nếu nhân vật không đứng trên mặt đất (isGrounded) thì không thể thực hiện đòn tấn công nhẹ.
            if (!playerPerformAction.playerLocomotionManager.isGrounded)
                return;

            // Thực hiện đòn tấn công nhẹ dựa trên các điều kiện kiểm tra.
            PerformLightAttack(playerPerformAction, weaponPerformAction);
        }

        // Phương thức để thực hiện đòn tấn công nhẹ.
        private void PerformLightAttack(PlayerManager playerPerformAction, WeaponItem weaponPerformAction)
        {
            // Nếu đang thực hiện combo và nhân vật đã thực hiện đòn tấn công trước đó.
            if (playerPerformAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformAction.isPerformingAction)
            {
                // Tắt khả năng thực hiện combo tiếp theo.
                playerPerformAction.playerCombatManager.canComboWithMainHandWeapon = false;

                // Nếu đòn tấn công gần nhất là light_Attack_01, thì thực hiện đòn tấn công light_Attack_02.
                if (playerPerformAction.playerCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_Attack_02, true);
                }
                else
                {
                    // Nếu không, thực hiện đòn tấn công light_Attack_01.
                    playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            else if (!playerPerformAction.isPerformingAction)
            {
                // Nếu nhân vật không đang thực hiện hành động nào, thực hiện đòn tấn công nhẹ đầu tiên (light_Attack_01).
                playerPerformAction.playerAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_Attack_01, true);
            }
        }
    }
}

