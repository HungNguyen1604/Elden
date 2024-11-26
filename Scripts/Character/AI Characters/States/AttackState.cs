using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo một menu tùy chọn để tạo trạng thái tấn công cho AI trong Unity
    [CreateAssetMenu(menuName = "A.I/States/Attack")]

    // Lớp đại diện cho trạng thái tấn công trong hệ thống AI
    public class AttackState : AIState
    {
        [Header("Current Attack")]
        [HideInInspector] public AICharacterAttackAction currentAttack; // Hành động tấn công hiện tại mà AI sẽ thực hiện
        [HideInInspector] public bool willPerformCombo = false;         // Cờ để xác định nếu AI sẽ thực hiện combo sau tấn công

        [Header("State Flags")]
        protected bool hasPerformedAttack = false;  // Cờ xác định nếu AI đã thực hiện một lần tấn công trong trạng thái này
        protected bool hasPerformedCombo = false;   // Cờ xác định nếu AI đã thực hiện combo sau tấn công

        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false; // Xác định nếu AI sẽ quay hướng sau khi tấn công

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Kiểm tra nếu AI không có mục tiêu, chuyển sang trạng thái đứng yên
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);
            // Kiểm tra nếu mục tiêu đã chết, chuyển sang trạng thái đứng yên hoặc
            if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // Nếu mục tiêu còn sống, thực hiện quay về phía mục tiêu trong khi tấn công
            aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);

            // Cập nhật thông số chuyển động cho hoạt ảnh, đặt AI ở trạng thái đứng yên
            aiCharacter.characterAnimatorManager.UpdateAnimationMovementParameters(0, 0, false);

            // Nếu có thể thực hiện combo và AI chưa thực hiện combo
            if (willPerformCombo && !hasPerformedCombo)
            {
                // Nếu hành động combo có sẵn, chuẩn bị thực hiện combo
                if (currentAttack.comboAction != null)
                {
                    // Đánh dấu đã thực hiện combo và thực hiện hành động combo
                    hasPerformedCombo = true;
                    currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
                }
            }
            // Nếu AI đang thực hiện hành động khác, giữ nguyên trạng thái hiện tại
            if (aiCharacter.isPerformingAction)
                return this;

            // Kiểm tra và thực hiện tấn công nếu AI chưa tấn công
            if (!hasPerformedAttack)
            {
                // Nếu thời gian phục hồi hành động vẫn còn, giữ nguyên trạng thái hiện tại
                if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
                    return this;

                // Thực hiện tấn công nếu không có hành động nào và thời gian phục hồi đã hết
                PerformAttack(aiCharacter);
                return this;
            }
            // Nếu tùy chọn xoay AI sau khi tấn công được kích hoạt, thực hiện xoay về phía mục tiêu
            if (pivotAfterAttack)
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            // Chuyển trạng thái AI về chế độ chiến đấu sau khi tấn công
            return SwitchState(aiCharacter, aiCharacter.combatStance);

        }


        // Thực hiện tấn công và thiết lập thời gian phục hồi cho AI
        protected void PerformAttack(AICharacterManager aiCharacter)
        {
            // Đánh dấu rằng AI đã thực hiện một cuộc tấn công
            hasPerformedAttack = true;

            // Gọi hành động tấn công từ currentAttack để thực thi hoạt ảnh hoặc hiệu ứng
            currentAttack.AttemptToPerformAction(aiCharacter);

            // Thiết lập thời gian phục hồi cho AI sau khi tấn công, tránh thực hiện hành động ngay lập tức
            aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
        }


        //Phương thức để đặt lại các cờ trạng thái cho AICharacter.
        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            // Gọi phương thức ResetStateFlags của lớp cha để đặt lại các cờ trạng thái cơ bản
            base.ResetStateFlags(aiCharacter);

            // Đặt lại cờ hasPerformedAttack về false để cho phép thực hiện tấn công mới
            hasPerformedAttack = false;

            // Đặt lại cờ hasPerformedCombo về false để cho phép thực hiện combo mới
            hasPerformedCombo = false;
        }

    }
}
