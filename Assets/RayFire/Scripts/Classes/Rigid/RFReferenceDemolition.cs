using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RayFire
{
    [Serializable]
    public class RFReferenceDemolition
    {
        public enum ActionType
        {
            Instantiate = 0,
            SetActive   = 1
        }

        // UI
        public GameObject       reference;
        public List<GameObject> randomList;
        public ActionType       action;
        public bool             addRigid;
        public bool             inheritScale;
        public bool             inheritMaterials;
        
        /// /////////////////////////////////////////////////////////
        /// Constructor
        /// /////////////////////////////////////////////////////////
        
        // Constructor
        public RFReferenceDemolition()
        {
            InitValues();
        }
        
        void InitValues()
        {
            reference        = null;
            randomList       = null;
            addRigid         = true;
            inheritScale     = true;
            inheritMaterials = false;
        }
        
        // Pool Reset
        public void GlobalReset()
        {
            InitValues();
        }
        
        // Copy from
        public void CopyFrom (RFReferenceDemolition source)
        {
            reference        = source.reference;
            randomList       = source.randomList;
            addRigid         = source.addRigid;
            inheritScale     = source.inheritScale;
            inheritMaterials = source.inheritMaterials;
        }
        
        /// /////////////////////////////////////////////////////////
        /// Methods
        /// /////////////////////////////////////////////////////////   
        
        public bool HasRandomRefs { get { return randomList != null && randomList.Count > 0; } }
        
        // Get reference
        public GameObject GetReference()
        {
            // Return reference if action type is SetActive
            if (action == ActionType.SetActive)
            {
                // Reference not defined or destroyed
                if (reference == null)
                    return null;
                
                // Reference is prefab asset
                if (reference.scene.rootCount == 0)
                    return null;
                
                return reference;
            }

            // Return single ref
            if (reference != null && HasRandomRefs == false)
                return reference;
            
            // Get random ref
            if (randomList.Count > 0)
                return randomList[Random.Range (0, randomList.Count)];
            
            return null;
        }
        
        // Demolish object to reference
        public static bool DemolishReference (RayfireRigid scr)
        {
            if (scr.demolitionType == DemolitionType.ReferenceDemolition)
            {
                // Demolished
                scr.limitations.demolished = true;
                
                // Turn off original
                scr.gameObject.SetActive (false);
                
                // Get reference
                GameObject refGo = scr.referenceDemolition.GetReference();

                // Has no reference
                if (refGo == null)
                    return true;

                // Check if reference has already initialized Rigid
                RayfireRigid refScr = refGo.gameObject.GetComponent<RayfireRigid>();
                if (refScr != null && refScr.initialized == true)
                {
                    if (RayfireMan.debugStatic == true)
                        Debug.Log (RFLimitations.rigidStr + scr.name + " Demolition Reference object has already initialized Rigid. Set By Method Initialization type or Deactivate reference.", scr.gameObject);
                    return true;
                }
                
                // Set object to swap
                GameObject instGo = GetInstance (scr, refGo);

                // Set root to manager or to the same parent
                RayfireMan.SetParentByManager (instGo.transform, scr.transForm);
                
                // Set tm
                scr.rootChild = instGo.transform;
                
                // Copy scale
                if (scr.referenceDemolition.inheritScale == true)
                    scr.rootChild.localScale = scr.transForm.localScale;

                // Inherit materials
                InheritMaterials (scr, instGo);

                // Clear list for fragments
                scr.fragments = new List<RayfireRigid>();
                
                // Check root for rigid props
                RayfireRigid instScr = instGo.gameObject.GetComponent<RayfireRigid>();

                // Reference Root has not rigid. Add to
                if (instScr == null && scr.referenceDemolition.addRigid == true)
                {
                    // Add rigid and copy
                    instScr = instGo.gameObject.AddComponent<RayfireRigid>();

                    // Copy rigid
                    scr.CopyPropertiesTo (instScr);

                    // Copy particles from demolished rigid to instanced rigid
                    RFPoolingParticles.CopyParticlesRigid (scr, instScr);   
                    
                    // Set fragments sim type
                    RFDemolitionMesh.SetFragmentSimulationType (instScr, scr);

                    // Single mesh
                    if (instGo.transform.childCount == 0)
                    {
                        instScr.objectType = ObjectType.Mesh;
                    }

                    // Multiple meshes
                    if (instGo.transform.childCount > 0)
                    {
                        instScr.objectType = ObjectType.MeshRoot;
                    }
                }

                // Activate and init rigid
                instGo.transform.gameObject.SetActive (true);

                // Reference has rigid
                if (instScr != null)
                {
                    // Init if not initialized yet
                    instScr.Initialize();
                    
                    // Create rigid for root children
                    if (instScr.objectType == ObjectType.MeshRoot)
                    {
                        // Collect referenced fragments
                        scr.fragments.AddRange (instScr.fragments);
                    }

                    // Get ref rigid
                    else if (instScr.objectType == ObjectType.Mesh || instScr.objectType == ObjectType.SkinnedMesh)
                    {
                        // Disable runtime caching
                        instScr.meshDemolition.ch.tp = CachingType.Disable;
                        
                        // Instance has no meshes
                        if (instScr.meshFilter == null && instScr.skr == null)
                            return true;

                        // Demolish mesh instance
                        RFDemolitionMesh.DemolishMesh(instScr);
                        
                        // Collect fragments
                        if (instScr.HasFragments == true)
                            scr.fragments.AddRange (instScr.fragments);
                        
                        // Destroy instance
                        RayfireMan.DestroyFragment (instScr, instScr.rootParent, 1f);
                    }

                    // Get ref rigid
                    else if (instScr.objectType == ObjectType.NestedCluster || instScr.objectType == ObjectType.ConnectedCluster)
                    {
                        instScr.Default();
                        
                        // Copy contact data
                        instScr.limitations.contactPoint   = scr.limitations.contactPoint;
                        instScr.limitations.contactVector3 = scr.limitations.contactVector3;
                        instScr.limitations.contactNormal  = scr.limitations.contactNormal;
                        
                        // Demolish
                        RFDemolitionCluster.DemolishCluster (instScr);
                        
                        // Collect new fragments
                        scr.fragments.AddRange (instScr.fragments);
                        
                        // Collect demolished cluster
                        if (instScr.clusterDemolition.cluster.shards.Count > 0)
                            scr.fragments.Add (instScr);
                    }
                }

                else
                {
                    Rigidbody rb = instGo.GetComponent<Rigidbody>();
                    if (rb != null && scr.physics.rigidBody != null)
                    {
                        rb.linearVelocity        = scr.physics.rigidBody.linearVelocity;
                        rb.angularVelocity = scr.physics.rigidBody.angularVelocity;
                    }
                }
            }

            return true;
        }

        // Get final instance accordingly to action type
        static GameObject GetInstance (RayfireRigid scr, GameObject refGo)
        {
            GameObject instGo;
            
            // Instantiate turned off reference with null parent
            if (scr.referenceDemolition.action == ActionType.Instantiate)
            {
                instGo = Object.Instantiate (refGo, scr.transForm.position, scr.transForm.rotation);
                instGo.name = refGo.name;
            }
                
            // Set active
            else
            {
                instGo                    = refGo;
                instGo.transform.position = scr.transForm.position;
                instGo.transform.rotation = scr.transForm.rotation;
            }
            return instGo;
        }

        // Inherit materials from original object to referenced fragments
        static void InheritMaterials (RayfireRigid scr, GameObject instGo)
        {
            if (scr.referenceDemolition.inheritMaterials == true)
            {
                Renderer[] renderers = instGo.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                    for (int r = 0; r < renderers.Length; r++)
                    {
                        int min = Math.Min (scr.meshRenderer.sharedMaterials.Length, renderers[r].sharedMaterials.Length);
                        for (int m = 0; m < min; m++)
                            renderers[r].sharedMaterials[m] = scr.meshRenderer.sharedMaterials[m];
                    }
            }
        }
    }
}