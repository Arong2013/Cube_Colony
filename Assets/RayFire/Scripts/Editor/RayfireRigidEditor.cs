using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;

namespace RayFire
{
    [CanEditMultipleObjects]
    [CustomEditor (typeof(RayfireRigid), true)]
    public class RayfireRigidEditor : Editor
    {
        RayfireRigid rigid;
        
        SerializedProperty refsProp;
        ReorderableList    refsList;  
        
        /// /////////////////////////////////////////////////////////
        /// Static
        /// /////////////////////////////////////////////////////////
        
        // static int space = 3;
        static readonly string rfRig = "RayFire Rigid: ";
        static readonly string misShards = " has missing shards. Reset or Setup cluster.";
        static readonly int    space     = 3;
        
        static bool exp_phy;
        static bool exp_act;
        static bool exp_lim;
        static bool exp_msh;
        static bool exp_prp;
        static bool exp_cls;
        static bool exp_clp;
        static bool exp_ref;
        static bool exp_mat;
        static bool exp_dmg;
        static bool exp_fade;
        static bool exp_res;
        
        
        static readonly GUIContent gui_mn_ini      = new GUIContent ("Initialization",     "");
        static readonly GUIContent gui_mn_obj      = new GUIContent ("Object Type",        "");
        static readonly GUIContent gui_mn_sim      = new GUIContent ("Simulation Type",    "Defines behaviour of object during simulation.");
        static readonly GUIContent gui_mn_dml      = new GUIContent ("Demolition Type",    "Defines when and how object will be demolished.");
        static readonly GUIContent gui_phy         = new GUIContent ("Physics",            "Defines all physics properties for simulated object.");
        static readonly GUIContent gui_phy_mtp     = new GUIContent ("Type",               "Material preset with predefined density, friction, elasticity and solidity. Can be edited in Rayfire Man component.");
        static readonly GUIContent gui_phy_mat     = new GUIContent ("Material",           "Allows to define own Physic Material.");
        static readonly GUIContent gui_phy_mby     = new GUIContent ("Mass By",            "");
        static readonly GUIContent gui_phy_mss     = new GUIContent ("Mass",               "Mass which will be applied to object if Mass By set to By Mass Property.");
        static readonly GUIContent gui_phy_ctp     = new GUIContent ("Type",               "");
        static readonly GUIContent gui_phy_pln     = new GUIContent ("Planar Check",       "Do not add Mesh Collider to objects with planar low poly mesh.");
        static readonly GUIContent gui_phy_ign     = new GUIContent ("Ignore Near",        "");
        static readonly GUIContent gui_phy_grv     = new GUIContent ("Use Gravity",        "Enables gravity for simulated object.");
        static readonly GUIContent gui_phy_slv     = new GUIContent ("Solver Iterations",  "");
        static readonly GUIContent gui_phy_slt     = new GUIContent ("Sleeping Threshold", "");
        static readonly GUIContent gui_phy_dmp     = new GUIContent ("Dampening",          "Multiplier for demolished fragments velocity.");
        static readonly GUIContent gui_act         = new GUIContent ("Activation",         "Allows to activate ( make dynamic ) inactive and kinematic objects.");
        static readonly GUIContent gui_act_loc     = new GUIContent ("Local",              "Activation By Local Offset relative to parent.");
        static readonly GUIContent gui_act_ofs     = new GUIContent ("Offset",             "Inactive object will be activated if will be pushed from it's original position farther than By Offset value.");
        static readonly GUIContent gui_act_vel     = new GUIContent ("Velocity",           "Inactive object will be activated when it's velocity will be higher than By Velocity value when pushed by other dynamic objects.");
        static readonly GUIContent gui_act_dmg     = new GUIContent ("Damage",             "Inactive object will be activated if will get total damage higher than this value.");
        static readonly GUIContent gui_act_act     = new GUIContent ("Activator",          "Inactive object will be activated by overlapping with object with RayFire Activator component.");
        static readonly GUIContent gui_act_imp     = new GUIContent ("Impact",             "Inactive object will be activated when it will be shot by RayFireGun component.");
        static readonly GUIContent gui_act_con     = new GUIContent ("Connectivity",       "Inactive object will be activated by Connectivity component if it will not be connected with Unyielding zone.");
        static readonly GUIContent gui_act_uny     = new GUIContent ("Unyielding",         "Allows to define Inactive/Kinematic object as Unyielding to check for connection with other Inactive/Kinematic objects with enabled By Connectivity activation type.");
        static readonly GUIContent gui_act_acd     = new GUIContent ("Activatable",        "Unyielding object can not be activate by default. When On allows to activate Unyielding objects as well.");
        static readonly GUIContent gui_act_l       = new GUIContent ("Change Layer",       "Change layer for activated objects.");
        static readonly GUIContent gui_act_lay     = new GUIContent ("Layer",              "Custom layer for activated objects.");
        static readonly GUIContent gui_lim         = new GUIContent ("Limitations",        "");
        static readonly GUIContent gui_lim_col     = new GUIContent ("By Collision",       "Enables demolition by collision.");
        static readonly GUIContent gui_lim_sol     = new GUIContent ("Solidity",           "Local Object solidity multiplier for object. Low Solidity makes object more fragile at collision.");
        static readonly GUIContent gui_lim_tag     = new GUIContent ("Tag",                "Object will be demolished only if it will collide with other objects with defined Tag.");
        static readonly GUIContent gui_lim_dep     = new GUIContent ("Depth",              "Defines how deep object can be demolished. Depth is limitless if set to 0.");
        static readonly GUIContent gui_lim_tim     = new GUIContent ("Time",               "Safe time. Measures in seconds and allows to prevent fragments from being demolished right after they were just created.");
        static readonly GUIContent gui_lim_siz     = new GUIContent ("Size",               "Prevent objects with bounding box size less than defined value to be demolished.");
        static readonly GUIContent gui_lim_vis     = new GUIContent ("Visible",            "Object will be demolished only if it is visible to any camera including scene camera.");
        static readonly GUIContent gui_lim_slc     = new GUIContent ("Slice By Blade",     "Allows object to be sliced by object with RayFire Blade component.");
        static readonly GUIContent gui_msh         = new GUIContent ("Mesh Demolition",    "");
        static readonly GUIContent gui_msh_am      = new GUIContent ("Amount",             "Defines amount of points in point cloud for fragments after demolition.");
        static readonly GUIContent gui_msh_vr      = new GUIContent ("Variation",          "Defines additional amount variation for object in percents.");
        static readonly GUIContent gui_msh_dp      = new GUIContent ("Depth Fade",         "Amount multiplier for next Depth level. Allows to decrease fragments amount of every next demolition level.");
        static readonly GUIContent gui_msh_cb      = new GUIContent ("Contact Bias",       "Higher value allows to create more tiny fragments closer to collision contact point and bigger fragments far from it.");
        static readonly GUIContent gui_msh_sd      = new GUIContent ("Seed",               "Defines Seed for fragmentation algorithm. Same Seed will produce same fragments for same object every time.");
        static readonly GUIContent gui_msh_sh      = new GUIContent ("Use Shatter",        "Allows to use RayFire Shatter properties for fragmentation. Works only if object has RayFire Shatter component.");
        static readonly GUIContent gui_msh_ch      = new GUIContent ("Add Children",       "Add children mesh objects to fragments.");
        static readonly GUIContent gui_msh_adv_cls = new GUIContent ("Clusterize",         "Convert demolished fragments into Connected Cluster and demolish it instantly relative to contact point."); 
        static readonly GUIContent gui_msh_adv_sim = new GUIContent ("Sim Type",           "Simulation type for demolished fragments."); 
        static readonly GUIContent gui_msh_adv_inp = new GUIContent ("Mesh Input",         "Defines time for Mesh Input to process it and prepare for demolition. Useful for mid and hi poly objects.");
        static readonly GUIContent gui_msh_rnt     = new GUIContent ("Runtime Caching",    ""); 
        static readonly GUIContent gui_msh_rnt_fr  = new GUIContent ("Frames",             "");
        static readonly GUIContent gui_msh_rnt_fg  = new GUIContent ("Fragments",          "");
        static readonly GUIContent gui_msh_rnt_sk  = new GUIContent ("Skip First",         "Only initiate Runtime Caching on first demolition and demolish at second.");
        static readonly GUIContent gui_msh_adv     = new GUIContent ("Properties",         "");
        static readonly GUIContent gui_msh_adv_col = new GUIContent ("Collider",           "");
        static readonly GUIContent gui_msh_adv_szl = new GUIContent ("Size Filter",        "Fragments with size less than this value will not get collider.");
        static readonly GUIContent gui_msh_adv_rem = new GUIContent ("Remove Collinear",   "Remove collier vertices to decrease amount of triangles.");
        static readonly GUIContent gui_msh_adv_dec = new GUIContent ("Decompose",          "Detach all disconnected triangles into separate fragments.");
        static readonly GUIContent gui_msh_adv_cap = new GUIContent ("Precap",             "Cap open edges before fragment mesh.");
        static readonly GUIContent gui_msh_adv_l   = new GUIContent ("Inherit Layer",      "Inherit Layer for fragments.");
        static readonly GUIContent gui_msh_adv_lay = new GUIContent ("  Custom Layer",     "Custom layer for fragments.");
        static readonly GUIContent gui_msh_adv_t   = new GUIContent ("Inherit Tag",        "Inherit Tag for fragments.");
        static readonly GUIContent gui_msh_adv_tag = new GUIContent ("  Custom Tag",       "Custom Tag fr fragments.");
        static readonly GUIContent gui_cls         = new GUIContent ("Cluster Demolition", "");
        static readonly GUIContent gui_cls_conn    = new GUIContent ("Connectivity",       "Defines Connectivity algorithm for clusters.");
        static readonly GUIContent gui_cls_fl_ar   = new GUIContent ("Minimum Area",       "Two shards will have connection if their shared area is bigger than this value.");
        static readonly GUIContent gui_cls_fl_sz   = new GUIContent ("Minimum Size",       "Two shards will have connection if their size is bigger than this value.");
        static readonly GUIContent gui_cls_fl_pr   = new GUIContent ("Percentage",         "Random percentage of connections will be discarded.");
        static readonly GUIContent gui_cls_fl_sd   = new GUIContent ("Seed",               "Seed for random percentage filter and for Random Collapse.");
        static readonly GUIContent gui_cls_ds_tp   = new GUIContent ("Type",               "");
        static readonly GUIContent gui_cls_ds_rt   = new GUIContent ("Ratio",              "Defines demolition distance from contact point in percentage relative to object's size.");
        static readonly GUIContent gui_cls_ds_un   = new GUIContent ("Units",              "Defines demolition distance from contact point in world units.");
        static readonly GUIContent gui_cls_sh_ar   = new GUIContent ("Area",               "");
        static readonly GUIContent gui_cls_sh_dm   = new GUIContent ("Demolition",         "");
        static readonly GUIContent gui_cls_min     = new GUIContent ("Minimum",            "");
        static readonly GUIContent gui_cls_max     = new GUIContent ("Maximum",            "");
        static readonly GUIContent gui_cls_dml     = new GUIContent ("Demolishable",       "");
        static readonly GUIContent gui_clp_type    = new GUIContent ("Type", " By Area: Shard will loose it's connections if it's shared area surface is less then defined value.\n" + 
                                                                          " By Size: Shard will loose it's connections if it's Size is less then defined value.\n" + 
                                                                          " Random: Shard will loose it's connections if it's random value in range from 0 to 100 is less then defined value.");
        static readonly GUIContent gui_clp_str    = new GUIContent ("Start",                "Defines start value in percentage relative to whole range of picked type.");
        static readonly GUIContent gui_clp_end    = new GUIContent ("End",                  "Defines end value in percentage relative to whole range of picked type.");
        static readonly GUIContent gui_clp_step   = new GUIContent ("Steps",                "Amount of times when defined threshold value will be set during Duration period.");
        static readonly GUIContent gui_clp_dur    = new GUIContent ("Duration",             "Time which it will take Start value to be increased to End value.");
        static readonly GUIContent gui_clp_var    = new GUIContent ("Variation",            "Percentage Variation for By Area and By Size collapse.");
        static readonly GUIContent gui_clp_seed   = new GUIContent ("Seed",                 "Seed for Random collapse.");
        static readonly GUIContent gui_ref        = new GUIContent ("Reference Demolition", "");
        static readonly GUIContent gui_ref_ref    = new GUIContent ("Reference",            "");
        static readonly GUIContent gui_ref_lst    = new GUIContent ("Random List",          "");
        static readonly GUIContent gui_ref_act    = new GUIContent ("Action",               "");
        static readonly GUIContent gui_ref_add    = new GUIContent ("Add Rigid",            "Add RayFire Rigid component to reference with mesh.");
        static readonly GUIContent gui_ref_scl    = new GUIContent ("Inherit Scale",        "");
        static readonly GUIContent gui_ref_mat    = new GUIContent ("Inherit Materials",    "");
        static readonly GUIContent gui_mat        = new GUIContent ("Materials",            "");
        static readonly GUIContent gui_mat_scl    = new GUIContent ("Mapping",              "Mapping scale for inner surface");
        static readonly GUIContent gui_mat_inn    = new GUIContent ("Inner",                "Material for inner fragments surface");
        static readonly GUIContent gui_mat_out    = new GUIContent ("Outer",                "Material for outer fragments surface");
        static readonly GUIContent gui_dmg        = new GUIContent ("Damage",               "Allows to demolish object by it's own floating Damage value.");
        static readonly GUIContent gui_dmg_en     = new GUIContent ("Enable",               "");
        static readonly GUIContent gui_dmg_max    = new GUIContent ("Max Damage",           "Defines maximum allowed damage for object to be demolished.");
        static readonly GUIContent gui_dmg_cur    = new GUIContent ("Current Damage",       "Shows current damage value. Can be increased by public method: \nApplyDamage(float damageValue, Vector3 damagePosition)");
        static readonly GUIContent gui_dmg_col    = new GUIContent ("Collect",              "Allows to accumulate damage value by collisions during dynamic simulation.");
        static readonly GUIContent gui_dmg_mul    = new GUIContent ("Multiplier",           "Multiplier for every collision damage.");
        static readonly GUIContent gui_dmg_sh     = new GUIContent ("To Shards",            "Apply damage to Connected Cluster shards.");
        static readonly GUIContent gui_fade       = new GUIContent ("Fading",               "");
        static readonly GUIContent gui_fade_dml   = new GUIContent ("On Demolition",        "");
        static readonly GUIContent gui_fade_act   = new GUIContent ("On Activation",        "");
        static readonly GUIContent gui_fade_ofs   = new GUIContent ("By Offset",            "");
        static readonly GUIContent gui_fade_tp    = new GUIContent ("Type",                 "");
        static readonly GUIContent gui_fade_tm    = new GUIContent ("Time",                 "Fade duration time.");
        static readonly GUIContent gui_fade_lf_tp = new GUIContent ("Type",                 "");
        static readonly GUIContent gui_fade_lf_tm = new GUIContent ("Time",                 "Time which object will be simulated before start to fade.");
        static readonly GUIContent gui_fade_lf_vr = new GUIContent ("Variation",            "");
        static readonly GUIContent gui_fade_sz    = new GUIContent ("Size",                 "Fade won't affect objects with size bigger than this value. Disabled if set to 0.");
        static readonly GUIContent gui_fade_sh    = new GUIContent ("Shards",               "Fade won't affect Connected clusters with shard amount bigger than this value. Disabled if set to 0.");
        static readonly GUIContent gui_res        = new GUIContent ("Reset",                "");
        static readonly GUIContent gui_res_tm     = new GUIContent ("Transform",            "Reset transform to position and rotation when object was initialized.");
        static readonly GUIContent gui_res_dm     = new GUIContent ("Damage",               "Reset damage value.");
        static readonly GUIContent gui_res_cn     = new GUIContent ("Connectivity",         "Reset Connectivity.");
        static readonly GUIContent gui_res_ac     = new GUIContent ("Action",               "");
        static readonly GUIContent gui_res_dl     = new GUIContent ("Destroy Delay",        "Object will be destroyed after defined delay.");
        static readonly GUIContent gui_res_ms     = new GUIContent ("Mesh",                 "");
        static readonly GUIContent gui_res_fr     = new GUIContent ("Fragments",            "");

