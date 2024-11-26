using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AICharacterAnimatorManager : CharacterAnimatorManager
    {
        // Khởi tạo aiCharacter từ thành phần AICharacterManager trên đối tượng
        AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp cha
            aiCharacter = GetComponent<AICharacterManager>(); // Lấy thành phần AICharacterManager từ đối tượng
        }

        // Phương thức này được gọi khi Animator cập nhật chuyển động
        private void OnAnimatorMove()
        {
            // Kiểm tra nếu đây là chủ sở hữu đối tượng
            if (aiCharacter.IsOwner)
            {
                // Kiểm tra xem AI có đang ở trên mặt đất không
                if (!aiCharacter.characterLocomotionManager.isGrounded)
                    return; // Nếu không, thoát khỏi phương thức

                // Lấy vận tốc từ Animator deltaPosition
                Vector3 velocity = aiCharacter.animator.deltaPosition;
                // Di chuyển character controller theo vận tốc
                aiCharacter.characterController.Move(velocity);
                // Cập nhật góc quay của nhân vật dựa trên deltaRotation từ Animator
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
            }
            else // Nếu không phải là chủ sở hữu
            {
                // Kiểm tra xem AI có đang ở trên mặt đất không
                if (!aiCharacter.characterLocomotionManager.isGrounded)
                    return; // Nếu không, thoát khỏi phương thức

                // Lấy vận tốc từ Animator deltaPosition
                Vector3 velocity = aiCharacter.animator.deltaPosition;
                // Di chuyển character controller theo vận tốc
                aiCharacter.characterController.Move(velocity);
                // Cập nhật vị trí của AI từ mạng với hiệu ứng làm mịn
                aiCharacter.transform.position = Vector3.SmoothDamp(transform.position,
                    aiCharacter.characterNetworkManager.networkPosition.Value, // Vị trí mạng
                    ref aiCharacter.characterNetworkManager.networkPositionVelocity, // Biến để lưu vận tốc làm mịn
                    aiCharacter.characterNetworkManager.networkPositionSmoothTime); // Thời gian làm mịn
                                                                                    // Cập nhật góc quay của nhân vật dựa trên deltaRotation từ Animator
                aiCharacter.transform.rotation *= aiCharacter.animator.deltaRotation;
            }
        }
    }
}

