using NewHorizons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Components.Orbital;
using HarmonyLib;
using NewHorizons.External;

namespace OWJam5ModProject
{
    public class GrandOrreryController : MonoBehaviour
    {
        public INewHorizons NewHorizons;
        string systemName = "Jam5";

        public class SystemAndBodies
        {
            public NHAstroObject centerBody;
            public NHAstroObject[] childBodies;
        }
        public SystemAndBodies[] systems;

        public void Start()
        {
            IModHelper helper = OWJam5ModProject.Instance.ModHelper;

            OWJam5ModProject.DebugLog("IT ACTUALLY GOT UPDATED!!!");

            Apply(Main.BodyDict[systemName]);

            







            foreach(SystemAndBodies s in systems)
            {
                OWJam5ModProject.DebugLog("found" + s.centerBody.name.ToString());
                foreach(NHAstroObject b in s.childBodies)
                {
                    OWJam5ModProject.DebugLog("has child" + b.name.ToString());
                }
            }

        }

        public void CreateOrbs()
        {

        }

        public static void Apply(IEnumerable<NewHorizonsBody> bodies)
        {
            foreach(NewHorizonsBody body in bodies)
            {
                string uniquename = body.Mod.ModHelper.Manifest.UniqueName;
            }
        }
    }
}
