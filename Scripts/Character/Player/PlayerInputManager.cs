using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;// Sử dụng để quản lý các cảnh trong Unity

namespace SG
{
    // Lớp quản lý các input của người chơi, sử dụng Singleton để đảm bảo chỉ có một instance duy nhất
    public class PlayerInputManager : MonoBehaviour
    {
        public static PlayerInputManager instance;// Singleton instance để dễ dàng truy cập từ các lớp khác
        public PlayerManager player;// Tham chiếu đến PlayerManager để biết nhân vật nào cần theo dõi
        private PlayerControls playerControls;// Đối tượng chứa các hành động input được tạo từ Input Actions Asset

        [Header("CAMERA MOVEMENT INPUT")]
        [SerializeField] Vector2 cameraInput;// Lưu trữ giá trị đầu vào của camera
        public float cameraVerticalInput; // Lưu trữ giá trị đầu vào của camera theo trục Y (thẳng đứng)
        public float cameraHorizontalInput; // Lưu trữ giá trị đầu vào của camera theo trục X (ngang)


        [Header("LOCK ON INPUT")]
        [SerializeField] bool lockOn_Input; // lockOn_Input sẽ kiểm tra xem người chơi có nhấn phím để bật/tắt tính năng lock-on hay không
        [SerializeField] bool lockOn_Left_Input; // Khai báo biến để theo dõi trạng thái đầu vào của khóa mục tiêu bên trái
        [SerializeField] bool lockOn_Right_Input; // Khai báo biến để theo dõi trạng thái đầu vào của khóa mục tiêu bên phải
        private Coroutine lockOnCoroutine; // Coroutine để quản lý trạng thái khóa mục tiêu

        [Header("PLAYER MOVEMENT INPUT")]
        [SerializeField] Vector2 movementInput;// Lưu trữ giá trị đầu vào di chuyển của người chơi
        public float vertical_Input; // Lưu trữ giá trị đầu vào theo trục Y (thẳng đứng)
        public float horizontal_Input; // Lưu trữ giá trị đầu vào theo trục X (ngang)
        public float moveAmount; // Lưu trữ tổng lượng di chuyển dựa trên đầu vào

        [Header("PLAYER ACTION INPUT")]
        // Giá trị mặc định là false, tức là không có hành động nào được thực hiện cho đến khi người chơi nhấn nút(tránh, chạy nhanh, nhảy).
        [SerializeField] bool dodge_Input = false; // Biến để theo dõi trạng thái đầu vào cho hành động dodge (tránh). 
        [SerializeField] bool sprintInput = false; // Biến để theo dõi trạng thái đầu vào cho hành động sprint (chạy nhanh).
        [SerializeField] bool jumpInput = false; // Biến để theo dõi trạng thái đầu vào cho hành động jump (nhảy). 
        [SerializeField] bool switch_Right_Weapon_Input = false; // Biến theo dõi trạng thái đầu vào cho việc đổi vũ khí tay phải.
        [SerializeField] bool switch_Left_Weapon_Input = false; // Biến theo dõi trạng thái đầu vào cho việc đổi vũ khí tay trái.


        [Header("BUMPER INPUTS")]
        [SerializeField] bool LMB_Input = false; // Biến để theo dõi trạng thái đầu vào cho hành động LMB (Left Mouse Button - chuột trái).

        [Header("TRIGGER INPUTS")]
        [SerializeField] bool RMB_Input = false; // Biến để theo dõi trạng thái đầu vào cho hành động RMB (Right Mouse Button - chuột phải).
        [SerializeField] bool Hold_RMP_Input = false; // Biến để theo dõi trạng thái giữ nút RMB (chuột phải) lâu.


        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // Gán instance nếu chưa có
            }
            else
            {
                Destroy(gameObject); // Hủy đối tượng hiện tại nếu đã có instance khác
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); // Giữ đối tượng không bị hủy khi tải cảnh mới
            SceneManager.activeSceneChanged += OnSceneChanged; // Đăng ký phương thức OnSceneChanged để xử lý khi cảnh thay đổi
            instance.enabled = false; // Vô hiệu hóa PlayerInputManager ban đầu

