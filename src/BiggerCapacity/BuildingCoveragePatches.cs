using HarmonyLib;
using UnityEngine;

namespace BiggerCapacity;

[HarmonyPatch(typeof(ContactConductivePipeBridgeConfig), "ConfigureBuildingTemplate")]
internal static class ConductionPanelFlowPatch
{
	private static void Postfix(GameObject go)
	{
		// Configure once on the prefab, not on spawn: instances share this Def.
		var def = go.GetDef<ContactConductivePipeBridge.Def>();
		def.pumpKGRate = BiggerCapacity.config.Capacity.ConductionPanel * BiggerCapacity.config.PipeMultiplier;
	}
}

[HarmonyPatch(typeof(StorageTile.Instance), nameof(StorageTile.Instance.MaxCapacity), MethodType.Getter)]
internal static class StorageTileMaxCapacityPatch
{
	private static void Postfix(ref float __result)
	{
		// Storage.OnSpawn scales the storage itself. Match the capacity control's
		// independent upper bound without modifying the shared Def on every spawn.
		__result = BiggerCapacity.config.Capacity.StorageTile * BiggerCapacity.config.StorageMultiplier;
	}
}
