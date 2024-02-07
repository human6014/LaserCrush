using UnityEngine;

namespace LaserSystem2D
{
    public class LaserComponents
    {
        public readonly LaserInteraction Interaction;
        public readonly TransformMapper TransformMapper;
        public readonly LaserCollision Collision;
        public readonly LaserDissolve Dissolve;
        public readonly LaserActiveHits ActiveHits = new LaserActiveHits();
        public readonly LaserRaycast Raycast;
        public readonly LaserHitEffect HitEffect;
        public readonly LaserLength Length;
        public readonly LaserAudio Audio;
        public readonly LaserView View;
        public readonly LaserHitEvent HitEvent;
    
        public LaserComponents(LaserData data, Laser laser)
        {
            Interaction = new LaserInteraction(laser);
            HitEffect = new LaserHitEffect(data.HitEffectPrefab);
            Raycast = new LaserRaycast(laser.transform);
            Dissolve = new LaserDissolve();
            Length = new LaserLength();
            TransformMapper = new TransformMapper(laser.transform);
            View = new LaserView(laser.transform.GetComponent<MeshRenderer>(), data.Line, Dissolve, Length);
            Collision = new LaserCollision(laser.transform, Length, data.Line);
            HitEvent = new LaserHitEvent(ActiveHits);
            Audio = new LaserAudio(data.LaserAudioSource, data.HitAudioSource, HitEvent);
        }
    }
}