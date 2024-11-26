using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        // Khai báo biến bảo vệ chứa đối tượng Collider, được sử dụng để xác định vùng va chạm gây sát thương
        [SerializeField] protected Collider damageCollider;


        [Header("Damage")]
        public float physicalDamage = 0; // Sát thương vật lý
        public float magicDamage = 0; // Sát thương phép thuật
        public float fireDamage = 0; // Sát thương lửa
        public float lightningDamage = 0; // Sát thương sét
        public float holyDamage = 0; // Sát thương thánh

        [Header("Contact Point")]
        // Điểm va chạm nơi nhân vật bị trúng đòn
        protected Vector3 contactPoint;

        [Header("Characters Damaged")]
        // Danh sách các nhân vật đã bị sát thương
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        protected virtual void Awake()
        {

        }

        // Phương thức được gọi khi có vật thể đi vào vùng trigger
        protected virtual void OnTriggerEnter(Collider other)
        {
            // Kiểm tra nếu đối tượng va chạm là một nhân vật bằng cách tìm kiếm CharacterManager trong đối tượng cha
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            // Nếu tìm thấy đối tượng có CharacterManager, tức là nhân vật bị tấn công
            if (damageTarget != null)
            {
                // Tính toán vị trí va chạm gần nhất với đối tượng 'other' so với vị trí của đối tượng hiện tại
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                // Gây sát thương cho mục tiêu nếu damageTarget hợp lệ
                DamageTarget(damageTarget);
            }
        }


        // Phương thức gây sát thương cho mục tiêu
        protected virtual void DamageTarget(CharacterManager damageTarget)
        {
            // Nếu nhân vật đã bị sát thương trước đó, không gây thêm sát thương
            if (charactersDamaged.Contains(damageTarget))
                return;

            // Thêm nhân vật vào danh sách đã bị sát thương
            charactersDamaged.Add(damageTarget);

            // Tạo một hiệu ứng sát thương (TakeDamageEffect) từ hiệu ứng toàn cục
            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage; // Gán sát thương vật lý
            damageEffect.magicDamage = magicDamage; // Gán sát thương phép thuật
            damageEffect.fireDamage = fireDamage; // Gán sát thương lửa
            damageEffect.holyDamage = holyDamage; // Gán sát thương thánh
            damageEffect.contactPoint = contactPoint; // Gán vị trí va chạm

            // Xử lý hiệu ứng sát thương trên nhân vật
            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }

        public virtual void EnableDamageCollider()
        {
            // Kích hoạt collider, cho phép gây sát thương khi va chạm
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            // Vô hiệu hóa collider, ngăn việc gây sát thương khi va chạm
            damageCollider.enabled = false;

            // Xóa danh sách các nhân vật đã nhận sát thương từ collider(reset danh sach để có thể tiếp tục gây damage)
            charactersDamaged.Clear();
        }


    }
}
