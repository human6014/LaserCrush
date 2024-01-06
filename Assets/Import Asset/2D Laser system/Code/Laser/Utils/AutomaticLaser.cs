using System.Collections;
using UnityEngine;

namespace LaserSystem2D
{
    [RequireComponent(typeof(Laser))]
    public class AutomaticLaser : MonoBehaviour
    {
        [SerializeField] private bool _infiniteWorking = true;
        [SerializeField] private bool _startWithSleepMode = true;
        [SerializeField] [Min(0)] private float _waitTimeAtStart = 2;
        [SerializeField] [Min(0)] private float _workTime = 4;
        [SerializeField] [Min(0)] private float _sleepTime = 2;

        private Laser _laser;
        private Laser Laser
        {
            get 
            {
                if (_laser == null)
                {
                    _laser = GetComponent<Laser>();
                }

                return _laser;
            } 
        }

        public bool InfiniteWorking => _infiniteWorking;
    
        private void OnEnable()
        {
            StartLaser();
        }

        private void StartLaser()
        {
            StopAllCoroutines();

            if (_infiniteWorking)
            {
                Laser.Enable();
            }
            else
            {
                StartCoroutine(StartLaserCoroutine());
            }
        }

        private IEnumerator StartLaserCoroutine()
        {
            if (_startWithSleepMode == false)
            {
                Laser.Enable();
            }
        
            yield return new WaitForSeconds(_waitTimeAtStart);

            while (true)
            {
                Laser.Enable(transform);
            
                yield return new WaitForSeconds(_workTime);
            
                Laser.Disable();
            
                yield return new WaitForSeconds(_sleepTime + Laser.Data.DissolveTime);
            }
        }
    }
}