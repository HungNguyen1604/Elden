using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SG
{
    // Lớp AICharacterManager quản lý các hành vi và trạng thái của nhân vật AI, kế thừa từ CharacterManager
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICharacterNetworkManager aiCharacterNetworkManager; // Quản lý mạng của AI
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager; // Quản lý hành vi chiến đấu của AI
        [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager; // Quản lý di chuyển của AI

        [Header("Navmesh Agent")]
        public NavMeshAgent navMeshAgent; // Tác nhân NavMesh để AI di chuyển theo NavMesh

        [Header("Current State")]
        [SerializeField] AIState currentState; // Trạng thái hiện tại của AI (thiết lập trong Inspector)

        [Header("States")]
        public IdleState idle;               // Trạng thái đứng yên, khi AI không có mục tiêu hoặc không thực hiện hành động
        public PursueTargetState pursueTarget; // Trạng thái theo đuổi mục tiêu, khi AI phát hiện và di chuyển về phía mục tiêu
        public CombatStanceState combatStance; // Trạng thái vào tư thế chiến đấu, chuẩn bị tấn công hoặc phản ứng với mục tiêu
        public AttackState attack;            // Trạng thái tấn công, khi AI thực hiện một hành động tấn công đối với mục tiêu

        // Thiết lập các thành phần cần thiết cho AI
        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp CharacterManager
            aiCharacterNetworkManager = GetComponent<AICharacterNetworkManager>(); // Lấy tham chiếu đến AICharacterNetworkManager
            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>(); // Lấy tham chiếu đến AICharacterCombatManager
            aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>(); // Lấy tham chiếu đến AICharacterLocomotionManager
            navMeshAgent = GetComponentInChildren<NavMeshAgent>(); // Lấy NavMeshAgent từ đối tượng con

            // Khởi tạo các trạng thái bằng cách tạo bản sao (Instance)
            idle = Instantiate(idle);
            pursueTarget = Instantiate(pursueTarget);

            currentState = idle; // Đặt trạng thái ban đầu là Idle
        }


        // Cập nhật trạng thái mỗi khung hình, bao gồm xử lý phục hồi hành động của AI
        protected override void Update()
        {
            // Gọi phương thức Update của lớp cơ sở để cập nhật các trạng thái cơ bản
            base.Update();

            // Kích hoạt cơ chế phục hồi hành động trong AICombatManager
            aiCharacterCombatManager.ActionRecovery(this);
        }


        protected override void FixedUpdate()
        {
            base.FixedUpdate(); // Gọi FixedUpdate của lớp CharacterManager

            // Nếu là chủ sở hữu, xử lý logic AI bằng cách gọi máy trạng thái
            if (IsOwner)
                ProcessStateMachine();
        }


        // Phương thức xử lý logic máy trạng thái của AI
        private void ProcessStateMachine()
        {
            // Tick trạng thái hiện tại và lấy trạng thái tiếp theo nếu có
            AIState nextState = currentState?.Tick(this);

            // Nếu có trạng thái mới, chuyển đổi sang trạng thái đó
            if (nextState != null)
            {
                currentState = nextState;
            }

            // Đặt vị trí và hướng của NavMeshAgent về gốc để giữ đồng nhất với đối tượng AI
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;

            // Nếu AI có mục tiêu hiện tại, tính toán hướng và góc nhìn về phía mục tiêu
            if (aiCharacterCombatManager.currentTarget != null)
            {
                // Tính toán hướng từ AI đến mục tiêu bằng cách lấy vector từ vị trí của AI đến vị trí của mục tiêu
                aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
                // Lấy góc nhìn giữa AI và mục tiêu dựa trên hướng đã tính, sử dụng phương thức GetAngleOfTarget
                aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
                // Tính khoảng cách giữa AI và mục tiêu
                aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
            }


            // Nếu NavMeshAgent đang hoạt động, kiểm tra trạng thái di chuyển
            if (navMeshAgent.enabled)
            {
                Vector3 agentDestination = navMeshAgent.destination; // Điểm đến của NavMeshAgent
                float remainingDistance = Vector3.Distance(agentDestination, transform.position); // Khoảng cách còn lại đến điểm đến

                // Cập nhật trạng thái di chuyển của AI thông qua network manager
                aiCharacterNetworkManager.isMoving.Value = remainingDistance > navMeshAgent.stoppingDistance;
            }
            else
            {
                aiCharacterNetworkManager.isMoving.Value = false; // Ngừng cập nhật khi NavMeshAgent không hoạt động
            }
        }
    }
}
