using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp PlayerInventoryManager kế thừa từ CharacterInventoryManager
    // Dùng để quản lý kho đồ cụ thể cho nhân vật người chơi
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        // Vũ khí hiện tại ở tay phải của nhân vật
        public WeaponItem currentRightHandWeapon;

        // Vũ khí hiện tại ở tay trái của nhân vật
        public WeaponItem currentLeftHandWeapon;


        [Header("Quick Slots")]
        // Mảng chứa các vũ khí có thể trang bị nhanh ở tay phải, tối đa 3 vũ khí
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        // Chỉ số của vũ khí hiện tại đang được trang bị ở tay phải (vị trí trong mảng)
        public int rightHandWeaponIndex = 0;
        // Mảng chứa các vũ khí có thể trang bị nhanh ở tay trái, tối đa 3 vũ khí
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
        // Chỉ số của vũ khí hiện tại đang được trang bị ở tay trái (vị trí trong mảng)
        public int leftHandWeaponIndex = 0;
    }
}