            // Nếu playerControls đã được khởi tạo, vô hiệu hóa các điều khiển của người chơi
            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            // Kiểm tra nếu chỉ số cảnh mới bằng chỉ số cảnh thế giới từ WorldSaveGameManager
            if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
            {
                instance.enabled = true; // Kích hoạt PlayerInputManager
                // Nếu playerControls đã được khởi tạo, vô hiệu hóa các điều khiển của người chơi
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
            else
            {
                instance.enabled = false; // Vô hiệu hóa PlayerInputManager
                // Nếu playerControls đã được khởi tạo, vô hiệu hóa các điều khiển của người chơi
                if (playerControls != null)
                {
                    playerControls.Disable();
                }
            }
        }

        private void OnEnable()
        {
            // Kiểm tra xem playerControls có chưa, nếu chưa thì khởi tạo
            if (playerControls == null)
            {
                playerControls = new PlayerControls(); // Tạo một instance mới của PlayerControls

                // Đăng ký sự kiện khi người chơi thực hiện hành động di chuyển
                playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
                // Đăng ký sự kiện cho camera khi người chơi di chuyển
                playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

                // Đăng ký sự kiện cho người chơi thực hiện hành động né tránh
                playerControls.PlayerActions.Dodge.performed += i => dodge_Input = true;
                // Đăng ký sự kiện cho người chơi thực hiện hành động nhảy
                playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

                // Gán sự kiện khi người chơi thực hiện hành động đổi vũ khí tay phải
                playerControls.PlayerActions.SwitchRightWeapon.performed += instance => switch_Right_Weapon_Input = true;
                // Gán sự kiện khi người chơi thực hiện hành động đổi vũ khí tay trái
                playerControls.PlayerActions.SwitchLeftWeapon.performed += instance => switch_Left_Weapon_Input = true;


                // Đăng ký sự kiện khi người chơi thực hiện hành động tấn công bằng chuột trái (LMB)
                playerControls.PlayerActions.LMB.performed += i => LMB_Input = true; // Khi nhấn LMB, đặt LMB_Input thành true.
                // Đăng ký sự kiện khi người chơi thực hiện hành động tấn công bằng chuột phải (RMB)
                playerControls.PlayerActions.RMB.performed += i => RMB_Input = true; // Khi nhấn RMB, đặt RMB_Input thành true.
                // Đăng ký sự kiện khi người chơi giữ chuột phải (HoldRMB)
                playerControls.PlayerActions.HoldRMB.performed += i => Hold_RMP_Input = true; // Khi giữ RMB, đặt Hold_RMP_Input thành true.
                playerControls.PlayerActions.HoldRMB.canceled += i => Hold_RMP_Input = false; // Khi thả RMB, đặt Hold_RMP_Input thành false.



                // Khi người chơi nhấn nút khóa mục tiêu, biến lockOn_Input sẽ được đặt thành true, kích hoạt cơ chế khóa mục tiêu
                playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
                // Khi người chơi nhấn nút để tìm mục tiêu khóa bên trái, biến lockOn_Left_Input sẽ được đặt thành true
                playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
                // Khi người chơi nhấn nút để tìm mục tiêu khóa bên phải, biến lockOn_Right_Input sẽ được đặt thành true
                playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;


                // Đăng ký sự kiện cho hành động chạy nhanh (sprint)
                // Khi người chơi nhấn giữ phím Shift, sự kiện "performed" sẽ kích hoạt và đặt sprintInput thành true.
                playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
                // Khi người chơi thả phím Shift, sự kiện "canceled" sẽ kích hoạt và đặt sprintInput thành false.
                playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;


            }
            playerControls.Enable(); // Kích hoạt các điều khiển đầu vào
        }

