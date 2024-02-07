using UnityEngine;

namespace LaserSystem2D
{
    public class LaserAudio
    {
        private readonly LaserHitEvent _hitEvent;
        private readonly AudioSource _laser;
        private readonly AudioSource _hit;

        public LaserAudio(AudioSource laser, AudioSource hit, LaserHitEvent hitEvent)
        {
            _laser = laser;
            _hit = hit;
            _hitEvent = hitEvent;
        }

        public void Start()
        {
            _hitEvent.AddListener(PlayHitSound);
        
            if (_laser != null)
                _laser.Play();
        }

        public void Stop()
        {
            _hitEvent.RemoveListener(PlayHitSound);
        
            if (_laser != null)
                _laser.Stop();
        }

        private void PlayHitSound(LaserHit hit)
        {
            if (_hit != null)
                _hit.Play();
        }   
    }
}