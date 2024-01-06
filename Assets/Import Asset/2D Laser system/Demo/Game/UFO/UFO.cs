using UnityEngine;

namespace LaserSystem2D
{
    public class UFO : MonoBehaviour, ILaserEntered, IRestart
    {
        [SerializeField] private UFOAnimation _animation;
        [SerializeField] private UFOMovement _movement;
        [SerializeField] private SpriteFlip _spriteFlip;
        [SerializeField] private LevelRestart _restart;
        private readonly UFOInput _input = new UFOInput();
        private UFODeath _death;
    
        private void Start()
        {
            _death = new UFODeath(transform);
        }

        private void FixedUpdate()
        {
            _movement.Update(_input.Value);
        }

        private void Update()
        {
            _input.Update();
            _animation.Update();
            _spriteFlip.Update(_input.Value);
        }

        public void OnLaserEntered(Laser laser)
        {
            _restart.RestartForEach();
        }

        public void Restart()
        {
            _death.Restart();
        }
    }
}