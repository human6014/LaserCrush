using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LaserSystem2D
{
    public class InteractedObject
    {
        private readonly IEnumerable<ILaserStay> _onStay;
        private readonly IEnumerable<ILaserExited> _onExited;
        private readonly IEnumerable<ILaserEntered> _onEntered;
        private readonly Laser _laser;

        public InteractedObject(Transform context, Laser laser)
        {
            _laser = laser;
            _onStay = GetComponents<ILaserStay>(context);
            _onExited = GetComponents<ILaserExited>(context);
            _onEntered = GetComponents<ILaserEntered>(context);
        }

        public void Enter()
        {
            foreach (ILaserEntered entered in _onEntered)
            {
                entered?.OnLaserEntered(_laser);
            }
        }

        public void Update()
        {
            foreach (ILaserStay stay in _onStay)
            {
                stay?.OnLaserStay(_laser);
            }
        }

        public void Exit()
        {
            foreach (ILaserExited exited in _onExited)
            {
                exited?.OnLaserExited(_laser);
            }
        }

        private IEnumerable<T> GetComponents<T>(Transform context)
        {
            MonoBehaviour[] monoBehaviours = context.GetComponents<MonoBehaviour>();

            return monoBehaviours.OfType<T>();
        }
    }
}