        static readonly GUIStyle damageStyle = new GUIStyle();
        
        /// /////////////////////////////////////////////////////////
        /// Enable
        /// /////////////////////////////////////////////////////////

        private void OnEnable()
        {
            refsProp                     = serializedObject.FindProperty("referenceDemolition.randomList");
            refsList                     = new ReorderableList(serializedObject, refsProp, true, true, true, true);
            refsList.drawElementCallback = DrawRefListItems;
            refsList.drawHeaderCallback  = DrawRefHeader;
            refsList.onAddCallback       = AddRed;
            refsList.onRemoveCallback    = RemoveRef;
            
            if (EditorPrefs.HasKey ("rf_rp") == true) exp_phy  = EditorPrefs.GetBool ("rf_rp");
            if (EditorPrefs.HasKey ("rf_ra") == true) exp_act  = EditorPrefs.GetBool ("rf_ra");
            if (EditorPrefs.HasKey ("rf_rl") == true) exp_lim  = EditorPrefs.GetBool ("rf_rl");
            if (EditorPrefs.HasKey ("rf_rm") == true) exp_msh  = EditorPrefs.GetBool ("rf_rm");
            if (EditorPrefs.HasKey ("rf_rc") == true) exp_cls  = EditorPrefs.GetBool ("rf_rc");
            if (EditorPrefs.HasKey ("rf_rp") == true) exp_clp  = EditorPrefs.GetBool ("rf_rp");
            if (EditorPrefs.HasKey ("rf_rr") == true) exp_ref  = EditorPrefs.GetBool ("rf_rr");
            if (EditorPrefs.HasKey ("rf_rm") == true) exp_mat  = EditorPrefs.GetBool ("rf_rm");
            if (EditorPrefs.HasKey ("rf_rd") == true) exp_dmg  = EditorPrefs.GetBool ("rf_rd");
            if (EditorPrefs.HasKey ("rf_rf") == true) exp_fade = EditorPrefs.GetBool ("rf_rf");
            if (EditorPrefs.HasKey ("rf_re") == true) exp_res  = EditorPrefs.GetBool ("rf_re");
        }

