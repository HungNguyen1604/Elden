using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp InstantCharacterEffect kế thừa từ ScriptableObject
    public class InstantCharacterEffect : ScriptableObject
    {
        // Thông tin về ID của hiệu ứng
        [Header("Effect ID")]
        public int instantEffectID;// ID của hiệu ứng

        // Phương thức định nghĩa hành động cụ thể mà hiệu ứng sẽ thực hiện khi được áp dụng cho một nhân vật
        public virtual void ProcessEffect(CharacterManager character)
        {
            // Chưa có nội dung, có thể được triển khai trong các lớp con
        }
    }
}

