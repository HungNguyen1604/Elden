using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Đầu tiên, chúng ta định nghĩa một class tên là "Enums", đây là nơi chứa các enum mà chúng ta sẽ sử dụng trong trò chơi
public class Enums : MonoBehaviour
{
    // Các enum sẽ được đặt ở đây (nếu có thêm nhiều loại enum khác)
}

// Đây là enum để đại diện cho các slot nhân vật
public enum CharacterSlot
{
    CharacterSlot_01,  // Slot cho nhân vật 1
    CharacterSlot_02,  // Slot cho nhân vật 2
    CharacterSlot_03,  // Slot cho nhân vật 3
    CharacterSlot_04,  // Slot cho nhân vật 4
    CharacterSlot_05,  // Slot cho nhân vật 5
    CharacterSlot_06,  // Slot cho nhân vật 6
    CharacterSlot_07,  // Slot cho nhân vật 7
    CharacterSlot_08,  // Slot cho nhân vật 8
    CharacterSlot_09,  // Slot cho nhân vật 9
    CharacterSlot_10,  // Slot cho nhân vật 10
    NO_SLOT            // Trạng thái không có slot (trường hợp không chọn slot nào)
}


// Đây là enum để đại diện cho nhóm của các nhân vật
public enum CharacterGroup
{
    Team1,  // Nhóm 1 của nhân vật (ví dụ đội 1 trong trò chơi)
    Team2   // Nhóm 2 của nhân vật (ví dụ đội 2 trong trò chơi)
}

// Enum định nghĩa các vị trí cho mô hình vũ khí
public enum WeaponModelSlot
{
    RightHand, // Vị trí tay phải
    LeftHand,  // Vị trí tay trái
}


// Enum định nghĩa các loại tấn công của nhân vật
// Định nghĩa các loại tấn công có thể có trong trò chơi.
public enum AttackType
{
    LightAttack01,  // Loại tấn công nhẹ (Light Attack) đầu tiên.
    LightAttack02,  // Loại tấn công nhẹ (Light Attack) thứ hai.
    HeavyAttack01,  // Loại tấn công nặng (Heavy Attack) đầu tiên.
    HeavyAttack02,  // Loại tấn công nặng (Heavy Attack) thứ hai.
    ChargedAttack01, // Loại tấn công sạc (Charged Attack) đầu tiên.
    ChargedAttack02  // Loại tấn công sạc (Charged Attack) thứ hai.
}

