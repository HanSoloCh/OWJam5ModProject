using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class ProxyMeshCreator : MonoBehaviour
    {
        [Header("Parameters")]
        public Transform parent;
        public Mesh[] meshes;
        public GameObject newPrefab;
        public Transform newParent;
        public bool Use;
        [Header("Transfer original (no undo, be careful)")]
        public bool TransferOldToOther;
        public Transform otherParent;

        public void OnDrawGizmosSelected()
        {
            if (Use)
            {
                Use = false;
                MeshFilter[] r = parent.GetComponentsInChildren<MeshFilter>();
                foreach(MeshFilter f in r)
                {
                    bool hasMesh = false;
                    foreach(Mesh m in meshes)
                    {
                        if (f.sharedMesh == m) hasMesh = true;
                    }
                    if (hasMesh)
                    {
                        GameObject newObject = Instantiate(newPrefab);
                        newObject.transform.parent = newParent;
                        newObject.transform.position = f.transform.position;
                        newObject.transform.rotation = f.transform.rotation;
                        newObject.transform.localScale = f.transform.localScale;
                        if (TransferOldToOther) f.transform.parent = otherParent;
                    }
                }
            }
        }
    }
}
