using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace SG
{
    // Lớp này quản lý các nhân vật AI trong thế giới, bao gồm việc sinh và hủy chúng.
    public class WorldAIManger : MonoBehaviour
    {
        // Tạo instance duy nhất của WorldAIManger
        public static WorldAIManger instance;

        [Header("Characters")]
        // Danh sách các đối tượng sinh nhân vật AI trong trò chơi
        [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;

        // Danh sách lưu các nhân vật AI đã được sinh ra trong thế giới
        [SerializeField] List<GameObject> spawnInCharacters;

        // Phương thức Awake được gọi khi script được tải
        // Đảm bảo chỉ có một instance của WorldAIManger tồn tại trong cảnh
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject); // Hủy các instance thừa nếu đã tồn tại
            }
        }



        // Sinh tất cả các nhân vật AI được định nghĩa trong mảng aiCharacters
        public void SpawnCharacters(AICharacterSpawner aiCharacterSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                aiCharacterSpawners.Add(aiCharacterSpawner); // Thêm đối tượng AICharacterSpawner vào danh sách nếu là server
                aiCharacterSpawner.AttemptToSpawnCharacter(); // Thực hiện việc sinh nhân vật AI tại vị trí của AICharacterSpawner
            }
        }

        // Hủy tất cả các nhân vật AI hiện đang hoạt động
        private void DespawnAllCharacters()
        {
            foreach (var character in spawnInCharacters)
            {
                // Hủy nhân vật AI này trên mạng
                character.GetComponent<NetworkObject>().Despawn();
            }
        }

        // Hàm chưa được thực hiện, dùng để tắt tất cả nhân vật (nếu cần)
        private void DisableAllCharacters()
        {

        }
    }

}
