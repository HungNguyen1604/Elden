using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    // Tạo menu tùy chọn trong Unity để cho phép người dùng tạo đối tượng Combat Stance cho AI từ menu A.I
    [CreateAssetMenu(menuName = "A.I/States/Combat Stance")]

    public class CombatStanceState : AIState
    {
        // Cờ tấn công của con AI
        [Header("Attacks")]
        // Danh sách các hành động tấn công của AI
        public List<AICharacterAttackAction> aiCharacterAttacks;
        // Danh sách các hành động tấn công tiềm năng của AI
        protected List<AICharacterAttackAction> potentialAttacks;

        // Hành động tấn công được chọn để thực hiện
        private AICharacterAttackAction choosenAttack;
        // Hành động tấn công trước đó đã được thực hiện, dùng để kiểm tra hoặc xử lý hành động liên tiếp
        private AICharacterAttackAction previousAttack;
        // Cờ xác định liệu đã có hành động tấn công được thực hiện hay chưa
        protected bool hasAttack = false;


        // Cờ cho phép thực hiện combo và tỉ lệ phần trăm thực hiện combo
        [Header("Combo")]
        [SerializeField] protected bool canPerformCombo = false;          // Có thể thực hiện combo hay không
        [SerializeField] protected int chanceToPerformCombo = 25;         // Tỉ lệ phần trăm thực hiện combo
        [SerializeField] protected bool hasRolledForComboChance = false;  // Đã kiểm tra tỉ lệ combo chưa


        // Khoảng cách tối đa để AI bắt đầu tương tác
        [Header("Engagement Distance")]
        [SerializeField] public float maxiumEngagementDistance = 5;


        public override AIState Tick(AICharacterManager aiCharacter)
        {
            // Nếu AI đang thực hiện một hành động, thoát khỏi phương thức hiện tại
            if (aiCharacter.isPerformingAction)
                return this;


            // Nếu navMeshAgent của AI không được kích hoạt, kích hoạt nó
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;


            // Kiểm tra xem AI có đang di chuyển hay không
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                // Nếu góc nhìn về phía mục tiêu nằm ngoài khoảng -30 đến 30 độ
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    // Quay AI hướng về phía mục tiêu
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }

            // Gọi phương thức xoay AI về hướng của tác nhân (agent) trong quản lý chiến đấu
            aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

            // Kiểm tra xem AI có mục tiêu hiện tại hay không, nếu không có mục tiêu, chuyển trạng thái về chế độ đứng yên
            if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            // Kiểm tra xem AI có hành động tấn công nào hay không
            if (!hasAttack)
            {
                // Nếu chưa có hành động tấn công, gọi phương thức để lấy một hành động tấn công mới
                GetNewAttack(aiCharacter);
            }
            else
            {
                // Nếu đã có hành động tấn công, thiết lập hành động tấn công hiện tại cho AI
                aiCharacter.attack.currentAttack = choosenAttack;
                // Chuyển trạng thái của AI sang trạng thái tấn công
                return SwitchState(aiCharacter, aiCharacter.attack);
            }


            // Kiểm tra xem khoảng cách đến mục tiêu có lớn hơn khoảng cách tối đa để tham gia tấn công hay không
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maxiumEngagementDistance)
                // Nếu khoảng cách lớn hơn, chuyển sang trạng thái theo đuổi mục tiêu
                return SwitchState(aiCharacter, aiCharacter.pursueTarget);

            // Tạo một đối tượng NavMeshPath để lưu trữ đường đi
            NavMeshPath path = new NavMeshPath();
            // Tính toán đường đi từ AI đến vị trí của mục tiêu hiện tại
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            // Thiết lập đường đi đã tính toán cho navMeshAgent của AI
            aiCharacter.navMeshAgent.SetPath(path);
            // Trả về trạng thái hiện tại (có thể để tiếp tục thực hiện hành động tiếp theo)
            return this;

        }

        // Lấy một hành động tấn công mới dựa trên khoảng cách và góc nhìn đến mục tiêu
        protected virtual void GetNewAttack(AICharacterManager aiCharacter)
        {
            // Khởi tạo danh sách các hành động tấn công tiềm năng
            potentialAttacks = new List<AICharacterAttackAction>();

            // Duyệt qua tất cả các hành động tấn công tiềm năng
            foreach (var potentialAttack in aiCharacterAttacks)
            {
                // Bỏ qua nếu khoảng cách đến mục tiêu nhỏ hơn khoảng cách tối thiểu để tấn công
                if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;
                // Bỏ qua nếu khoảng cách đến mục tiêu lớn hơn khoảng cách tối đa để tấn công
                if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                    continue;
                // Bỏ qua nếu góc nhìn nhỏ hơn góc tối thiểu để tấn công
                if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;
                // Bỏ qua nếu góc nhìn lớn hơn góc tối đa để tấn công
                if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                    continue;

                // Thêm hành động tấn công vào danh sách nếu thỏa mãn tất cả điều kiện
                potentialAttacks.Add(potentialAttack);
            }


            // Nếu không có hành động tấn công tiềm năng nào, thoát khỏi hàm
            if (potentialAttacks.Count <= 0)
                return;

            // Tính tổng trọng số của các hành động tấn công tiềm năng
            var totalWeight = 0;

            foreach (var attack in potentialAttacks)
            {
                totalWeight += attack.attackWeight; // Cộng dồn trọng số của từng hành động
            }

            // Lấy một giá trị ngẫu nhiên dựa trên tổng trọng số
            var randomWeightValue = Random.Range(1, totalWeight + 1);
            var processedWeight = 0;

            // Duyệt qua các hành động tấn công tiềm năng để chọn một hành động dựa trên trọng số
            foreach (var attack in potentialAttacks)
            {
                processedWeight += attack.attackWeight; // Cộng dồn trọng số đã xử lý

                // Nếu giá trị ngẫu nhiên nằm trong trọng số đã xử lý, chọn hành động đó
                if (randomWeightValue <= processedWeight)
                {
                    choosenAttack = attack;  // Gán hành động đã chọn
                    previousAttack = choosenAttack; // Lưu lại hành động trước đó
                    hasAttack = true; // Đánh dấu rằng đã có hành động tấn công
                    return; // Thoát khỏi phương thức sau khi đã chọn được hành động
                }
            }
        }


        // Xác định khả năng thực hiện một hành động dựa trên tỉ lệ phần trăm
        protected virtual bool RollForOutcomeChance(int outcomeChance)
        {
            // Cờ xác định liệu hành động có được thực hiện hay không (mặc định là không thực hiện)
            bool outcomeWillBePerformed = false;

            // Tạo một số ngẫu nhiên từ 0 đến 99 để so sánh với tỉ lệ đầu vào
            int randomPercentage = Random.Range(0, 100);

            // Nếu số ngẫu nhiên nhỏ hơn tỉ lệ đầu vào, hành động sẽ được thực hiện
            if (randomPercentage < outcomeChance)
                outcomeWillBePerformed = true;

            // Trả về kết quả, true nếu hành động sẽ được thực hiện, false nếu không
            return outcomeWillBePerformed;
        }


        // Đặt lại các cờ trạng thái của AI
        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasAttack = false;                 // Đặt lại cờ tấn công
            hasRolledForComboChance = false;   // Đặt lại cờ kiểm tra tỉ lệ combo
        }

    }

}