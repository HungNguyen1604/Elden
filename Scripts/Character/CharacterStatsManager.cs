using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SG
{
    // Quản lý chỉ số của nhân vật
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character; // Tham chiếu tới CharacterManager của nhân vật.

        [Header("Stamina Regeneration")]
        private float staminaRegenerationTimer = 0; // Biến dùng để theo dõi thời gian tái tạo stamina.
        private float staminaTickTimer = 0; // Biến dùng để theo dõi thời gian cho từng tick tái tạo stamina (tránh việc hồi stamina quá nhanh).
        [SerializeField] float staminaRegenerationAmount = 2; // Số lượng stamina sẽ được tái tạo mỗi tick.
        [SerializeField] float staminaRegenerationDelay = 2; // Thời gian chờ trước khi bắt đầu tái tạo stamina.

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>(); // Lấy đối tượng CharacterManager trên đối tượng hiện tại.
        }

        protected virtual void Start()
        {
            // Có thể thêm logic khởi tạo hoặc sự kiện nào đó khi bắt đầu.
        }

        // Phương thức tính toán lượng máu dựa trên chỉ số sinh lực (vitality)
        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0; // Khai báo biến health, bắt đầu với giá trị 0.
            health = vitality * 15; // Tính toán lượng máu dựa trên chỉ số sinh lực của nhân vật.
            return Mathf.RoundToInt(health); // Làm tròn và trả về giá trị sức khỏe.
        }

        // Phương thức này tính toán giá trị stamina dựa trên chỉ số "endurance" (sức bền) của nhân vật.
        public int CalculateStaminaBasedOnEnduranceLevel(int endurance)
        {
            float stamina = 0; // Khai báo biến stamina, bắt đầu với giá trị 0.
            stamina = endurance * 10;// Công thức tính toán stamina: lấy chỉ số endurance nhân với 10 để tạo ra lượng stamina.
            return Mathf.RoundToInt(stamina); // Trả về giá trị stamina sau khi làm tròn.
        }

        // Phương thức này dùng để kiểm tra nhân vật có đang đủ điều kiện để tái tạo stamina hay không.
        public virtual void RegenerateStamina()
        {
            // Kiểm tra xem nhân vật có phải là chủ sở hữu không
            if (!character.IsOwner)
                return;

            // Nếu nhân vật đang chạy nhanh, không tái tạo stamina
            if (character.characterNetworkManager.isSprinting.Value)
                return;

            // Nếu nhân vật đang thực hiện hành động, không tái tạo stamina
            if (character.isPerformingAction)
                return;

            // Cập nhật timer tái tạo stamina
            staminaRegenerationTimer += Time.deltaTime;

            // Kiểm tra nếu timer đã vượt quá thời gian chờ tái tạo
            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                // Kiểm tra nếu stamina hiện tại nhỏ hơn stamina tối đa
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    // Cập nhật timer cho từng tick tái tạo stamina
                    staminaTickTimer += Time.deltaTime;

                    // Kiểm tra nếu đã đến thời gian cho tick tái tạo
                    if (staminaTickTimer >= 0.1) // Thời gian cho mỗi tick là 0.1 giây
                    {
                        staminaTickTimer = 0; // Reset timer tick
                        // Tăng stamina hiện tại theo số lượng đã định
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        //Phương thức này dùng để reset thời gian tái tạo stamina ,để stamina ko thể tái tạo ngay lập tức (sau khi người chơi dùng nhảy , chạy nhanh,...) 
        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
        {
            // Kiểm tra xem lượng stamina hiện tại có thấp hơn lượng stamina trước đó hay không.
            // Điều này cho thấy người chơi đã tiêu tốn stamina (ví dụ: bắt đầu chạy nhanh).
            if (currentStaminaAmount < previousStaminaAmount)
            {
                // Nếu stamina hiện tại thấp hơn trước đó, đặt lại bộ đếm thời gian tái tạo stamina về 0.
                staminaRegenerationTimer = 0;
            }
        }
    }
}


