using HarmonyLib;

namespace BiggerCapacity;

[HarmonyPatch(typeof(Generator), nameof(Generator.CalculateCapacity))]
internal static class GeneratorCapacityPatch
{
	private static void Postfix(BuildingDef def, ref float __result)
	{
		// Transformers already scale their base capacity with TransformerMultiplier.
		if (def.PrefabID == "PowerTransformer" || def.PrefabID == "PowerTransformerSmall")
		{
			return;
		}

		// Scale the calculated value, not the shared BuildingDef. OnSpawn (including
		// loading an existing save) can therefore recalculate without compounding.
		__result *= BiggerCapacity.config.GeneratorMultiplier;
	}
}
