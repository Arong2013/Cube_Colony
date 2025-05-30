﻿using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireRigidRoot))]
    public class RayfireRigidRootEditor : Editor
    {
        RayfireRigidRoot root;

        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////

        static int space = 3;
        
        static bool exp_phy;
        static bool exp_act;
        static bool exp_lim;
        static bool exp_cls;
        static bool exp_clp;
        static bool exp_fade;
        static bool exp_res;
        
        static GUIContent gui_mn_ini = new GUIContent ("Initialization", "");

        static GUIContent gui_props     = new GUIContent ("Properties",         "");
        static GUIContent gui_mn_sim    = new GUIContent ("Simulation Type",    "Defines behaviour of object during simulation.");
        static GUIContent gui_phy       = new GUIContent ("Physics",            "Defines all physics properties for simulated object.");
        static GUIContent gui_phy_mtp   = new GUIContent ("Type",               "Material preset with predefined density, friction, elasticity and solidity. Can be edited in Rayfire Man component.");
        static GUIContent gui_phy_mat   = new GUIContent ("Material",           "Allows to define own Physic Material.");
        static GUIContent gui_phy_mby   = new GUIContent ("Mass By",            "");
        static GUIContent gui_phy_mss   = new GUIContent ("Mass",               "Mass which will be applied to object if Mass By set to By Mass Property.");
        static GUIContent gui_phy_ctp   = new GUIContent ("Type",               "");
        static GUIContent gui_phy_pln   = new GUIContent ("Planar Check",       "Do not add Mesh Collider to objects with planar low poly mesh.");
        static GUIContent gui_phy_ign   = new GUIContent ("Ignore Near",        "");
        static GUIContent gui_phy_grv   = new GUIContent ("Use Gravity",        "Enables gravity for simulated object.");
        static GUIContent gui_phy_slv   = new GUIContent ("Solver Iterations",  "");
        static GUIContent gui_phy_slt   = new GUIContent ("Sleeping Threshold", "");
        static GUIContent gui_phy_dmp   = new GUIContent ("Dampening",          "Multiplier for demolished fragments velocity.");
        static GUIContent gui_act       = new GUIContent ("Activation",         "Allows to activate ( make dynamic ) inactive and kinematic objects.");
        static GUIContent gui_act_loc   = new GUIContent ("Local",              "Activation By Local Offset relative to parent.");
        static GUIContent gui_act_ofs   = new GUIContent ("Offset",             "Inactive object will be activated if will be pushed from it's original position farther than By Offset value.");
        static GUIContent gui_act_vel   = new GUIContent ("Velocity",           "Inactive object will be activated when it's velocity will be higher than By Velocity value when pushed by other dynamic objects.");
        static GUIContent gui_act_dmg   = new GUIContent ("Damage",             "Inactive object will be activated if will get total damage higher than this value.");
        static GUIContent gui_act_act   = new GUIContent ("Activator",          "Inactive object will be activated by overlapping with object with RayFire Activator component.");
        static GUIContent gui_act_imp   = new GUIContent ("Impact",             "Inactive object will be activated when it will be shot by RayFireGun component.");
        static GUIContent gui_act_con   = new GUIContent ("Connectivity",       "Inactive object will be activated by Connectivity component if it will not be connected with Unyielding zone.");
        static GUIContent gui_act_uny   = new GUIContent ("Unyielding",         "Allows to define Inactive/Kinematic object as Unyielding to check for connection with other Inactive/Kinematic objects with enabled By Connectivity activation type.");
        static GUIContent gui_act_acd   = new GUIContent ("Activatable",        "Unyielding object can not be activate by default. When On allows to activate Unyielding objects as well.");
        static GUIContent gui_act_l     = new GUIContent ("Change Layer",       "Change layer for activated objects.");
        static GUIContent gui_act_lay   = new GUIContent ("Layer",              "Custom layer for activated objects.");
        static GUIContent gui_lim       = new GUIContent ("Limitations",        "");
        static GUIContent gui_lim_col   = new GUIContent ("By Collision",       "Enables demolition by collision.");
        static GUIContent gui_lim_sol   = new GUIContent ("Solidity",           "Local Object solidity multiplier for object. Low Solidity makes object more fragile at collision.");
        static GUIContent gui_lim_tag   = new GUIContent ("Tag",                "Object will be demolished only if it will collide with other objects with defined Tag.");
        static GUIContent gui_lim_dep   = new GUIContent ("Depth",              "Defines how deep object can be demolished. Depth is limitless if set to 0.");
        static GUIContent gui_lim_tim   = new GUIContent ("Time",               "Safe time. Measures in seconds and allows to prevent fragments from being demolished right after they were just created.");
        static GUIContent gui_lim_siz   = new GUIContent ("Size",               "Prevent objects with bounding box size less than defined value to be demolished.");
        static GUIContent gui_lim_vis   = new GUIContent ("Visible",            "Object will be demolished only if it is visible to any camera including scene camera.");
        static GUIContent gui_lim_slc   = new GUIContent ("Slice By Blade",     "Allows object to be sliced by object with RayFire Blade component.");
        static GUIContent gui_cls       = new GUIContent ("Cluster Demolition", "");
        static GUIContent gui_cls_conn  = new GUIContent ("Connectivity",       "Defines Connectivity algorithm for clusters.");
        static GUIContent gui_cls_fl_ar = new GUIContent ("Minimum Area",       "Two shards will have connection if their shared area is bigger than this value.");
        static GUIContent gui_cls_fl_sz = new GUIContent ("Minimum Size",       "Two shards will have connection if their size is bigger than this value.");
        static GUIContent gui_cls_fl_pr = new GUIContent ("Percentage",         "Random percentage of connections will be discarded.");
        static GUIContent gui_cls_fl_sd = new GUIContent ("Seed",               "Seed for random percentage filter and for Random Collapse.");
        static GUIContent gui_cls_ds_tp = new GUIContent ("Type",               "");
        static GUIContent gui_cls_ds_rt = new GUIContent ("Ratio",              "Defines demolition distance from contact point in percentage relative to object's size.");
        static GUIContent gui_cls_ds_un = new GUIContent ("Units",              "Defines demolition distance from contact point in world units.");
        static GUIContent gui_cls_sh_ar = new GUIContent ("Area",               "");
        static GUIContent gui_cls_sh_dm = new GUIContent ("Demolition",         "");
        static GUIContent gui_cls_min   = new GUIContent ("Minimum",            "");
        static GUIContent gui_cls_max   = new GUIContent ("Maximum",            "");
        static GUIContent gui_cls_dml   = new GUIContent ("Demolishable",       "");
        static GUIContent gui_clp_type  = new GUIContent ("Type", " By Area: Shard will loose it's connections if it's shared area surface is less then defined value.\n" + 
                                                                 " By Size: Shard will loose it's connections if it's Size is less then defined value.\n" + 
                                                                 " Random: Shard will loose it's connections if it's random value in range from 0 to 100 is less then defined value.");
        static GUIContent gui_clp_str    = new GUIContent ("Start",         "Defines start value in percentage relative to whole range of picked type.");
        static GUIContent gui_clp_end    = new GUIContent ("End",           "Defines end value in percentage relative to whole range of picked type.");
        static GUIContent gui_clp_step   = new GUIContent ("Steps",         "Amount of times when defined threshold value will be set during Duration period.");
        static GUIContent gui_clp_dur    = new GUIContent ("Duration",      "Time which it will take Start value to be increased to End value.");
        static GUIContent gui_fade       = new GUIContent ("Fading",        "");
        static GUIContent gui_fade_dml   = new GUIContent ("On Demolition", "");
        static GUIContent gui_fade_act   = new GUIContent ("On Activation", "");
        static GUIContent gui_fade_ofs   = new GUIContent ("By Offset",     "");
        static GUIContent gui_fade_tp    = new GUIContent ("Type",          "");
        static GUIContent gui_fade_tm    = new GUIContent ("Time",          "Fade duration time.");
        static GUIContent gui_fade_lf_tp = new GUIContent ("Type",          "");
        static GUIContent gui_fade_lf_tm = new GUIContent ("Time",          "Time which object will be simulated before start to fade.");
        static GUIContent gui_fade_lf_vr = new GUIContent ("Variation",     "");
        static GUIContent gui_fade_sz    = new GUIContent ("Size",          "Fade won't affect objects with size bigger than this value. Disabled if set to 0.");
        static GUIContent gui_fade_sh    = new GUIContent ("Shards",        "Fade won't affect Connected clusters with shard amount bigger than this value. Disabled if set to 0.");
        static GUIContent gui_res        = new GUIContent ("Reset",         "");
        static GUIContent gui_res_tm     = new GUIContent ("Transform",     "Reset transform to position and rotation when object was initialized.");
        static GUIContent gui_res_dm     = new GUIContent ("Damage",        "Reset damage value.");
        static GUIContent gui_res_cn     = new GUIContent ("Connectivity",  "Reset Connectivity.");
        static GUIContent gui_res_ac     = new GUIContent ("Action",        "");
        static GUIContent gui_res_dl     = new GUIContent ("Destroy Delay", "Object will be destroyed after defined delay.");
        static GUIContent gui_res_ms     = new GUIContent ("Mesh",          "");
        static GUIContent gui_res_fr     = new GUIContent ("Fragments",     "");

        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////

        private void OnEnable()
        {
            if (EditorPrefs.HasKey ("rf_tp") == true) exp_phy  = EditorPrefs.GetBool ("rf_tp");
            if (EditorPrefs.HasKey ("rf_ta") == true) exp_act  = EditorPrefs.GetBool ("rf_ta");
            if (EditorPrefs.HasKey ("rf_tl") == true) exp_lim  = EditorPrefs.GetBool ("rf_tl");
            if (EditorPrefs.HasKey ("rf_tc") == true) exp_cls  = EditorPrefs.GetBool ("rf_tc");
            if (EditorPrefs.HasKey ("rf_tp") == true) exp_clp  = EditorPrefs.GetBool ("rf_tp");
            if (EditorPrefs.HasKey ("rf_tf") == true) exp_fade = EditorPrefs.GetBool ("rf_tf");
            if (EditorPrefs.HasKey ("rf_te") == true) exp_res  = EditorPrefs.GetBool ("rf_te");
        }
        
        /// /////////////////////////////////////////////////////////
        /// Simulation
        /// /////////////////////////////////////////////////////////

        void UI_Simulation()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Simulation", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            SimType simulationType = (SimType)EditorGUILayout.EnumPopup (gui_mn_sim, root.simulationType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_mn_sim.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.simulationType = simulationType;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            UI_Physic();

            if (ActivatableState() == false)
                return;
            
            GUILayout.Space (space);

            UI_Activation();
        }
        
        void UI_Physic()
        {
            SetFoldoutPref (ref exp_phy, "rf_tp", gui_phy, true);
            if (exp_phy == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                GUILayout.Label ("  Material", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                MaterialType mt = (MaterialType)EditorGUILayout.EnumPopup (gui_phy_mtp, root.physics.mt);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_mtp.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.mt = mt;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                PhysicsMaterial material = (PhysicsMaterial)EditorGUILayout.ObjectField (gui_phy_mat, root.physics.material, typeof(PhysicsMaterial), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_mat.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.material = material;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                GUILayout.Label ("  Mass", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                MassType massBy = (MassType)EditorGUILayout.EnumPopup (gui_phy_mby, root.physics.massBy);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_mby.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.massBy = massBy;
                        SetDirty (scr);
                    }
                }

                if (root.physics.massBy == MassType.MassProperty)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float mass = EditorGUILayout.Slider (gui_phy_mss, root.physics.mass, 0.1f, 100f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_phy_mss.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.physics.mass = mass;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);
                
                GUILayout.Label ("  Collider", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                RFColliderType ct = (RFColliderType)EditorGUILayout.EnumPopup (gui_phy_ctp, root.physics.ct);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_ctp.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.ct = ct;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool pc = EditorGUILayout.Toggle (gui_phy_pln, root.physics.pc);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_pln.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.pc = pc;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool ine = EditorGUILayout.Toggle (gui_phy_ign, root.physics.ine);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_ign.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.ine = ine;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                
                GUILayout.Label ("  Other", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                bool gr = EditorGUILayout.Toggle (gui_phy_grv, root.physics.gr);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_grv.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.gr = gr;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int si = EditorGUILayout.IntSlider (gui_phy_slv, root.physics.si, 1, 20);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_slv.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.si = si;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float st = EditorGUILayout.Slider (gui_phy_slt, root.physics.st, 0.001f, 0.1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_slt.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.st = st;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                GUILayout.Label ("  Fragments", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                float dm = EditorGUILayout.Slider (gui_phy_dmp, root.physics.dm, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_dmp.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.physics.dm = dm;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        void UI_Activation()
        {
            SetFoldoutPref (ref exp_act, "rf_ta", gui_act, true);
            if (exp_act == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                GUILayout.Label ("  Activation By", EditorStyles.boldLabel);
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float off = EditorGUILayout.Slider (gui_act_ofs, root.activation.off, 0, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_act_ofs.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.off = off;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                if (root.activation.off > 0)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUI.BeginChangeCheck();
                    bool loc = EditorGUILayout.Toggle (gui_act_loc, root.activation.loc);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_loc.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.activation.loc = loc;
                            SetDirty (scr);
                        }
                    }
                
                    GUILayout.Space (space);
                    
                    EditorGUI.indentLevel--;
                }

                EditorGUI.BeginChangeCheck();
                float vel = EditorGUILayout.Slider (gui_act_vel, root.activation.vel, 0, 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_vel.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.vel = vel;
                        SetDirty (scr);
                    }
                }

                /*
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                root.activation.byDamage = EditorGUILayout.Slider (gui_act_dmg, root.activation.byDamage, 0, 100f);
                if (EditorGUI.EndChangeCheck() == true)
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.byDamage = root.activation.byDamage;
                        SetDirty (scr);
                    }
                
                */
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool act = EditorGUILayout.Toggle (gui_act_act, root.activation.act);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_act.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.act = act;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool imp = EditorGUILayout.Toggle (gui_act_imp, root.activation.imp);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_imp.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.imp = imp;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool con = EditorGUILayout.Toggle (gui_act_con, root.activation.con);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_con.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.con = con;
                        SetDirty (scr);
                    }
                }

                if (root.activation.con == true)
                {
                    EditorGUI.indentLevel++;
                    
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    bool uny = EditorGUILayout.Toggle (gui_act_uny, root.activation.uny);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_uny.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.activation.uny = uny;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    bool atb = EditorGUILayout.Toggle (gui_act_acd, root.activation.atb);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_acd.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.activation.atb = atb;
                            SetDirty (scr);
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                
                GUILayout.Space (space);

                GUILayout.Label ("  Post Activation", EditorStyles.boldLabel);
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool l = EditorGUILayout.Toggle (gui_act_l, root.activation.l);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_l.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.activation.l = l;
                        SetDirty (scr);
                    }
                }
                
                if (root.activation.l == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int lay = EditorGUILayout.LayerField (gui_act_lay, root.activation.lay);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_lay.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.activation.lay = lay;
                            SetDirty (scr);
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        bool ActivatableState()
        {
            foreach (RayfireRigidRoot scr in targets)
                if (ActivatableState(scr) == true)
                    return true;
            return false;
        }
        
        static bool ActivatableState(RayfireRigidRoot scr)
        {
            if (scr.simulationType == SimType.Inactive || scr.simulationType == SimType.Kinematic)
                return true;
            return false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Demolition
        /// /////////////////////////////////////////////////////////

        void UI_Demolition()
        {
            if (DemolitionState() == false)
                return;
            
            GUILayout.Space (space);
            GUILayout.Label ("  Demolition", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            UI_Limitations();
            
            GUILayout.Space (space);
            
            UI_Cluster();
        }
        
        bool DemolitionState()
        {
            foreach (RayfireRigidRoot scr in targets)
                if (DemolitionState(scr) == true)
                    return true;
            return false;
        }

        static bool DemolitionState(RayfireRigidRoot scr)
        {
            if (scr.simulationType == SimType.Inactive || scr.simulationType == SimType.Kinematic)
                if (scr.activation.con == true)
                    return true;
            return false;
        }

        /// /////////////////////////////////////////////////////////
        /// Limitations
        /// /////////////////////////////////////////////////////////

        void UI_Limitations()
        {
            SetFoldoutPref (ref exp_lim, "rf_tl", gui_lim, true);
            if (exp_lim == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                
                GUILayout.Label ("  Collision", EditorStyles.boldLabel);
                
                EditorGUI.BeginChangeCheck();
                bool col = EditorGUILayout.Toggle (gui_lim_col, root.demolition.limitations.col);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_col.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.col = col;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                if (root.demolition.limitations.col == true)
                {
                    EditorGUI.BeginChangeCheck();
                    float sol = EditorGUILayout.Slider (gui_lim_sol, root.demolition.limitations.sol, 0, 10f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_lim_sol.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.demolition.limitations.sol = sol;
                            SetDirty (scr);
                        }
                    }
                    
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    string tag = EditorGUILayout.TagField (gui_lim_tag, root.demolition.limitations.tag);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_lim_tag.text);
                        foreach (RayfireRigidRoot scr in targets)
                        {
                            scr.demolition.limitations.tag = tag;
                            SetDirty (scr);
                        }
                    }
                }
                
                GUILayout.Label ("  Other", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();
                int depth = EditorGUILayout.IntSlider (gui_lim_dep, root.demolition.limitations.depth, 0, 7);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_dep.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.depth = depth;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float time = EditorGUILayout.Slider (gui_lim_tim, root.demolition.limitations.time, 0.05f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_tim.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.time = time;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float size = EditorGUILayout.Slider (gui_lim_siz, root.demolition.limitations.size, 0.01f, 5f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_siz.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.size = size;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool vis = EditorGUILayout.Toggle (gui_lim_vis, root.demolition.limitations.vis);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_vis.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.vis = vis;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool bld = EditorGUILayout.Toggle (gui_lim_slc, root.demolition.limitations.bld);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_slc.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.limitations.bld = bld;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster
        /// /////////////////////////////////////////////////////////

        void UI_Cluster()
        {
            SetFoldoutPref (ref exp_cls, "rf_tc", gui_cls, true);
            if (exp_cls == true)
            {
                EditorGUI.indentLevel++;
                
                UI_Cluster_Props();
                
                GUILayout.Space (space);

                UI_Cluster_Filters();
                
                GUILayout.Space (space);

                UI_Cluster_Dist();
                
                GUILayout.Space (space);

                UI_Cluster_Shard();
                
                GUILayout.Space (space);

                UI_Cluster_Cls();
                
                GUILayout.Space (space);

                UI_Cluster_Collapse();
                
                EditorGUI.indentLevel--;
            }
        }

        void UI_Cluster_Props()
        {
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            ConnectivityType connectivity = (ConnectivityType)EditorGUILayout.EnumPopup (gui_cls_conn, root.demolition.clusterDemolition.connectivity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_conn.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.connectivity = connectivity;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Filters () 
        {
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            
            if (root.demolition.clusterDemolition.connectivity != ConnectivityType.ByBoundingBox)
            {
                EditorGUI.BeginChangeCheck();
                float minimumArea = EditorGUILayout.Slider (gui_cls_fl_ar, root.demolition.clusterDemolition.minimumArea, 0, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_fl_ar.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.minimumArea = minimumArea;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
            }

            EditorGUI.BeginChangeCheck();
            float minimumSize = EditorGUILayout.Slider (gui_cls_fl_sz, root.demolition.clusterDemolition.minimumSize, 0, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_sz.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.minimumSize = minimumSize;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int percentage = EditorGUILayout.IntSlider (gui_cls_fl_pr, root.demolition.clusterDemolition.percentage, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_pr.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.percentage = percentage;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int seed = EditorGUILayout.IntSlider (gui_cls_fl_sd, root.demolition.clusterDemolition.seed, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_sd.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.seed = seed;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Dist()
        {
            GUILayout.Label ("  Demolition Distance", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            RFDemolitionCluster.RFDetachType type = (RFDemolitionCluster.RFDetachType)EditorGUILayout.EnumPopup (gui_cls_ds_tp, root.demolition.clusterDemolition.type);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_ds_tp.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.type = type;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            if (root.demolition.clusterDemolition.type == RFDemolitionCluster.RFDetachType.RatioToSize)
            {
                EditorGUI.BeginChangeCheck();
                int ratio = EditorGUILayout.IntSlider (gui_cls_ds_rt, root.demolition.clusterDemolition.ratio, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_ds_rt.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.ratio = ratio;
                        SetDirty (scr);
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                float units = EditorGUILayout.Slider (gui_cls_ds_un, root.demolition.clusterDemolition.units, 0, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_ds_un.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.units = units;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_Cluster_Shard()
        {
            GUILayout.Label ("  Shards", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int shardArea = EditorGUILayout.IntSlider (gui_cls_sh_ar, root.demolition.clusterDemolition.shardArea, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_sh_ar.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.shardArea = shardArea;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool shardDemolition = EditorGUILayout.Toggle (gui_cls_sh_dm, root.demolition.clusterDemolition.shardDemolition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_sh_dm.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.shardDemolition = shardDemolition;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Cluster_Cls()
        {
            GUILayout.Label ("  Clusters", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int minAmount = EditorGUILayout.IntSlider (gui_cls_min, root.demolition.clusterDemolition.minAmount, 2, 20);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_min.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.minAmount = minAmount;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int maxAmount = EditorGUILayout.IntSlider (gui_cls_max, root.demolition.clusterDemolition.maxAmount, 2, 20);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_max.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.maxAmount = maxAmount;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool demolishable = EditorGUILayout.Toggle (gui_cls_dml, root.demolition.clusterDemolition.demolishable);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_dml.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.demolition.clusterDemolition.demolishable = demolishable;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Collapse()
        {
            GUILayout.Label ("  Collapse", EditorStyles.boldLabel);

            SetFoldoutPref (ref exp_clp, "rf_tp", gui_props, true);
            if (exp_clp == true)
            {
                GUILayout.Space (space);

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                RFCollapse.RFCollapseType type = (RFCollapse.RFCollapseType)EditorGUILayout.EnumPopup (gui_clp_type, root.demolition.clusterDemolition.collapse.type);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_type.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.collapse.type = type;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int start = EditorGUILayout.IntSlider (gui_clp_str, root.demolition.clusterDemolition.collapse.start, 0, 99);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_str.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.collapse.start = start;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int end = EditorGUILayout.IntSlider (gui_clp_end, root.demolition.clusterDemolition.collapse.end, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_end.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.collapse.end = end;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int steps = EditorGUILayout.IntSlider (gui_clp_step, root.demolition.clusterDemolition.collapse.steps, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_step.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.collapse.steps = steps;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float duration = EditorGUILayout.Slider (gui_clp_dur, root.demolition.clusterDemolition.collapse.duration, 0, 60f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_dur.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.demolition.clusterDemolition.collapse.duration = duration;
                        SetDirty (scr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Fade
        /// /////////////////////////////////////////////////////////

        void UI_Fade()
        {
            SetFoldoutPref (ref exp_fade, "rf_tf", gui_fade, true);
            if (exp_fade == true)
            {
                EditorGUI.indentLevel++;

                UI_Fade_Init();
                
                GUILayout.Space (space);

                UI_Fade_Type();
                
                GUILayout.Space (space);
                
                UI_Fade_Life();
                
                GUILayout.Space (space);

                UI_Fade_Filt();

                EditorGUI.indentLevel--;
            }
        }

        void UI_Fade_Init()
        {
            GUILayout.Label ("  Initiate", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            bool onDemolition = EditorGUILayout.Toggle (gui_fade_dml, root.fading.onDemolition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_dml.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.onDemolition = onDemolition;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool onActivation = EditorGUILayout.Toggle (gui_fade_act, root.fading.onActivation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_act.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.onActivation = onActivation;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float byOffset = EditorGUILayout.Slider (gui_fade_ofs, root.fading.byOffset, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_ofs.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.byOffset = byOffset;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Fade_Type()
        {
            GUILayout.Label ("  Type", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            FadeType fadeType = (FadeType)EditorGUILayout.EnumPopup (gui_fade_tp, root.fading.fadeType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_tp.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.fadeType = fadeType;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float fadeTime = EditorGUILayout.Slider (gui_fade_tm, root.fading.fadeTime, 1f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_tm.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.fadeTime = fadeTime;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Fade_Life()
        {
            GUILayout.Label ("  Life", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            RFFadeLifeType lifeType = (RFFadeLifeType)EditorGUILayout.EnumPopup (gui_fade_lf_tp, root.fading.lifeType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_tp.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.lifeType = lifeType;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float lifeTime = EditorGUILayout.Slider (gui_fade_lf_tm, root.fading.lifeTime, 0f, 90f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_tm.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.lifeTime = lifeTime;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float lifeVariation = EditorGUILayout.Slider (gui_fade_lf_vr, root.fading.lifeVariation, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_vr.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.lifeVariation = lifeVariation;
                    SetDirty (scr);
                }
            }
        }

        void UI_Fade_Filt()
        {
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            float sizeFilter = EditorGUILayout.Slider (gui_fade_sz, root.fading.sizeFilter, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_sz.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.sizeFilter = sizeFilter;
                    SetDirty (scr);
                }
            }
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int shardAmount = EditorGUILayout.IntSlider (gui_fade_sh, root.fading.shardAmount, 0, 50);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_sh.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.fading.shardAmount = shardAmount;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Reset
        /// /////////////////////////////////////////////////////////

        void UI_Reset()
        {
            SetFoldoutPref (ref exp_res, "rf_te", gui_res, true);
            if (exp_res == true )
            {
                EditorGUI.indentLevel++;
                
                UI_Reset_Types();
                
                /*
                GUILayout.Space (space);

                if (root.demolitionType != DemolitionType.None)
                {
                    UI_Reset_Dml();

                    GUILayout.Space (space);

                    if (ReuseState (rigid) == true)
                        UI_Reset_Reuse();
                }
                */

                EditorGUI.indentLevel--;
            }
        }

        void UI_Reset_Types()
        {
            GUILayout.Label ("  Reset", EditorStyles.boldLabel);
                
            EditorGUI.BeginChangeCheck();
            bool transform = EditorGUILayout.Toggle (gui_res_tm, root.reset.transform);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_tm.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.transform = transform;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            bool damage = EditorGUILayout.Toggle (gui_res_dm, root.reset.damage);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_dm.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.damage = damage;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            bool connectivity = EditorGUILayout.Toggle (gui_res_cn, root.reset.connectivity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_cn.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.connectivity = connectivity;
                    SetDirty (scr);
                }
            }
        }

        void UI_Reset_Dml()
        {
            GUILayout.Label ("  Demolition", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            RFReset.PostDemolitionType action = (RFReset.PostDemolitionType)EditorGUILayout.EnumPopup (gui_res_ac, root.reset.action);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_res_ac.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.action = action;
                    SetDirty (scr);
                }
            }

            if (root.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float destroyDelay = EditorGUILayout.Slider (gui_res_dl, root.reset.destroyDelay, 0, 60);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_res_dl.text);
                    foreach (RayfireRigidRoot scr in targets)
                    {
                        scr.reset.destroyDelay = destroyDelay;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_Reset_Reuse()
        {
            GUILayout.Label ("  Reuse", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            RFReset.MeshResetType mesh = (RFReset.MeshResetType)EditorGUILayout.EnumPopup (gui_res_ms, root.reset.mesh);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_res_ms.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.mesh = mesh;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            RFReset.FragmentsResetType fragments = (RFReset.FragmentsResetType)EditorGUILayout.EnumPopup (gui_res_fr, root.reset.fragments);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_res_fr.text);
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.reset.fragments = fragments;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////
        
        public override void OnInspectorGUI()
        {
            // Get target
            root = target as RayfireRigidRoot;
            if (root == null)
                return;
            
            // Space
            GUILayout.Space (8);
            
            // Initialize
            if (Application.isPlaying == true)
            {
                if (root.initialized == false)
                {
                    if (GUILayout.Button ("Initialize", GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigidRoot != null)
                                if ((targ as RayfireRigidRoot).initialized == false)
                                    (targ as RayfireRigidRoot).Initialize();
                }
                
                // Reuse
                else
                {
                    if (GUILayout.Button ("Reset Rigid Root", GUILayout.Height (25)))
                            foreach (var targ in targets)
                                if (targ as RayfireRigidRoot != null)
                                    if ((targ as RayfireRigidRoot).initialized == true)
                                        (targ as RayfireRigidRoot).ResetRigidRoot();
                }
                GUILayout.Space (2);
            }
            
            RigidRootSetupUI();
            
            GUILayout.Space (3);

            if (root.cluster.shards.Count > 0)
            {
                GUILayout.Label ("    Cluster Shards: " + root.cluster.shards.Count);
                // GUILayout.Label ("    Amount Integrity: " + conn.AmountIntegrity + "%");
            }
            
            if (root.physics.HasIgnore == true)
                GUILayout.Label ("    Ignore Pairs: " + root.physics.ignoreList.Count / 2);
            
  //          if (Application.isPlaying == true)
  //              RigidManUI();
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            root.initialization = (RayfireRigidRoot.InitType)EditorGUILayout.EnumPopup (gui_mn_ini, root.initialization);
            if (EditorGUI.EndChangeCheck())
                foreach (RayfireRigidRoot scr in targets)
                {
                    scr.initialization = root.initialization;
                    SetDirty (scr);
                }
            
            GUILayout.Space (space);

            UI_Simulation();
            
            GUILayout.Space (space);
            
            UI_Demolition();
            
            GUILayout.Space (space);
            
            GUILayout.Label ("  Common", EditorStyles.boldLabel);

            GUILayout.Space (space);
            
            UI_Fade();
            
            GUILayout.Space (space);

            UI_Reset();
            
            GUILayout.Space (8);
        }
        
        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////

        void RigidRootSetupUI()
        {
            if (Application.isPlaying == false)
            {
                GUILayout.Space (2);
                GUILayout.BeginHorizontal();

                if (root.cluster.shards.Count == 0)
                    if (GUILayout.Button ("Editor Setup", GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigidRoot != null)
                            {
                                (targ as RayfireRigidRoot).EditorSetup();
                                SetDirty (targ as RayfireRigidRoot);
                            }
                    
                if (root.cluster.shards.Count > 0)
                    if (GUILayout.Button ("Reset Setup", GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigidRoot != null)
                            {
                                //RFPhysic.DestroyColliders (targ as RayfireRigidRoot);
                                (targ as RayfireRigidRoot).ResetSetup();
                                SetDirty (targ as RayfireRigidRoot);
                            }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space (2);
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////
        
        void SetDirty(RayfireRigidRoot scr)
        {
            if (Application.isPlaying == false)
            {
                EditorUtility.SetDirty (scr);
                EditorSceneManager.MarkSceneDirty (scr.gameObject.scene);
                SceneView.RepaintAll();
            }
        }
        
        void SetFoldoutPref (ref bool val, string pref, GUIContent caption, bool state) 
        {
            EditorGUI.BeginChangeCheck();
            val = EditorGUILayout.Foldout (val, caption, state);
            if (EditorGUI.EndChangeCheck() == true)
                EditorPrefs.SetBool (pref, val);
        }
    }
}

/*
        void RigidManUI()
        {
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button ("Activate", GUILayout.Height (25)))
                Activate();
            
            if (GUILayout.Button ("Fade", GUILayout.Height (25)))
                Fade();
            
            EditorGUILayout.EndHorizontal();
        }
        
        void Activate()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigidRoot != null)
                        if ((targ as RayfireRigidRoot).simulationType == SimType.Inactive || (targ as RayfireRigidRoot).simulationType == SimType.Kinematic)
                            (targ as RayfireRigidRoot).ac();
        }
        
        void Fade()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigidRoot != null)
                        (targ as RayfireRigidRoot).Fade();
        }
*/

