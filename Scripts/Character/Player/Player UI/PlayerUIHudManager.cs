using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [Header("Stat Bars")]
        [SerializeField] UI_StatBar healthBar; // Tham chiếu tới thanh hiển thị máu trên giao diện người dùng
        [SerializeField] UI_StatBar staminaBar; // Tham chiếu tới thanh hiển thị stamina trên giao diện người dùng

        [Header("Quick Slots")]
        [SerializeField] Image rightWeaponQuickSlotIcon; // Biểu tượng của vũ khí trang bị ở ô nhanh tay phải
        [SerializeField] Image leftWeaponQuickSlotIcon;  // Biểu tượng của vũ khí trang bị ở ô nhanh tay trái


        // Làm mới giao diện HUD (ẩn và hiển thị lại các thanh)
        public void RefreshHUD()
        {
            // Tạm thời ẩn thanh máu và hiển thị lại nó để làm mới
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);

            // Tạm thời ẩn thanh stamina và hiển thị lại nó để làm mới
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
        }

        // Cập nhật giá trị máu mới cho thanh hiển thị
        public void SetNewHeathValue(int oldValue, int newValue) // Tham số 'oldValue' là giá trị máu cũ, 'newValue' là giá trị máu mới
        {

            healthBar.SetStat(newValue); // Làm tròn giá trị máu mới thành số nguyên và cập nhật thanh máu
        }

        // Thiết lập giá trị máu tối đa cho thanh máu
        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth); // Đặt giá trị tối đa cho thanh máu
        }

        // Cập nhật giá trị stamina mới cho thanh hiển thị
        public void SetNewStaminaValue(float oldValue, float newValue) // Tham số 'oldValue' là giá trị stamina cũ, 'newValue' là giá trị stamina mới
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue)); // Làm tròn giá trị stamina mới thành số nguyên và cập nhật thanh stamina
        }

        // Thiết lập giá trị stamina tối đa cho thanh stamina
        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);// Đặt giá trị tối đa cho thanh stamina
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            // Lấy thông tin vũ khí từ cơ sở dữ liệu dựa trên ID vũ khí
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            // Nếu không tìm thấy vũ khí (vũ khí là null)
            if (weapon == null)
            {
                Debug.Log("Vũ khí bên tay phải không tồn tại");
                rightWeaponQuickSlotIcon.enabled = false; // Tắt biểu tượng ô nhanh
                rightWeaponQuickSlotIcon.sprite = null;   // Xóa biểu tượng hiển thị
                return;
            }

            // Nếu vũ khí không có biểu tượng (icon)
            if (weapon.itemIcon == null)
            {
                Debug.Log("Vũ khí bên tay phải không có biểu tượng");
                rightWeaponQuickSlotIcon.enabled = false; // Tắt biểu tượng ô nhanh
                rightWeaponQuickSlotIcon.sprite = null;   // Xóa biểu tượng hiển thị
                return;
            }

            // Gán biểu tượng cho vũ khí và hiển thị ô nhanh
            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;
        }

        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            // Lấy thông tin vũ khí từ cơ sở dữ liệu dựa trên ID vũ khí
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(weaponID);

            // Nếu không tìm thấy vũ khí (vũ khí là null)
            if (weapon == null)
            {
                Debug.Log("Vũ khí bên tay trái không tồn tại");
                leftWeaponQuickSlotIcon.enabled = false;  // Tắt biểu tượng ô nhanh
                leftWeaponQuickSlotIcon.sprite = null;    // Xóa biểu tượng hiển thị
                return;
            }

            // Nếu vũ khí không có biểu tượng (icon)
            if (weapon.itemIcon == null)
            {
                Debug.Log("Vũ khí bên tay trái không có biểu tượng");
                leftWeaponQuickSlotIcon.enabled = false;  // Tắt biểu tượng ô nhanh
                leftWeaponQuickSlotIcon.sprite = null;    // Xóa biểu tượng hiển thị
                return;
            }

            // Gán biểu tượng cho vũ khí và hiển thị ô nhanh
            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;
        }
    }
}
