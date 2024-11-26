using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SG
{
    // Lớp AICharacterSpawner dùng để sinh ra các nhân vật AI trong trò chơi
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;          // Đối tượng nhân vật AI để sinh ra
        [SerializeField] GameObject instantiatedGameObject;       // Đối tượng nhân vật đã được sinh ra

        // Phương thức Awake khởi tạo các đối tượng và biến cần thiết khi khởi động
        private void Awake()
        {
        }

        // Phương thức Start sẽ kích hoạt việc sinh ra nhân vật AI và ẩn đối tượng hiện tại
        private void Start()
        {
            WorldAIManger.instance.SpawnCharacters(this);        // Gọi WorldAIManger để sinh các nhân vật AI
            gameObject.SetActive(false);                          // Ẩn đối tượng sinh AI
        }

        // Phương thức AttemptToSpawnCharacter thực hiện việc sinh ra nhân vật AI nếu đối tượng characterGameObject tồn tại
        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)                      // Kiểm tra nếu có đối tượng nhân vật để sinh
            {
                instantiatedGameObject = Instantiate(characterGameObject);  // Tạo mới nhân vật AI
                instantiatedGameObject.transform.position = transform.position;  // Đặt vị trí nhân vật
                instantiatedGameObject.transform.rotation = transform.rotation;  // Đặt hướng nhân vật
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();    // Kích hoạt nhân vật trong mạng
            }
        }
    }
}


