using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    // Quản lý hiệu ứng âm thanh cho nhân vật
    public class CharacterSoundFXManager : MonoBehaviour
    {
        // Thành phần AudioSource của nhân vật, dùng để phát âm thanh
        AudioSource audioSource;

        // Mảng các âm thanh khi nhân vật nhận sát thương (ví dụ: tiếng rên đau khi bị đánh).
        // Các âm thanh này sẽ tạo hiệu ứng âm thanh chân thực khi nhân vật chịu đòn.
        [Header("Damage Grunts")]
        [SerializeField] protected AudioClip[] damageGrunts;

        // Mảng các âm thanh khi con undead thực hiện tấn công (ví dụ: tiếng cào, xé, hoặc đấm).
        // Những âm thanh này sẽ tạo cảm giác đáng sợ và bạo lực, giúp tăng tính chân thực cho các đòn tấn công của con undead.
        [Header("Attack Grunts")]
        [SerializeField] protected AudioClip[] attackGrunts;


        // Khởi tạo và lấy tham chiếu tới AudioSource trên đối tượng này
        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Phát một âm thanh hiệu ứng (soundFX) với âm lượng và tùy chọn điều chỉnh cao độ ngẫu nhiên.
        public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
        {
            audioSource.PlayOneShot(soundFX, volume); // Phát âm thanh hiệu ứng với âm lượng xác định.
            audioSource.pitch = 1; // Đặt cao độ âm thanh về giá trị mặc định.

            if (randomizePitch) // Nếu tùy chọn điều chỉnh cao độ được bật
            {
                audioSource.pitch += Random.Range(-pitchRandom, pitchRandom); // Điều chỉnh cao độ ngẫu nhiên.
            }
        }

        // Phương thức phát âm thanh khi nhân vật thực hiện hành động lăn
        public void PlayRollSoundFX()
        {
            // Sử dụng PlayOneShot để phát âm thanh một lần mà không chồng lấn âm thanh nếu đã phát trước đó
            // WorldSoundFXManager là singleton quản lý các hiệu ứng âm thanh chung trong thế giới
            audioSource.PlayOneShot(WorldSoundFXManager.instance.rollFX);
        }

        // Phát âm thanh rên đau khi nhân vật nhận sát thương
        public virtual void PlayDamageGrunt()
        {
            // Chọn ngẫu nhiên một âm thanh từ mảng damageGrunts và phát ra âm thanh
            // để tăng cảm giác chân thực khi nhân vật bị tấn công.
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }

        // Phát âm thanh khi con undead tấn công (ví dụ: tiếng cào, xé, đấm)
        public virtual void PlayAttackGrunt()
        {
            // Chọn ngẫu nhiên một âm thanh từ mảng attackGrunts và phát ra âm thanh
            // để tạo cảm giác đáng sợ, chân thực khi con undead thực hiện đòn tấn công.
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }

    }
}

