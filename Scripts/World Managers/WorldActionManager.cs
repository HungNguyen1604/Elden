using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SG
{
    // Quản lý các hành động dựa trên vật phẩm (vũ khí) trong game
    public class WorldActionManager : MonoBehaviour
    {
        // Biến instance tĩnh để đảm bảo quản lý các hành động dựa trên vũ khí là duy nhất (singleton)
        public static WorldActionManager instance;

        [Header("Weapon Item Actions")]
        // Mảng chứa các hành động dựa trên vũ khí
        public WeaponItemAction[] weaponItemActions;

        private void Awake()
        {
            // Kiểm tra xem instance đã được gán hay chưa. Nếu chưa, gán đối tượng hiện tại
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                // Nếu đã có instance khác, phá hủy đối tượng hiện tại để đảm bảo chỉ có một instance tồn tại
                Destroy(gameObject);
            }
            // Đảm bảo đối tượng này không bị phá hủy khi tải lại scene mới
            DontDestroyOnLoad(gameObject);
        }

        // Phương thức Start được gọi ngay sau khi Awake và trước khi cập nhật khung hình đầu tiên
        private void Start()
        {
            // Gán ID cho từng hành động trong mảng weaponItemActions dựa trên vị trí của chúng trong mảng
            for (int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionID = i;
            }
        }

        // Phương thức lấy hành động dựa trên vũ khí theo ID
        // ID: Mã định danh của hành động cần tìm
        public WeaponItemAction GetWeaponItemActionByID(int ID)
        {
            // Tìm kiếm hành động trong mảng weaponItemActions với ID tương ứng
            return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
        }
    }
}

