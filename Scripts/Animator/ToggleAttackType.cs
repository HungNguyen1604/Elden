using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp ToggleAttackType kế thừa từ StateMachineBehaviour để điều chỉnh loại tấn công trong Animator
    public class ToggleAttackType : StateMachineBehaviour
    {
        // Biến để lưu trữ thông tin của CharacterManager
        CharacterManager character;

        // Biến lưu loại tấn công sẽ được áp dụng khi vào trạng thái
        [SerializeField] AttackType attackType;

        // Phương thức gọi khi trạng thái bắt đầu
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Kiểm tra xem character đã được khởi tạo chưa
            if (character == null)
            {
                // Lấy thành phần CharacterManager từ Animator
                character = animator.GetComponent<CharacterManager>();
            }

            // Cập nhật loại tấn công hiện tại của nhân vật
            character.characterCombatManager.currentAttackType = attackType;
        }

        // Phương thức gọi khi trạng thái đang được cập nhật (nếu cần thiết có thể sử dụng trong tương lai)
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // Phương thức gọi khi trạng thái kết thúc (nếu cần thiết có thể sử dụng trong tương lai)
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // Phương thức gọi ngay sau khi Animator di chuyển
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // Phương thức gọi ngay sau khi Animator thực hiện IK
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
