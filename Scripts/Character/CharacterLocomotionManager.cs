using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        private CharacterManager character; // Tham chiếu đến đối tượng quản lý nhân vật

        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -5.55f; // Gia tốc trọng lực
        [SerializeField] LayerMask groundLayer; // Layer của mặt đất để kiểm tra
        [SerializeField] float groundCheckSphereRadius = 1; // Bán kính của hình cầu kiểm tra mặt đất
        [SerializeField] protected Vector3 yVelocity; // Vận tốc theo chiều dọc (trục y)
        [SerializeField] protected float groundedYVelocity = -20; // Vận tốc khi giữ nhân vật trên mặt đất
        [SerializeField] protected float fallStartYVelocity = -5; // Tốc độ bắt đầu rơi khi không còn trên mặt đất

        protected bool fallingVelocityHasBeenSet = false; // Kiểm tra xem vận tốc rơi đã được thiết lập hay chưa
        protected float inAirTimer = 0; // Thời gian mà nhân vật ở trên không

        [Header("Flags")]
        public bool isRolling = false; // Biến kiểm tra trạng thái lăn của nhân vật.
        public bool canRotate = true;// Cờ kiểm soát xem nhân vật có thể xoay trong khi thực hiện hành động hay không. 
                                     // Nếu true, nhân vật có thể xoay theo hướng camera hoặc input, nếu false, nhân vật sẽ không bị xoay.
        public bool canMove = true;// Cờ kiểm soát xem nhân vật có thể di chuyển hay không. 
                                   // Nếu true, nhân vật có thể di chuyển tự do, nếu false, nhân vật sẽ không thể di chuyển 
                                   // trong khi thực hiện hành động (ví dụ: khi lăn hoặc tấn công). 
        public bool isGrounded = true; // Trạng thái đứng trên mặt đất của nhân vật

        protected virtual void Awake()
        {
            // Lấy tham chiếu đến CharacterManager khi khởi tạo
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            // Kiểm tra xem nhân vật có đang đứng trên mặt đất hay không
            HandleGroundCheck();

            if (isGrounded) // Nếu nhân vật đang đứng trên mặt đất
            {
                if (yVelocity.y < 0) // Nếu vận tốc theo trục y nhỏ hơn 0 (đang rơi)
                {
                    inAirTimer = 0; // Đặt lại thời gian ở trên không
                    fallingVelocityHasBeenSet = false; // Đặt lại trạng thái vận tốc rơi
                    yVelocity.y = groundedYVelocity; // Đặt vận tốc y về giá trị giữ trên mặt đất
                }
            }
            else // Nếu nhân vật không đứng trên mặt đất (đang ở trên không)
            {
                // Nếu không đang nhảy và vận tốc rơi chưa được thiết lập
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
                {
                    fallingVelocityHasBeenSet = true; // Đánh dấu rằng vận tốc rơi đã được thiết lập
                    yVelocity.y = fallStartYVelocity; // Thiết lập vận tốc bắt đầu rơi
                }

                // Tăng thời gian ở trên không
                inAirTimer += Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer); // Cập nhật animator với thời gian ở trên không

                // Cập nhật vận tốc y với gia tốc trọng lực
                yVelocity.y += gravityForce * Time.deltaTime;
            }

            // Di chuyển nhân vật theo vận tốc y đã được tính toán
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            // Kiểm tra xem nhân vật có đang đứng trên mặt đất hay không
            isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
        }

        protected void OnDrawGizmosSelected()
        {
            // Hiển thị gizmo hình cầu để kiểm tra bán kính mặt đất trong chế độ chỉnh sửa (editor)
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }

        // Phương thức cho phép nhân vật quay
        public void EnableCanRotation()
        {
            canRotate = true; // Thiết lập biến canRotate thành true, cho phép nhân vật quay
        }

        // Phương thức ngăn không cho nhân vật quay
        public void DisableCanRotation()
        {
            canRotate = false; // Thiết lập biến canRotate thành false, không cho phép nhân vật quay
        }

    }
}


