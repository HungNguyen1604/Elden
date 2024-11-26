using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp PlayerLocomotionManager kế thừa từ CharacterLocomotionManager
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        private PlayerManager player; // Tham chiếu đến PlayerManager để quản lý nhân vật

        [HideInInspector] public float verticalMovement; // Giá trị di chuyển theo trục Y (thẳng đứng)
        [HideInInspector] public float horizontalMovement; // Giá trị di chuyển theo trục X (ngang)
        [HideInInspector] public float moveAmount; // Tổng lượng di chuyển dựa trên đầu vào

        [Header("Movement Settings")]
        private Vector3 moveDirection; // Hướng di chuyển của nhân vật
        private Vector3 targetRotationDirection;//Lưu trữ hướng mà nhân vật sẽ xoay về
        [SerializeField] float walkingSpeed = 2; // Tốc độ đi bộ
        [SerializeField] float runningSpeed = 4; // Tốc độ chạy
        [SerializeField] float sprintingSpeed = 8; // Tốc độ chạy nhanh(nước rút)
        [SerializeField] float rotationSpeed = 15;//Xử lí xoay mượt mà cho nhân vật
        [SerializeField] int sprintingStaminaCost = 2; // Lượng stamina tiêu tốn mỗi giây khi chạy nhanh

        [Header("Jump")]
        [SerializeField] float jumpStaminaCost = 25;   // Lượng stamina tiêu hao cho động tác nhảy.
        [SerializeField] float jumpHeight = 4; // Chiều cao nhảy (tức là nhảy tới độ cao bao nhiêu)
        [SerializeField] float jumpForwardSpeed = 5; // Tốc độ di chuyển về phía trước khi nhảy (jumping)
        [SerializeField] float freeFallSpeed = 2; // Tốc độ di chuyển khi rơi tự do (free fall)
        private Vector3 jumpDirection; // Nhảy về hướng nào 

        [Header("Dodge")]
        private Vector3 rollDirection;  // Hướng di chuyển khi nhân vật thực hiện động tác dodge (né tránh).
        [SerializeField] float dodgeStaminaCost = 25;  // Lượng stamina tiêu hao cho động tác dodge.

        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp cha (CharacterLocomotionManager)

            player = GetComponent<PlayerManager>(); // Lấy component PlayerManager từ GameObject hiện tại
        }

        protected override void Update()
        {
            // Gọi phương thức Update() của lớp cha để đảm bảo các chức năng cơ bản được thực hiện.
            base.Update();

            // Kiểm tra xem nhân vật hiện tại có phải là chủ sở hữu (client) hay không.
            if (player.IsOwner)
            {
                // Nếu là chủ sở hữu, cập nhật giá trị cho các biến di chuyển trên mạng.
                player.characterNetworkManager.verticalMovement.Value = verticalMovement; // Cập nhật giá trị di chuyển dọc.
                player.characterNetworkManager.horizontalMovement.Value = horizontalMovement; // Cập nhật giá trị di chuyển ngang.
                player.characterNetworkManager.moveAmount.Value = moveAmount; // Cập nhật giá trị tổng di chuyển.
            }
            else
            {
                // Nếu không phải là chủ sở hữu, đồng bộ hóa các giá trị từ mạng cho nhân vật này.
                verticalMovement = player.characterNetworkManager.verticalMovement.Value; // Lấy giá trị di chuyển dọc từ mạng.
                horizontalMovement = player.characterNetworkManager.horizontalMovement.Value; // Lấy giá trị di chuyển ngang từ mạng.
                moveAmount = player.characterNetworkManager.moveAmount.Value; // Lấy giá trị tổng di chuyển từ mạng.

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
                    player.playerAnimatorManager.UpdateAnimationMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
                }
            }
        }


        // Phương thức AllMovement được gọi để xử lý tất cả các hành động di chuyển của nhân vật.
        public void AllMovement()
        {
            GroundedMovement(); // Gọi phương thức di chuyển khi nhân vật đang trên mặt đất
            Rotation();// Gọi phương thức xoay của nhân vật đang
            JumpingMovement(); // Gọi phương thức xử lý chuyển động khi nhân vật đang nhảy
            FreeFallMovement();// Gọi Phương thức xử lý chuyển động khi nhân vật đang rơi tự do
        }

        // Phương thức GetMovementValues lấy các giá trị đầu vào từ PlayerInputManager.
        private void GetMovementValues()
        {
            verticalMovement = PlayerInputManager.instance.vertical_Input; // Lấy giá trị di chuyển theo trục Y từ PlayerInputManager
            horizontalMovement = PlayerInputManager.instance.horizontal_Input; // Lấy giá trị di chuyển theo trục X từ PlayerInputManager
            moveAmount = PlayerInputManager.instance.moveAmount; // Lấy tổng lượng di chuyển từ PlayerInputManager
        }

        // Phương thức GroundedMovement xử lý di chuyển khi nhân vật đang trên mặt đất.
        private void GroundedMovement()
        {
            // Kiểm tra xem nhân vật có được phép di chuyển không
            if (!canMove)
                return;// Nếu không được phép di chuyển, kết thúc hàm

            GetMovementValues(); // Lấy các giá trị đầu vào di chuyển

            // Tính toán hướng di chuyển dựa trên hướng camera
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize(); // Chuẩn hóa vector để đảm bảo tốc độ di chuyển đồng nhất
            moveDirection.y = 0; // Đảm bảo nhân vật không bay lên hoặc xuống

            if (player.playerNetworkManager.isSprinting.Value)
            {
                // Nếu moveAmount lớn hơn 1, nhân vật chạy với tốc độ chạy nhanh(nước rút)
                player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
            }
            else
            {
                // Kiểm tra lượng di chuyển để xác định tốc độ di chuyển
                if (moveAmount > 0.5f)
                {
                    // Nếu moveAmount lớn hơn 0.5, nhân vật chạy với tốc độ chạy
                    player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
                }
                else if (moveAmount <= 0.5f && moveAmount > 0f)
                {
                    // Nếu moveAmount nhỏ hơn hoặc bằng 0.5 nhưng lớn hơn 0, nhân vật đi bộ với tốc độ đi bộ
                    player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
                }
                // Nếu moveAmount == 0, không di chuyển
            }


        }

        // Phương thức xử lý chuyển động khi nhân vật đang nhảy
        private void JumpingMovement()
        {
            // Kiểm tra xem nhân vật có đang ở trạng thái nhảy hay không
            if (player.playerNetworkManager.isJumping.Value)
            {
                // Di chuyển nhân vật theo hướng nhảy (jumpDirection) với tốc độ nhảy (jumpForwardSpeed)
                // Time.deltaTime được sử dụng để đảm bảo chuyển động diễn ra mượt mà, không phụ thuộc vào tốc độ khung hình
                player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
            }
        }


        // Phương thức xử lý chuyển động khi nhân vật đang rơi tự do
        private void FreeFallMovement()
        {
            // Kiểm tra xem nhân vật có đang ở trên mặt đất hay không
            if (!isGrounded)
            {
                Vector3 freeFallDirection; // Biến để lưu trữ hướng rơi tự do

                // Tính toán hướng rơi tự do dựa trên hướng nhìn của camera và đầu vào từ người chơi
                freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.vertical_Input;
                freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontal_Input;
                freeFallDirection.y = 0; // Đặt thành 0 để chỉ di chuyển trên mặt phẳng ngang

                // Di chuyển nhân vật theo hướng rơi tự do với tốc độ rơi (freeFallSpeed)
                // Time.deltaTime được sử dụng để đảm bảo chuyển động diễn ra mượt mà
                player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
            }
        }



        //Rotation() dùng để kiểm tra xem nhân vật có đủ điều kiện xoay không
        private void Rotation()
        {
            // Nếu nhân vật đã chết, không thực hiện các hành động tiếp theo và kết thúc hàm
            if (player.isDead.Value)
                return;

            // Kiểm tra xem nhân vật có được phép xoay không
            if (!canRotate)
                return; // Nếu không được phép xoay, kết thúc hàm

            // Kiểm tra nếu nhân vật đang khóa mục tiêu hoặc đang trong trạng thái lăn.
            if (player.playerNetworkManager.isLockedOn.Value || isRolling)
            {
                // Kiểm tra xem nhân vật có đang chạy nhanh không
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    Vector3 targetDirection = Vector3.zero; // Khởi tạo hướng đích

                    // Lấy hướng di chuyển từ camera theo trục dọc
                    targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                    // Thêm hướng di chuyển theo trục ngang
                    targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                    targetDirection.Normalize(); // Chuẩn hóa hướng

                    targetDirection.y = 0; // Đặt giá trị Y thành 0 để xoay trên mặt phẳng

                    // Nếu không có hướng di chuyển, giữ nguyên hướng hiện tại
                    if (targetDirection == Vector3.zero)
                        targetDirection = transform.forward;

                    // Tạo góc xoay theo hướng đích
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    // Tạo góc xoay mượt mà giữa góc hiện tại và góc đích
                    Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRotation; // Cập nhật góc xoay cho nhân vật
                }
                else
                {
                    // Nếu không có mục tiêu đang chiến đấu, kết thúc hàm
                    if (player.playerCombatManager.currentTarget == null)
                        return;

                    // Tính toán hướng đến mục tiêu hiện tại
                    Vector3 targetDirection;
                    targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                    targetDirection.y = 0; // Đặt giá trị Y thành 0 để xoay trên mặt phẳng
                    targetDirection.Normalize(); // Chuẩn hóa hướng

                    // Tạo góc xoay theo hướng đích
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    // Tạo góc xoay mượt mà giữa góc hiện tại và góc đích
                    Quaternion finalRoation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = finalRoation; // Cập nhật góc xoay cho nhân vật
                }
            }
            else
            {
                // Nếu không đang khóa mục tiêu, khởi tạo hướng xoay về (0,0,0)
                targetRotationDirection = Vector3.zero;
                // Lấy hướng di chuyển từ camera theo trục dọc
                targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                // Thêm hướng ngang
                targetRotationDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                targetRotationDirection.Normalize(); // Chuẩn hóa hướng xoay
                targetRotationDirection.y = 0; // Đặt giá trị Y thành 0 để xoay trên mặt phẳng

                // Nếu không có hướng xoay
                if (targetRotationDirection == Vector3.zero)
                {
                    targetRotationDirection = transform.forward; // Giữ nguyên hướng hiện tại
                }

                // Tạo góc xoay mới theo hướng đích
                Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);

                // Tạo góc xoay mượt mà giữa góc hiện tại và góc đích
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = targetRotation; // Cập nhật góc xoay cho nhân vật
            }
        }

        // Sprinting() dùng để kiểm tra điều kiện thực hiện sprinting
        public void Sprinting()
        {
            // Kiểm tra nếu nhân vật đang thực hiện một hành động khác (ví dụ: backstep, roll,...)
            if (player.isPerformingAction)
            {
                // Nếu đang thực hiện hành động, không cho phép chạy nhanh (sprint) và đặt trạng thái sprint trên mạng là false.
                player.playerNetworkManager.isSprinting.Value = false;
            }

            // Kiểm tra xem stamina hiện tại có đủ để chạy hay không
            if (player.playerNetworkManager.currentStamina.Value <= 0)
            {
                // Nếu không còn stamina, không cho phép chạy nhanh
                player.playerNetworkManager.isSprinting.Value = false; // Ngăn chạy nhanh nếu hết stamina
                return; // Kết thúc hàm nếu không đủ stamina
            }

            // Kiểm tra nếu người chơi đang di chuyển (moveAmount >= 0.5 nghĩa là người chơi đang di chuyển nhanh hơn một ngưỡng nhất định)
            if (moveAmount >= 0.5)
            {
                // Nếu di chuyển đủ nhanh, bật trạng thái sprint trên mạng (sprint is true).
                player.playerNetworkManager.isSprinting.Value = true;
            }
            // Nếu người chơi đứng im hoặc di chuyển chậm (moveAmount < 0.5), thì tắt trạng thái sprint
            else
            {
                // Đặt trạng thái sprint trên mạng là false khi không chạy hoặc di chuyển chậm.
                player.playerNetworkManager.isSprinting.Value = false;
            }
            if (player.playerNetworkManager.isSprinting.Value)
            {
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
            }
            // Nếu trạng thái chạy nhanh đang được kích hoạt
            if (player.playerNetworkManager.isSprinting.Value)
            {
                // Giảm stamina khi đang chạy nhanh, tính toán dựa trên chi phí stamina
                player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime; // Giảm stamina theo thời gian
            }
        }


        //AttemptToPerformDodge() dùng để kiểm tra điều kiện thực hiện dodge
        public void AttemptToPerformDodge()
        {
            // Kiểm tra xem nhân vật có đang thực hiện hành động khác hay không (để tránh thực hiện dodge trong lúc đang bận).
            if (player.isPerformingAction)
                return; // Nếu đang bận, thoát khỏi phương thức và không thực hiện dodge.

            // Kiểm tra xem nhân vật có đủ stamina để thực hiện dodge không.
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return; // Nếu không đủ stamina, thoát khỏi phương thức và không thực hiện dodge.

            // Kiểm tra xem người chơi có đang di chuyển không (moveAmount > 0). Nếu có, sẽ thực hiện hành động dodge lăn (roll).
            if (PlayerInputManager.instance.moveAmount > 0)
            {
                // Xác định hướng lăn (rollDirection) dựa trên hướng camera và đầu vào di chuyển của người chơi.
                // Lấy hướng di chuyển theo chiều dọc dựa vào camera và đầu vào vertical.
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.vertical_Input;

                // Thêm hướng ngang vào rollDirection dựa trên đầu vào horizontal và hướng của camera.
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontal_Input;

                // Đảm bảo hướng lăn không bị ảnh hưởng bởi trục y (lên xuống).
                rollDirection.y = 0;

                // Chuẩn hóa hướng lăn để đảm bảo độ dài là 1 (hướng chính xác mà không bị kéo dài).
                rollDirection.Normalize();

                // Xoay nhân vật để hướng về phía hướng lăn.
                Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                // Gọi phương thức từ playerAnimatorManager để phát hoạt ảnh lăn về phía trước ("Roll_Forward_01").
                // Đối số thứ hai (true) cho biết nhân vật đang thực hiện một hành động (dodge), và đối số thứ ba có thể là để điều khiển trạng thái vật lý.
                player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);

                // Đặt trạng thái lăn của nhân vật thành true, cho phép nhân vật thực hiện hành động lăn.
                player.playerLocomotionManager.isRolling = true;

            }
            else
            {
                // Trường hợp này để dành cho khi nhân vật đứng yên và thực hiện dodge (backstep).
                player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);
            }

            // Giảm stamina của nhân vật sau khi thực hiện dodge.
            player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
        }


        public void AttemptToPerformJump()
        {
            // Kiểm tra xem nhân vật có đang thực hiện hành động nào không
            if (player.isPerformingAction)
                return; // Nếu đang thực hiện hành động khác, không cho phép nhảy

            // Kiểm tra xem nhân vật có đủ stamina để nhảy không
            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return; // Nếu không đủ stamina, không cho phép nhảy

            // Kiểm tra xem nhân vật có đang nhảy không
            if (player.playerNetworkManager.isJumping.Value)
                return; // Nếu đã nhảy rồi, không cho phép nhảy lại

            // Kiểm tra xem nhân vật có đang trên mặt đất không
            if (!isGrounded)
                return; // Nếu không trên mặt đất, không cho phép nhảy

            // Phát hoạt ảnh nhảy
            player.playerAnimatorManager.PlayTargetActionAnimation("Main_Jump_01", false);

            // Đánh dấu rằng nhân vật đang nhảy
            player.playerNetworkManager.isJumping.Value = true;

            // Giảm stamina khi nhảy
            player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost; // Trừ đi stamina theo chi phí của nhảy

            // Xác định hướng nhảy dựa trên hướng camera và đầu vào di chuyển của người chơi
            jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.vertical_Input;
            jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontal_Input;

            // Đảm bảo nhân vật không nhảy theo trục y
            jumpDirection.y = 0;

            // Nếu có di chuyển, điều chỉnh hướng và tốc độ nhảy dựa trên input của người chơi
            if (jumpDirection != Vector3.zero)
            {
                // Nếu đang chạy nước rút, nhảy về phía trước với lực mạnh hơn
                if (player.playerNetworkManager.isSprinting.Value)
                {
                    jumpDirection *= 1; // Nhảy về phía trước với tốc độ bình thường khi đang chạy nước rút
                }
                // Nếu di chuyển nhiều, nhảy về phía trước với tốc độ trung bình
                else if (PlayerInputManager.instance.moveAmount > 0.5)
                {
                    jumpDirection *= 0.5f; // Giảm vận tốc nhảy xuống một nửa khi không chạy nước rút nhưng vẫn di chuyển
                }
                // Nếu di chuyển ít, nhảy với tốc độ nhỏ hơn
                else if (PlayerInputManager.instance.moveAmount <= 0.5)
                {
                    jumpDirection *= 0.25f; // Giảm vận tốc nhảy nhiều hơn khi di chuyển ít
                }
            }
        }


        // Tính toán vận tốc cần thiết để nhảy lên với chiều cao xác định.
        public void ApplyJumpingVelocity()
        {
            // Công thức: v = sqrt(2 * g * h)
            // Trong đó: 
            //   g = lực hấp dẫn (gravityForce)
            //   h = chiều cao nhảy (jumpHeight)
            yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
        }
    }
}

