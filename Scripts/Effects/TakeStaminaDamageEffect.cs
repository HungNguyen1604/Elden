using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo menu cho ScriptableObject trong Unity
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage; // Biến để lưu lượng stamina bị mất

        // Ghi đè phương thức ProcessEffect từ lớp cha
        public override void ProcessEffect(CharacterManager character)
        {
            // Gọi hàm tính toán sát thương stamina
            CalculateStaminaDamage(character);
        }

        // Phương thức để tính toán sát thương stamina
        private void CalculateStaminaDamage(CharacterManager character)
        {
            // Kiểm tra xem nhân vật có phải là chủ sở hữu không
            if (character.IsOwner)
            {
                // Ghi log thông báo về lượng stamina bị mất
                Debug.Log("Ôi không! Nhân vật đã mất " + staminaDamage + " điểm stamina. Cần hồi phục ngay!");

                // Trừ lượng stamina bị mất từ stamina hiện tại của nhân vật
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}

