using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        // Biến để lưu trữ tham chiếu đến CharacterManager
        CharacterManager character;

        protected virtual void Awake()
        {
            // Lấy component CharacterManager từ đối tượng gắn kèm
            character = GetComponent<CharacterManager>();
        }

        [Header("VFX")]
        // Biến tham chiếu đến một GameObject (Prefab) VFX hiệu ứng máu. 
        [SerializeField] GameObject bloodSplatterVFX;


        // Phương thức để xử lý hiệu ứng ngay lập tức
        public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
        {
            // Gọi phương thức ProcessEffect từ hiệu ứng
            effect.ProcessEffect(character);
        }


        // Phương thức cho phép các lớp hoặc đối tượng khác có thể gọi để phát hiệu ứng máu tại điểm va chạm
        public void PlayBloodSplatterVFX(Vector3 contactPoint)
        {
            // Kiểm tra xem biến bloodSplatterVFX có được gán trong Inspector không
            if (bloodSplatterVFX != null)
            {
                // Tạo hiệu ứng máu tại vị trí va chạm (contactPoint) nếu bloodSplatterVFX đã được gán
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            else
            {
                // Nếu biến bloodSplatterVFX chưa được gán, sử dụng hiệu ứng máu từ WorldCharacterEffectsManager
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

    }
}

