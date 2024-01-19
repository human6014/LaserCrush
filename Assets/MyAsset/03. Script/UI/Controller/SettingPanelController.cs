using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.UI.Receiver;

namespace LaserCrush.UI.Controller
{
    public enum SoundType
    {
        Master,
        BGM,
        SE
    }

    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private SliderReceiver m_SettingMasterSliderReceiver;
        [SerializeField] private SliderReceiver m_SettingBgmSliderReceiver;
        [SerializeField] private SliderReceiver m_SettingSESliderReceiver;

        public void Init()
        {
            m_SettingMasterSliderReceiver.SliderValueChanged += SoundChange;
            m_SettingBgmSliderReceiver.SliderValueChanged += SoundChange;
            m_SettingSESliderReceiver.SliderValueChanged += SoundChange;
        }

        private void SoundChange(float value, SoundType soundType)
            => AudioManager.AudioManagerInstance.SetVolume(value,soundType);
    }
}
