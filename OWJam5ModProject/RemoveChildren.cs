using UnityEngine;

namespace OWJam5ModProject
{
    internal class RemoveChildren : MonoBehaviour
    {
        [SerializeField] string[] children;

        void Start()
        {
            foreach (string childPath in children)
            {
                Transform child = transform.Find(childPath);
                Destroy(child.gameObject);
            }
        }
    }
}
