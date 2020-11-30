using UnityEngine;

namespace PhysSound
{
    public class PhysSoundTempAudio : MonoBehaviour
    {
        public AudioSource Audio { get; private set; }

        public void Initialize(PhysSoundTempAudioPool pool)
        {
            Audio = gameObject.AddComponent<AudioSource>();

            transform.SetParent(pool.transform);
            gameObject.SetActive(false);
        }

        public void PlayClip(AudioClip clip, Vector3 point, AudioSource template, float volume, float pitch)
        {
            PhysSoundTempAudioPool.CopyAudioSource(template, Audio);

            transform.position = point;

            Audio.clip = clip;
            Audio.volume = volume;
            Audio.pitch = pitch;

            gameObject.SetActive(true);

            Audio.Play();
        }

        void Update()
        {
            if (!Audio.isPlaying)
            {
                transform.position = Vector3.zero;
                gameObject.SetActive(false);
            }
        }
    }
}