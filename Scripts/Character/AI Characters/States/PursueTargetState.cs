using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    // Định nghĩa trạng thái truy đuổi mục tiêu của AI
    [CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
    public class PursueTargetState : AIState
    {
        // Phương thức Tick kiểm soát logic khi AI đang ở trạng thái truy đuổi
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Kiểm tra nếu AI đang thực hiện hành động (không truy đuổi khi bận)
            if (aiCharacter.isPerformingAction)
                return this;

            // Nếu mất mục tiêu, chuyển về trạng thái Idle
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // Kích hoạt NavMeshAgent nếu nó chưa được bật
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            // Nếu mục tiêu ngoài góc nhìn, xoay AI hướng về mục tiêu
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.miniumFOV ||
                aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maxiumFOV)
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);

            // Điều chỉnh hướng AI về phía mục tiêu
            aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            // option 1: Kiểm tra khoảng cách tới mục tiêu để chuyển sang trạng thái Combat Stance
            // if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStance.maxiumEngagementDistance)
            //     return SwitchState(aiCharacter, aiCharacter.combatStance);

            // option 2: Nếu khoảng cách đến mục tiêu nhỏ hơn hoặc bằng khoảng cách dừng của agent, chuyển sang Combat Stance
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.combatStance);

            // option 1: Đặt điểm đến trực tiếp dựa vào vị trí của mục tiêu
            // aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);

            // option 2: Tạo đường dẫn dựa trên NavMesh và đặt đường dẫn cho agent
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            // Trả về chính trạng thái hiện tại (tiếp tục truy đuổi)
            return this;
        }
    }
}

