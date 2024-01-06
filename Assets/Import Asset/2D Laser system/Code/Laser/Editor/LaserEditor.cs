using UnityEditor;

namespace LaserSystem2D
{
    [CustomEditor(typeof(Laser))]
    [CanEditMultipleObjects]
    public class LaserEditor : Editor 
    {
        private SerializedProperty _raycastMask;
        private SerializedProperty _nonHitDistance;
        private SerializedProperty _collisionMask;
        private SerializedProperty _shootingSpeed;
        private SerializedProperty _dissolveTime;
        private SerializedProperty _collisionWidth;
        private SerializedProperty _collisionPenetration;
        private SerializedProperty _sortingOrder;
        private SerializedProperty _maxPoints;
        private SerializedProperty _width;
        private SerializedProperty _hitEffectPrefab;
        private SerializedProperty _laserAudioSource;
        private SerializedProperty _hitAudioSource;

        private void OnEnable()
        {
            SerializedProperty laserData = serializedObject.FindProperty("_data");
            SerializedProperty line = laserData.FindPropertyRelative("_line");
        
            _raycastMask = laserData.FindPropertyRelative("_raycastMask");
            _nonHitDistance = laserData.FindPropertyRelative("_nonHitDistance");
            _collisionMask = laserData.FindPropertyRelative("_collisionMask");
            _shootingSpeed = laserData.FindPropertyRelative("_shootingSpeed");
            _dissolveTime = laserData.FindPropertyRelative("_dissolveTime");
            _collisionWidth = laserData.FindPropertyRelative("_collisionWidth");
            _collisionPenetration = laserData.FindPropertyRelative("_collisionPenetration");
            _sortingOrder = laserData.FindPropertyRelative("_sortingOrder");
            _maxPoints = line.FindPropertyRelative("_maxPoints");
            _width = line.FindPropertyRelative("_width");
            _hitEffectPrefab = laserData.FindPropertyRelative("_hitEffectPrefab");
            _laserAudioSource = laserData.FindPropertyRelative("_laserAudioSource");
            _hitAudioSource = laserData.FindPropertyRelative("_hitAudioSource");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.LabelField("Raycast", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_raycastMask);
            EditorGUILayout.PropertyField(_shootingSpeed);
            EditorGUILayout.PropertyField(_nonHitDistance);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Collision", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_collisionMask);
            EditorGUILayout.PropertyField(_collisionWidth);
            EditorGUILayout.PropertyField(_collisionPenetration);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Line", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_maxPoints);
            EditorGUILayout.PropertyField(_width);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("On hit", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_hitEffectPrefab);
            EditorGUILayout.PropertyField(_hitAudioSource);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        
            EditorGUILayout.PropertyField(_dissolveTime);
            EditorGUILayout.PropertyField(_sortingOrder);
            EditorGUILayout.PropertyField(_laserAudioSource);

            serializedObject.ApplyModifiedProperties();
        }
    }
}