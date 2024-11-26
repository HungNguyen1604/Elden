using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp AIState là một lớp cơ bản (base class) cho các trạng thái của AI, được kế thừa từ ScriptableObject.
    // Mỗi trạng thái của AI sẽ kế thừa từ lớp này và có thể thực hiện hành động cụ thể của trạng thái đó.
    public class AIState : ScriptableObject
    {
        // Phương thức ảo Tick được gọi trong mỗi frame để xử lý logic của AI.
        // Trả về trạng thái hiện tại của AI (có thể chuyển đổi sang trạng thái khác khi cần).
        // aICharacter đại diện cho nhân vật AI sẽ thực thi trạng thái này.
        public virtual AIState Tick(AICharacterManager aiCharacter)
        {
            // Trả về chính trạng thái hiện tại (có thể ghi đè phương thức này trong các lớp con để thay đổi logic).
            return this;
        }

        // Phương thức chuyển đổi trạng thái của AI
        // Nhận vào tham số là AICharacterManager và trạng thái mới cần chuyển đến
        protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
        {
            ResetStateFlags(aiCharacter); // Đặt lại các cờ trạng thái để chuẩn bị cho trạng thái mới
            return newState; // Trả về trạng thái mới sau khi reset
        }

        // Phương thức đặt lại các cờ trạng thái của AI
        // Giúp đảm bảo rằng các cờ (flags) của trạng thái cũ không ảnh hưởng đến trạng thái mới
        protected virtual void ResetStateFlags(AICharacterManager aiCharacter)
        {
            // Hiện tại chưa có logic cụ thể, nhưng sẽ dùng để reset các cờ như "isMoving" hoặc "isAttacking" trong tương lai
        }

    }
}

