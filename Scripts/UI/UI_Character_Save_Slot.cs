using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SG
{
    // Lớp này quản lý việc hiển thị và chức năng của một ô lưu trò chơi trong UI.
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileWriter; // Tham chiếu để xử lý việc lưu và tải dữ liệu.

        [Header("Game Slot")]
        public CharacterSlot characterSlot; // Chỉ định slot (1-10) mà nút này liên kết tới.

        [Header("Character Info")]
        public TextMeshProUGUI characterName; // Phần tử UI để hiển thị tên nhân vật trong slot này.
        public TextMeshProUGUI timePlayed;    // Phần tử UI để hiển thị thời gian chơi của nhân vật trong slot này.

        private void OnEnable()
        {
            // Kích hoạt việc tải dữ liệu lưu cho slot cụ thể này.
            LoadSaveSlot();
        }

        // LoadSaveSlot() để hiển thị thông tin của từng file lưu

        private void LoadSaveSlot()
        {
            // Tạo một đối tượng SaveFileDataWriter mới để quản lý việc đọc/ghi file save
            saveFileWriter = new SaveFileDataWriter();
            // Đặt đường dẫn lưu trữ dữ liệu lưu game (thường là thư mục cố định trên thiết bị)
            saveFileWriter.saveDataDirectoryPath = Application.persistentDataPath;


            // Kiểm tra nếu characterSlot là Slot 1 thì thực hiện tiếp theo
            if (characterSlot == CharacterSlot.CharacterSlot_01)
            {
                // Quyết định tên file lưu tương ứng với Slot 1 và lưu vào saveFileName
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                // Kiểm tra xem file lưu có tồn tại hay không
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    // Nếu file tồn tại, hiển thị tên nhân vật đã lưu trên UI
                    characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
                }
                else
                {
                    // Nếu file không tồn tại, ẩn nút hoặc slot này khỏi UI
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 2
            else if (characterSlot == CharacterSlot.CharacterSlot_02)
            {
                // Quyết định tên file lưu tương ứng với Slot 2 và lưu vào saveFileName
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                // Kiểm tra xem file lưu có tồn tại hay không
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    // Nếu file tồn tại, hiển thị tên nhân vật đã lưu trên UI
                    characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
                }
                else
                {
                    // Nếu file không tồn tại, ẩn nút hoặc slot này khỏi UI
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 3
            else if (characterSlot == CharacterSlot.CharacterSlot_03)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 4
            else if (characterSlot == CharacterSlot.CharacterSlot_04)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot04.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 5
            else if (characterSlot == CharacterSlot.CharacterSlot_05)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot05.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 6
            else if (characterSlot == CharacterSlot.CharacterSlot_06)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot06.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 7
            else if (characterSlot == CharacterSlot.CharacterSlot_07)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot07.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 8
            else if (characterSlot == CharacterSlot.CharacterSlot_08)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot08.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 9
            else if (characterSlot == CharacterSlot.CharacterSlot_09)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot09.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            // Kiểm tra nếu characterSlot là Slot 10
            else if (characterSlot == CharacterSlot.CharacterSlot_10)
            {
                saveFileWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
                if (saveFileWriter.CheckToSeeIfFileExists())
                {
                    characterName.text = WorldSaveGameManager.instance.characterSlot10.characterName;
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        // Hàm này dùng để load game từ một slot lưu trữ nhân vật
        public void LoadGameFromCharacterSlot()
        {
            // Gán giá trị slot nhân vật hiện tại vào biến của WorldSaveGameManager
            WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;

            // Gọi hàm LoadGame từ WorldSaveGameManager để bắt đầu quá trình tải game
            WorldSaveGameManager.instance.LoadGame();
        }


        // Phương thức để chọn slot hiện tại khi người chơi nhấn nút UI
        public void SelectCurrentSlot()
        {
            // Gọi TitleScreenManager để chọn slot nhân vật hiện tại
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot);
        }
    }
}
