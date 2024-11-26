using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp CharacterSaveData là một lớp serializable, chứa các thông tin cần thiết để lưu trữ trạng thái của nhân vật.
    // Khi người chơi save game, các thông tin như tên nhân vật, thời gian chơi và vị trí hiện tại trong thế giới sẽ được lưu lại dưới dạng đối tượng của lớp này.
    [System.Serializable]
    public class CharacterSaveData
    {
        // 'sceneIndex' lưu chỉ số của cảnh (scene) hiện tại mà nhân vật đang ở.
        // Khi game có nhiều cảnh khác nhau (ví dụ: các màn chơi hoặc khu vực khác nhau), chỉ số này giúp xác định người chơi đang ở cảnh nào.
        // Điều này quan trọng khi người chơi load lại game, cần biết phải load cảnh nào.
        [Header("Scene Index")]
        public int sceneIndex = 1;  // Chỉ số mặc định của cảnh là 1 (ví dụ cảnh đầu tiên).

        // 'characterName' lưu trữ tên của nhân vật người chơi. Đây có thể là tên do người chơi tự chọn hoặc một tên mặc định.
        // Tên này có thể được hiển thị trong các bảng thông tin, danh sách save game, hoặc giao diện HUD của trò chơi.
        [Header("Character Name")]
        public string characterName = "Character";  // Tên mặc định của nhân vật là "Character".

        // 'secondsPlayed' là biến lưu trữ tổng thời gian chơi của người chơi (tính bằng giây).
        // Dữ liệu này thường được dùng để hiển thị thời gian đã chơi trong giao diện game, ví dụ: trong menu hoặc bảng thông tin save game.
        [Header("Time Played")]
        public float secondsPlayed;  // Thời gian đã chơi (tính bằng giây).

        // Phần lưu trữ vị trí hiện tại của nhân vật trong thế giới game.
        // Khi lưu game, vị trí X, Y, Z của nhân vật sẽ được lưu lại để khi người chơi load lại, nhân vật sẽ xuất hiện tại đúng vị trí đã lưu.
        [Header("World Coordinates")]
        public float xPosition;  // Tọa độ X của nhân vật trong thế giới.
        public float yPosition;  // Tọa độ Y của nhân vật trong thế giới.
        public float zPosition;  // Tọa độ Z của nhân vật trong thế giới.

        // Dữ liệu về tài nguyên của nhân vật (sức khỏe, stamina hiện tại)
        [Header("Resources")]
        public int currentHealth;  // Lượng máu hiện tại của nhân vật.
        public float currentStamina;  // Lượng stamina hiện tại của nhân vật.

        // Chỉ số nhân vật (vitality và endurance)
        [Header("Stats")]
        public int vitality;  // Chỉ số sinh lực của nhân vật.
        public int endurance;  // Chỉ số sức bền của nhân vật.

    }
}
