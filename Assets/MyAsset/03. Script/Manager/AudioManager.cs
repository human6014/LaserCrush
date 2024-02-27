using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
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

        private readonly float m_MaxWaitTime = 3;

        private float m_WaitTime;

        private float m_MasterSound;
        private float m_BGMSound;
        private float m_SESound;

        private readonly int m_MaxConcurrentAudioCount = 5;
        private int m_ConcurrentAudioCount = 0;

        private static AudioManager m_AudioManager;

        public static AudioManager AudioManagerInstance { get => m_AudioManager; }

        #region Init
        public void Init()
        {
            if (m_AudioManager == null)
            {
                m_AudioManager = this;
                DontDestroyOnLoad(gameObject);
                m_AudioData.DataToDictionary();
                
                m_MasterSound = DataManager.SettingData.m_MasterSound;
                m_BGMSound = DataManager.SettingData.m_BGMSound;
                m_SESound = DataManager.SettingData.m_SESound;
            }
            else Destroy(gameObject);
        }

        private void Start()
        {
            SetVolume(m_MasterSound, SoundType.Master);
            SetVolume(m_BGMSound, SoundType.BGM);
            SetVolume(m_SESound, SoundType.SE);
        }
        #endregion

        public void SetVolume(float value, SoundType soundType)
        {
            float setValue = value == -40f ? -80 : value;

            if (soundType == SoundType.Master)
                m_MasterSound = setValue;
            else if (soundType == SoundType.BGM)
                m_BGMSound = setValue;
            else 
                m_SESound = setValue;
            
            m_AudioMixer.SetFloat(soundType.ToString(), setValue);
        }

        #region BGM
        public void OnOffAutoBGMLoop(bool isOnOff)
            => m_IsAutoBGMMode = isOnOff;
        
        private void Update()
        {
            if (!m_IsAutoBGMMode) return;

            if (m_BGMAudioSource.isPlaying) return;

            m_WaitTime += Time.deltaTime;
            if (m_WaitTime >= m_MaxWaitTime)
            {
                string randomKey = m_AudioData.GetRandomBGMKey();
                PlayBGM(randomKey);
                m_WaitTime = 0;
            }
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

        #region SE
        public void PlayOneShotNormalSE(string audioName)
        {
            if (m_AudioData.GetSENormal(audioName, out AudioClip audioClip))
                m_SEAudioSource.PlayOneShot(audioClip);
            else Debug.LogWarning("No value for [" + audioName+ "] name");
        }

        public void PlayOneShotUISE(string audioName)
        {
            if (m_AudioData.GetSEUI(audioName, out AudioClip audioClip))
                m_SEAudioSource.PlayOneShot(audioClip);
            else Debug.LogWarning("No value for [" + audioName + "] name");
        }

        public void PlayOneShotConcurrent(string audioName)
        {
            if(m_AudioData.GetSENormal(audioName, out AudioClip audioClip))
            {
                if (m_ConcurrentAudioCount >= m_MaxConcurrentAudioCount)
                    return;

                m_ConcurrentAudioCount++;
                m_SEAudioSource.PlayOneShot(audioClip);
                StartCoroutine(CheckAudioEnd(audioClip));
            }
            else Debug.LogWarning("No value for [" + audioName + "] name");
        }

        private IEnumerator CheckAudioEnd(AudioClip audioClip)
        {
            yield return new WaitForSeconds(audioClip.length);
            m_ConcurrentAudioCount--;
        }
        #endregion

        public void SaveAllData()
        {
            DataManager.SettingData.m_MasterSound = m_MasterSound;
            DataManager.SettingData.m_BGMSound = m_BGMSound;
            DataManager.SettingData.m_SESound = m_SESound;

            DataManager.SaveSettingData();
        }
    }
}