        /// /////////////////////////////////////////////////////////
        /// Main
        /// /////////////////////////////////////////////////////////

        void UI_Main()
        {
            GUILayout.Label ("  Main", EditorStyles.boldLabel);
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RayfireRigid.InitType initialization = (RayfireRigid.InitType)EditorGUILayout.EnumPopup (gui_mn_ini, rigid.initialization);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_mn_ini.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.initialization = initialization;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            ObjectType objectType = (ObjectType)EditorGUILayout.EnumPopup (gui_mn_obj, rigid.objectType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_mn_obj.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.objectType = objectType;
                    SetDirty (scr);
                }
            }
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
            SimType simulationType = (SimType)EditorGUILayout.EnumPopup (gui_mn_sim, rigid.simulationType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_mn_sim.text);
                foreach (RayfireRigid scr in targets)
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
            SetFoldoutPref (ref exp_phy, "rf_rp", gui_phy, true);
            if (exp_phy == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("  Material", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                MaterialType mt = (MaterialType)EditorGUILayout.EnumPopup (gui_phy_mtp, rigid.physics.mt);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_mtp.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.mt = mt;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                PhysicsMaterial material = (PhysicsMaterial)EditorGUILayout.ObjectField (gui_phy_mat, rigid.physics.material, typeof(PhysicsMaterial), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_mat.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.material = material;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("  Mass", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                MassType massBy = (MassType)EditorGUILayout.EnumPopup (gui_phy_mby, rigid.physics.massBy);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_mby.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.massBy = massBy;
                        SetDirty (scr);
                    }
                }

                if (rigid.physics.massBy == MassType.MassProperty)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    float mass = EditorGUILayout.Slider (gui_phy_mss, rigid.physics.mass, 0.1f, 500f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_phy_mss.text);
                        foreach (RayfireRigid scr in targets)
                        {
                            scr.physics.mass = mass;
                            SetDirty (scr);
                        }
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("  Collider", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFColliderType ct = (RFColliderType)EditorGUILayout.EnumPopup (gui_phy_ctp, rigid.physics.ct);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_ctp.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.ct = ct;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool pc = EditorGUILayout.Toggle (gui_phy_pln, rigid.physics.pc);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_pln.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.pc = pc;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool ine = EditorGUILayout.Toggle (gui_phy_ign, rigid.physics.ine);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_ign.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.ine = ine;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                GUILayout.Label ("  Other", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool gr = EditorGUILayout.Toggle (gui_phy_grv, rigid.physics.gr);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_phy_grv.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.gr = gr;
                        SetDirty (scr);
                    }
                }
                    
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int si = EditorGUILayout.IntSlider (gui_phy_slv, rigid.physics.si, 1, 20);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_slv.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.si = si;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float st = EditorGUILayout.Slider (gui_phy_slt, rigid.physics.st, 0.001f, 0.1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_slt.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.physics.st = st;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                GUILayout.Label ("  Fragments", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float dm = EditorGUILayout.Slider (gui_phy_dmp, rigid.physics.dm, 0f, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_phy_dmp.text);
                    foreach (RayfireRigid scr in targets)
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
            SetFoldoutPref (ref exp_act, "rf_ra", gui_act, true);
            if (exp_act == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("  Activation By", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float off = EditorGUILayout.Slider (gui_act_ofs, rigid.activation.off, 0, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_act_ofs.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.off = off;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);

                if (rigid.activation.off > 0)
                {
                    EditorGUI.indentLevel++;
                    
                    EditorGUI.BeginChangeCheck();
                    bool loc = EditorGUILayout.Toggle (gui_act_loc, rigid.activation.loc);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_loc.text);
                        foreach (RayfireRigid scr in targets)
                        {
                            scr.activation.loc = loc;
                            SetDirty (scr);
                        }
                    }
                
                    GUILayout.Space (space);
                    
                    EditorGUI.indentLevel--;
                }

                EditorGUI.BeginChangeCheck();
                float vel = EditorGUILayout.Slider (gui_act_vel, rigid.activation.vel, 0, 5f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_vel.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.vel = vel;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float dmg = EditorGUILayout.Slider (gui_act_dmg, rigid.activation.dmg, 0, 100f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_act_dmg.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.dmg = dmg;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool act = EditorGUILayout.Toggle (gui_act_act, rigid.activation.act);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_act.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.act = act;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool imp = EditorGUILayout.Toggle (gui_act_imp, rigid.activation.imp);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_imp.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.imp = imp;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool con = EditorGUILayout.Toggle (gui_act_con, rigid.activation.con);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_con.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.con = con;
                        SetDirty (scr);
                    }
                }

                if (rigid.activation.con == true)
                {
                    EditorGUI.indentLevel++;
                    
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    bool uny = EditorGUILayout.Toggle (gui_act_uny, rigid.activation.uny);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_uny.text);
                        foreach (RayfireRigid scr in targets)
                        {
                            scr.activation.uny = uny;
                            SetDirty (scr);
                        }
                    }

                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    bool atb = EditorGUILayout.Toggle (gui_act_acd, rigid.activation.atb);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_acd.text);
                        foreach (RayfireRigid scr in targets)
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
                bool l = EditorGUILayout.Toggle (gui_act_l, rigid.activation.l);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_act_l.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.activation.l = l;
                        SetDirty (scr);
                    }
                }
                
