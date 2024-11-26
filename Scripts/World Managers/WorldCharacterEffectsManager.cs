using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {

        // Biến tĩnh để lưu instance duy nhất của WorldCharacterEffectsManager
        public static WorldCharacterEffectsManager instance;

        [Header("VFX")]
        // Biến công khai tham chiếu đến một GameObject chứa hiệu ứng máu (VFX).
        public GameObject bloodSplatterVFX;

        [Header("Damage")]
        // Biến để tham chiếu tới hiệu ứng nhận sát thương của nhân vật
        public TakeDamageEffect takeDamageEffect;


        // Danh sách chứa các hiệu ứng ngay lập tức
        [SerializeField] List<InstantCharacterEffect> instantEffects;


        private void Awake()
        {
            // Kiểm tra nếu instance chưa được khởi tạo
            if (instance == null)
            {
                // Khởi tạo instance
                instance = this;
            }
            else
            {
                // Nếu instance đã tồn tại, hủy đối tượng này
                Destroy(gameObject);
            }
            // Gọi hàm để tạo ID cho các hiệu ứng
            GenerateEffectIDs();
        }

        // Hàm để tạo ID cho từng hiệu ứng trong danh sách
        private void GenerateEffectIDs()
        {
            // Lặp qua từng hiệu ứng trong danh sách
            for (int i = 0; i < instantEffects.Count; i++)
            {
                // Gán ID cho hiệu ứng, tương ứng với chỉ số trong danh sách
                instantEffects[i].instantEffectID = i;
            }
        }
    }
}
