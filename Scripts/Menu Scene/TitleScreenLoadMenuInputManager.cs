using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Class quản lý các thao tác nhập liệu trong menu load của màn hình tiêu đề
    public class TitleScreenLoadMenuInputManager : MonoBehaviour
    {
        // Biến quản lý các điều khiển của người chơi
        PlayerControls playerControls;

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false; // Biến cờ đánh dấu khi người chơi yêu cầu xóa slot nhân vật

        // Phương thức Update được gọi mỗi frame
        private void Update()
        {
            // Nếu deleteCharacterSlot được đặt thành true, thực hiện xóa slot nhân vật
            if (deleteCharacterSlot)
            {
                deleteCharacterSlot = false; // Đặt lại giá trị cờ để không lặp lại việc xóa
                TitleScreenManager.instance.AttemptToDeleteCharacterSlot(); // Gọi phương thức xóa slot trong TitleScreenManager
            }
        }

        // Phương thức OnEnable được gọi khi script hoặc game object được kích hoạt
        private void OnEnable()
        {
            // Kiểm tra xem playerControls đã được khởi tạo chưa
            if (playerControls == null)
            {
                playerControls = new PlayerControls(); // Tạo mới đối tượng điều khiển người chơi

                // Gán hành động xóa slot nhân vật khi nút delete được nhấn
                playerControls.UI.DeleteCharacterSlot.performed += i => deleteCharacterSlot = true;
            }
            playerControls.Enable(); // Kích hoạt các điều khiển của người chơi
        }

        // Phương thức OnDisable được gọi khi script hoặc game object bị vô hiệu hóa
        private void OnDisable()
        {
            playerControls.Disable(); // Vô hiệu hóa các điều khiển của người chơi
        }
    }
}
