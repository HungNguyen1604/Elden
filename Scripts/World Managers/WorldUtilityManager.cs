using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager instance;// Tham chiếu

        [Header("Layers")]
        [SerializeField] LayerMask characterLayers; // Lớp cho nhân vật
        [SerializeField] LayerMask enviroLayers;   // Lớp cho môi trường

        private void Awake()
        {
            // Kiểm tra xem có một instance nào của WorldUtilityManager đã tồn tại chưa
            if (instance == null)
            {
                instance = this; // Nếu chưa, gán instance này
            }
            else
            {
                Destroy(gameObject); // Nếu đã tồn tại, hủy đối tượng hiện tại
            }
        }
        // Phương thức trả về lớp nhân vật
        public LayerMask GetCharacterLayers()
        {
            return characterLayers;
        }
        // Phương thức trả về lớp môi trường
        public LayerMask GetEnviroLayers()
        {
            return enviroLayers;
        }

        // Phương thức kiểm tra xem nhân vật tấn công có thể gây sát thương lên mục tiêu hay không
        // Dựa trên nhóm (team) của nhân vật tấn công và nhóm của nhân vật mục tiêu
        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
        {
            // Nếu nhân vật tấn công thuộc nhóm Team1
            if (attackingCharacter == CharacterGroup.Team1)
            {
                // Xác định nhóm của nhân vật mục tiêu
                switch (targetCharacter)
                {
                    case CharacterGroup.Team1:
                        return false; // Không thể tấn công đồng đội (cùng nhóm Team1)
                    case CharacterGroup.Team2:
                        return true;  // Có thể tấn công kẻ địch (thuộc nhóm Team2)
                    default:
                        break;
                }
            }
            // Nếu nhân vật tấn công thuộc nhóm Team2
            else if (attackingCharacter == CharacterGroup.Team2)
            {
                // Xác định nhóm của nhân vật mục tiêu
                switch (targetCharacter)
                {
                    case CharacterGroup.Team1:
                        return true;  // Có thể tấn công kẻ địch (thuộc nhóm Team1)
                    case CharacterGroup.Team2:
                        return false; // Không thể tấn công đồng đội (cùng nhóm Team2)
                    default:
                        break;
                }
            }
            // Trả về false nếu không có trường hợp nào phù hợp
            return false;
        }


        // Tính góc giữa hướng của nhân vật và hướng mục tiêu
        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetsDirection)
        {
            targetsDirection.y = 0; // Loại bỏ độ cao của mục tiêu để tính góc phẳng
            float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection); // Tính góc giữa hướng nhìn và mục tiêu
            Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection); // Tìm vector pháp tuyến để xác định hướng quay

            if (cross.y < 0) // Nếu giá trị y của vector cross âm, góc quay là âm
                viewableAngle = -viewableAngle;

            return viewableAngle; // Trả về góc quay đã tính
        }

    }
}
