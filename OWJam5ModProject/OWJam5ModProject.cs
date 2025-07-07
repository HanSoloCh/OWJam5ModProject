using System.Reflection;
using HarmonyLib;
using NewHorizons;
using NewHorizons.Components.SizeControllers;
using NewHorizons.Utility;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace OWJam5ModProject
{
    public class OWJam5ModProject : ModBehaviour
    {
        public static OWJam5ModProject Instance;
        public INewHorizons NewHorizons;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(OWJam5ModProject)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("2walker2.OWJam5ModProject").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            NewHorizons.GetStarSystemLoadedEvent().AddListener((string system) => { OnStarSystemLoaded(system); });
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }

        void OnStarSystemLoaded(string system)
        {
            if (system != "Jam5")
                return;

            InitializeFunnels();
        }

        const string SAND_FUNNEL_NAME = "Walker_Jam5_Planet3Funnel_Body";
        const string WATER_FUNNEL_NAME = "Walker_Jam5_Planet4Funnel_Body";
        void InitializeFunnels()
        {
            // Sand funnel
            GameObject sandFunnel = SearchUtilities.Find(SAND_FUNNEL_NAME);
            if (sandFunnel != null)
                sandFunnel.AddComponent<FunnelProximityActivator>();

            // Water funnel
            GameObject waterFunnel = SearchUtilities.Find(WATER_FUNNEL_NAME);
            if (waterFunnel != null)
                waterFunnel.AddComponent<FunnelProximityActivator>();
        }
    }
}
