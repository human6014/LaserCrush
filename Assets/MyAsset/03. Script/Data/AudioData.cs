using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [System.Serializable]
    public struct AudioGroup
    {
        public string m_AudioName;
        public AudioClip m_AudioClip;
    }
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Audio Data", order = int.MaxValue)]
    public class AudioData : ScriptableObject
    {
        [SerializeField] private AudioGroup[] m_BGMAudioGroups;
        [SerializeField] private AudioGroup[] m_SENormalAudioGroups;
        [SerializeField] private AudioGroup[] m_SEUIAudioGroups;

        private Dictionary<string, AudioClip> m_BGMAudioDictionary;
        private Dictionary<string, AudioClip> m_SENormalAuidoDictionary;
        private Dictionary<string, AudioClip> m_SEUIAudioDictionary;

        public string GetRandomBGMKey()
        {
            int index = Random.Range(0, m_BGMAudioGroups.Length);
            return m_BGMAudioGroups[index].m_AudioName;
        }

        public bool GetBGM(string key, out AudioClip audioClip)
            => m_BGMAudioDictionary.TryGetValue(key, out audioClip);
        
        public bool GetSENormal(string key, out AudioClip audioClip)
            => m_SENormalAuidoDictionary.TryGetValue(key, out audioClip);

        public bool GetSEUI(string key, out AudioClip audioClip)
            => m_SEUIAudioDictionary.TryGetValue(key, out audioClip);


        public void DataToDictionary()
        {
            m_BGMAudioDictionary = new Dictionary<string, AudioClip>();
            m_SENormalAuidoDictionary = new Dictionary<string, AudioClip>();
            m_SEUIAudioDictionary = new Dictionary<string, AudioClip>();

            foreach (AudioGroup audioGroup in m_BGMAudioGroups)
                m_BGMAudioDictionary.Add(audioGroup.m_AudioName, audioGroup.m_AudioClip);
            
            foreach (AudioGroup audioGroup in m_SENormalAudioGroups)
                m_SENormalAuidoDictionary.Add(audioGroup.m_AudioName, audioGroup.m_AudioClip);

            foreach (AudioGroup audioGroup in m_SEUIAudioGroups)
                m_SEUIAudioDictionary.Add(audioGroup.m_AudioName, audioGroup.m_AudioClip);
        }
    }
}
