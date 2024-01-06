using UnityEngine;

namespace LaserSystem2D
{
    public class LaserView
    {
        private readonly MeshRenderer _meshRenderer;
        private readonly LaserMaterial _material;
        private readonly LaserDissolve _dissolve;
        private readonly LaserLength _length;
        private readonly FullRectLine _line;
    
        public LaserView(MeshRenderer meshRenderer, FullRectLine line, LaserDissolve dissolve, LaserLength length)
        {
            _material = new LaserMaterial();
            _meshRenderer = meshRenderer;
            _dissolve = dissolve;
            _length = length;
            _line = line;
        }

        public void Update(int sortingOrder)
        {
            _material.Clear();
            _material.SetShape(_line.Width, _line.Length, _line.GeneratedMesh.uv, _length.Fill, _dissolve.Value);
            _meshRenderer.SetPropertyBlock(_material.PropertyBlock);
            _meshRenderer.sortingOrder = sortingOrder;
        }
    }
}