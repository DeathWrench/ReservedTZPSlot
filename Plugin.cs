using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using System.Reflection;
using ReservedItemSlotCore.Data;
using ReservedTZPSlot.Config;

namespace ReservedTZPSlot
{
    [BepInPlugin($"{PluginInfo.PLUGIN_GUID}", $"{PluginInfo.PLUGIN_NAME}", $"{PluginInfo.PLUGIN_VERSION}")]
    [BepInDependency("FlipMods.ReservedItemSlotCore", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
		public static Plugin instance;
        static ManualLogSource logger;
		Harmony _harmony;

        public static ReservedItemSlotData tzpInhalantSlotData;
        public static ReservedItemData tzpInhalantData;

        //public static List<ReservedItemData> additionalItemData = new List<ReservedItemData>();

        void Awake()
        {
			instance = this;
            CreateCustomLogger();
			ConfigSettings.BindConfigSettings();

            CreateReservedItemSlots();
            //CreateAdditionalReservedItemSlots();

            _harmony = new Harmony("ReservedTZPSlot");
			PatchAll();
			Log($"{PluginInfo.PLUGIN_GUID} loaded");
		}


        void CreateReservedItemSlots()
        {
            tzpInhalantSlotData = ReservedItemSlotData.CreateReservedItemSlotData("TZP-Inhalant", ConfigSettings.overrideItemSlotPriority.Value, ConfigSettings.overridePurchasePrice.Value);
            tzpInhalantData = tzpInhalantSlotData.AddItemToReservedItemSlot(new ReservedItemData("TZP-Inhalant"));
        }

        
        /*void CreateAdditionalReservedItemSlots()
        {
            string[] additionalItemNames = ConfigSettings.ParseAdditionalItems();
            foreach (string itemName in additionalItemNames)
            {
                if (!tzpInhalantSlotData.ContainsItem(itemName))
                {
                    LogWarning("Adding additional item to reserved item slot. Item: " + itemName);
                    var itemData = new ReservedItemData(itemName);
                    additionalItemData.Add(itemData);
                    tzpInhalantSlotData.AddItemToReservedItemSlot(itemData);
                }
            }
        }*/


        void PatchAll()
        {
            IEnumerable<Type> types;
            try { types = Assembly.GetExecutingAssembly().GetTypes(); }
            catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null); }
            foreach (var type in types)
                this._harmony.PatchAll(type);
        }


        void CreateCustomLogger()
        {
            try { logger = BepInEx.Logging.Logger.CreateLogSource(string.Format("{0}-{1}", Info.Metadata.Name, Info.Metadata.Version)); }
            catch { logger = Logger; }
        }

        public static void Log(string message) => logger.LogInfo(message);
        public static void LogError(string message) => logger.LogError(message);
        public static void LogWarning(string message) => logger.LogWarning(message);

        public static bool IsModLoaded(string guid) => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(guid);
    }
}
