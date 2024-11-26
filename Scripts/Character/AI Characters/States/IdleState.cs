using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp IdleState kế thừa từ AIState, dùng để quản lý trạng thái chờ (Idle) của AI
    [CreateAssetMenu(menuName = "A.I/States/Idle")]
    public class IdleState : AIState
    {
        // Ghi đè phương thức Tick để xử lý logic của trạng thái Idle cho AI
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Kiểm tra nếu AI đã có mục tiêu trong tầm ngắm
            if (aiCharacter.characterCombatManager.currentTarget != null)
            {
                // Chuyển sang trạng thái truy đuổi nếu có mục tiêu
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);
            }
            else
            {
                // Nếu chưa có mục tiêu, AI sẽ tìm kiếm trong tầm nhìn
                aiCharacter.aiCharacterCombatManager.FindATargetViaLindOfSight(aiCharacter);

                // Giữ AI trong trạng thái Idle nếu chưa tìm thấy mục tiêu
                return this;
            }
        }

    }
}