        private void OnDestroy()
        {
            // Hủy đăng ký sự kiện khi GameObject bị hủy
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnApplicationFocus(bool focus) // Phương thức này được gọi khi trạng thái "focus" của ứng dụng thay đổi
        {
            // Kiểm tra xem script có đang hoạt động hay không
            if (enabled)
            {
                // Nếu ứng dụng đang được chọn (focus == true)
                if (focus)
                {
                    // Bật các điều khiển input cho người chơi
                    // Điều này cho phép người chơi tương tác với game
                    playerControls.Enable();
                }
                else
                {
                    // Nếu ứng dụng không còn được chọn (focus == false)
                    // Tắt các điều khiển input cho người chơi
                    // Điều này giúp ngăn chặn tương tác không mong muốn khi người dùng không chú ý
                    playerControls.Disable();
                }
            }
        }

        private void Update()
        {
            AllInputs();
        }
        private void AllInputs()
        {
            LockOnInput(); // Gọi phương thức xử lý đầu vào lock-on
            LockOnSwitchTargetInput(); // Gọi phương thức chuyển đổi mục tiêu khóa
            PlayerMovementInput(); // Gọi phương thức xử lý đầu vào di chuyển của nguời chơi
            CameraMovementInput(); // Gọi phương thức xử lý đầu vào di chuyển của camera
            DodgeInput(); // Gọi phương thức né tránh của người chơi
            SprintInput(); // Gọi phương thức chạy nhanh của người chơi
            JumpInput();// Gọi phương thức nhảy của người chơi
            LMBInput();// Gọi phương thức tấn công (nhấp chuột trái)của người chơi
            RMBInput(); // Gọi phương thức xử lý đầu vào tấn công bằng chuột phải (RMB).
            RMBChargeRMBInput(); // Gọi phương thức xử lý đầu vào cho hành động sạc đòn tấn công bằng chuột phải (giữ RMB).
            SwitchLeftWeaponInput(); // Gọi phương thức xử lý đầu vào để đổi vũ khí tay trái.
            SwitchRightWeaponInput(); // Gọi phương thức xử lý đầu vào để đổi vũ khí tay phải.
        }

        //Lock on
        private void LockOnInput()
        {
            // Kiểm tra xem nhân vật đang ở trạng thái lock-on hay không
            if (player.playerNetworkManager.isLockedOn.Value)
            {
                // Nếu không có mục tiêu nào được chọn thì thoát
                if (player.playerCombatManager.currentTarget == null)
                    return;

                // Nếu mục tiêu hiện tại đã chết, tắt trạng thái lock-on
                if (player.playerCombatManager.currentTarget.isDead.Value)
                {
                    player.playerNetworkManager.isLockedOn.Value = false; // Tắt lock-on nếu mục tiêu chết
                }

                // Kiểm tra xem coroutine đang chạy có tồn tại không
                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine); // Dừng coroutine hiện tại nếu có

                // Khởi động một coroutine mới để tìm mục tiêu
                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }


            // Nếu đang nhấn nút lock-on nhưng đã ở trạng thái lock-on thì thoát
            if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false; // Đặt lại trạng thái đầu vào lock-on
                PlayerCamera.instance.ClearLockOnTargets(); // Xóa danh sách các mục tiêu lock-on
                player.playerNetworkManager.isLockedOn.Value = false; // Đặt trạng thái lock-on thành false
                return; // Thoát phương thức
            }


