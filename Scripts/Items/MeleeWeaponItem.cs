using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo một menu trong Unity để tạo đối tượng Melee Weapon
    [CreateAssetMenu(menuName = "Items/Weapons/Melee Weapon")]
    public class MeleeWeaponItem : WeaponItem
    {
        // Lớp này kế thừa từ WeaponItem, nên sẽ có tất cả các thuộc tính của WeaponItem
        // Hiện tại chưa có thêm thuộc tính hoặc phương thức gì đặc biệt cho vũ khí cận chiến
    }
}