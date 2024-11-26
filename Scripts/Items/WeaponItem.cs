using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp 'WeaponItem' kế thừa từ lớp 'Item', dùng để lưu trữ thông tin về vũ khí
    public class WeaponItem : Item
    {
        [Header("Weapon Model")]
        // Mô hình 3D của vũ khí
        public GameObject weaponModel;

        [Header("Weapon Requirements")]
        // Yêu cầu về các chỉ số sức mạnh (strength), khéo léo (dexterity), trí tuệ (intelligence), đức tin (faith) để sử dụng vũ khí
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;

        [Header("Weapon Base Damage")]
        // Sát thương cơ bản của vũ khí (vật lý, phép thuật, lửa, thần thánh, sét)
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int holyDamage = 0;
        public int lightningDamage = 0;

        [Header("Weapon Poise")]
        // Lượng sát thương làm giảm khả năng giữ vững của kẻ địch (poise damage)
        public float poiseDamage = 10;

        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.0f; // Hệ số cho đòn tấn công nhẹ 01
        public float light_Attack_02_Modifier = 1.2f; // Hệ số cho đòn tấn công nhẹ 02
        public float heavy_Attack_01_Modifier = 1.4f; // Hệ số cho đòn tấn công nặng 01
        public float heavy_Attack_02_Modifier = 1.6f; // Hệ số cho đòn tấn công nặng 02
        public float charge_Attack_01_Modifier = 2f;  // Hệ số cho đòn tấn công sạc 01
        public float charge_Attack_02_Modifier = 2.2f; // Hệ số cho đòn tấn công sạc 02
        // Giải thích:
        // Hệ số điều chỉnh dùng để tăng hoặc giảm sát thương.
        // Ví dụ:
        // - Heavy Attack 01: 100 sát thương cơ bản x 1.4 = 140 sát thương thực tế.
        // - Charged Attack 01: 100 sát thương cơ bản x 2 = 200 sát thương thực tế.


        [Header("Stamina Cost Modifiers")] // Phần này hiển thị tiêu đề trong Inspector của Unity, giúp dễ dàng quản lý các giá trị liên quan đến tiêu hao stamina
        public int baseStaminaCost = 20;
        // Giá trị tiêu hao stamina cơ bản khi thực hiện hành động tấn công. 
        // Đây là lượng stamina mặc định mà nhân vật sẽ mất cho các hành động tiêu chuẩn.
        public float lightAttackStaminaCostMultiplier = 0.9f;
        // Hệ số nhân để điều chỉnh lượng stamina tiêu hao khi thực hiện tấn công nhẹ (Light Attack). 
        // Ví dụ, nếu baseStaminaCost là 20, thì với hệ số 0.9, tấn công nhẹ sẽ tiêu hao 18 stamina (20 * 0.9).


        [Header("Actions")]
        public WeaponItemAction ob_LMB_Action; // Hành động khi bấm nút chuột trái (LMB - Left Mouse Button).
        public WeaponItemAction ob_RMB_Action; // Hành động khi bấm nút chuột phải (RMB - Right Mouse Button).


        // Mảng các âm thanh "whoosh" để sử dụng khi thực hiện các đòn tấn công hoặc chuyển động nhanh. 
        // Âm thanh "whoosh" giúp tạo cảm giác về tốc độ và sức mạnh của hành động.
        [Header("Whooshes")]
        public AudioClip[] whooshes;

    }
}
