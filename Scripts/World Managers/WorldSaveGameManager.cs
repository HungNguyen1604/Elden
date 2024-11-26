using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour //Lớp quản lý lưu trữ và tải lại thế giới trong trò chơi
    {
        // Singleton instance để dễ dàng truy cập từ các lớp khác
        public static WorldSaveGameManager instance;

        // Biến lưu trữ tham chiếu đến đối tượng PlayerManager, quản lý các hành động và trạng thái của nhân vật người chơi
        public PlayerManager player;

        [Header("SAVE/LOAD")] // Tiêu đề nhóm cho các biến liên quan đến chức năng lưu và tải game.
        [SerializeField] bool saveGame; // Biến này được sử dụng để xác định xem có nên thực hiện lưu game hay không.
        [SerializeField] bool loadGame; // Biến này được sử dụng để xác định xem có nên thực hiện tải game hay không.

        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1; // Chỉ số của cảnh thế giới trong Build Settings

        [Header("Save Data Writer")]
        // Biến lưu trữ đối tượng SaveFileDataWriter, quản lý việc đọc/ghi dữ liệu save game
        private SaveFileDataWriter saveFileDataWriter;

        // Hiển thị dữ liệu liên quan đến nhân vật hiện tại
        [Header("Current Character Data")]
        // Biến này lưu trữ slot nhân vật hiện tại mà người chơi đang sử dụng, dựa trên enum "CharacterSlot"
        public CharacterSlot currentCharacterSlotBeingUsed;
        // Biến này lưu dữ liệu nhân vật hiện tại (bao gồm vị trí, thời gian chơi, tên nhân vật, v.v.)
        public CharacterSaveData currentCharacterData;
        // Biến lưu trữ tên file dùng để lưu hoặc nạp dữ liệu nhân vật từ hệ thống tệp
        private string saveFileName;


        // Khai báo các slot, sẽ lưu trữ dữ liệu nhân vật như vị trí và các thông tin khác.
        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;
        public CharacterSaveData characterSlot06;
        public CharacterSaveData characterSlot07;
        public CharacterSaveData characterSlot08;
        public CharacterSaveData characterSlot09;
        public CharacterSaveData characterSlot10;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; // Gán instance nếu chưa có
            }
            else
            {
                Destroy(gameObject); // Hủy đối tượng nếu đã có instance khác
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); // Giữ đối tượng không bị hủy khi tải cảnh mới
            LoadAllCharacterProfiles(); // Tải tất cả các hồ sơ nhân vật từ các tệp đã lưu
        }

        private void Update()
        {
            if (saveGame) // Kiểm tra xem biến saveGame có giá trị true hay không.
            {
                saveGame = false; // Reset biến saveGame về false để tránh việc lưu game nhiều lần trong một khung hình.
                SaveGame(); // Gọi phương thức SaveGame để thực hiện lưu trạng thái hiện tại của nhân vật vào file.
            }

            if (loadGame) // Kiểm tra xem biến loadGame có giá trị true hay không.
            {
                loadGame = false; // Reset biến loadGame về false để tránh việc tải game nhiều lần trong một khung hình.
                LoadGame(); // Gọi phương thức LoadGame để thực hiện tải dữ liệu nhân vật từ file lưu vào trò chơi.
            }
        }


        // Phương thức quyết định tên tệp lưu dựa trên slot nhân vật hiện tại được sử dụng
        public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
        {
            string fileName = string.Empty; // Khởi tạo biến để lưu tên tệp
            // Kiểm tra slot nhân vật hiện tại và gán tên tệp tương ứng
            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    fileName = "characterSlot_01"; // Gán tên tệp cho slot 1
                    break;
                case CharacterSlot.CharacterSlot_02:
                    fileName = "characterSlot_02"; // Gán tên tệp cho slot 2
                    break;
                case CharacterSlot.CharacterSlot_03:
                    fileName = "characterSlot_03"; // Gán tên tệp cho slot 3
                    break;
                case CharacterSlot.CharacterSlot_04:
                    fileName = "characterSlot_04"; // Gán tên tệp cho slot 4
                    break;
                case CharacterSlot.CharacterSlot_05:
                    fileName = "characterSlot_05"; // Gán tên tệp cho slot 5
                    break;
                case CharacterSlot.CharacterSlot_06:
                    fileName = "characterSlot_06"; // Gán tên tệp cho slot 6
                    break;
                case CharacterSlot.CharacterSlot_07:
                    fileName = "characterSlot_07"; // Gán tên tệp cho slot 7
                    break;
                case CharacterSlot.CharacterSlot_08:
                    fileName = "characterSlot_08"; // Gán tên tệp cho slot 8
                    break;
                case CharacterSlot.CharacterSlot_09:
                    fileName = "characterSlot_09"; // Gán tên tệp cho slot 9
                    break;
                case CharacterSlot.CharacterSlot_10:
                    fileName = "characterSlot_10"; // Gán tên tệp cho slot 10
                    break;
                default:
                    Debug.LogError("Không có slot hợp lệ!"); // Thông báo lỗi nếu không có slot hợp lệ
                    break;
            }
            return fileName; // Trả về tên tệp đã gán
        }


        // Phương thức tạo một trò chơi mới
        public void AttemptToCreateNewGame()
        {
            // Tạo một đối tượng để ghi dữ liệu file lưu trữ
            saveFileDataWriter = new SaveFileDataWriter();
            // Thiết lập đường dẫn thư mục lưu trữ cho file dựa trên thư mục persistent của ứng dụng
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // Kiểm tra slot 1
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
            // Kiểm tra xem file lưu trữ có tồn tại không
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                // Nếu không có file nào tại vị trí slot, khởi tạo một slot mới
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
                // Tạo dữ liệu lưu trữ mới cho nhân vật
                currentCharacterData = new CharacterSaveData();
                // Bắt đầu quá trình tải cảnh thế giới mới
                NewGame();
                return; // Kết thúc phương thức nếu đã khởi tạo thành công
            }

            // Kiểm tra slot 2
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
            // Kiểm tra xem file lưu trữ có tồn tại không
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02; // Thiết lập slot hiện tại là slot 2
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 3
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03; // Thiết lập slot hiện tại là slot 3
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 4
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04; // Thiết lập slot hiện tại là slot 4
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 5
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05; // Thiết lập slot hiện tại là slot 5
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 6
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06; // Thiết lập slot hiện tại là slot 6
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 7
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07; // Thiết lập slot hiện tại là slot 7
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 8
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08; // Thiết lập slot hiện tại là slot 8
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 9
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09; // Thiết lập slot hiện tại là slot 9
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Kiểm tra slot 10
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10; // Thiết lập slot hiện tại là slot 10
                currentCharacterData = new CharacterSaveData(); // Tạo dữ liệu lưu trữ mới cho nhân vật
                NewGame(); // Bắt đầu trò chơi mới
                return; // Kết thúc phương thức
            }

            // Nếu không còn slot trống, hiển thị thông báo không có slot trống cho người chơi
            TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
        }

        // Phương thức khởi tạo một trò chơi mới
        private void NewGame()
        {
            // Đặt giá trị ban đầu cho nhân vật khi bắt đầu trò chơi mới
            player.playerNetworkManager.vitality.Value = 15; // Sức sống ban đầu là 15
            player.playerNetworkManager.endurance.Value = 10; // Sức bền ban đầu là 10

            SaveGame(); // Lưu dữ liệu trò chơi hiện tại
            StartCoroutine(LoadWorldScene()); // Bắt đầu tải cảnh thế giới mới
        }

        // Phương thức tải một trò chơi đã lưu
        public void LoadGame()
        {
            // Quyết định tên tệp lưu dựa trên slot nhân vật đang được sử dụng
            saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);
            // Tạo đối tượng SaveFileDataWriter để quản lý việc đọc/ghi file save
            saveFileDataWriter = new SaveFileDataWriter();
            // Gán đường dẫn dữ liệu chung có thể sử dụng trên nhiều loại máy (Application.persistentDataPath)
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            // Gán tên tệp lưu cho SaveFileDataWriter
            saveFileDataWriter.saveFileName = saveFileName;
            // Tải dữ liệu nhân vật từ tệp lưu vào currentCharacterData
            currentCharacterData = saveFileDataWriter.LoadSaveFile();
            // Bắt đầu tải cảnh thế giới (load World Scene)
            StartCoroutine(LoadWorldScene());
        }

        // Phương thức lưu trò chơi hiện tại
        public void SaveGame()
        {
            // Quyết định tên tệp lưu dựa trên slot nhân vật đang được sử dụng
            saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);
            // Tạo đối tượng SaveFileDataWriter để quản lý việc ghi file save
            saveFileDataWriter = new SaveFileDataWriter();
            // Gán đường dẫn dữ liệu chung có thể sử dụng trên nhiều loại máy (Application.persistentDataPath)
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            // Gán tên tệp lưu cho SaveFileDataWriter
            saveFileDataWriter.saveFileName = saveFileName;
            // Lưu dữ liệu của nhân vật vào currentCharacterData thông qua phương thức SaveGameDataToCurrentCharacterData
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
            // Tạo một tệp lưu mới với dữ liệu của nhân vật hiện tại
            saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
        }

        // Phương thức để xóa dữ liệu trò chơi dựa trên slot nhân vật đã chọn
        public void DeleteGame(CharacterSlot characterSlot)
        {
            // Tạo đối tượng SaveFileDataWriter để thao tác với file lưu
            saveFileDataWriter = new SaveFileDataWriter();
            // Đặt đường dẫn tới thư mục lưu trữ dữ liệu của trò chơi
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            // Chọn file lưu tương ứng với slot nhân vật được chọn để xóa
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            // Xóa file lưu của nhân vật
            saveFileDataWriter.DeleteSaveFile();
        }

        // Phương thức để tải tất cả các hồ sơ nhân vật
        private void LoadAllCharacterProfiles()
        {
            // Tạo một đối tượng SaveFileDataWriter mới để quản lý việc ghi và đọc file save
            saveFileDataWriter = new SaveFileDataWriter();
            // Gán đường dẫn dữ liệu chung có thể sử dụng trên nhiều loại máy (Application.persistentDataPath)
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

            // Tải dữ liệu cho các slot từ 1 đến 10
            // Quyết định tên file lưu cho slot 1 bằng cách sử dụng phương thức DecideCharacterFileNameBasedOnCharacterSlotBeingUsed
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
            // Tải dữ liệu từ file lưu và gán cho characterSlot01
            characterSlot01 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
            characterSlot02 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
            characterSlot03 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
            characterSlot04 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
            characterSlot05 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
            characterSlot06 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
            characterSlot07 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
            characterSlot08 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
            characterSlot09 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
            characterSlot10 = saveFileDataWriter.LoadSaveFile();
        }

        // Coroutine để tải cảnh thế giới một cách bất đồng bộ
        public IEnumerator LoadWorldScene()
        {
            // Nếu bạn muốn một thế giới (world) scene thì dùng cái này
            // Bắt đầu quá trình tải cảnh không đồng bộ
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

            // Nếu muốn dùng nhiều scene khác nhau thì dùng cái này
            // AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);

            // Tải dữ liệu của người chơi từ dữ liệu nhân vật hiện tại
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

            // Chờ một frame để tiếp tục tiến trình tải cảnh
            yield return null;
        }

        // Phương thức này trả về chỉ số của Scene
        public int GetWorldSceneIndex()
        {
            return worldSceneIndex; // Trả về giá trị của biến worldSceneIndex
        }
    }
}

