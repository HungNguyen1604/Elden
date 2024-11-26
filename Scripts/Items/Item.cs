using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp 'Item' kế thừa từ 'ScriptableObject' để quản lý thông tin về các vật phẩm
    public class Item : ScriptableObject
    {
        // Tiêu đề cho phần thông tin vật phẩm trong Inspector
        [Header("Item Infomation")]
        // Tên của vật phẩm
        public string itemName;
        // Icon của vật phẩm
        public Sprite itemIcon;

        // Mô tả vật phẩm, có thể bao gồm nhiều dòng văn bản
        [TextArea]
        public string itemDescription;
        // ID của vật phẩm, dùng để phân biệt các vật phẩm khác nhau
        public int itemID;
    }
}