using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace SG
{
    public class TitleScreenManager : MonoBehaviour //Lớp quản lý màn hình tiêu đề
    {
        // instance tĩnh để truy cập vào TitleScreenManager từ các lớp khác
        public static TitleScreenManager instance;

        [Header("Menus")]
        [SerializeField] GameObject mainMenu; // Nút Main Menu: Đối tượng GameObject quản lý giao diện chính của game (main menu)
        [SerializeField] GameObject titleScreenLoadMenu; // Nút Load Menu: Đối tượng GameObject quản lý giao diện màn hình load game (load menu)

        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton; // Nút quay lại menu load
        [SerializeField] Button mainMenuLoadGameButton; // Nút vào menu chính khi load game
        [SerializeField] Button mainMenuNewGameButton; // Nút để bắt đầu trò chơi mới
        [SerializeField] Button deleteCharacterPopUpConfirmButton; // Nút để xác nhận xóa nhân vật trong pop-up xóa

        [Header("Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp; // Pop-up thông báo không còn slot trống
        [SerializeField] Button okayButton; // Nút xác nhận trong pop-up
        [SerializeField] GameObject deleteCharacterSlotPopUp; // Pop-up để xác nhận xóa slot nhân vật

        [Header("Character Slots")]
        // Biến lưu trữ slot nhân vật hiện tại được chọn, mặc định là NO_SLOT (không có slot nào được chọn)
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

        private void Awake()
        {
            // Thiết lập instance cho phép truy cập dễ dàng đến TitleScreenManager
            if (instance == null)
            {
                instance = this; // Gán instance nếu chưa có
            }
            else
            {
                Destroy(gameObject); // Nếu đã có instance, hủy bỏ đối tượng này để đảm bảo chỉ có một instance duy nhất
            }
        }

        // Phương thức để khởi động mạng như là Host
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost(); // Bắt đầu máy chủ mạng đơn (singleton)
        }


        // Phương thức để bắt đầu một trò chơi mới
        public void StartNewGame()
        {
            // Gọi phương thức từ WorldSaveGameManager để tạo một trò chơi mới
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }


        // Phương thức mở menu load game
        public void OpenLoadGameMenu()
        {
            mainMenu.SetActive(false);  // Ẩn menu chính
            titleScreenLoadMenu.SetActive(true);    // Hiển thị menu load game
            loadMenuReturnButton.Select(); // Chọn nút Return trong menu load
        }

        // Phương thức đóng menu load game
        public void CloseLoadGameMenu()
        {
            mainMenu.SetActive(true);  // Hiển thị lại menu chính
            titleScreenLoadMenu.SetActive(false);  // Ẩn menu load game
            mainMenuLoadGameButton.Select(); // Chọn nút Load trong menu chính
        }

        // Hiển thị pop-up thông báo không còn slot trống cho người chơi
        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(true); // Kích hoạt pop-up thông báo
            okayButton.Select(); // Chọn nút xác nhận trong pop-up để người chơi có thể nhấn
        }

        // Đóng pop-up thông báo không còn slot trống và quay lại nút mới game
        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(false); // Hủy kích hoạt pop-up
            mainMenuNewGameButton.Select(); // Chọn nút bắt đầu trò chơi mới để người chơi có thể tiếp tục
        }


        //Character Slots
        // Phương thức để chọn slot nhân vật
        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot; // Gán slot được chọn cho biến currentSelectedSlot
        }

        // Phương thức để bỏ chọn tất cả các slot (chọn trạng thái không slot)
        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT; // Gán trạng thái NO_SLOT (không có slot nào được chọn)
        }

        // Phương thức để cố gắng xóa slot nhân vật
        public void AttemptToDeleteCharacterSlot()
        {
            // Nếu có slot được chọn (không phải NO_SLOT)
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                // Hiển thị pop-up xác nhận xóa slot
                deleteCharacterSlotPopUp.SetActive(true);
                // Tự động chọn nút xác nhận trong pop-up để người dùng có thể nhấn
                deleteCharacterPopUpConfirmButton.Select();
            }
        }

        // Phương thức để xóa slot nhân vật đã chọn
        public void DeleteCharacterSlot()
        {
            // Ẩn pop-up xác nhận xóa slot
            deleteCharacterSlotPopUp.SetActive(false);
            // Gọi WorldSaveGameManager để xóa game dựa trên slot được chọn
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
            // Làm mới giao diện menu load bằng cách tắt và bật lại
            titleScreenLoadMenu.SetActive(false); // Tắt menu load
            titleScreenLoadMenu.SetActive(true);  // Bật lại menu load
            // Chọn nút quay lại trong menu load sau khi xóa
            loadMenuReturnButton.Select();
        }

        // Phương thức để đóng pop-up xác nhận xóa mà không thực hiện xóa
        public void CloseDeleteCharacterPopUp()
        {
            // Ẩn pop-up xác nhận xóa slot
            deleteCharacterSlotPopUp.SetActive(false);
            // Chọn nút quay lại trong menu load
            loadMenuReturnButton.Select();
        }

    }
}

