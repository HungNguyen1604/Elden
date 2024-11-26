using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Quản lý âm thanh trong thế giới trò chơi, sử dụng thiết kế Singleton để đảm bảo chỉ có một phiên bản duy nhất của lớp này.
    public class WorldSoundFXManager : MonoBehaviour
    {
        // Singleton instance để truy cập từ bất kỳ đâu trong trò chơi
        public static WorldSoundFXManager instance;

        [Header("Damage Sounds")]
        // Mảng chứa các âm thanh sát thương vật lý để phát khi nhân vật nhận sát thương.
        public AudioClip[] physicalDamageSFX;


        [Header("Action Sounds")] // Phân loại âm thanh hành động trong Inspector
        public AudioClip rollFX; // Âm thanh khi nhân vật thực hiện hành động lăn

        // Phương thức Awake được gọi khi đối tượng được khởi tạo
        private void Awake()
        {
            // Thiết lập Singleton, đảm bảo chỉ có một phiên bản duy nhất tồn tại
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                // Nếu đã tồn tại một instance, hủy đối tượng này để ngăn tạo thêm phiên bản mới
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Có thể thêm bất kỳ khởi tạo hoặc thiết lập bổ sung nào ở đây nếu cần
        }

        // Chọn và trả về một âm thanh ngẫu nhiên từ mảng âm thanh đã cho.
        public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
        {
            int index = Random.Range(0, array.Length); // Tạo chỉ số ngẫu nhiên trong khoảng từ 0 đến độ dài mảng.
            return array[index]; // Trả về âm thanh tại chỉ số ngẫu nhiên.
        }
    }
}

