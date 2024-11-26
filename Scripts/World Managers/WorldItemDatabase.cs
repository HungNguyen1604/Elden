using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;
using System;

namespace SG
{
    // Đóng vai trò quan trọng trong việc quản lý các item trong game.
    public class WorldItemDatabase : MonoBehaviour
    {
        // Singleton instance của WorldItemDatabase để dễ dàng truy cập từ bất kỳ nơi nào trong game
        public static WorldItemDatabase instance;

        // Vũ khí tay không (unarmed) dùng khi không có vũ khí nào được trang bị
        public WeaponItem unarmedWeapon;

        [Header("Weapons")]
        // Danh sách các vũ khí có sẵn trong game
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Items")]
        // Danh sách các vật phẩm (items) trong game
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            // Kiểm tra xem instance đã tồn tại chưa
            if (instance == null)
            {
                // Nếu chưa tồn tại, gán giá trị cho instance
                instance = this;
            }
            else
            {
                // Nếu đã tồn tại, hủy đối tượng này để đảm bảo chỉ có một instance duy nhất
                Destroy(gameObject);
            }

            // Thêm tất cả các vũ khí vào danh sách items
            foreach (var weapon in weapons)
            {
                items.Add(weapon);
            }

            // Gán ID cho từng item trong danh sách
            for (int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i; // Gán itemID từ 0 đến items.Count - 1
            }
        }

        // Hàm trả về vũ khí theo ID
        public WeaponItem GetWeaponByID(int ID)
        {
            // Sử dụng LINQ để tìm kiếm vũ khí theo itemID
            return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
        }
    }
}

