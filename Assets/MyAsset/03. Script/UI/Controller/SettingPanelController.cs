using LaserCrush.UI.Receiver;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        {

        }
    }
}
