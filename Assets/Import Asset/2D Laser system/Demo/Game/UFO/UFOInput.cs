using UnityEngine;

namespace LaserSystem2D
{
    public class UFOInput
    {
        public Vector2 Value { get; private set; }
    
        public void Update()
        {
            Value =  new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}