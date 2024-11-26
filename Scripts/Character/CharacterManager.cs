using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        // Trạng thái của nhân vật (biến mạng), kiểm tra xem nhân vật có chết hay chưa
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // Khai báo biến characterController để quản lý điều khiển nhân vật
        [HideInInspector] public CharacterController characterController;
        // Khai báo biến animator để quản lý hoạt cảnh của nhân vật
        [HideInInspector] public Animator animator;
        // Khai báo biến để quản lý mạng của nhân vật
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        // Khai báo biến để quản lý các hiệu ứng của nhân vật
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        // Khai báo biến để quản lý hoạt ảnh của nhân vật
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        // Khai báo biến này sẽ quản lý các hành động tấn công của nhân vật, như loại tấn công nào đang được thực hiện và cách thức tấn công.
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        // Quản lý âm thanh hiệu ứng của nhân vật.
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        // Quản lý di chuyển của nhân vật.
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;


        // Lưu trữ nhóm mà nhân vật thuộc về, dùng để xác định nhóm của nhân vật khi cần kiểm tra đồng đội hay kẻ thù trong các tình huống chiến đấu
        [Header("Character Group")]
        public CharacterGroup characterGroup;


        [Header("Flags")]
        public bool isPerformingAction = false;// Cờ kiểm tra xem nhân vật có đang thực hiện một hành động nào đó không. 
                                               // Nếu true, nhân vật sẽ không thể thực hiện các hành động khác cho đến khi hoàn tất hành động hiện tại.



        protected virtual void Awake()
        {
            // Giữ đối tượng này không bị hủy khi chuyển sang cảnh khác
            DontDestroyOnLoad(gameObject);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        }

        protected virtual void Start()
        {
            // Gọi hàm IgnoreMyOwnColliders để đảm bảo các collider của chính nhân vật không va chạm với nhau
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            // Cập nhật trạng thái "isGrounded" cho animator
            animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);

            // Kiểm tra xem máy khách có phải là chủ sở hữu không
            if (IsOwner)
            {
                // Cập nhật vị trí và góc quay cho nhân vật
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                // Điều chỉnh vị trí cho nhân vật theo giá trị mạng
                transform.position = Vector3.SmoothDamp(transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);

                // Điều chỉnh góc quay cho nhân vật theo giá trị mạng
                transform.rotation = Quaternion.Slerp(transform.rotation,
                   characterNetworkManager.networkRotation.Value,
                   characterNetworkManager.networkRotationSmoothTime);
            }
        }


        protected virtual void FixedUpdate()
        {

        }
        protected virtual void LateUpdate()
        {

        }



        // Phương thức được gọi khi đối tượng mạng được tạo
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn(); // Gọi phương thức OnNetworkSpawn của lớp cha

            // Cập nhật trạng thái "isMoving" của Animator dựa trên giá trị isMoving của characterNetworkManager
            animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);

            // Đăng ký sự kiện thay đổi giá trị cho biến isMoving
            // Khi giá trị của isMoving thay đổi, phương thức OnIsMovingChanged sẽ được gọi
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        }

        // Phương thức được gọi khi đối tượng mạng bị hủy
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn(); // Gọi phương thức OnNetworkDespawn của lớp cha

            // Hủy đăng ký sự kiện thay đổi giá trị cho biến isMoving
            // Điều này giúp ngăn ngừa lỗi khi đối tượng không còn tồn tại trong mạng
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        }



        // Coroutine dùng để xử lý sự kiện chết của nhân vật trong game.
        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            // Kiểm tra nếu nhân vật là chủ sở hữu trên mạng (đảm bảo chỉ nhân vật này mới thực thi quá trình chết)
            if (IsOwner)
            {
                // Đặt máu của nhân vật về 0 để thể hiện nhân vật đã chết
                characterNetworkManager.currentHealth.Value = 0;
                // Đánh dấu rằng nhân vật đã chết bằng cách cập nhật biến isDead
                isDead.Value = true;
                // Kiểm tra xem có cần chọn hoạt ảnh chết thủ công không
                if (!manuallySelectDeathAnimation) // Nếu không, chọn hoạt ảnh chết mặc định
                {
                    // Phát hoạt ảnh chết (ở đây là "Dead_01") và ngăn nhân vật thực hiện các hành động khác
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }
            // Tạm dừng quá trình trong 5 giây trước khi thực hiện các hành động khác (như hồi sinh hoặc tải giao diện kết thúc trò chơi)
            yield return new WaitForSeconds(5);
        }

        // Phương thức này thực hiện logic hồi sinh nhân vật.
        public virtual void ReviveCharacter()
        {

        }


        // Hàm bảo vệ IgnoreMyOwnColliders có mục đích để bỏ qua việc xử lý va chạm giữa các collider của chính nhân vật
        protected virtual void IgnoreMyOwnColliders()
        {
            // Lấy collider của CharacterController hiện tại
            Collider characterControllerCollider = GetComponent<Collider>();

            // Lấy tất cả các collider con thuộc về nhân vật mà có khả năng nhận damage
            Collider[] damagebleCharacterColliders = GetComponentsInChildren<Collider>();

            // Tạo danh sách các collider sẽ bị bỏ qua va chạm
            List<Collider> ignoreColliders = new List<Collider>();

            // Thêm tất cả các collider con vào danh sách ignoreColliders
            foreach (var collider in damagebleCharacterColliders)
            {
                ignoreColliders.Add(collider);
            }

            // Thêm collider của CharacterController vào danh sách ignoreColliders
            ignoreColliders.Add(characterControllerCollider);

            // Bỏ qua va chạm giữa tất cả các collider trong danh sách ignoreColliders
            foreach (var collider in ignoreColliders)
            {
                foreach (var otherCollider in ignoreColliders)
                {
                    // Thiết lập để hai collider này không va chạm với nhau
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }

    }
}


