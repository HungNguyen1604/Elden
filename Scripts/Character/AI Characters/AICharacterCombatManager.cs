using SG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    // Lớp AICharacterCombatManager kế thừa từ CharacterCombatManager, xử lý logic chiến đấu của nhân vật AI
    public class AICharacterCombatManager : CharacterCombatManager
    {
        // Tham chiếu đến AICharacterManager của nhân vật AI.
        protected AICharacterManager aiCharacter;


        [Header("Action Recovery")]
        // Thời gian phục hồi sau khi AI thực hiện một hành động, giúp kiểm soát tần suất tấn công hoặc hành vi khác
        public float actionRecoveryTimer = 0;


        // Thông tin về mục tiêu mà AI đang tương tác
        [Header("Target Information")]
        // Khoảng cách hiện tại từ AI đến mục tiêu
        public float distanceFromTarget;
        // Góc nhìn của AI tới mục tiêu, giúp xác định nếu mục tiêu nằm trong tầm nhìn.
        public float viewableAngle;
        // Hướng từ AI tới mục tiêu, hỗ trợ AI di chuyển và xoay về phía mục tiêu.
        public Vector3 targetsDirection;


        [Header("Detection")]
        // Bán kính phát hiện mục tiêu xung quanh AI
        [SerializeField] float detectionRadius = 15;
        // Góc tối thiểu để phát hiện mục tiêu (góc nhìn)
        public float miniumFOV = -35;
        // Góc tối đa để phát hiện mục tiêu (góc nhìn)
        public float maxiumFOV = 35;

        [Header("Attack Rotation Speed")]
        // Tốc độ xoay của AI khi thực hiện tấn công
        public float attackRotationSpeed = 25;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
            // Gán lockOnTransform bằng cách tìm và lấy component LockOnTransform từ các đối tượng con
            // Sau đó lấy Transform của đối tượng đó để xác định điểm khóa mục tiêu
            lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
        }


        // Phương thức tìm kiếm mục tiêu dựa trên tầm nhìn của AI
        public void FindATargetViaLindOfSight(AICharacterManager aiCharacter)
        {
            // Nếu đã có mục tiêu, không cần tìm nữa
            if (currentTarget != null)
                return;

            // Sử dụng OverlapSphere để tìm các đối tượng trong bán kính phát hiện
            // Phạm vi phát hiện dựa trên position của AI và bán kính detectionRadius, chỉ lấy các đối tượng trong layer nhân vật
            Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

            // Duyệt qua tất cả các đối tượng phát hiện được
            for (int i = 0; i < colliders.Length; i++)
            {
                // Lấy thành phần CharacterManager từ đối tượng để kiểm tra xem đó có phải là nhân vật hay không
                CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

                // Nếu không phải nhân vật, bỏ qua
                if (targetCharacter == null)
                    continue;

                // Nếu mục tiêu là chính AI, bỏ qua
                if (targetCharacter == aiCharacter)
                    continue;

                // Nếu mục tiêu đã chết, bỏ qua
                if (targetCharacter.isDead.Value)
                    continue;

                // Kiểm tra xem AI có thể tấn công mục tiêu này không (dựa vào nhóm)
                if (WorldUtilityManager.instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                {
                    // Tính toán hướng của mục tiêu so với AI
                    Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    // Tính toán góc giữa hướng của mục tiêu và hướng nhìn của AI
                    float angleOfPotentialTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                    // Nếu mục tiêu nằm trong góc nhìn của AI (nằm giữa góc FOV tối thiểu và tối đa của AI)
                    if (angleOfPotentialTarget > miniumFOV && angleOfPotentialTarget < maxiumFOV)
                    {
                        // Sử dụng Linecast để kiểm tra xem có vật cản nào giữa AI và mục tiêu hay không
                        // Nếu có vật cản thuộc layer môi trường, tức là giữa AI và mục tiêu bị chặn
                        if (Physics.Linecast(
                            aiCharacter.characterCombatManager.lockOnTransform.position, // Vị trí xuất phát của tia là vị trí lock-on của AI
                            targetCharacter.characterCombatManager.lockOnTransform.position, // Vị trí kết thúc là vị trí lock-on của mục tiêu
                            WorldUtilityManager.instance.GetEnviroLayers())) // Lớp môi trường cần kiểm tra
                        {
                            // Nếu có vật cản, vẽ một đường thẳng trong chế độ Debug từ AI đến mục tiêu để dễ kiểm tra trong khi phát triển
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                            // Có thể thêm lệnh Debug.Log để ghi lại thông tin về vật cản nếu cần thiết
                        }
                        else
                        {
                            // Nếu không có vật cản giữa AI và mục tiêu, tiếp tục thiết lập hướng di chuyển về phía mục tiêu
                            // Tính toán hướng của mục tiêu dựa trên vị trí của AI và mục tiêu
                            targetsDirection = targetCharacter.transform.position - transform.position;

                            // Lấy góc giữa AI và hướng của mục tiêu từ hàm GetAngleOfTarget
                            viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);

                            // Thiết lập mục tiêu của AI là targetCharacter, để bắt đầu quá trình theo dõi và hành động
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                            // Xoay AI hướng về phía mục tiêu để chuẩn bị cho hành động tiếp theo
                            PivotTowardsTarget(aiCharacter);
                        }
                    }
                }
            }
        }

        // Phương thức điều chỉnh hướng AI để quay về phía mục tiêu theo góc cụ thể
        public void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            // Nếu AI đang thực hiện hành động khác, thoát khỏi phương thức
            if (aiCharacter.isPerformingAction)
                return;

            // Nếu góc trong khoảng 20 đến 60 độ, quay phải 45 độ
            if (viewableAngle >= 20 && viewableAngle <= 60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R45", true);
            }
            // Nếu góc trong khoảng -20 đến -60 độ, quay trái 45 độ
            if (viewableAngle >= -20 && viewableAngle <= -60)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L45", true);
            }
            // Nếu góc trong khoảng 61 đến 110 độ, quay phải 90 độ
            if (viewableAngle >= 61 && viewableAngle <= 110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R90", true);
            }
            // Nếu góc trong khoảng -61 đến -110 độ, quay trái 90 độ
            if (viewableAngle >= -61 && viewableAngle <= -110)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L90", true);
            }
            // Nếu góc trong khoảng 110 đến 145 độ, quay phải 135 độ
            if (viewableAngle >= 110 && viewableAngle <= 145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R135", true);
            }
            // Nếu góc trong khoảng -110 đến -145 độ, quay trái 135 độ
            if (viewableAngle >= -110 && viewableAngle <= -145)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L135", true);
            }
            // Nếu góc trong khoảng 146 đến 180 độ, quay phải 180 độ
            if (viewableAngle >= 146 && viewableAngle <= 180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_R180", true);
            }
            // Nếu góc trong khoảng -146 đến -180 độ, quay trái 180 độ
            if (viewableAngle >= -146 && viewableAngle <= -180)
            {
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_L180", true);
            }
        }



        // Phương thức xoay AI về hướng của tác nhân (agent) nếu đang di chuyển
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            // Kiểm tra xem tác nhân có đang di chuyển hay không
            if (aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                // Nếu đang di chuyển, cập nhật góc quay của AI để khớp với góc quay của navMeshAgent
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }

        // Phương thức để xoay AI về phía mục tiêu trong khi đang tấn công
        public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
        {
            // Kiểm tra xem mục tiêu hiện tại có tồn tại không
            if (currentTarget == null)
                return;

            // Kiểm tra xem AI có thể xoay hay không
            if (!aiCharacter.characterLocomotionManager.canRotate)
                return;

            // Kiểm tra xem AI có đang thực hiện hành động không
            if (!aiCharacter.isPerformingAction)
                return;

            // Tính toán hướng từ AI đến mục tiêu
            Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
            targetDirection.y = 0; // Đặt trục Y về 0 để không xoay theo chiều dọc
            targetDirection.Normalize(); // Chuẩn hóa vector hướng

            // Nếu hướng là vector không, sử dụng hướng hiện tại của AI
            if (targetDirection == Vector3.zero)
                targetDirection = aiCharacter.transform.forward;

            // Tính toán góc quay mục tiêu
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Xoay AI về phía mục tiêu với tốc độ xoay nhất định
            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
        }


        // Quản lý thời gian phục hồi sau hành động cho AI
        public void ActionRecovery(AICharacterManager aiCharacter)
        {
            // Nếu thời gian phục hồi còn lại lớn hơn 0
            if (actionRecoveryTimer > 0)
            {
                // Nếu AI không đang thực hiện hành động, giảm dần thời gian phục hồi
                if (!aiCharacter.isPerformingAction)
                {
                    actionRecoveryTimer -= Time.deltaTime;
                }
            }
        }

    }
}

