using SG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class ResetIsJumping : StateMachineBehaviour
    {
        // Biến tham chiếu tới CharacterManager của nhân vật để quản lý trạng thái.
        CharacterManager character;

        // OnStateEnter được gọi khi quá trình chuyển đổi trạng thái bắt đầu và state machine bắt đầu đánh giá trạng thái này.
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Nếu chưa có tham chiếu tới CharacterManager, lấy nó từ component của Animator.
            if (character == null)
            {
                character = animator.GetComponent<CharacterManager>();
            }

            // Nếu đây là nhân vật của người chơi hiện tại, đặt lại trạng thái nhảy của nhân vật về false khi hành động nhảy kết thúc.
            if (character.IsOwner)
            {
                character.characterNetworkManager.isJumping.Value = false;

            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
