using UnityEngine;
using UnityEngine.Audio;
using LaserCrush.Data;
using LaserCrush.UI.Controller;

namespace LaserCrush.Manager
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioData m_AudioData;
        [SerializeField] private AudioMixer m_AudioMixer;
        [SerializeField] private AudioSource m_BGMAudioSource;
        [SerializeField] private AudioSource m_SEAudioSource;

        private bool m_IsAutoBGMMode;

        private static AudioManager m_AudioManager;

        public static AudioManager AudioManagerInstance
        {
            get => m_AudioManager;
        }

        public void Init()
        {
            if (m_AudioManager == null)
            {
                m_AudioManager = this;
                DontDestroyOnLoad(gameObject);
                m_AudioData.DataToDictionary();
            }
            else Destroy(gameObject);
        }

        public void SetVolume(float value, SoundType soundType)
        {
            float setValue = value == -40f ? -80 : value;
            m_AudioMixer.SetFloat(soundType.ToString(), setValue);
        }

        #region BGM
        public void OnOffAutoBGMLoop(bool isOnOff)
        {
            m_IsAutoBGMMode = isOnOff;
        }

        private void Update()
        {
            if (!m_IsAutoBGMMode) return;

            if (m_BGMAudioSource.isPlaying) return;

            string randomKey = m_AudioData.GetRandomBGMKey();
            PlayBGM(randomKey);
        }

        public void PlayBGM(string audioName)
        {
            if (m_AudioData.GetBGM(audioName, out AudioClip audioClip))
            {
                m_BGMAudioSource.clip = audioClip;
                m_BGMAudioSource.Play();
            }
        }
        #endregion

        public void PlayNormalSE(string audioName)
        {
            if (m_SEAudioSource.isPlaying) return;
            if (m_AudioData.GetSENormal(audioName, out AudioClip audioClip))
            {
                m_SEAudioSource.clip = audioClip;
                m_SEAudioSource.Play();
            }
        }

        public void StopNormalSE(string audioName)
        {
            if (!m_SEAudioSource.isPlaying) return;
            if (m_SEAudioSource.clip is not null && m_SEAudioSource.clip.name == audioName) m_SEAudioSource.Stop();
        }

        public void PlayOneShotNormalSE(string audioName)
        {
            if (m_AudioData.GetSENormal(audioName, out AudioClip audioClip))
                m_SEAudioSource.PlayOneShot(audioClip);
        }

        public void PlayOneShotUISE(string audioName)
        {
            if (m_AudioData.GetSEUI(audioName, out AudioClip audioClip))
                m_SEAudioSource.PlayOneShot(audioClip);
        }
    }
}
