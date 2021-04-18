
using System;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon;

[AddComponentMenu("Udon Sharp/Video/API Video Player")]
public class ApiVideoPlayer : UdonSharpBehaviour
{
    public VRCUnityVideoPlayer unityVideoPlayer;
    public ReaderCamera readerCamera;
    public Renderer screenRenderer;

    RenderTexture videoRenderTex;
    //public Camera readerCamera; // Camera used to read the screen on to a Texture2D object
    //public Texture2D readerTexture; // Texture to parse through

    public VRCUrl[] controllers;
    public VRCUrl[] methods;
    public VRCUrl[] letters;
    public VRCUrl[] digits;
    public VRCUrl[] symbols;

    public VRCUrl currentUrl;
    public VRCUrl[] query;
    public int queryIndex = 0;

    private VRCUrl[] endpoints;

    private string[] exampleRunThroughEndpoints;
    private int exampleRunThroughIndex = 0;

    void Start()
    {
        unityVideoPlayer.Loop = false;
        unityVideoPlayer.Stop();

        videoRenderTex = (RenderTexture)screenRenderer.sharedMaterial.GetTexture("_EmissionMap");

        endpoints = Concate(Concate(Concate(Concate(controllers, methods), letters), digits), symbols);

        exampleRunThroughEndpoints = new string[]
        {
            "users/create/schleepy/john/hippity",
            "users/get/schleepy",
            "users/update/schleepy/richard/hoppity",
            "users/get/schleepy",
            "users/delete/schleepy"
        };
    }

    public void Update()
    {
        // Simple example run through
        if (query == null)
        {
            if (exampleRunThroughIndex < exampleRunThroughEndpoints.Length)
            {
                string currentEndpoint = exampleRunThroughEndpoints[exampleRunThroughIndex++];
                Debug.Log($"Current endpoint: {currentEndpoint}");
                VRCUrl[] playlist = StringToPlaylist(endpoints, currentEndpoint, '/');

                query = playlist;
            }
        }

        if (query != null)
        {
            if (currentUrl != null && queryIndex != 0)
                return;

            if (queryIndex == query.Length)
            {
                query = null;
                queryIndex = 0;
                Debug.Log($"Returned string: {readerCamera.lastRead}");
                return;
            }

            PlayVideo(query[queryIndex]);

            queryIndex++;
        }
    }

    #region Utility
    /// <summary>
    /// Changes a normal URI to an array of accepted VRCUrl objects
    /// </summary>
    /// <example>
    ///     Input: user/get/foo
    ///     Output: { "http://localhost/user", "http://localhost/get", "http://localhost/f", "http://localhost/o", "http://localhost/o" }
    /// </example>
    /// <param name="alphabet">VRCUrl array</param>
    /// <param name="input">the string we want to convert</param>
    /// <param name="delimiter">how is the URI delimited? are we using / or ?</param>
    /// <returns></returns>
    private VRCUrl[] StringToPlaylist(VRCUrl[] alphabet, string input, char delimiter = '/')
    {
        if (!input.EndsWith("]"))
            input += "]";

        VRCUrl[] rtrn = new VRCUrl[0] { };
        VRCUrl delimiterUrl = GetUrlRelatingToInput(alphabet, delimiter.ToString());
        string[] sections = input.Split(delimiter);

        for (int i = 0; i < sections.Length; i++)
        {
            string section = sections[i];
            VRCUrl link = GetUrlRelatingToInput(alphabet, section);

            if (link != null) // section exists
            {
                if (rtrn.Length == 0)
                {
                    rtrn = new VRCUrl[2] { link, delimiterUrl };
                }
                else
                {
                    VRCUrl[] resized = new VRCUrl[rtrn.Length + 2]; // 2, we need 1 for the delimiter
                    Array.Copy(rtrn, 0, resized, 0, rtrn.Length); // simple copy
                    resized[resized.Length - 1] = delimiterUrl; // end with the delimiter
                    resized[resized.Length - 2] = link;
                    rtrn = resized;
                }
            }
            else // no known section exists we gotta iterate through the characters
            {
                int rtrnLength = rtrn.Length;
                VRCUrl[] resized = new VRCUrl[rtrn.Length + section.Length + (i == sections.Length - 1 ? 0 : 1)]; // if we're at the last section we don't need an additional index for a delimiter

                for (int k = 0; k < section.Length; k++)
                {
                    link = GetUrlRelatingToInput(alphabet, section.Substring(k, 1)); // would've just wanted to use the indexer on the string here but Udon doesn't allow indexers on strings what?
                    Array.Copy(rtrn, 0, resized, 0, rtrn.Length);
                    resized[rtrnLength + k] = link;
                    rtrn = resized;
                }

                // if we're not at the last section we add a delimiter
                if (i != sections.Length - 1)
                {
                    rtrn[rtrn.Length - 1] = delimiterUrl;
                }
            }
        }

        return rtrn;
    }

