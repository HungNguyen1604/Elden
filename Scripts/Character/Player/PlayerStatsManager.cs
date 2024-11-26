using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp này quản lý các chỉ số (stats) của người chơi, kế thừa từ lớp CharacterStatsManager.
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player; // Biến lưu trữ tham chiếu đến PlayerManager.

        protected override void Awake()
        {
            base.Awake(); // Gọi phương thức Awake của lớp cha để đảm bảo các thiết lập cần thiết được thực hiện.
            player = GetComponent<PlayerManager>(); // Lấy thành phần PlayerManager từ đối tượng hiện tại.
        }

        protected override void Start()
        {
            base.Start(); // Gọi phương thức Start của lớp cha để đảm bảo các thiết lập cần thiết được thực hiện.

            // Tính toán lượng máu tối đa dựa trên chỉ số sinh lực (vitality) của nhân vật.
            // Việc tính toán ở đây là cần thiết vì nó cho phép thiết lập chỉ số ngay khi tạo nhân vật mới.
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value);

            // Tính toán lượng stamina tối đa dựa trên chỉ số sức bền (endurance) của nhân vật.
            // Điều này cũng giúp đảm bảo rằng các chỉ số được thiết lập ngay khi nhân vật được tạo.
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
        }
    }
}

