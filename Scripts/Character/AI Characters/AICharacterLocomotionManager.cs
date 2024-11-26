using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Lớp AICharacterLocomotionManager kế thừa từ CharacterLocomotionManager
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        // Phương thức để xoay nhân vật AI về phía agent
        public void RotateTowardsAgent(AICharacterManager aiCharacter)
        {
            // Kiểm tra xem nhân vật AI có đang di chuyển hay không
            if (aiCharacter.characterNetworkManager.isMoving.Value)
            {
                // Nếu đang di chuyển, gán hướng xoay của nhân vật AI bằng với hướng của NavMeshAgent
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
    }
}

