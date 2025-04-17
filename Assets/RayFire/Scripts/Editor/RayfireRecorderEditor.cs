using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireRecorder))]
    public class RayfireRecorderEditor : Editor
    {
        // Target
        RayfireRecorder recorder;
        string          rec   = "Recording...   ";
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static int      space = 3;

        static GUIContent gui_recordStart    = new GUIContent ("On Start",         "Automatically start recording at Start.");
        static GUIContent gui_recordClip     = new GUIContent ("Clip Name",        "");
        static GUIContent gui_recordDuration = new GUIContent ("Duration",         "Maximum duration for recorded animation clip.");
        static GUIContent gui_recordRate     = new GUIContent ("Rate",             "Amount of keys per second.");
        static GUIContent gui_recordReduce   = new GUIContent ("Reduce Keys",      "Optimize amount of keys for still objects.");
        static GUIContent gui_recordThresh   = new GUIContent ("Threshold",        "Reduce Keys threshold.");
        static GUIContent gui_recordStr      = new GUIContent ("Start Record",     "");
        static GUIContent gui_recordStop     = new GUIContent ("Stop Record",      "");
        static GUIContent gui_playStart      = new GUIContent ("On Start",         "Automatically start playback at Start.");
        static GUIContent gui_playClip       = new GUIContent ("Clip",             "");
        static GUIContent gui_playCont       = new GUIContent ("Controller",       "");
        static GUIContent gui_playKin        = new GUIContent ("Set To Kinematic", "");
        static GUIContent gui_playStr        = new GUIContent ("Start Play", "");
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            // Get target
            recorder = target as RayfireRecorder;
            if (recorder == null)
                return;

            GUILayout.Space (8);

            UI_Mode();
            
            GUILayout.Space (space);

            if (recorder.mode == RayfireRecorder.AnimatorType.Record)
                UI_Record();
            if (recorder.mode == RayfireRecorder.AnimatorType.Play)
                UI_Play();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Mode
        /// /////////////////////////////////////////////////////////
        
        void UI_Mode()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Mode", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRecorder.AnimatorType mode = (RayfireRecorder.AnimatorType)EditorGUILayout.EnumPopup (" ", recorder.mode);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Recorder Mode");
                foreach (RayfireRecorder scr in targets)
                {
                    scr.mode = mode;
                    SetDirty (scr);
                }
            }
            
            SceneView.RepaintAll();
        }
        
        /// /////////////////////////////////////////////////////////
        /// Record
        /// /////////////////////////////////////////////////////////
        
        void UI_Record()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            UI_RecordStart();
            UI_RecordButtons();
            
            GUILayout.Space (space);
                
            UI_RecordClip();

            GUILayout.Space (space);

            UI_RecordDuration();
            
            GUILayout.Space (space);

            UI_RecordRate();
            
            GUILayout.Space (space);

            UI_RecordReduce();

            if (recorder.reduceKeys == true)
            {
                GUILayout.Space (space);

                UI_RecordThresh();
            }
        }
        
        void UI_RecordStart()
        {
            if (Application.isPlaying == false)
            {
                EditorGUI.BeginChangeCheck();
                bool recordOnStart = EditorGUILayout.Toggle (gui_recordStart, recorder.recordOnStart);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_recordStart.text);
                    foreach (RayfireRecorder scr in targets)
                    {
                        scr.recordOnStart = recordOnStart;
                        SetDirty (scr);
                    }
                }
            }

            if (Application.isPlaying == true && recorder.recordOnStart == true)
            {
                if (recorder.recorder == true)
                {
                    if (GUILayout.Button (gui_recordStop, GUILayout.Height (25)))
                        recorder.StopRecord();
                    
                    GUILayout.Space (space);
                    GUILayout.Label (rec + recorder.recordedTime.ToString("N1") + "/" + recorder.duration);
                }
            }
        }
        
        void UI_RecordButtons()
        {
            if (Application.isPlaying == true && recorder.recordOnStart == false)
            {
                GUILayout.BeginHorizontal();
                if (recorder.recorder == false)
                    if (GUILayout.Button (gui_recordStr, GUILayout.Height (25)))
                        recorder.StartRecord();
                if (recorder.recorder == true)
                    if (GUILayout.Button (gui_recordStop, GUILayout.Height (25)))
                        recorder.StopRecord();
                EditorGUILayout.EndHorizontal();
            }
            
            if (Application.isPlaying == true && recorder.recordOnStart == false)
            {
                if (recorder.recorder == true)
                {
                    GUILayout.Space (space);
                    GUILayout.Label (rec + recorder.recordedTime.ToString("N1") + "/" + recorder.duration);
                }
            }
        }
        
        void UI_RecordClip()
        {
            EditorGUI.BeginChangeCheck();
            recorder.clipName = EditorGUILayout.TextField (gui_recordClip, recorder.clipName);
            if (EditorGUI.EndChangeCheck())
                SetDirty (recorder);
        }
        
        void UI_RecordDuration()
        {
            EditorGUI.BeginChangeCheck();
            float duration = EditorGUILayout.Slider (gui_recordDuration, recorder.duration, 1f, 60f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_recordDuration.text);
                foreach (RayfireRecorder scr in targets)
                {
                    scr.duration = duration;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_RecordRate()
        {
            EditorGUI.BeginChangeCheck();
            int rate = EditorGUILayout.IntSlider (gui_recordRate, recorder.rate, 1, 60);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_recordRate.text);
                foreach (RayfireRecorder scr in targets)
                {
                    scr.rate = rate;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_RecordReduce()
        {
            EditorGUI.BeginChangeCheck();
            bool reduceKeys = EditorGUILayout.Toggle (gui_recordReduce, recorder.reduceKeys);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_recordReduce.text);
                foreach (RayfireRecorder scr in targets)
                {
                    scr.reduceKeys = reduceKeys;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_RecordThresh()
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            float threshold = EditorGUILayout.Slider (gui_recordThresh, recorder.threshold, 0f, 0.05f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_recordThresh.text);
                foreach (RayfireRecorder scr in targets)
                {
                    scr.threshold = threshold;
                    SetDirty (scr);
                }
            }
            
            EditorGUI.indentLevel--;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Play
        /// /////////////////////////////////////////////////////////
        
        void UI_Play()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            UI_PlayStart();
            UI_PlayButtons();
            
            GUILayout.Space (space);

            UI_PlayClip();
            
            GUILayout.Space (space);

            UI_PlayCont();
            
            GUILayout.Space (space);

            UI_PlayKin();
        }
        
        void UI_PlayStart()
        {
            if (Application.isPlaying == false)
            {
                EditorGUI.BeginChangeCheck();
                bool playOnStart = EditorGUILayout.Toggle (gui_playStart, recorder.playOnStart);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_playStart.text);
                    foreach (RayfireRecorder scr in targets)
                    {
                        scr.playOnStart = playOnStart;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_PlayButtons()
        {
            if (Application.isPlaying == true && recorder.playOnStart == false && recorder.recorder == false)
                if (GUILayout.Button (gui_playStr, GUILayout.Height (25)))
                    recorder.StartPlay();
        }
        
        void UI_PlayClip()
        {
            EditorGUI.BeginChangeCheck();
            recorder.animationClip = (AnimationClip)EditorGUILayout.ObjectField (gui_playClip, recorder.animationClip, typeof(AnimationClip), true);
            if (EditorGUI.EndChangeCheck() == true)
                SetDirty (recorder);
        }
        
        void UI_PlayCont()
        {
            EditorGUI.BeginChangeCheck();
            recorder.controller = (RuntimeAnimatorController)EditorGUILayout.ObjectField (gui_playCont, recorder.controller, typeof(RuntimeAnimatorController), true);
            if (EditorGUI.EndChangeCheck() == true)
                SetDirty (recorder);
        }
        
        void UI_PlayKin()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Rayfire Rigid", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRecorder.RigidActionType rigidAction = (RayfireRecorder.RigidActionType)EditorGUILayout.EnumPopup ("Action", recorder.rigidAction);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, "Action");
                foreach (RayfireRecorder scr in targets)
                {
                    scr.rigidAction = rigidAction;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireRecorder scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
                SceneView.RepaintAll();
            }
        }
    }
}