    private VRCUrl GetUrlRelatingToInput(VRCUrl[] alphabet, string input)
    {
        foreach (var item in alphabet)
            if (item.Get().EndsWith($"/{input}"))
                return item;

        return null;
    }

    public VRCUrl[] Concate(VRCUrl[] first, VRCUrl[] second)
    {
        VRCUrl[] concated = new VRCUrl[first.Length + second.Length];

        first.CopyTo(concated, 0);
        second.CopyTo(concated, first.Length);

        return concated;
    }

    #endregion Utility

    #region Video player
    public void PlayVideo(VRCUrl url)
    {
        StopVideo();
        currentUrl = url;
        unityVideoPlayer.LoadURL(url);
    }

    public override void OnVideoReady()
    {
        unityVideoPlayer.Play();
        // do camera magic
    }

    public override void OnVideoStart()
    {
        unityVideoPlayer.SetTime(0f);

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "AllowRender");
    }

    public override void OnVideoEnd()
    {
        StopVideo();
    }

    private void OnDisable()
    {
#if COMPILER_UDONSHARP
            screenRenderer.sharedMaterial.SetTexture("_EmissionMap", videoRenderTex);
#endif
    }

    public void StopVideo()
    {
        unityVideoPlayer.Stop();
        currentUrl = null;
    }

    public override void OnVideoError(VideoError videoError)
    {
        unityVideoPlayer.Stop();
        Debug.LogError("[APIVideo] Video failed: " + currentUrl);

        switch (videoError)
        {
            case VideoError.RateLimited:
                Debug.Log("Rate limited, try again in a few seconds");
                break;
            case VideoError.PlayerError:
                Debug.Log("Video player error");
                break;
            case VideoError.InvalidURL:
                Debug.Log("Invalid URL");
                break;
            case VideoError.AccessDenied:
                Debug.Log("Video blocked, enable untrusted URLs");
                break;
            default:
                Debug.Log("Failed to load video");
                break;
        }
    }

    #endregion Video player

#if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ApiVideoPlayer))]
    internal class USharpVideoPlayerInspector : Editor
    {
        static bool _showUIEndpointsDropdown = false;
        static bool _showUIReferencesDropdown = false;
        static bool _showUISettingsDropdown = false;

        SerializedProperty unityVideoPlayerProperty;

        SerializedProperty screenRendererProperty;

        SerializedProperty readerCameraProperty;

        SerializedProperty readerTextureProperty;

        SerializedProperty readerDelayProperty;

        ReorderableList controllerList, methodList, letterList, digitList, symbolList;

        SerializedProperty controllersProperty, methodsProperty, lettersProperty, digitsProperty, symbolsProperty;

        private void OnEnable()
        {
            unityVideoPlayerProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.unityVideoPlayer));

            screenRendererProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.screenRenderer));

            readerCameraProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.readerCamera));

            //readerTextureProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.readerTexture));

            //readerDelayProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.readerDelay));

            controllersProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.controllers));
            methodsProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.methods));
            lettersProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.letters));
            digitsProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.digits));
            symbolsProperty = serializedObject.FindProperty(nameof(ApiVideoPlayer.symbols));

            // Controllers
            controllerList = new ReorderableList(serializedObject, controllersProperty, true, true, true, true);
            controllerList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, controllerList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            controllerList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Accepted controllers"); };

            // Methods
            methodList = new ReorderableList(serializedObject, methodsProperty, true, true, true, true);
            methodList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, methodList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            methodList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Accepted methods"); };

            // Letters
            letterList = new ReorderableList(serializedObject, lettersProperty, true, true, true, true);
            letterList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, letterList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            letterList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Accepted letters"); };

            // Digits
            digitList = new ReorderableList(serializedObject, digitsProperty, true, true, true, true);
            digitList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, digitList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            digitList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Accepted digits"); };

            // Symbols
            symbolList = new ReorderableList(serializedObject, symbolsProperty, true, true, true, true);
            symbolList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                Rect testFieldRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(testFieldRect, symbolList.serializedProperty.GetArrayElementAtIndex(index), label: new GUIContent());
            };
            symbolList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Accepted symbols"); };
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawConvertToUdonBehaviourButton(target) ||
                UdonSharpGUI.DrawProgramSource(target))
                return;

            EditorGUILayout.PropertyField(unityVideoPlayerProperty);


            EditorGUILayout.Space();
            _showUIEndpointsDropdown = EditorGUILayout.Foldout(_showUIEndpointsDropdown, "Endpoints");

            if (_showUIEndpointsDropdown)
            {
                controllerList.DoLayoutList();
                methodList.DoLayoutList();
                letterList.DoLayoutList();
                digitList.DoLayoutList();
                symbolList.DoLayoutList();
            }

            EditorGUILayout.Space();
            _showUIReferencesDropdown = EditorGUILayout.Foldout(_showUIReferencesDropdown, "Object References");

            if (_showUIReferencesDropdown)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(screenRendererProperty);
                EditorGUILayout.PropertyField(readerCameraProperty);
                //EditorGUILayout.PropertyField(readerTextureProperty);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