                if (rigid.activation.l == true)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int lay = EditorGUILayout.LayerField (gui_act_lay, rigid.activation.lay);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObjects (targets, gui_act_lay.text);
                        foreach (RayfireRigid scr in targets)
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
            foreach (RayfireRigid scr in targets)
                if (ActivatableState(scr) == true)
                    return true;
            return false;
        }
        
        static bool ActivatableState(RayfireRigid scr)
        {
            if (scr.simulationType == SimType.Inactive || scr.simulationType == SimType.Kinematic)
                    return true;
            if (scr.meshDemolition.sim == FragSimType.Inactive || scr.meshDemolition.sim == FragSimType.Kinematic)
                    return true;
            return false;
        }

        /// /////////////////////////////////////////////////////////
        /// Demolition
        /// /////////////////////////////////////////////////////////

        void UI_Demolition()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Demolition", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            DemolitionType demolitionType = (DemolitionType)EditorGUILayout.EnumPopup (gui_mn_dml, rigid.demolitionType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_mn_dml.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.demolitionType = demolitionType;
                    SetDirty (scr);
                }
            }
            
            if (rigid.objectType == ObjectType.MeshRoot || rigid.demolitionType != DemolitionType.None)
                UI_Limitations();
            
            if (MeshState() == true && rigid.demolitionType != DemolitionType.None)
                UI_Mesh();
            
            if (rigid.IsCluster == true || rigid.meshDemolition.cls == true || rigid.objectType == ObjectType.MeshRoot)
                UI_Cluster();
            
            if (rigid.demolitionType == DemolitionType.ReferenceDemolition)
                UI_Reference();
            
            if (MeshState() == true)
                UI_Materials();

            UI_Damage();
        }
        
        bool MeshState()
        {
            foreach (RayfireRigid scr in targets)
                if (MeshState(scr) == true)
                    return true;
            return false;
        }
        
        static bool MeshState(RayfireRigid scr)
        {
            if (scr.objectType == ObjectType.Mesh ||
                scr.objectType == ObjectType.MeshRoot ||
                scr.objectType == ObjectType.SkinnedMesh)
                return true;
            if (scr.clusterDemolition.shardDemolition == true)
                return true;
            return false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Limitations
        /// /////////////////////////////////////////////////////////

        void UI_Limitations()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_lim, "rf_rl", gui_lim, true);
            if (exp_lim == true)
            {
                EditorGUI.indentLevel++;
                
                GUILayout.Space (space);
                GUILayout.Label ("  Collision", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool col = EditorGUILayout.Toggle (gui_lim_col, rigid.limitations.col);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_col.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.col = col;
                        SetDirty (scr);
                    }
                }
                
                if (rigid.limitations.col == true)
                {
                    GUILayout.Space (space);
                    
                    EditorGUI.BeginChangeCheck();
                    float sol = EditorGUILayout.Slider (gui_lim_sol, rigid.limitations.sol, 0, 10f);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_lim_sol.text);
                        foreach (RayfireRigid scr in targets)
                        {
                            scr.limitations.sol = sol;
                            SetDirty (scr);
                        }
                    }
                }
                
                GUILayout.Space (space);
                    
                EditorGUI.BeginChangeCheck();
                string tag = EditorGUILayout.TagField (gui_lim_tag, rigid.limitations.tag);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_tag.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.tag = tag;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                GUILayout.Label ("  Other", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int depth = EditorGUILayout.IntSlider (gui_lim_dep, rigid.limitations.depth, 0, 7);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_dep.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.depth = depth;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float time = EditorGUILayout.Slider (gui_lim_tim, rigid.limitations.time, 0.05f, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_tim.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.time = time;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float size = EditorGUILayout.Slider (gui_lim_siz, rigid.limitations.size, 0.01f, 5f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_lim_siz.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.size = size;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool vis = EditorGUILayout.Toggle (gui_lim_vis, rigid.limitations.vis);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_vis.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.vis = vis;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool bld = EditorGUILayout.Toggle (gui_lim_slc, rigid.limitations.bld);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_lim_slc.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.limitations.bld = bld;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Mesh
        /// /////////////////////////////////////////////////////////

        void UI_Mesh()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_msh, "rf_rm", gui_msh, true);
            if (exp_msh == true)
            {
                EditorGUI.indentLevel++;
                
                UI_Mesh_Frags();
                
                GUILayout.Space (space);
                GUILayout.Label ("  Advanced", EditorStyles.boldLabel);
                GUILayout.Space (space);

                UI_Mesh_Adv();
                
                GUILayout.Space (space);

                UI_Mesh_Runtime();
                
                GUILayout.Space (space);
                
                UI_Mesh_Props();
                
                EditorGUI.indentLevel--;
            }
        }

        void UI_Mesh_Frags ()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Fragments", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int am = EditorGUILayout.IntSlider (gui_msh_am, rigid.meshDemolition.am, 2, 300);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_am.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.am = am;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int var = EditorGUILayout.IntSlider (gui_msh_vr, rigid.meshDemolition.var, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_vr.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.var = var;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float dpf = EditorGUILayout.Slider (gui_msh_dp, rigid.meshDemolition.dpf, 0.01f, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_dp.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.dpf = dpf;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float bias = EditorGUILayout.Slider (gui_msh_cb, rigid.meshDemolition.bias, 0, 1f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_cb.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.bias = bias;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int sd = EditorGUILayout.IntSlider (gui_msh_sd, rigid.meshDemolition.sd, 0, 50);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_sd.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.sd = sd;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool use = EditorGUILayout.Toggle (gui_msh_sh, rigid.meshDemolition.use);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_sh.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.use = use;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool cld = EditorGUILayout.Toggle (gui_msh_ch, rigid.meshDemolition.cld);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_ch.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.cld = cld;
                    SetDirty (scr);
                }
            }
        }

        void UI_Mesh_Adv()
        {
            EditorGUI.BeginChangeCheck();
            bool cls = EditorGUILayout.Toggle (gui_msh_adv_cls, rigid.meshDemolition.cls);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_adv_cls.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.cls = cls;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            FragSimType sim = (FragSimType)EditorGUILayout.EnumPopup (gui_msh_adv_sim, rigid.meshDemolition.sim);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_adv_sim.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.sim = sim;
                    SetDirty (scr);
                }
            }
        }

        void UI_Mesh_Runtime()
        {
            EditorGUI.BeginChangeCheck();
            CachingType tp = (CachingType)EditorGUILayout.EnumPopup (gui_msh_rnt, rigid.meshDemolition.ch.tp);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_msh_rnt.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.ch.tp = tp;
                    SetDirty (scr);
                }
            }
            
            if (rigid.meshDemolition.ch.tp == CachingType.Disable)
                return;
            
            EditorGUI.indentLevel++;
            
            if (rigid.meshDemolition.ch.tp == CachingType.ByFrames)
            {
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int frm = EditorGUILayout.IntSlider (gui_msh_rnt_fr, rigid.meshDemolition.ch.frm, 2, 300);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_msh_rnt_fr.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.ch.frm = frm;
                        SetDirty (scr);
                    }
                }
            }
            
            if (rigid.meshDemolition.ch.tp == CachingType.ByFragmentsPerFrame)
            {
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                int frg = EditorGUILayout.IntSlider (gui_msh_rnt_fg, rigid.meshDemolition.ch.frg, 1, 20);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_msh_rnt_fg.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.ch.frg = frg;
                        SetDirty (scr);
                    }
                }
            }
            
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            bool skp = EditorGUILayout.Toggle (gui_msh_rnt_sk, rigid.meshDemolition.ch.skp);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_rnt_sk.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.ch.skp = skp;
                    SetDirty (scr);
                }
            }

            EditorGUI.indentLevel--;
        }

        void UI_Mesh_Props()
        {
            exp_prp = EditorGUILayout.Foldout (exp_prp, gui_msh_adv, true);
            if (exp_prp == true)
            {
                GUILayout.Space (space);
                
                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                bool rem = EditorGUILayout.Toggle (gui_msh_adv_rem, rigid.meshDemolition.prp.rem);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_msh_adv_rem.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.rem = rem;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool dec = EditorGUILayout.Toggle (gui_msh_adv_dec, rigid.meshDemolition.prp.dec);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_msh_adv_dec.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.dec = dec;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                bool cap = EditorGUILayout.Toggle (gui_msh_adv_cap, rigid.meshDemolition.prp.cap);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_msh_adv_cap.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.cap = cap;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFDemolitionMesh.MeshInputType inp = (RFDemolitionMesh.MeshInputType)EditorGUILayout.EnumPopup (gui_msh_adv_inp, rigid.meshDemolition.inp);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_msh_adv_inp.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.inp = inp;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFColliderType col = (RFColliderType)EditorGUILayout.EnumPopup (gui_msh_adv_col, rigid.meshDemolition.prp.col);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_msh_adv_col.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.col = col;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float szF = EditorGUILayout.Slider (gui_msh_adv_szl, rigid.meshDemolition.prp.szF, 0, 10);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_msh_adv_szl.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.szF = szF;
                        SetDirty (scr);
                    }
                }

                UI_Dml_Lay_Tag();

                EditorGUI.indentLevel--;
            }
        }
        
        void UI_Dml_Lay_Tag()
        {
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool l = EditorGUILayout.Toggle (gui_msh_adv_l, rigid.meshDemolition.prp.l);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_adv_l.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.prp.l = l;
                    SetDirty (scr);
                }
            }
            
            if (rigid.meshDemolition.prp.l == false)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int lay = EditorGUILayout.LayerField (gui_msh_adv_lay, rigid.meshDemolition.prp.lay);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_msh_adv_lay.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.lay = lay;
                        SetDirty (scr);
                    }
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool t = EditorGUILayout.Toggle (gui_msh_adv_t, rigid.meshDemolition.prp.t);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_msh_adv_t.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.meshDemolition.prp.t = t;
                    SetDirty (scr);
                }
            }

            if (rigid.meshDemolition.prp.t == false)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                string tag = EditorGUILayout.TagField (gui_msh_adv_tag, rigid.meshDemolition.prp.tag);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_msh_adv_tag.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.meshDemolition.prp.tag = tag;
                        SetDirty (scr);
                    }
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster
        /// /////////////////////////////////////////////////////////

        void UI_Cluster()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_cls, "rf_rc", gui_cls, true);
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
                
                GUILayout.Space (space);
                
                GUILayout.Label ("  Layer and Tag", EditorStyles.boldLabel);
                UI_Dml_Lay_Tag();
                
                EditorGUI.indentLevel--;
            }
        }

        void UI_Cluster_Props()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            ConnectivityType connectivity = (ConnectivityType)EditorGUILayout.EnumPopup (gui_cls_conn, rigid.clusterDemolition.connectivity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_conn.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.connectivity = connectivity;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Filters () 
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            if (rigid.clusterDemolition.connectivity != ConnectivityType.ByBoundingBox)
            {
                EditorGUI.BeginChangeCheck();
                float minimumArea = EditorGUILayout.Slider (gui_cls_fl_ar, rigid.clusterDemolition.minimumArea, 0, 1f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_fl_ar.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.minimumArea = minimumArea;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
            }

            EditorGUI.BeginChangeCheck();
            float minimumSize = EditorGUILayout.Slider (gui_cls_fl_sz, rigid.clusterDemolition.minimumSize, 0, 10f);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_sz.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.minimumSize = minimumSize;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int percentage = EditorGUILayout.IntSlider (gui_cls_fl_pr, rigid.clusterDemolition.percentage, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_pr.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.percentage = percentage;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int seed = EditorGUILayout.IntSlider (gui_cls_fl_sd, rigid.clusterDemolition.seed, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_fl_sd.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.seed = seed;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Dist()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Demolition Distance", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RFDemolitionCluster.RFDetachType type = (RFDemolitionCluster.RFDetachType)EditorGUILayout.EnumPopup (gui_cls_ds_tp, rigid.clusterDemolition.type);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_ds_tp.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.type = type;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);

            if (rigid.clusterDemolition.type == RFDemolitionCluster.RFDetachType.RatioToSize)
            {
                EditorGUI.BeginChangeCheck();
                int ratio = EditorGUILayout.IntSlider (gui_cls_ds_rt, rigid.clusterDemolition.ratio, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_ds_rt.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.ratio = ratio;
                        SetDirty (scr);
                    }
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                float units = EditorGUILayout.Slider (gui_cls_ds_un, rigid.clusterDemolition.units, 0, 10f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_cls_ds_un.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.units = units;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_Cluster_Shard()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Shards", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int shardArea = EditorGUILayout.IntSlider (gui_cls_sh_ar, rigid.clusterDemolition.shardArea, 0, 100);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_sh_ar.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.shardArea = shardArea;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool shardDemolition = EditorGUILayout.Toggle (gui_cls_sh_dm, rigid.clusterDemolition.shardDemolition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_sh_dm.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.shardDemolition = shardDemolition;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Cluster_Cls()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Clusters", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int minAmount = EditorGUILayout.IntSlider (gui_cls_min, rigid.clusterDemolition.minAmount, 1, 20);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_min.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.minAmount = minAmount;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int maxAmount = EditorGUILayout.IntSlider (gui_cls_max, rigid.clusterDemolition.maxAmount, 1, 20);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_cls_max.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.maxAmount = maxAmount;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool demolishable = EditorGUILayout.Toggle (gui_cls_dml, rigid.clusterDemolition.demolishable);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_cls_dml.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.clusterDemolition.demolishable = demolishable;
                    SetDirty (scr);
                }
            }
        }

        void UI_Cluster_Collapse()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Collapse", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_clp, "rf_rp", gui_msh_adv, true);
            if (exp_clp == true)
            {
                GUILayout.Space (space);

                EditorGUI.indentLevel++;

                EditorGUI.BeginChangeCheck();
                RFCollapse.RFCollapseType type = (RFCollapse.RFCollapseType)EditorGUILayout.EnumPopup (gui_clp_type, rigid.clusterDemolition.collapse.type);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_type.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.type = type;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int start = EditorGUILayout.IntSlider (gui_clp_str, rigid.clusterDemolition.collapse.start, 0, 99);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_str.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.start = start;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int end = EditorGUILayout.IntSlider (gui_clp_end, rigid.clusterDemolition.collapse.end, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_end.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.end = end;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int steps = EditorGUILayout.IntSlider (gui_clp_step, rigid.clusterDemolition.collapse.steps, 1, 100);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_step.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.steps = steps;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float duration = EditorGUILayout.Slider (gui_clp_dur, rigid.clusterDemolition.collapse.duration, 0, 60f);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_dur.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.duration = duration;
                        SetDirty (scr);
                    }
                }
                
                // Variation
                if (rigid.clusterDemolition.collapse.type != RFCollapse.RFCollapseType.Random)
                {
                    GUILayout.Space (space);

                    EditorGUI.BeginChangeCheck();
                    int var = EditorGUILayout.IntSlider (gui_clp_var, rigid.clusterDemolition.collapse.var, 0, 100);
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        Undo.RecordObjects (targets, gui_clp_var.text);
                        foreach (RayfireRigid scr in targets)
                        {
                            scr.clusterDemolition.collapse.var = var;
                            SetDirty (scr);
                        }
                    }
                }
                
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                int seed = EditorGUILayout.IntSlider (gui_clp_seed, rigid.clusterDemolition.collapse.seed, 0, 99);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_clp_seed.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.clusterDemolition.collapse.seed = seed;
                        SetDirty (scr);
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Reference
        /// /////////////////////////////////////////////////////////

        void UI_Reference()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_ref, "rf_rr", gui_ref, true);
            if (exp_ref == true)
            {
                EditorGUI.indentLevel++;
                
                UI_Reference_Props();
                
                GUILayout.Space (space);
                
                UI_Reference_Source();
                
                EditorGUI.indentLevel--;
            }
        }
        
        void UI_Reference_Props()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RFReferenceDemolition.ActionType action = (RFReferenceDemolition.ActionType)EditorGUILayout.EnumPopup (gui_ref_act, rigid.referenceDemolition.action);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ref_act.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.referenceDemolition.action = action;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool addRigid = EditorGUILayout.Toggle (gui_ref_add, rigid.referenceDemolition.addRigid);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ref_add.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.referenceDemolition.addRigid = addRigid;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool inheritScale = EditorGUILayout.Toggle (gui_ref_scl, rigid.referenceDemolition.inheritScale);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ref_scl.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.referenceDemolition.inheritScale = inheritScale;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool inheritMaterials = EditorGUILayout.Toggle (gui_ref_mat, rigid.referenceDemolition.inheritMaterials);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_ref_mat.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.referenceDemolition.inheritMaterials = inheritMaterials;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
        }
        
        void UI_Reference_Source()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Source", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            GameObject reference = (GameObject)EditorGUILayout.ObjectField (gui_ref_ref, rigid.referenceDemolition.reference, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_ref_ref.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.referenceDemolition.reference = reference;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            serializedObject.Update();
            refsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawRefListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = refsList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y+2, EditorGUIUtility.currentViewWidth - 80f, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }
        
        void DrawRefHeader(Rect rect)
        {
            rect.x += 10;
            EditorGUI.LabelField(rect, gui_ref_lst);
        }

        void AddRed(ReorderableList list)
        {
            if (rigid.referenceDemolition.randomList == null)
                rigid.referenceDemolition.randomList = new List<GameObject>();
            rigid.referenceDemolition.randomList.Add (null);
            list.index = list.count;
        }
        
        void RemoveRef(ReorderableList list)
        {
            if (rigid.referenceDemolition.randomList != null)
            {
                rigid.referenceDemolition.randomList.RemoveAt (list.index);
                list.index = list.index - 1;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Materials
        /// /////////////////////////////////////////////////////////

        void UI_Materials()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_mat, "rf_rm", gui_mat, true);
            if (exp_mat == true)
            {
                EditorGUI.indentLevel++;

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                float mScl = EditorGUILayout.Slider (gui_mat_scl, rigid.materials.mScl, 0.01f, 2f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_mat_scl.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.materials.mScl = mScl;
                        SetDirty (scr);
                    }
                }
                
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                Material iMat = (Material)EditorGUILayout.ObjectField (gui_mat_inn, rigid.materials.iMat, typeof(Material), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_inn.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.materials.iMat = iMat;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                Material oMat = (Material)EditorGUILayout.ObjectField (gui_mat_out, rigid.materials.oMat, typeof(Material), true);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_mat_out.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.materials.oMat = oMat;
                        SetDirty (scr);
                    }
                }
                
                EditorGUI.indentLevel--;
            }
        }

        /// /////////////////////////////////////////////////////////
        /// Damage
        /// /////////////////////////////////////////////////////////

        void UI_Damage()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_dmg, "rf_rd", gui_dmg, true);
            if (exp_dmg == true)
            {
                EditorGUI.indentLevel++;

                UI_Damage_Props();
                
                GUILayout.Space (space);
                
                UI_Damage_Coll();
                
                EditorGUI.indentLevel--;
            }
        }
        
        void UI_Damage_Props()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Properties", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool en = EditorGUILayout.Toggle (gui_dmg_en, rigid.damage.en);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_dmg_en.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.damage.en = en;
                    SetDirty (scr);
                }
            }

            if (rigid.objectType == ObjectType.ConnectedCluster)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                bool shr = EditorGUILayout.Toggle (gui_dmg_sh, rigid.damage.shr);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_dmg_sh.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.damage.shr = shr;
                        SetDirty (scr);
                    }
                }
            }

            GUILayout.Space (space);

            EditorGUI.BeginChangeCheck();
            float max = EditorGUILayout.FloatField (gui_dmg_max, rigid.damage.max);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_dmg_max.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.damage.max = max;
                    SetDirty (scr);
                }
            }

            if (rigid.objectType == ObjectType.ConnectedCluster && rigid.damage.shr == true)
            {
                // To Damage preview
            }
            else
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float cur = EditorGUILayout.FloatField (gui_dmg_cur, rigid.damage.cur);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObjects (targets, gui_dmg_cur.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.damage.cur = cur;
                        SetDirty (scr);
                    }
                }
            }
        }
        
        void UI_Damage_Coll()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Collision", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool col = EditorGUILayout.Toggle (gui_dmg_col, rigid.damage.col);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_dmg_col.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.damage.col = col;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float mlt = EditorGUILayout.Slider (gui_dmg_mul, rigid.damage.mlt, 0.01f, 10f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_dmg_mul.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.damage.mlt = mlt;
                    SetDirty (scr);
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Fade
        /// /////////////////////////////////////////////////////////

        void UI_Fade()
        {
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_fade, "rf_rf", gui_fade, true);
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
            GUILayout.Space (space);
            GUILayout.Label ("  Initiate", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool onDemolition = EditorGUILayout.Toggle (gui_fade_dml, rigid.fading.onDemolition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_dml.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.onDemolition = onDemolition;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool onActivation = EditorGUILayout.Toggle (gui_fade_act, rigid.fading.onActivation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_act.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.onActivation = onActivation;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float byOffset = EditorGUILayout.Slider (gui_fade_ofs, rigid.fading.byOffset, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_ofs.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.byOffset = byOffset;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Fade_Type()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Type", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            FadeType fadeType = (FadeType)EditorGUILayout.EnumPopup (gui_fade_tp, rigid.fading.fadeType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_tp.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.fadeType = fadeType;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float fadeTime = EditorGUILayout.Slider (gui_fade_tm, rigid.fading.fadeTime, 1f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_tm.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.fadeTime = fadeTime;
                    SetDirty (scr);
                }
            }
        }
        
        void UI_Fade_Life()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Life", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RFFadeLifeType lifeType = (RFFadeLifeType)EditorGUILayout.EnumPopup (gui_fade_lf_tp, rigid.fading.lifeType);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_tp.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.lifeType = lifeType;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float lifeTime = EditorGUILayout.Slider (gui_fade_lf_tm, rigid.fading.lifeTime, 0f, 90f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_tm.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.lifeTime = lifeTime;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float lifeVariation = EditorGUILayout.Slider (gui_fade_lf_vr, rigid.fading.lifeVariation, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_lf_vr.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.lifeVariation = lifeVariation;
                    SetDirty (scr);
                }
            }
        }

        void UI_Fade_Filt()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Filters", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            float sizeFilter = EditorGUILayout.Slider (gui_fade_sz, rigid.fading.sizeFilter, 0f, 20f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_sz.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.fading.sizeFilter = sizeFilter;
                    SetDirty (scr);
                }
            }
            
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            int shardAmount = EditorGUILayout.IntSlider (gui_fade_sh, rigid.fading.shardAmount, 0, 50);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_fade_sh.text);
                foreach (RayfireRigid scr in targets)
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
            GUILayout.Space (space);
            
            SetFoldoutPref (ref exp_res, "rf_re", gui_res, true);
            if (exp_res == true )
            {
                EditorGUI.indentLevel++;
                
                UI_Reset_Types();
                
                GUILayout.Space (space);

                if (rigid.demolitionType != DemolitionType.None)
                {
                    UI_Reset_Dml();

                    GUILayout.Space (space);

                    if (ReuseState (rigid) == true)
                        UI_Reset_Reuse();
                }

                EditorGUI.indentLevel--;
            }
        }

        void UI_Reset_Types()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Reset", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            bool transform = EditorGUILayout.Toggle (gui_res_tm, rigid.reset.transform);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_tm.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.reset.transform = transform;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            bool damage = EditorGUILayout.Toggle (gui_res_dm, rigid.reset.damage);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_dm.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.reset.damage = damage;
                    SetDirty (scr);
                }
            }

            GUILayout.Space (space);
                
            EditorGUI.BeginChangeCheck();
            bool connectivity = EditorGUILayout.Toggle (gui_res_cn, rigid.reset.connectivity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects (targets, gui_res_cn.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.reset.connectivity = connectivity;
                    SetDirty (scr);
                }
            }
        }

        void UI_Reset_Dml()
        {
            GUILayout.Space (space);
            GUILayout.Label ("  Demolition", EditorStyles.boldLabel);
            GUILayout.Space (space);
            
            EditorGUI.BeginChangeCheck();
            RFReset.PostDemolitionType action = (RFReset.PostDemolitionType)EditorGUILayout.EnumPopup (gui_res_ac, rigid.reset.action);
            if (EditorGUI.EndChangeCheck() == true)
            {
                Undo.RecordObjects (targets, gui_res_ac.text);
                foreach (RayfireRigid scr in targets)
                {
                    scr.reset.action = action;
                    SetDirty (scr);
                }
            }

            if (rigid.reset.action == RFReset.PostDemolitionType.DestroyWithDelay)
            {
                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                float destroyDelay = EditorGUILayout.Slider (gui_res_dl, rigid.reset.destroyDelay, 0, 60);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_res_dl.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.reset.destroyDelay = destroyDelay;
                        SetDirty (scr);
                    }
                }
            }
        }

        void UI_Reset_Reuse()
        {
            if (rigid.reset.action == RFReset.PostDemolitionType.DeactivateToReset)
            {
                GUILayout.Space (space);
                GUILayout.Label ("  Reuse", EditorStyles.boldLabel);
                GUILayout.Space (space);
                
                EditorGUI.BeginChangeCheck();
                RFReset.MeshResetType mesh = (RFReset.MeshResetType)EditorGUILayout.EnumPopup (gui_res_ms, rigid.reset.mesh);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_res_ms.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.reset.mesh = mesh;
                        SetDirty (scr);
                    }
                }

                GUILayout.Space (space);

                EditorGUI.BeginChangeCheck();
                RFReset.FragmentsResetType fragments = (RFReset.FragmentsResetType)EditorGUILayout.EnumPopup (gui_res_fr, rigid.reset.fragments);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    Undo.RecordObjects (targets, gui_res_fr.text);
                    foreach (RayfireRigid scr in targets)
                    {
                        scr.reset.fragments = fragments;
                        SetDirty (scr);
                    }
                }
            }
        }

        bool ReuseState(RayfireRigid scr)
        {
            if (scr.objectType == ObjectType.Mesh || scr.objectType == ObjectType.MeshRoot)
                return true;

            if (scr.clusterDemolition.shardDemolition == true)
                return true;

            return false;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Inspector
        /// /////////////////////////////////////////////////////////

        public override void OnInspectorGUI()
        {
            rigid = target as RayfireRigid;
            if (rigid == null)
                return;
            
            // Space
            GUILayout.Space (8);
            
            // Initialize
            if (Application.isPlaying == true)
            {
                if (rigid.initialized == false)
                {
                    if (GUILayout.Button ("Initialize", GUILayout.Height (25)))
                        foreach (var targ in targets)
                            if (targ as RayfireRigid != null)
                                if ((targ as RayfireRigid).initialized == false)
                                    (targ as RayfireRigid).Initialize();
                }
                
                // Reuse
                else
                {
                    if (GUILayout.Button ("Reset Rigid", GUILayout.Height (25)))
                            foreach (var targ in targets)
                                if (targ as RayfireRigid != null)
                                    if ((targ as RayfireRigid).initialized == true)
                                        (targ as RayfireRigid).ResetRigid();
                }
            }
            
            RigidManUI();

            // Setup
            if (Application.isPlaying == false)
            {
                // Clusters
                if (rigid.objectType == ObjectType.MeshRoot)
                {
                    GUILayout.Label ("  MeshRoot", EditorStyles.boldLabel);
                    SetupUI();
                }
            }
            
            // Clusters
            if (rigid.IsCluster == true)
            {
                GUILayout.Label ("  Cluster", EditorStyles.boldLabel);

                if (Application.isPlaying == false)
                    SetupUI();

                GUILayout.Space (1);

                ClusterPreviewUI (rigid);

                if (Application.isPlaying == true)
                    ClusterCollapseUI();
            }
            
            InfoUI();
            
            GUILayout.Space (space);

            UI_Main();
            
            GUILayout.Space (space);
            GUILayout.Space (space);
            
            UI_Simulation();
            
            GUILayout.Space (space);
            GUILayout.Space (space);
            
            UI_Demolition();     
            
            GUILayout.Space (space);
            GUILayout.Space (space);
            
            GUILayout.Label ("  Common", EditorStyles.boldLabel);

            UI_Fade();

            UI_Reset();
            
            GUILayout.Space (8);
        }

        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////
        
        void InfoUI()
        {
            // Cache info
            if (rigid.HasMeshes == true)
                GUILayout.Label ("    Precached Unity Meshes: " + rigid.meshes.Length);
            if (rigid.HasFragments == true)
                GUILayout.Label ("    Fragments: " + rigid.fragments.Count);
            if (rigid.HasRfMeshes == true)
                GUILayout.Label ("    Precached Serialized Meshes: " + rigid.rfMeshes.Length);

            // Demolition info
            if (Application.isPlaying == true && rigid.enabled == true && rigid.initialized == true && rigid.objectType != ObjectType.MeshRoot)
            {
                // Space
                GUILayout.Space (3);

                // Info
                GUILayout.Label ("Info", EditorStyles.boldLabel);

                // Excluded
                if (rigid.physics.exclude == true)
                    GUILayout.Label ("WARNING: Object excluded from simulation.");

                // Size
                GUILayout.Label ("    Size: " + rigid.limitations.bboxSize.ToString());

                // Demolition
                GUILayout.Label ("    Demolition depth: " + rigid.limitations.currentDepth.ToString() + "/" + rigid.limitations.depth.ToString());

                // Damage
                if (rigid.damage.en == true)
                    GUILayout.Label ("    Damage applied: " + rigid.damage.cur.ToString() + "/" + rigid.damage.max.ToString());
                
                // Fading
                if (rigid.fading.state == 1)
                    GUILayout.Label ("    Object about to fade...");
                
                // Fading
                if (rigid.fading.state == 2)
                    GUILayout.Label ("    Fading in progress...");

                // Bad mesh
                if (rigid.meshDemolition.badMesh > RayfireMan.inst.advancedDemolitionProperties.badMeshTry)
                    GUILayout.Label ("    Object has bad mesh and will not be demolished anymore");
            }
            
            // Mesh Root info
            if (rigid.objectType == ObjectType.MeshRoot)
            {
                if (rigid.physics.HasIgnore == true)
                    GUILayout.Label ("    Ignore Pairs: " + rigid.physics.ignoreList.Count / 2);
            }
            
            // Cluster info
            if (rigid.objectType == ObjectType.NestedCluster || rigid.objectType == ObjectType.ConnectedCluster)
            {
                if (rigid.physics.clusterColliders != null && rigid.physics.clusterColliders.Count > 0)
                {
                    if (rigid.clusterDemolition != null && rigid.clusterDemolition.cluster != null)
                    {
                        GUILayout.Label ("    Cluster Colliders: " + rigid.physics.clusterColliders.Count);

                        if (rigid.objectType == ObjectType.ConnectedCluster)
                        {
                            GUILayout.Label ("    Cluster Shards: " + rigid.clusterDemolition.cluster.shards.Count + "/" + rigid.clusterDemolition.am);
                            GUILayout.Label ("    Amount Integrity: " + rigid.AmountIntegrity + "%");
                        }

                        if (rigid.physics.HasIgnore == true)
                            GUILayout.Label ("    Ignore Pairs: " + rigid.physics.ignoreList.Count / 2);
                    }
                }
            }
        }

        void RigidManUI()
        {
            if (Application.isPlaying == true)
            {
                GUILayout.BeginHorizontal();

                // Demolition
                if (rigid.objectType != ObjectType.MeshRoot)
                {
                    if (GUILayout.Button ("Demolish", GUILayout.Height (25)))
                        Demolish();
                }

                // Activate
                if (rigid.simulationType == SimType.Inactive || rigid.simulationType == SimType.Kinematic)
                {
                    if (GUILayout.Button ("Activate", GUILayout.Height (25)))
                        Activate();
                }
                
                // Fade
                if (GUILayout.Button ("Fade",     GUILayout.Height (25))) 
                    Fade();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        void Demolish()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        (targ as RayfireRigid).Demolish();
        }
        
        void Activate()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        if ((targ as RayfireRigid).simulationType == SimType.Inactive || (targ as RayfireRigid).simulationType == SimType.Kinematic)
                            (targ as RayfireRigid).Activate();
        }
        
        void Fade()
        {
            if (Application.isPlaying == true)
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                        (targ as RayfireRigid).Fade();
        }
        
        void SetupUI()
        {
            GUILayout.BeginHorizontal();
             
            if (GUILayout.Button (" Editor Setup ", GUILayout.Height (25)))
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                    {
                        (targ as RayfireRigid).EditorSetup();
                        SetDirty (targ as RayfireRigid); 
                    }
            
            if (GUILayout.Button (  "Reset Setup", GUILayout.Height (25)))
                foreach (var targ in targets)
                    if (targ as RayfireRigid != null)
                    {
                        (targ as RayfireRigid).ResetSetup();
                        SetDirty (targ as RayfireRigid); 
                    }

            EditorGUILayout.EndHorizontal();
        }
        
        /// /////////////////////////////////////////////////////////
        /// Cluster UI
        /// /////////////////////////////////////////////////////////
        
        void ClusterCollapseUI()
        {
            if (rigid.objectType == ObjectType.ConnectedCluster)
            {
                GUILayout.Label ("  Collapse", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();

                GUILayout.Label ("By Area:", GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clusterDemolition.cluster.areaCollapse = EditorGUILayout.Slider (rigid.clusterDemolition.cluster.areaCollapse,
                    rigid.clusterDemolition.cluster.minimumArea, rigid.clusterDemolition.cluster.maximumArea);
                if (EditorGUI.EndChangeCheck() == true)
                    if (Application.isPlaying == true)
                        RFCollapse.AreaCollapse (rigid, rigid.clusterDemolition.cluster.areaCollapse);

                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label ("By Size:", GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clusterDemolition.cluster.sizeCollapse = EditorGUILayout.Slider (rigid.clusterDemolition.cluster.sizeCollapse,
                    rigid.clusterDemolition.cluster.minimumSize, rigid.clusterDemolition.cluster.maximumSize);
                if (EditorGUI.EndChangeCheck() == true)
                    if (Application.isPlaying == true)
                        RFCollapse.SizeCollapse (rigid, rigid.clusterDemolition.cluster.sizeCollapse);

                EditorGUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label ("Random:", GUILayout.Width (55));

                // Start check for slider change
                EditorGUI.BeginChangeCheck();
                rigid.clusterDemolition.cluster.randomCollapse = EditorGUILayout.IntSlider (rigid.clusterDemolition.cluster.randomCollapse, 0, 100);
                if (EditorGUI.EndChangeCheck() == true)
                    RFCollapse.RandomCollapse (rigid, rigid.clusterDemolition.cluster.randomCollapse);

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button ("Start Collapse", GUILayout.Height (25)))
                    if (Application.isPlaying)
                        foreach (var targ in targets)
                            if (targ as RayfireRigid != null)
                                RFCollapse.StartCollapse (targ as RayfireRigid);
            }
        }
        
        void ClusterPreviewUI(RayfireRigid scr)
        {
            if (rigid.objectType == ObjectType.ConnectedCluster)
            {
                GUILayout.BeginHorizontal();

                // Show nodes
                EditorGUI.BeginChangeCheck();
                scr.clusterDemolition.cn = GUILayout.Toggle (scr.clusterDemolition.cn, "Show Connections",   "Button", GUILayout.Height (22));
                scr.clusterDemolition.nd = GUILayout.Toggle (scr.clusterDemolition.nd, "    Show Nodes    ", "Button", GUILayout.Height (22));
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (Object targ in targets)
                        if (targ as RayfireRigid != null)
                        {
                            (targ as RayfireRigid).clusterDemolition.cn = rigid.clusterDemolition.cn;
                            (targ as RayfireRigid).clusterDemolition.nd = rigid.clusterDemolition.nd;
                            SetDirty (targ as RayfireRigid); 
                        }
                    SceneView.RepaintAll();
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Draw
        /// /////////////////////////////////////////////////////////

        [DrawGizmo (GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void DrawGizmosSelected (RayfireRigid targ, GizmoType gizmoType)
        {
            // Missing shards
            if (RFCluster.IntegrityCheck (targ.clusterDemolition.cluster) == false)
                Debug.Log (rfRig + targ.name + misShards, targ.gameObject);
            
            ClusterDraw (targ);
        }
        
        // CLuster connection and nodes viewport preview
        static void ClusterDraw(RayfireRigid targ)
        {
            if (targ.objectType == ObjectType.ConnectedCluster)
            {
                // Damage style
                damageStyle.fontSize         = 15;
                damageStyle.normal.textColor = Color.red;
                
                if (targ.clusterDemolition.cluster != null && targ.clusterDemolition.cluster.shards.Count > 0)
                {
                    // Reinit connections
                    if (targ.clusterDemolition.cluster.initialized == false)
                        RFCluster.InitCluster (targ, targ.clusterDemolition.cluster);
                    
                    // Draw
                    for (int i = 0; i < targ.clusterDemolition.cluster.shards.Count; i++)
                    {
                        if (targ.clusterDemolition.cluster.shards[i].tm != null)
                        {
                            // Damage
                            if (targ.damage.shr == true)
                            {
                                if (targ.clusterDemolition.cluster.shards[i].dm > 0)
                                {
                                    Vector3 pos = targ.clusterDemolition.cluster.shards[i].tm.position;
                                    Handles.Label (pos, targ.clusterDemolition.cluster.shards[i].dm.ToString ("F1"), damageStyle);
                                }
                            }

                            // Set color
                            if (targ.clusterDemolition.cluster.shards[i].uny == false)
                            {
                                Gizmos.color = targ.clusterDemolition.cluster.shards[i].nIds.Count > 0 
                                    ? Color.blue 
                                    : Color.gray;
                            }
                            else
                                Gizmos.color = targ.clusterDemolition.cluster.shards[i].act == true ? Color.magenta : Color.red;

                            // Nodes
                            if (targ.clusterDemolition.nd == true) 
                                Gizmos.DrawWireSphere (targ.clusterDemolition.cluster.shards[i].tm.position, targ.clusterDemolition.cluster.shards[i].sz / 12f);
                            
                            // Connections
                            if (targ.clusterDemolition.cn == true)
                                if (targ.clusterDemolition.cluster.shards[i].neibShards != null)
                                    for (int j = 0; j < targ.clusterDemolition.cluster.shards[i].neibShards.Count; j++)
                                        if (targ.clusterDemolition.cluster.shards[i].neibShards[j].tm != null)
                                            Gizmos.DrawLine (targ.clusterDemolition.cluster.shards[i].tm.position, targ.clusterDemolition.cluster.shards[i].neibShards[j].tm.position);
                        }
                    }
                }
            }
        }
        
        /// /////////////////////////////////////////////////////////
        /// Common
        /// /////////////////////////////////////////////////////////

        void SetDirty (RayfireRigid scr)
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