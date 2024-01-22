using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.UI.Receiver;
using LaserCrush.UI.Displayer;

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
        [SerializeField] private SliderReceiver m_SettingBGMSliderReceiver;
        [SerializeField] private SliderReceiver m_SettingSESliderReceiver;

        [SerializeField] private SliderDisplayer m_SettingMasterSliderDisplayer;
        [SerializeField] private SliderDisplayer m_SettingBGMSliderDisplayer;
        [SerializeField] private SliderDisplayer m_SettingSESliderDisplayer;

        public void Init()
        {
            m_SettingMasterSliderDisplayer.Init(DataManager.SettingData.m_MasterSound);
            m_SettingBGMSliderDisplayer.Init(DataManager.SettingData.m_BGMSound);
            m_SettingSESliderDisplayer.Init(DataManager.SettingData.m_SESound);

            m_SettingMasterSliderReceiver.SliderValueChanged += SoundChange;
            m_SettingBGMSliderReceiver.SliderValueChanged += SoundChange;
            m_SettingSESliderReceiver.SliderValueChanged += SoundChange;
        }

        private void SoundChange(float value, SoundType soundType)
            => AudioManager.AudioManagerInstance.SetVolume(value, soundType);
    }
}