            // Nếu đang nhấn nút lock-on và chưa ở trạng thái lock-on thì tìm mục tiêu
            if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
            {
                lockOn_Input = false; // Đặt lại trạng thái đầu vào lock-on
                PlayerCamera.instance.LocatingLockOnTargets(); // Tìm kiếm mục tiêu để lock-on

                // Kiểm tra nếu tìm thấy mục tiêu lock-on gần nhất
                if (PlayerCamera.instance.nearestLockOnTarget != null)
                {
                    // Đặt mục tiêu cho người chơi là mục tiêu lock-on tìm được
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                    player.playerNetworkManager.isLockedOn.Value = true; // Đánh dấu trạng thái đã lock-on
                }
            }
        }

        // Phương thức này xử lý đầu vào để chuyển đổi mục tiêu khóa khi người chơi nhấn nút khóa mục tiêu bên trái hoặc bên phải.
        private void LockOnSwitchTargetInput()
        {
            // Kiểm tra nếu đầu vào khóa mục tiêu bên trái được kích hoạt
            if (lockOn_Left_Input)
            {
                // Đặt lại biến lockOn_Left_Input về false để không kích hoạt lại
                lockOn_Left_Input = false;

                // Kiểm tra nếu người chơi đang khóa mục tiêu
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    // Gọi phương thức để xác định vị trí các mục tiêu khóa
                    PlayerCamera.instance.LocatingLockOnTargets();

                    // Nếu có mục tiêu khóa bên trái
                    if (PlayerCamera.instance.leftLockOnTarget != null)
                    {
                        // Đặt mục tiêu cho người chơi là mục tiêu khóa bên trái
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                    }
                }
            }

            // Kiểm tra nếu đầu vào khóa mục tiêu bên phải được kích hoạt
            if (lockOn_Right_Input)
            {
                // Đặt lại biến lockOn_Right_Input về false để không kích hoạt lại
                lockOn_Right_Input = false;

                // Kiểm tra nếu người chơi đang khóa mục tiêu
                if (player.playerNetworkManager.isLockedOn.Value)
                {
                    // Gọi phương thức để xác định vị trí các mục tiêu khóa
                    PlayerCamera.instance.LocatingLockOnTargets();

                    // Nếu có mục tiêu khóa bên phải
                    if (PlayerCamera.instance.rightLockOnTarget != null)
                    {
                        // Đặt mục tiêu cho người chơi là mục tiêu khóa bên phải
                        player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                    }
                }
            }
        }

        //Movement
        private void PlayerMovementInput()
        {
            vertical_Input = movementInput.y; // Lấy giá trị di chuyển theo trục Y
            horizontal_Input = movementInput.x; // Lấy giá trị di chuyển theo trục X

            // Tính moveAmount dựa trên tổng giá trị tuyệt đối của đầu vào
            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical_Input) + Mathf.Abs(horizontal_Input));

            // Điều chỉnh moveAmount để đảm bảo giá trị tối thiểu và tối đa
            if (moveAmount <= 0.5f && moveAmount > 0)
            {
                moveAmount = 0.5f; // Đặt moveAmount tối thiểu là 0.5
            }
            else if (moveAmount > 0.5f && moveAmount <= 1f)
            {
                moveAmount = 1f; // Đặt moveAmount tối đa là 1
            }

            // Kiểm tra xem player có tồn tại không
            if (player == null)
                return;

            // Nếu có di chuyển (moveAmount khác 0), cập nhật trạng thái di chuyển là true
            if (moveAmount != 0)
            {
                player.playerNetworkManager.isMoving.Value = true;
            }
            else
            {
                // Nếu không có di chuyển (moveAmount bằng 0), cập nhật trạng thái di chuyển là false
                player.playerNetworkManager.isMoving.Value = false;
            }

            // Kiểm tra xem nhân vật có đang khóa mục tiêu hay không, hoặc có đang chạy nhanh không
            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                // Cập nhật thông số hoạt ảnh cho nhân vật
                // Nếu không đang khóa mục tiêu hoặc đang chạy nhanh, truyền vào các thông số mặc định (0, moveAmount)
                player.playerAnimatorManager.UpdateAnimationMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            }
            else
            {
                // Nếu đang khóa mục tiêu, cập nhật thông số hoạt ảnh dựa trên đầu vào ngang và dọc
                player.playerAnimatorManager.UpdateAnimationMovementParameters(horizontal_Input, vertical_Input, player.playerNetworkManager.isSprinting.Value);
            }
        }

        private void CameraMovementInput()
        {
            cameraVerticalInput = cameraInput.y; // Lấy giá trị camera theo trục Y
            cameraHorizontalInput = cameraInput.x; // Lấy giá trị camera theo trục X
        }

        //Action
        private void DodgeInput()
        {
            // Kiểm tra nếu người chơi đã nhấn phím dodge (tránh)
            if (dodge_Input)
            {
                // Sau khi xử lý đầu vào, đặt lại biến dodgeInput thành false để tránh xử lý nhiều lần.
                dodge_Input = false;

                // Gọi hàm trong playerLocomotionManager để cố gắng thực hiện hành động dodge.
                // playerLocomotionManager sẽ quản lý việc di chuyển của nhân vật và thực hiện hành động tránh khi thích hợp.
                player.playerLocomotionManager.AttemptToPerformDodge();
            }
        }

        private void SprintInput()
        {
            // Kiểm tra xem người chơi có nhấn nút chạy nhanh (sprintInput) hay không.
            if (sprintInput)
            {
                // Nếu có, gọi hàm Sprinting() từ playerLocomotionManager để thực hiện hành động chạy nhanh.
                player.playerLocomotionManager.Sprinting();
            }
            else
            {
                // Nếu không nhấn nút chạy nhanh, thiết lập giá trị isSprinting của playerNetworkManager thành false.
                // Điều này giúp cập nhật trạng thái chạy nhanh cho máy khách khác (nếu đang chơi multiplayer).
                player.playerNetworkManager.isSprinting.Value = false;
            }
        }

        private void JumpInput()
        {
            // Kiểm tra nếu jumpInput được kích hoạt
            if (jumpInput)
            {
                // Reset lại jumpInput để tránh nhảy liên tục
                jumpInput = false;

                // Gọi hàm AttemptToPerformJump từ playerLocomotionManager để thực hiện hành động nhảy
                player.playerLocomotionManager.AttemptToPerformJump();
            }
        }

        // Phương thức xử lý đầu vào từ nút chuột trái (LMB)
        private void LMBInput()
        {
            // Kiểm tra xem nút chuột trái đã được nhấn hay chưa
            if (LMB_Input)
            {
                LMB_Input = false; // Đặt LMB_Input về false để không xử lý lại hành động này trong cùng một khung hình

                // Thiết lập tay hành động của nhân vật là tay phải
                player.playerNetworkManager.SetCharacterActionHand(true);

                // Thực hiện hành động dựa trên vũ khí đang sử dụng ở tay phải
                player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.ob_LMB_Action, player.playerInventoryManager.currentRightHandWeapon);
            }
        }


        // Phương thức xử lý đầu vào từ nút chuột phải (RMB)
        private void RMBInput()
        {
            // Kiểm tra xem người chơi có nhấn chuột phải (RMB) hay không
            if (RMB_Input)
            {
                RMB_Input = false; // Đặt lại biến RMB_Input về false sau khi xử lý đầu vào.

                // Gọi hàm trong playerNetworkManager để thiết lập hành động tấn công từ tay phải (true = tay phải)
                player.playerNetworkManager.SetCharacterActionHand(true);

                // Gọi hàm trong playerCombatManager để thực hiện hành động dựa trên vũ khí trong tay phải
                // Sử dụng hành động RMB của vũ khí hiện tại và truyền vũ khí này làm tham số
                player.playerCombatManager.PerformWeaponBasedAction(
                    player.playerInventoryManager.currentRightHandWeapon.ob_RMB_Action, // Hành động tương ứng với RMB
                    player.playerInventoryManager.currentRightHandWeapon // Vũ khí hiện tại trong tay phải
                );
            }
        }


        //Phương thức này được sử dụng để kiểm tra xem người chơi có đang thực hiện một hành động (như tấn công hoặc phòng thủ) 
        //Có đang giữ chuột phải (Hold_RMB_Input) để sạc đòn tấn công không.
        private void RMBChargeRMBInput()
        {
            // Kiểm tra nếu người chơi đang thực hiện một hành động nào đó (ví dụ: tấn công hoặc di chuyển)
            if (player.isPerformingAction)
            {
                // Kiểm tra xem người chơi có đang sử dụng tay phải để tấn công không
                if (player.playerNetworkManager.isUsingRightHand.Value)
                {
                    // Gán trạng thái giữ chuột phải (Hold_RMP_Input) vào biến kiểm soát việc sạc đòn tấn công
                    player.playerNetworkManager.isChargingAttack.Value = Hold_RMP_Input;
                }
            }
        }

        // Phương thức xử lý đầu vào để đổi vũ khí tay phải
        private void SwitchRightWeaponInput()
        {
            if (switch_Right_Weapon_Input) // Nếu có yêu cầu đổi vũ khí tay phải
            {
                switch_Right_Weapon_Input = false; // Đặt lại trạng thái đầu vào sau khi xử lý
                player.playerEquipmentManager.SwitchRightWeapon(); // Gọi hàm đổi vũ khí tay phải
            }
        }

        // Phương thức xử lý đầu vào để đổi vũ khí tay trái
        private void SwitchLeftWeaponInput()
        {
            if (switch_Left_Weapon_Input) // Nếu có yêu cầu đổi vũ khí tay trái
            {
                switch_Left_Weapon_Input = false; // Đặt lại trạng thái đầu vào sau khi xử lý
                player.playerEquipmentManager.SwitchLeftWeapon(); // Gọi hàm đổi vũ khí tay trái
            }
        }
    }
}