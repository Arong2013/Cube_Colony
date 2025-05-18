using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireDebris))]
    public class RayfireDebrisEditor : Editor
    {
        RayfireDebris debris;
        List<string>  layerNames;

        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        static readonly int space = 3;
        static bool exp_emit;
        static bool exp_dyn;
        static bool exp_noise;
        static bool exp_coll;
        static bool exp_lim;
        static bool exp_rend;
        static bool exp_pool;

        static readonly GUIContent gui_emit_dml     = new GUIContent ("Demolition",     "");
        static readonly GUIContent gui_emit_act     = new GUIContent ("Activation",     "");
        static readonly GUIContent gui_emit_imp     = new GUIContent ("Impact",         "");
        static readonly GUIContent gui_main_ref     = new GUIContent ("Reference",      "");
        static readonly GUIContent gui_main_mat     = new GUIContent ("Material",       "");
        static readonly GUIContent gui_ems_tp       = new GUIContent ("Type",           "");
        static readonly GUIContent gui_ems_am       = new GUIContent ("Amount",         "");
        static readonly GUIContent gui_ems_var      = new GUIContent ("Variation",      "");
        static readonly GUIContent gui_ems_rate     = new GUIContent ("Rate",           "");
        static readonly GUIContent gui_ems_dur      = new GUIContent ("Duration",       "");
        static readonly GUIContent gui_ems_life_min = new GUIContent ("Life Min",       "");
        static readonly GUIContent gui_ems_life_max = new GUIContent ("Life Max",       "");
        static readonly GUIContent gui_ems_size_min = new GUIContent ("Size Min",       "");
        static readonly GUIContent gui_ems_size_max = new GUIContent ("Size Max",       "");
        static readonly GUIContent gui_ems_mat      = new GUIContent ("Material",       "");
        static readonly GUIContent gui_dn_speed_min = new GUIContent ("Speed Min",      "");
        static readonly GUIContent gui_dn_speed_max = new GUIContent ("Speed Max",      "");
        static readonly GUIContent gui_dn_vel_min   = new GUIContent ("Velocity Min",   "");
        static readonly GUIContent gui_dn_vel_max   = new GUIContent ("Velocity Max",   "");
        static readonly GUIContent gui_dn_grav_min  = new GUIContent ("Gravity Min",    "");
        static readonly GUIContent gui_dn_grav_max  = new GUIContent ("Gravity Max",    "");
        static readonly GUIContent gui_dn_rot       = new GUIContent ("Rotation Speed", "");
        static readonly GUIContent gui_ns_en        = new GUIContent ("Enable",         "");
        static readonly GUIContent gui_ns_qual      = new GUIContent ("Quality",        "");
        static readonly GUIContent gui_ns_str_min   = new GUIContent ("Strength Min",   "");
        static readonly GUIContent gui_ns_str_max   = new GUIContent ("Strength Max",   "");
        static readonly GUIContent gui_ns_freq      = new GUIContent ("Frequency",      "");
        static readonly GUIContent gui_ns_scroll    = new GUIContent ("Scroll Speed",   "");
        static readonly GUIContent gui_ns_damp      = new GUIContent ("Damping",        "");
        static readonly GUIContent gui_col_mask     = new GUIContent ("Collides With",  "");
        static readonly GUIContent gui_col_qual     = new GUIContent ("Quality",        "");
        static readonly GUIContent gui_col_rad      = new GUIContent ("Radius Scale",   "");
        static readonly GUIContent gui_col_dmp_tp   = new GUIContent ("Type",           "");
        static readonly GUIContent gui_col_dmp_min  = new GUIContent ("Minimum",        "");
        static readonly GUIContent gui_col_dmp_max  = new GUIContent ("Maximum",        "");
        static readonly GUIContent gui_col_bnc_tp   = new GUIContent ("Type",           "");
        static readonly GUIContent gui_col_bnc_min  = new GUIContent ("Minimum",        "");
        static readonly GUIContent gui_col_bnc_max  = new GUIContent ("Maximum",        "");
        static readonly GUIContent gui_lim_min      = new GUIContent ("Min Particles",  "");
        static readonly GUIContent gui_lim_max      = new GUIContent ("Max Particles",  "");
        static readonly GUIContent gui_lim_vis      = new GUIContent ("Visible",        "Emit debris if emitting object is visible in camera view");
        static readonly GUIContent gui_lim_perc     = new GUIContent ("Percentage",     "");
        static readonly GUIContent gui_lim_size     = new GUIContent ("Size Threshold", "");
        static readonly GUIContent gui_ren_cast     = new GUIContent ("Cast",           "");
        static readonly GUIContent gui_ren_rec      = new GUIContent ("Receive",        "");
        static readonly GUIContent gui_ren_prob     = new GUIContent ("Light Probes",   "");
        static readonly GUIContent gui_ren_vect     = new GUIContent ("Motion Vectors", "");
        static readonly GUIContent gui_ren_t        = new GUIContent ("Set Tag",        "");
        static readonly GUIContent gui_ren_tag      = new GUIContent ("Tag",            "");
        static readonly GUIContent gui_ren_l        = new GUIContent ("Set Layer",      "");
        static readonly GUIContent gui_ren_lay      = new GUIContent ("Layer",          "");
        static readonly GUIContent gui_pl_max       = new GUIContent ("Capacity",       "");
        static readonly GUIContent gui_pl_re        = new GUIContent ("Reuse",          "");
        static readonly GUIContent gui_pl_ov        = new GUIContent ("   Overflow",       "");
        static readonly GUIContent gui_pl_ph        = new GUIContent ("Warmup",         "Create all pool particles in Awake");
        static readonly GUIContent gui_pl_sk        = new GUIContent ("Skip",           "Do not instantiate debris particles if there are no any particles in the pool.");
        static readonly GUIContent gui_pl_rt        = new GUIContent ("Rate",           "Amount of particle systems that will be instantiated in pool every frame");
        static readonly GUIContent gui_pl_id        = new GUIContent ("Id",             "Emitter Pool Id");
        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////

        private void OnEnable()
        {
            if (EditorPrefs.HasKey ("rf_de") == true) exp_emit  = EditorPrefs.GetBool ("rf_de");
            if (EditorPrefs.HasKey ("rf_dd") == true) exp_dyn   = EditorPrefs.GetBool ("rf_dd");
            if (EditorPrefs.HasKey ("rf_dn") == true) exp_noise = EditorPrefs.GetBool ("rf_dn");
            if (EditorPrefs.HasKey ("rf_dc") == true) exp_coll  = EditorPrefs.GetBool ("rf_dc");
            if (EditorPrefs.HasKey ("rf_dl") == true) exp_lim   = EditorPrefs.GetBool ("rf_dl");
            if (EditorPrefs.HasKey ("rf_dr") == true) exp_rend  = EditorPrefs.GetBool ("rf_dr");
            if (EditorPrefs.HasKey ("rf_dp") == true) exp_pool  = EditorPrefs.GetBool ("rf_dp");
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            debris = target as RayfireDebris;
            if (debris == null)
                return;
            
            GUILayout.Space (8);

            UI_Buttons();
            
            GUILayout.Space (space);
            
            UI_Emit();

            GUILayout.Space (space);

            UI_Main();

            GUILayout.Space (space);

            UI_Properties();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Buttons
        /// /////////////////////////////////////////////////////////

        void UI_Buttons()
        {
            GUILayout.BeginHorizontal();

            if (Application.isPlaying == true)
            {
                if (GUILayout.Button ("Emit", GUILayout.Height (25)))
                    foreach (var targ in targets)
                        if (targ as RayfireDebris != null)
                            (targ as RayfireDebris).Emit();

                if (GUILayout.Button ("Clean", GUILayout.Height (25)))
                    foreach (var targ in targets)
                        if (targ as RayfireDebris != null)
                            (targ as RayfireDebris).Clean();
            }

            EditorGUILayout.EndHorizontal();
        }

        /// /////////////////////////////////////////////////////////
        /// Emit
        /// /////////////////////////////////////////////////////////
        
        void UI_Emit()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Emit Event", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            // EditorGUILayout.EnumFlagsField(gui_emit_dml, debris.emission.burstType);

            EditorGUI.BeginChangeCheck();
            bool onDemolition = EditorGUILayout.Toggle (gui_emit_dml, debris.onDemolition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_emit_dml.text);
                foreach (RayfireDebris scr in targets)
                {
                    scr.onDemolition = onDemolition;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool onActivation = EditorGUILayout.Toggle (gui_emit_act, debris.onActivation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_emit_act.text);
                foreach (RayfireDebris scr in targets)
                {
                    scr.onActivation = onActivation;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool onImpact = EditorGUILayout.Toggle (gui_emit_imp, debris.onImpact);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_emit_imp.text);
                foreach (RayfireDebris scr in targets)
                {
                    scr.onImpact = onImpact;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Main
        /// /////////////////////////////////////////////////////////
       
        void UI_Main()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Debris", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            GameObject debrisReference = (GameObject)EditorGUILayout.ObjectField (gui_main_ref, debris.debrisReference, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_main_ref.text);
                foreach (RayfireDebris scr in targets)
                {
                    scr.debrisReference = debrisReference;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            Material debrisMaterial = (Material)EditorGUILayout.ObjectField (gui_main_mat, debris.debrisMaterial, typeof(Material), true);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_main_mat.text);
                foreach (RayfireDebris scr in targets)
                {
                    scr.debrisMaterial = debrisMaterial;
                    SetDirty (scr);
                }
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Properties
        /// /////////////////////////////////////////////////////////
        
        void UI_Properties()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            UI_Pool();
            
            GUILayout.Space (space);
            
            UI_Emission();

            GUILayout.Space (space);

            UI_Dynamic();

            GUILayout.Space (space);

            UI_Noise();

            GUILayout.Space (space);

            UI_Collision();

            GUILayout.Space (space);

            UI_Limitations();
            
            GUILayout.Space (space);

            UI_Rendering();
        }

        /// /////////////////////////////////////////////////////////
        /// Emission
        /// /////////////////////////////////////////////////////////

        void UI_Emission()
        {
            SetFoldoutPref (ref exp_emit, "rf_de", "Emission", true);
            if (exp_emit == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("      Burst", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFParticles.BurstType burstType = (RFParticles.BurstType)EditorGUILayout.EnumPopup (gui_ems_tp, debris.emission.burstType);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_tp.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.burstType = burstType;
                        SetDirty (scr);
                    }
                }
                
                if (debris.emission.burstType != RFParticles.BurstType.None)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int burstAmount = EditorGUILayout.IntSlider (gui_ems_am, debris.emission.burstAmount, 0, 100);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ems_am.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.emission.burstAmount = burstAmount;
                            SetDirty (scr);
                        }
                    }
                    
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int burstVar = EditorGUILayout.IntSlider (gui_ems_var, debris.emission.burstVar, 0, 100);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ems_var.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.emission.burstVar = burstVar;
                            SetDirty (scr);
                        }
                    }
                }
                
                GUILayout.Space (space);
                GUILayout.Label ("      Distance", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float distanceRate = EditorGUILayout.Slider (gui_ems_rate, debris.emission.distanceRate, 0f, 5f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_rate.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.distanceRate = distanceRate;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float duration = EditorGUILayout.Slider (gui_ems_dur, debris.emission.duration, 0.5f, 10);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_dur.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.duration = duration;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Lifetime", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float lifeMin = EditorGUILayout.Slider (gui_ems_life_min, debris.emission.lifeMin, 1f, 60f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_life_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.lifeMin = lifeMin;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float lifeMax = EditorGUILayout.Slider (gui_ems_life_max, debris.emission.lifeMax, 1f, 60f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_life_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.lifeMax = lifeMax;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Size", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float sizeMin = EditorGUILayout.Slider (gui_ems_size_min, debris.emission.sizeMin, 0.001f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_size_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.sizeMin = sizeMin;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float sizeMax = EditorGUILayout.Slider (gui_ems_size_max, debris.emission.sizeMax, 0.1f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ems_size_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emission.sizeMax = sizeMax;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Material", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                Material emissionMaterial = (Material)EditorGUILayout.ObjectField (gui_ems_mat, debris.emissionMaterial, typeof(Material), true);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_ems_mat.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.emissionMaterial = emissionMaterial;
                        SetDirty (scr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Dynamic
        /// /////////////////////////////////////////////////////////
        
        void UI_Dynamic()
        {
            SetFoldoutPref (ref exp_dyn, "rf_dd", "Dynamic", true);
            if (exp_dyn == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("      Speed", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float speedMin = EditorGUILayout.Slider (gui_dn_speed_min, debris.dynamic.speedMin, 0f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_speed_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.speedMin = speedMin;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float speedMax = EditorGUILayout.Slider (gui_dn_speed_max, debris.dynamic.speedMax, 0f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_speed_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.speedMax = speedMax;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Inherit Velocity", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float velocityMin = EditorGUILayout.Slider (gui_dn_vel_min, debris.dynamic.velocityMin, 0f, 3f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_vel_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.velocityMin = velocityMin;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float velocityMax = EditorGUILayout.Slider (gui_dn_vel_max, debris.dynamic.velocityMax, 0f, 3f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_vel_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.velocityMax = velocityMax;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Gravity", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float gravityMin = EditorGUILayout.Slider (gui_dn_grav_min, debris.dynamic.gravityMin, -2f, 2f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_grav_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.gravityMin = gravityMin;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float gravityMax = EditorGUILayout.Slider (gui_dn_grav_max, debris.dynamic.gravityMax, -2f, 2f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_grav_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.gravityMax = gravityMax;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Rotation", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float rotationSpeed = EditorGUILayout.Slider (gui_dn_rot, debris.dynamic.rotationSpeed, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_dn_rot.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.dynamic.rotationSpeed = rotationSpeed;
                        SetDirty (scr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Noise
        /// /////////////////////////////////////////////////////////
        
        void UI_Noise()
        {
            SetFoldoutPref (ref exp_noise, "rf_dn", "Noise", true);
            if (exp_noise == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("      Main", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool enabled = EditorGUILayout.Toggle (gui_ns_en, debris.noise.enabled);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ns_en.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.noise.enabled = enabled;
                        SetDirty (scr);
                    }
                }

                if (debris.noise.enabled == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    ParticleSystemNoiseQuality quality = (ParticleSystemNoiseQuality)EditorGUILayout.EnumPopup (gui_ns_qual, debris.noise.quality);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_qual.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.quality = quality;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);
                    GUILayout.Label ("      Strength", EditorStyles.boldLabel);
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    float strengthMin = EditorGUILayout.Slider (gui_ns_str_min, debris.noise.strengthMin, 0f, 3f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_str_min.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.strengthMin = strengthMin;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float strengthMax = EditorGUILayout.Slider (gui_ns_str_max, debris.noise.strengthMax, 0f, 3f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_str_max.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.strengthMax = strengthMax;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);
                    GUILayout.Label ("      Other", EditorStyles.boldLabel);
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    float frequency = EditorGUILayout.Slider (gui_ns_freq, debris.noise.frequency, 0.001f, 3f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_freq.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.frequency = frequency;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float scrollSpeed = EditorGUILayout.Slider (gui_ns_scroll, debris.noise.scrollSpeed, 0f, 2f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_scroll.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.scrollSpeed = scrollSpeed;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    bool damping = EditorGUILayout.Toggle (gui_ns_damp, debris.noise.damping);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_ns_damp.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.noise.damping = damping;
                            SetDirty (scr);
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Collision
        /// /////////////////////////////////////////////////////////
        
        void UI_Collision()
        {
            SetFoldoutPref (ref exp_coll, "rf_dc", "Collision", true);
            if (exp_coll == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("      Common", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                // Layer mask
                if (layerNames == null)
                {
                    layerNames = new List<string>();
                    for (int i = 0; i <= 31; i++)
                        layerNames.Add (i + ". " + LayerMask.LayerToName (i));
                }

                EditorGUI.BeginChangeCheck();
                int collidesWith = EditorGUILayout.MaskField (gui_col_mask, debris.collision.collidesWith, layerNames.ToArray());
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_col_mask.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.collision.collidesWith = collidesWith;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                ParticleSystemCollisionQuality quality = (ParticleSystemCollisionQuality)EditorGUILayout.EnumPopup (gui_col_qual, debris.collision.quality);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_col_qual.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.collision.quality = quality;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float radiusScale = EditorGUILayout.Slider (gui_col_rad, debris.collision.radiusScale, 0.1f, 2f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_col_rad.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.collision.radiusScale = radiusScale;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Dampen", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFParticleCollisionDebris.RFParticleCollisionMatType dampenType = 
                    (RFParticleCollisionDebris.RFParticleCollisionMatType)EditorGUILayout.EnumPopup (gui_col_dmp_tp, debris.collision.dampenType);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_col_dmp_tp.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.collision.dampenType = dampenType;
                        SetDirty (scr);
                    }
                }

                if (debris.collision.dampenType == RFParticleCollisionDebris.RFParticleCollisionMatType.ByProperties)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float dampenMin = EditorGUILayout.Slider (gui_col_dmp_min, debris.collision.dampenMin, 0f, 1f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_col_dmp_min.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.collision.dampenMin = dampenMin;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float dampenMax = EditorGUILayout.Slider (gui_col_dmp_max, debris.collision.dampenMax, 0f, 1f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_col_dmp_max.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.collision.dampenMax = dampenMax;
                            SetDirty (scr);
                        }
                    }
                }
                
                GUILayout.Space (space);
                GUILayout.Label ("      Bounce", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFParticleCollisionDebris.RFParticleCollisionMatType bounceType = 
                    (RFParticleCollisionDebris.RFParticleCollisionMatType)EditorGUILayout.EnumPopup (gui_col_bnc_tp, debris.collision.bounceType);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_col_bnc_tp.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.collision.bounceType = bounceType;
                        SetDirty (scr);
                    }
                }

                if (debris.collision.bounceType == RFParticleCollisionDebris.RFParticleCollisionMatType.ByProperties)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float bounceMin = EditorGUILayout.Slider (gui_col_bnc_min, debris.collision.bounceMin, 0f, 1f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_col_bnc_min.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.collision.bounceMin = bounceMin;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float bounceMax = EditorGUILayout.Slider (gui_col_bnc_max, debris.collision.bounceMax, 0f, 1f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_col_bnc_max.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.collision.bounceMax = bounceMax;
                            SetDirty (scr);
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Limitations
        /// /////////////////////////////////////////////////////////

        void UI_Limitations()
        {
            SetFoldoutPref (ref exp_lim, "rf_dl", "Limitations", true);
            if (exp_lim == true)
            {
                EditorGUI.indentLevel++;
             
                GUILayout.Space (space);
                GUILayout.Label ("      Particle System", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int minParticles = EditorGUILayout.IntSlider (gui_lim_min, debris.limitations.minParticles, 3, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_min.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.limitations.minParticles = minParticles;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int maxParticles = EditorGUILayout.IntSlider (gui_lim_max, debris.limitations.maxParticles, 5, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.limitations.maxParticles = maxParticles;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool visible = EditorGUILayout.Toggle (gui_lim_vis, debris.limitations.visible);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_vis.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.limitations.visible = visible;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                GUILayout.Label ("      Fragments", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int percentage = EditorGUILayout.IntSlider (gui_lim_perc, debris.limitations.percentage, 10, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_perc.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.limitations.percentage = percentage;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float sizeThreshold = EditorGUILayout.Slider (gui_lim_size, debris.limitations.sizeThreshold, 0.05f, 5);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_size.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.limitations.sizeThreshold = sizeThreshold;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Rendering
        /// /////////////////////////////////////////////////////////

        void UI_Rendering()
        {
            SetFoldoutPref (ref exp_rend, "rf_dr", "Rendering", true);
            if (exp_rend == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("      Shadows", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool castShadows = EditorGUILayout.Toggle (gui_ren_cast, debris.rendering.castShadows);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_cast.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.castShadows = castShadows;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool receiveShadows = EditorGUILayout.Toggle (gui_ren_rec, debris.rendering.receiveShadows);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_rec.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.receiveShadows = receiveShadows;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("      Other", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                LightProbeUsage lightProbes = (LightProbeUsage)EditorGUILayout.EnumPopup (gui_ren_prob, debris.rendering.lightProbes);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_prob.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.lightProbes = lightProbes;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                MotionVectorGenerationMode motionVectors = (MotionVectorGenerationMode)EditorGUILayout.EnumPopup (gui_ren_vect, debris.rendering.motionVectors);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_vect.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.motionVectors = motionVectors;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool t = EditorGUILayout.Toggle (gui_ren_t, debris.rendering.t);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_t.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.t = t;
                        SetDirty (scr);
                    }
                }

                if (debris.rendering.t == true)
                {
                    GUILayout.Space (space);
                    
                    EditorGUI.indentLevel++;
                    
                    EditorGUI.BeginChangeCheck();
                    string tag = EditorGUILayout.TagField (gui_ren_tag, debris.rendering.tag);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_ren_tag.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.rendering.tag = tag;
                            SetDirty (scr);
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool l = EditorGUILayout.Toggle (gui_ren_l, debris.rendering.l);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ren_l.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.rendering.l = l;
                        SetDirty (scr);
                    }
                }
                
                if (debris.rendering.l == true)
                {
                    GUILayout.Space (space);
                    
                    EditorGUI.indentLevel++;
                    
                    EditorGUI.BeginChangeCheck();
                    int layer = EditorGUILayout.LayerField (gui_ren_lay, debris.rendering.layer);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_ren_lay.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.rendering.layer = layer;
                            SetDirty (scr);
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Pool
        /// /////////////////////////////////////////////////////////

        void UI_Pool()
        {
            SetFoldoutPref (ref exp_pool, "rf_dp", "Pool", true);
            if (exp_pool == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int id = EditorGUILayout.IntSlider (gui_pl_id, debris.pool.id, 0, 99);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_id.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.id = id;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool enable = EditorGUILayout.Toggle (gui_ns_en, debris.pool.enable);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_ns_en.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.Enable = enable;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool warmup = EditorGUILayout.Toggle (gui_pl_ph, debris.pool.warmup);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_ph.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.warmup = warmup;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int cap = EditorGUILayout.IntSlider (gui_pl_max, debris.pool.cap, 3, 300);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_max.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.Cap = cap;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int rate = EditorGUILayout.IntSlider (gui_pl_rt, debris.pool.rate, 1, 15);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_rt.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.Rate = rate;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool skip = EditorGUILayout.Toggle (gui_pl_sk, debris.pool.skip);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_sk.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.Skip = skip;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool reuse = EditorGUILayout.Toggle (gui_pl_re, debris.pool.reuse);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_pl_re.text);
                    foreach (RayfireDebris scr in targets)
                    {
                        scr.pool.Reuse = reuse;
                        SetDirty (scr);
                    }
                }

                if (debris.pool.reuse == true)
                {
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    int over = EditorGUILayout.IntSlider (gui_pl_ov, debris.pool.over, 0, 10);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_pl_ov.text);
                        foreach (RayfireDebris scr in targets)
                        {
                            scr.pool.Over = over;
                            SetDirty (scr);
                        }
                    }
                }

                // Caption
                if (debris.pool.enable == true && Application.isPlaying == true)
                {
                    GUILayout.Space (space);
                    
                    if (debris.pool.emitter != null)
                        GUILayout.Label ("     Available : " + debris.pool.emitter.queue.Count, EditorStyles.boldLabel);
                }
                
                // Edit
                if (Application.isPlaying == true)
                {
                    GUILayout.Space (space);
                    
                    if (GUILayout.Button ("Edit Emitter Particles", GUILayout.Height (20)))
                        foreach (var targ in targets)
                            if (targ as RayfireDebris != null)
                                (targ as RayfireDebris).EditEmitterParticles();
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty (RayfireDebris scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
            }
        }
        
        void SetFoldoutPref (ref bool val, string pref, string caption, bool state) 
        {
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.Foldout (val, caption, state);
            if (EditorGUI.EndChangeCheck() == true)
                EditorPrefs.SetBool (pref, val);
        }
    }
}