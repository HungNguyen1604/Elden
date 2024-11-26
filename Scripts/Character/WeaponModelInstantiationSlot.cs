using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp WeaponModelInstantiationSlot quản lý vị trí của vũ khí trên nhân vật
    public class WeaponModelInstantiationSlot : MonoBehaviour
    {
        // Loại vị trí của vũ khí (tay trái, tay phải, v.v.)
        public WeaponModelSlot weaponSlot;
        // Mô hình hiện tại của vũ khí được gán cho vị trí này
        public GameObject currentWeaponModel;

        // Phương thức để xóa mô hình vũ khí hiện tại khỏi vị trí
        public void UnloadWeapon()
        {
            // Nếu có mô hình vũ khí hiện tại
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);// Xóa nó
            }
        }

        // Phương thức để tải mô hình vũ khí mới vào vị trí này
        public void LoadWeapon(GameObject weaponModel)
        {
            // Gán mô hình vũ khí hiện tại
            currentWeaponModel = weaponModel;
            // Đặt mô hình vũ khí làm con của slot này
            weaponModel.transform.parent = transform;

            // Đặt vị trí, xoay và tỷ lệ của mô hình vũ khí
            weaponModel.transform.localPosition = Vector3.zero; // Vị trí gốc
            weaponModel.transform.localRotation = Quaternion.identity; // Không có xoay
            weaponModel.transform.localScale = Vector3.one; // Kích thước gốc
        }
    }
}

