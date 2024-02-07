using UnityEditor;

namespace LaserSystem2D
{
    [CustomEditor(typeof(AutomaticLaser))]
    [CanEditMultipleObjects]
    public class AutomaticLaserEditor : Editor
    {
        private SerializedProperty _infiniteWorking;
        private SerializedProperty _startWithSleepMode;
        private SerializedProperty _waitTimeAtStart;
        private SerializedProperty _workTime;
        private SerializedProperty _sleepTime;
    
        private void OnEnable()
        {
            _infiniteWorking = serializedObject.FindProperty("_infiniteWorking");
            _startWithSleepMode = serializedObject.FindProperty("_startWithSleepMode");
            _waitTimeAtStart = serializedObject.FindProperty("_waitTimeAtStart");
            _workTime = serializedObject.FindProperty("_workTime");
            _sleepTime = serializedObject.FindProperty("_sleepTime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.PropertyField(_infiniteWorking);
            AutomaticLaser laser = (AutomaticLaser)target;
        
            if (laser.InfiniteWorking == false)
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(_startWithSleepMode);
                EditorGUILayout.PropertyField(_waitTimeAtStart);
                EditorGUILayout.PropertyField(_workTime);
                EditorGUILayout.PropertyField(_sleepTime);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}