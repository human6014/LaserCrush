using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SimpleCapture
{
    public class SimpleCapture : EditorWindow
    {
        const int REF_X = 960;
        const int REF_Y = 540;

        // 4k = 3840 x 2160   1080p = 1920 x 1080
        private enum resPreset { _1k = 1, _2k = 2, _4k = 4, _8k = 8, custom = 5 }
        private resPreset res = resPreset._2k;

        private enum camOrientationPreset { Horizon, Vertical }
        private camOrientationPreset camOrientaion = camOrientationPreset.Vertical;

        private Camera camera;
        private bool transparent;        

        Vector2 captureSize = new Vector2(1920, 1080);
        
        [MenuItem("Capture/Capture")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<SimpleCapture>("Simple Capture");
            window.minSize = new Vector2(300, 150);
        }

        void OnGUI()
        {
            // 타이틀.
            GUILayout.Space(7);
            Rect controlRect = EditorGUILayout.GetControlRect();
            GUI.Label(controlRect, "SimpleCapture", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 16, normal = EditorStyles.label.normal });

            // 크레딧.
            GUIContent credit = new GUIContent("by Curiss");
            Vector2 labelSize = EditorStyles.label.CalcSize(credit);
            Rect creditRect = new Rect(controlRect.width - labelSize.x + 10, controlRect.y, labelSize.x, labelSize.y);

            if (GUI.Button(creditRect, credit, new GUIStyle(EditorStyles.label) { alignment = TextAnchor.LowerLeft, fontSize = 10 }))
            {
                Application.OpenURL("https://twitter.com/_Curiss");
            }

            GuiLine();

            // Inspector.
            camera = (Camera)EditorGUILayout.ObjectField("Camera", camera, typeof(Camera), true);
            res = (resPreset)EditorGUILayout.EnumPopup("Resolution", res);

            // 해상도 설정 메뉴.
            if (res != resPreset.custom)
            {
                camOrientaion = (camOrientationPreset)EditorGUILayout.EnumPopup(" ", camOrientaion);

                if (camOrientaion == camOrientationPreset.Horizon)
                {
                    captureSize.x = REF_X * (int)res;
                    captureSize.y = REF_Y * (int)res;
                }
                else
                {
                    captureSize.x = REF_Y * (int)res;
                    captureSize.y = REF_X * (int)res;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Size");
                captureSize = EditorGUILayout.Vector2Field("", captureSize, GUILayout.Width(EditorGUIUtility.currentViewWidth-EditorGUIUtility.labelWidth));
                EditorGUILayout.EndHorizontal();
            }            

            // 배경 투명화 메뉴.
            transparent = EditorGUILayout.Toggle("Transparent", transparent);

            GUILayout.Space(10);

            GUI.Label(EditorGUILayout.GetControlRect(false, 80), 
                "위에 Camera 필드 None일 경우 : \n" +
                "우측에 동그라미 누르면 MainCamera하나만 뜸\n" +
                "그거슬 더블클릭\n" +
                "이후 눈이 가리키는 여기 바로 밑에 Capture누르기\n"+
                "이미지는 Assets/ScreenShot 폴더에 저장댐", 
                new GUIStyle() { alignment = TextAnchor.UpperLeft, fontStyle = FontStyle.Normal, fontSize = 12, normal = EditorStyles.label.normal }
                );

            GUILayout.Space(10);

            // 캡쳐 버튼.
            EditorGUI.BeginDisabledGroup(!camera);
            if (GUILayout.Button("Capture"))
            {
                // 해상도.
                int captureWidth = (int)Mathf.Round(captureSize.x);
                int captureHeight = (int)Mathf.Round(captureSize.y);

                // 경로 적용.
                string path = Application.dataPath + "/ScreenShot/";
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    Directory.CreateDirectory(path);
                }

                // 파일명.
                string name;
                name = path + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

                // 텍스쳐 생성.
                TextureFormat format = transparent ? TextureFormat.ARGB32 : TextureFormat.RGB24;
                Texture2D screenShot = new Texture2D(captureWidth, captureHeight, format, false);
                RenderTexture rt = new RenderTexture(captureWidth, captureHeight, 24);

                // 카메라 백업.
                RenderTexture bak_cam_targetTexture = camera.targetTexture;
                RenderTexture bak_RenderTexture_active = RenderTexture.active;
                CameraClearFlags bak_cam_clearFlags = camera.clearFlags;
                Color bak_cam_background = camera.backgroundColor;                

                // 카메라 설정.
                RenderTexture.active = rt;
                camera.targetTexture = rt;
                if (transparent)
                {
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.backgroundColor = Color.clear;
                }

                // 캡쳐.
                Rect rec = new Rect(0, 0, screenShot.width, screenShot.height);
                camera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
                screenShot.Apply();

                // 카메라 복구.
                camera.targetTexture = bak_cam_targetTexture;
                RenderTexture.active = bak_RenderTexture_active;
                camera.clearFlags = bak_cam_clearFlags;
                camera.backgroundColor = bak_cam_background;

                camera.Render();

                // 저장.
                byte[] bytes = screenShot.EncodeToPNG();
                File.WriteAllBytes(name, bytes);

                AssetDatabase.Refresh();
            }
            EditorGUI.EndDisabledGroup();
        }

        // 경계선.
        void GuiLine(int i_height = 1, int padding = 5)
        {
            GUILayout.Space(padding);
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            rect.height = i_height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            GUILayout.Space(padding);
        }
    }
}