using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp PlayerEffectsManager kế thừa từ CharacterEffectsManager
    public class PlayerEffectsManager : CharacterEffectsManager
    {
        [Header("Debug Delete Later")] // Nhãn cho các biến dưới đây
        [SerializeField] InstantCharacterEffect effectToTest; // Biến để lưu hiệu ứng thử nghiệm
        [SerializeField] bool processEffect = false; // Biến kiểm soát việc xử lý hiệu ứng

        private void Update()
        {
            // Kiểm tra xem có yêu cầu xử lý hiệu ứng không
            if (processEffect)
            {
                processEffect = false; // Đặt lại biến để tránh xử lý nhiều lần

                // Tạo một thể hiện mới của hiệu ứng thử nghiệm
                InstantCharacterEffect effect = Instantiate(effectToTest);

                // Gọi phương thức để xử lý hiệu ứng
                ProcessInstantEffect(effect);
            }
        }
    }
}

