using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace SG
{
    // Tạo một ScriptableObject để xử lý hiệu ứng gây sát thương cho nhân vật
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; // Nhân vật gây sát thương

        [Header("Damage")]
        public float physicalDamage = 0; // Sát thương vật lý
        public float magicDamage = 0; // Sát thương phép thuật
        public float fireDamage = 0; // Sát thương lửa
        public float lightningDamage = 0; // Sát thương sét
        public float holyDamage = 0; // Sát thương thánh

        [Header("Final Damage")]
        private int finalDamageDealt = 0; // Tổng sát thương cuối cùng sau khi tính toán

        [Header("Poise")]
        public float poiseDamage = 0; // Sát thương lên poise (lực đỡ)
        public bool poiseIsBroken = false; // Kiểm tra xem poise đã bị phá vỡ hay chưa

        [Header("Animation")]
        public bool playDamageAnimation = true; // Cờ để kiểm tra có phát hoạt cảnh nhận damage không
        public bool manuallySelectDamageAnimation = false; // Kiểm tra xem có chọn hoạt cảnh nhận damage thủ công không
        public string damageAnimation; // Hoạt cảnh nhận damage

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true; // Cờ kiểm tra có phát hiệu ứng âm thanh khi nhận damage không
        public AudioClip elementalDamageSoundFX; // Âm thanh dành cho damage nguyên tố

        [Header("Direction Damage Taken From")]
        public float angleHitFrom; // Góc tấn công của đối thủ
        public Vector3 contactPoint; // Điểm va chạm khi nhân vật bị tấn công(máu bắn tung tóe :v)

        // Ghi đè phương thức xử lý hiệu ứng nhận sát thương
        public override void ProcessEffect(CharacterManager character)
        {
            base.ProcessEffect(character);

            // Nếu nhân vật đã chết thì không xử lý tiếp
            if (character.isDead.Value)
                return;

            // Tính toán sát thương gây ra cho nhân vật
            CalculateDamage(character);

            // Gọi phương thức PlayDirectionalBasedDamageAnimation để phát hoạt ảnh sát thương theo hướng tấn công (trái, phải, trước, sau) cho nhân vật (character).
            PlayDirectionalBasedDamageAnimation(character);

            // Gọi phương thức PlayDamageSFX để phát âm thanh sát thương cho nhân vật (character) khi nhận sát thương.
            PlayDamageSFX(character);

            // Phương thức này sẽ kích hoạt hiệu ứng máu (blood splatter) tại vị trí va chạm.
            PlayDamageVFX(character);
        }

        // Phương thức tính toán sát thương
        public void CalculateDamage(CharacterManager character)
        {
            // Nếu nhân vật không phải là chủ sở hữu (dành cho hệ thống mạng) thì không xử lý tiếp
            if (!character.IsOwner)
                return;

            // Nếu có nhân vật gây sát thương, có thể thêm logic xử lý tại đây (hiện tại để trống)
            if (characterCausingDamage != null)
            {

            }

            // Tính tổng sát thương cuối cùng từ tất cả các loại sát thương
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            // Nếu tổng sát thương <= 0, đặt giá trị sát thương cuối cùng thành 1 (để đảm bảo luôn có damage)
            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }
            Debug.Log("Sát thương cuối cùng gây ra là: " + finalDamageDealt);
            // Giảm sức khỏe hiện tại của nhân vật dựa trên sát thương cuối cùng tính được
            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
        }

        // Phương thức này dùng để kích hoạt hiệu ứng máu khi nhân vật bị tác động(nhận sát thương)
        private void PlayDamageVFX(CharacterManager character)
        {
            // Gọi phương thức phát hiệu ứng máu từ character's characterEffectsManager tại vị trí va chạm (contactPoint)
            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }

        // Phương thức này chọn một âm thanh ngẫu nhiên từ danh sách âm thanh sát thương và phát nó cho nhân vật khi nhân vật nhận sát thương
        private void PlayDamageSFX(CharacterManager character)
        {
            // Chọn một âm thanh ngẫu nhiên từ mảng âm thanh sát thương vật lý
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            // Phát âm thanh sát thương vật lý khi nhân vật bị tấn công
            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);

            // Phát âm thanh rên đau của nhân vật khi nhận sát thương
            // Âm thanh này được chọn ngẫu nhiên từ mảng damageGrunts để tạo cảm giác chân thực
            character.characterSoundFXManager.PlayDamageGrunt();

        }

        // Phương thức này tính toán hướng của đòn tấn công dựa trên góc tấn công (angleHitFrom) và chọn hoạt ảnh sát thương tương ứng với hướng đó
        private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            // Chỉ thực thi khi nhân vật là chủ sở hữu (Owner), nếu không thì thoát.
            if (!character.IsOwner)
                return;

            // Nếu nhân vật đã chết thì không xử lý tiếp
            if (character.isDead.Value)
                return;

            // Đánh dấu trạng thái rằng nhân vật bị phá vỡ tư thế (poise).
            poiseIsBroken = true;

            // Xử lý hướng của đòn tấn công dựa trên giá trị của `angleHitFrom`:
            // Nếu góc tấn công nằm trong khoảng từ 145 đến 180 độ (phía trước bên trái).
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                // Lấy một hoạt ảnh ngẫu nhiên từ danh sách hoạt ảnh sát thương phía trước.
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            // Nếu góc tấn công nằm trong khoảng từ -145 đến -180 độ (phía trước bên phải).
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                // Lấy một hoạt ảnh ngẫu nhiên từ danh sách hoạt ảnh sát thương phía trước.
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            // Nếu góc tấn công nằm trong khoảng từ -45 đến 45 độ (phía sau nhân vật).
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                // Lấy một hoạt ảnh ngẫu nhiên từ danh sách hoạt ảnh sát thương phía sau.
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            // Nếu góc tấn công nằm trong khoảng từ -144 đến -45 độ (phía bên trái).
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                // Lấy một hoạt ảnh ngẫu nhiên từ danh sách hoạt ảnh sát thương bên trái.
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            // Nếu góc tấn công nằm trong khoảng từ 45 đến 144 độ (phía bên phải).
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                // Lấy một hoạt ảnh ngẫu nhiên từ danh sách hoạt ảnh sát thương bên phải.
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }

            // Nếu tư thế (poise) của nhân vật đã bị phá vỡ:
            if (poiseIsBroken)
            {
                // Lưu lại hoạt ảnh sát thương cuối cùng vừa chơi.
                character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;

                // Phát hoạt ảnh sát thương phù hợp với hướng đòn tấn công.
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
            }
        }
    }
}
