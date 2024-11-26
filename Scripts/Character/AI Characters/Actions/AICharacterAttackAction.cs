using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Tạo một menu trong Unity Editor cho đối tượng tấn công của AI
    [CreateAssetMenu(menuName = "A.I/Actions/Attack")]

    // Định nghĩa lớp AICharacterAttackAction cho các hành động tấn công của AI
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Attack")]
        // Tên animation cho hành động tấn công của AI
        [SerializeField] private string attackAnimation;

        [Header("Combo Action")]
        // Hành động combo liên kết, nếu AI có thể thực hiện nhiều bước trong chuỗi tấn công
        public AICharacterAttackAction comboAction;

        [Header("Action Values")]
        // Loại tấn công (có thể là đánh gần, đánh xa, v.v.)
        [SerializeField] AttackType attackType;
        // Trọng số của hành động tấn công, ảnh hưởng đến khả năng AI chọn hành động này
        public int attackWeight = 50;

        // Thời gian hồi phục sau khi hoàn thành hành động tấn công
        public float actionRecoveryTime = 1.5f;
        // Góc tối thiểu để hành động tấn công có thể xảy ra
        public float minimumAttackAngle = -35;
        // Góc tối đa để hành động tấn công có thể xảy ra
        public float maximumAttackAngle = 35;
        // Khoảng cách tối thiểu để hành động tấn công có thể xảy ra
        public float minimumAttackDistance = 0;
        // Khoảng cách tối đa để hành động tấn công có thể xảy ra
        public float maximumAttackDistance = 2;

        // Thử thực hiện hành động tấn công, truyền dữ liệu hành động cho AI để AI thực hiện animation tấn công
        public void AttemptToPerformAction(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);
        }
    }
}

