using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SG
{
    // Lớp SaveFileDataWriter chịu trách nhiệm tạo, lưu và tải dữ liệu save game của nhân vật. Nó sử dụng JSON để lưu trữ thông tin và làm việc với hệ thống file của Unity.
    public class SaveFileDataWriter
    {
        // 'saveDataDirectoryPath' là đường dẫn đến thư mục lưu trữ dữ liệu save game.
        // 'saveFileName' là tên file lưu trữ mà game sẽ tạo ra hoặc đọc từ đó.
        public string saveDataDirectoryPath = string.Empty;  // Đường dẫn đến thư mục lưu file.
        public string saveFileName = string.Empty;  // Tên của file save.

        // Kiểm tra xem file save game có tồn tại hay không. Điều này giúp quyết định có ghi đè hay tạo mới file.
        public bool CheckToSeeIfFileExists()
        {
            // Sử dụng 'File.Exists' để kiểm tra xem file đã tồn tại tại đường dẫn đã chỉ định hay chưa.
            if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
            {
                return true;  // File tồn tại.
            }
            else
            {
                return false;  // File không tồn tại.
            }
        }

        // Xóa file save đã tồn tại. Được dùng khi người chơi chọn xóa file save hoặc tạo một file mới hoàn toàn.
        public void DeleteSaveFile()
        {
            // Sử dụng 'File.Delete' để xóa file tại đường dẫn đã chỉ định.
            File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
        }

        // Tạo một file save game mới khi người chơi bắt đầu trò chơi mới.
        // 'characterData' là đối tượng chứa thông tin của nhân vật (vị trí, thời gian chơi, tên, v.v.) cần được lưu.
        public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
        {
            // Tạo đường dẫn đầy đủ cho file save bằng cách kết hợp đường dẫn thư mục và tên file.
            string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

            try
            {
                // Tạo thư mục nếu nó chưa tồn tại để lưu trữ file save.
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                // Hiển thị thông báo tạo file save thành công với đường dẫn cụ thể.
                Debug.Log("Tạo file save tại đường dẫn : " + savePath);

                // Chuyển đổi đối tượng characterData thành chuỗi JSON để dễ dàng lưu trữ.
                string dataToStore = JsonUtility.ToJson(characterData, true);

                // Mở file và ghi dữ liệu vào hệ thống file.
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        // Ghi dữ liệu JSON vào file save.
                        fileWriter.Write(dataToStore);
                    }
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi nếu không thể lưu file.
                Debug.LogError("Lỗi khi cố gắng lưu dữ liệu nhân vật, KHÔNG LƯU GAME tại: " + savePath + "\n" + ex);
            }
        }

        // Tải dữ liệu save từ một file đã lưu trước đó. Dữ liệu này sẽ được dùng để khôi phục trạng thái nhân vật.
        public CharacterSaveData LoadSaveFile()
        {
            // Khởi tạo biến characterData với giá trị null, vì ban đầu chưa có dữ liệu nào được tải.
            CharacterSaveData characterData = null;

            // Tạo đường dẫn đầy đủ để mở file save.
            string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

            // Kiểm tra xem file save có tồn tại tại đường dẫn đã chỉ định hay không.
            if (File.Exists(loadPath))
            {
                try
                {
                    // Chuỗi để chứa dữ liệu đọc từ file.
                    string dataToLoad = string.Empty;

                    // Mở file để đọc dữ liệu từ đó.
                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            // Đọc toàn bộ nội dung của file và lưu vào biến 'dataToLoad'.
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // Chuyển đổi chuỗi JSON đã đọc thành đối tượng 'CharacterSaveData' mà Unity có thể sử dụng.
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                }
                catch (Exception ex)
                {
                    // Nếu file trống hoặc có lỗi khi đọc, hiển thị thông báo lỗi.
                    Debug.Log("File trống !!!" + ex);
                }
            }
            return characterData;  // Trả về đối tượng characterData đã được tải.
        }
    }
}
