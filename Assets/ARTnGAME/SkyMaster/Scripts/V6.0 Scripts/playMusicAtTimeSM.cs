using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Artngame.SKYMASTER
{
    public class playMusicAtTimeSM : MonoBehaviour
    {
        public AudioSource audioDawn;
        public AudioSource audioMidday;
        public AudioSource audioDusk;
        public AudioSource audioNight;

        public float volumeIncreaseSpeed = 0.01f;

        public float maxVolumeDawn = 0.65f;
        public float maxVolumeMidday = 0.65f;
        public float maxVolumeDusk = 0.65f;
        public float maxVolumeNight = 0.65f;

        public SkyMasterManager skymanager;

        public bool changeWeatherPerMusic = false;
        //public enum Weather_types {Sunny, Foggy, HeavyFog, Tornado, SnowStorm, 
        //FreezeStorm, FlatClouds, LightningStorm, HeavyStorm,HeavyStormDark, Cloudy, RollingFog, VolcanoErupt, Rain}
        public SkyMasterManager.Volume_Weather_types dawnWeather = SkyMasterManager.Volume_Weather_types.Sunny;
        public SkyMasterManager.Volume_Weather_types middayWeather = SkyMasterManager.Volume_Weather_types.Cloudy;
        public SkyMasterManager.Volume_Weather_types duskWeather = SkyMasterManager.Volume_Weather_types.LightningStorm;
        public SkyMasterManager.Volume_Weather_types nightWeather = SkyMasterManager.Volume_Weather_types.Rain;

        public float dawnTime = 9;
        public float middayTime = 14;
        public float duskTime = 20;
        public float nightTime = 23.5f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (skymanager.Current_Time > dawnTime && skymanager.Current_Time < middayTime)
            {
                if (audioDawn.volume < maxVolumeDawn) {
                    audioDawn.volume += volumeIncreaseSpeed * Time.deltaTime;
                }

                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = dawnWeather;
                }
            }
            if (skymanager.Current_Time >= middayTime && skymanager.Current_Time < duskTime)
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioMidday.volume < maxVolumeMidday)
                {
                    audioMidday.volume += volumeIncreaseSpeed * Time.deltaTime;
                }
                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = middayWeather;
                }
            }
            if (skymanager.Current_Time >= duskTime && skymanager.Current_Time < nightTime)
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioDusk.volume < maxVolumeDusk)
                {
                    audioDusk.volume += volumeIncreaseSpeed * Time.deltaTime;
                }
                audioNight.volume -= volumeIncreaseSpeed * Time.deltaTime;

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = duskWeather;
                }
            }
            if ((skymanager.Current_Time >= nightTime && skymanager.Current_Time <= 24) || (skymanager.Current_Time >= 0 && skymanager.Current_Time <= dawnTime))
            {
                audioDawn.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioMidday.volume -= volumeIncreaseSpeed * Time.deltaTime;
                audioDusk.volume -= volumeIncreaseSpeed * Time.deltaTime;
                if (audioNight.volume < maxVolumeNight)
                {
                    audioNight.volume += volumeIncreaseSpeed * Time.deltaTime;
                }

                if (changeWeatherPerMusic)
                {
                    skymanager.currentWeatherName = nightWeather;
                }
            }
        }
    }
}