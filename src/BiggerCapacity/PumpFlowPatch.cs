using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace BiggerCapacity;

[HarmonyPatch]
internal static class PumpFlowPatch
{
	private static IEnumerable<MethodBase> TargetMethods()
	{
		yield return AccessTools.Method(typeof(GasPumpConfig), "DoPostConfigureComplete");
		yield return AccessTools.Method(typeof(GasMiniPumpConfig), "DoPostConfigureComplete");
		yield return AccessTools.Method(typeof(LiquidPumpConfig), "DoPostConfigureComplete");
		yield return AccessTools.Method(typeof(LiquidMiniPumpConfig), "DoPostConfigureComplete");
	}

	private static void Postfix(GameObject go)
	{
		string id = go.GetComponent<Building>().Def.PrefabID;
		float rate = GetRate(id, BiggerCapacity.config);
		// Configure the prefab before ElementConsumer registers with the simulation.
		// All four vanilla pumps have storage equal to two seconds of consumption.
		go.GetComponent<ElementConsumer>().consumptionRate = rate;
		go.GetComponent<Storage>().capacityKg = rate * 2f;
	}

	internal static float GetRate(string id, BiggerCapacity.Config config)
	{
		switch (id)
		{
		case GasPumpConfig.ID:
			return config.Capacity.GasPump * config.GasPumpMultiplier;
		case GasMiniPumpConfig.ID:
			return config.Capacity.GasMiniPump * config.GasPumpMultiplier;
		case LiquidPumpConfig.ID:
			return config.Capacity.LiquidPump * config.LiquidPumpMultiplier;
		case LiquidMiniPumpConfig.ID:
			return config.Capacity.LiquidMiniPump * config.LiquidPumpMultiplier;
		default:
			throw new ArgumentException("Unsupported pump: " + id, nameof(id));
		}
	}
}
