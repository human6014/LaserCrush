using System.Collections.Generic;
using UnityEngine;

namespace LaserSystem2D
{
    public class LaserInteraction
    {
        private readonly Dictionary<Collider2D, InteractedObject> _interactedObjects = new Dictionary<Collider2D, InteractedObject>();
        private readonly List<Collider2D> _objectsToExit = new List<Collider2D>();
        private readonly Laser _laser;

        public LaserInteraction(Laser laser)
        {
            _laser = laser;
        }

        public void Update(HashSet<Collider2D> hits)
        {
            Enter(hits);
            Stay(hits);
            Exit();
        }

        public void ExitAll()
        {
            foreach (InteractedObject interactedObject in _interactedObjects.Values)
            {
                interactedObject.Exit();
            }
        
            _interactedObjects.Clear();
        }

        private void Enter(HashSet<Collider2D> hits)
        {
            foreach (Collider2D hit in hits)
            {
                if (_interactedObjects.ContainsKey(hit) == false)
                {
                    _interactedObjects[hit] = new InteractedObject(hit.transform, _laser);
                    _interactedObjects[hit].Enter();
                }
            }
        }

        private void Stay(HashSet<Collider2D> hits)
        {
            _objectsToExit.Clear();
        
            foreach (Collider2D overlappingObject in _interactedObjects.Keys)
            {
                if (hits.Contains(overlappingObject))
                {
                    _interactedObjects[overlappingObject]?.Update();
                }
                else
                {
                    _objectsToExit.Add(overlappingObject);
                }
            }
        }

        private void Exit()
        {
            foreach (Collider2D objectToExit in _objectsToExit)
            {
                _interactedObjects[objectToExit].Exit();
                _interactedObjects.Remove(objectToExit);
            }
        }
    }
}