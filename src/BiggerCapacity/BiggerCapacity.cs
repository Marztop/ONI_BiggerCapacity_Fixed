using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using TUNING;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BiggerCapacity;

public partial class BiggerCapacity : UserMod2
{
	[HarmonyPatch(typeof(Game), "OnPrefabInit")]
	private static class Game_OnPrefabInit_Patch
	{
		private static void Postfix(Game __instance)
		{
			try
			{
				// Keep the OnLoad snapshot: all settings take effect together on restart.
				Traverse.Create((object)Game.Instance.gasConduitFlow).Field("MaxMass").SetValue((object)(config.PipeMultiplier * config.Capacity.GasPipe));
				Debug.Log((object)$"[BiggerCapacity] GasPipe - multiplier {config.PipeMultiplier}, new capacity {config.PipeMultiplier * config.Capacity.GasPipe}");
				Traverse.Create((object)Game.Instance.liquidConduitFlow).Field("MaxMass").SetValue((object)(config.PipeMultiplier * config.Capacity.LiquidPipe));
				Debug.Log((object)$"[BiggerCapacity] LiquidPipe - multiplier {config.PipeMultiplier}, new capacity {config.PipeMultiplier * config.Capacity.LiquidPipe}");
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(ValveBase), "OnSpawn")]
	private static class ValveBase_OnSpawn_Patch
	{
		private static void Postfix(ValveBase __instance)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Invalid comparison between Unknown and I4
			try
			{
				if ((int)__instance.conduitType == 2)
				{
					float num = Mathf.Clamp01(__instance.CurrentFlow / __instance.maxFlow);
					__instance.maxFlow = config.PipeMultiplier * config.Capacity.LiquidPipe;
					__instance.CurrentFlow = __instance.maxFlow * num;
					Debug.Log((object)$"[BiggerCapacity] LiquidValve - multiplier {config.PipeMultiplier}, new max flow {config.PipeMultiplier * config.Capacity.LiquidPipe}");
				}
				else if ((int)__instance.conduitType == 1)
				{
					float num2 = Mathf.Clamp01(__instance.CurrentFlow / __instance.maxFlow);
					__instance.maxFlow = config.PipeMultiplier * config.Capacity.GasPipe;
					__instance.CurrentFlow = __instance.maxFlow * num2;
					Debug.Log((object)$"[BiggerCapacity] GasValve - multiplier {config.PipeMultiplier}, new max flow {config.PipeMultiplier * config.Capacity.GasPipe}");
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(Battery), "OnSpawn")]
	private static class Battery_OnSpawn_Patch
	{
		private static void Postfix(Battery __instance)
		{
			Building component = ((Component)__instance).GetComponent<Building>();
			if (!component)
			{
				return;
			}
			string prefabID = ((Def)component.Def).PrefabID;
			try
			{
				if (!(prefabID == "PowerTransformer") && !(prefabID == "PowerTransformerSmall"))
				{
					__instance.capacity *= config.BatteryMultiplier;
					Debug.Log((object)$"[BiggerCapacity] {prefabID} - multiplier {config.BatteryMultiplier}, new capacity {__instance.capacity}");
				}
			}
			catch (Exception ex)
			{
				Debug.Log((object)("[BiggerCapacity] " + prefabID + " - " + ex.Message));
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(PowerTransformerConfig), "CreateBuildingDef")]
	private static class PowerTransformerConfig_CreateBuildingDef_Patch
	{
		private static void Postfix(PowerTransformerConfig __instance, BuildingDef __result)
		{
			try
			{
				__result.GeneratorWattageRating *= config.TransformerMultiplier;
				__result.GeneratorBaseCapacity *= config.TransformerMultiplier;
				Debug.Log((object)$"[BiggerCapacity] PowerTransformer - multiplier {config.TransformerMultiplier}, new capacity {__result.GeneratorBaseCapacity}");
			}
			catch (Exception ex)
			{
				Debug.Log((object)("[BiggerCapacity] PowerTransformer - " + ex.Message));
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(PowerTransformerSmallConfig), "CreateBuildingDef")]
	private static class PowerTransformerSmallConfig_CreateBuildingDef_Patch
	{
		private static void Postfix(PowerTransformerSmallConfig __instance, BuildingDef __result)
		{
			try
			{
				__result.GeneratorWattageRating *= config.TransformerMultiplier;
				__result.GeneratorBaseCapacity *= config.TransformerMultiplier;
				Debug.Log((object)$"[BiggerCapacity] PowerTransformerSmall - multiplier {config.TransformerMultiplier}, new capacity {__result.GeneratorBaseCapacity}");
			}
			catch (Exception ex)
			{
				Debug.Log((object)("[BiggerCapacity] PowerTransformerSmall - " + ex.Message));
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(Storage), "OnSpawn")]
	private static class Storage_OnSpawn_Patch
	{
		private static void Postfix(Storage __instance)
		{
			Building component = ((Component)__instance).GetComponent<Building>();
			if (!component)
			{
				return;
			}
			string prefabID = ((Def)component.Def).PrefabID;
			try
			{
				float num = config.OtherMultiplier;
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 99999f;
				bool flag = false;
				switch (prefabID)
				{
				case "StorageLockerSmart":
				case "ObjectDispenser":
				case "StorageLocker":
					num = config.StorageMultiplier;
					num2 = (float)config.Capacity.Locker * num;
					break;
				case "LiquidReservoir":
					num = config.StorageMultiplier;
					num3 = (float)config.Capacity.LiquidReservoir * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "GasReservoir":
					num = config.StorageMultiplier;
					num3 = (float)config.Capacity.GasReservoir * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "RationBox":
					num = config.StorageMultiplier;
					num2 = (float)config.Capacity.RationBox * num;
					break;
				case "Refrigerator":
					num = config.StorageMultiplier;
					num2 = (float)config.Capacity.Refrigerator * num;
					break;
				case "MiniFridge":
					num = config.StorageMultiplier;
					num2 = config.Capacity.MiniFridge * num;
					break;
				case "StorageTile":
					num = config.StorageMultiplier;
					num2 = config.Capacity.StorageTile * num;
					break;
				case "WoodStorage":
					num = config.StorageMultiplier;
					num2 = (float)config.Capacity.WoodStorage * num;
					break;
				case "LiquidFuelTank":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "LiquidFuelTankCluster":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "OxidizerTankLiquid":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "OxidizerTankLiquidCluster":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "OxidizerTank":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "SmallOxidizerTank":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "OxidizerTankCluster":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "CargoBay":
					num = config.RocketCargoMultiplier;
					num2 = (float)config.Capacity.CargoBay * num;
					break;
				case "GasCargoBay":
				case "LiquidCargoBay":
					num = config.RocketCargoMultiplier;
					num3 = (float)config.Capacity.CargoBay * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "SpecialCargoBay":
					num = config.RocketCargoMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "SolidConduitInbox":
					num = config.ConveyorMultiplier;
					num2 = (float)config.Capacity.ConveyorInbox * num;
					break;
				case "SolidConduitOutbox":
					num = config.ConveyorMultiplier;
					num2 = (float)config.Capacity.ConveyorOutbox * num;
					break;
				case "SugarEngine":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "CO2Engine":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "HEPEngine":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = 4800f * num;
					break;
				case "KeroseneEngineClusterSmall":
					flag = true;
					num = config.RocketFuelMultiplier;
					num2 = __instance.capacityKg * num;
					break;
				case "SteamEngine":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "SteamEngineCluster":
					flag = true;
					num = config.RocketFuelMultiplier;
					num3 = __instance.capacityKg * num;
					num3 = Mathf.Min(num3, num4);
					break;
				case "HydrogenEngine":
					return;
				case "KeroseneEngine":
					return;
				case "HydrogenEngineCluster":
					return;
				case "KeroseneEngineCluster":
					return;
				case "GasPump":
				case "GasMiniPump":
				case "LiquidPump":
				case "LiquidMiniPump":
					// PumpFlowPatch already sizes the buffer for the configured flow.
					return;
				default:
					if (num == 1f)
					{
						return;
					}
					num2 = __instance.capacityKg * num;
					break;
				}
				if (num2 > 0f || num3 > 0f)
				{
					num2 = Mathf.Max(num2, num3);
					IUserControlledCapacity component2 = ((Component)__instance).GetComponent<IUserControlledCapacity>();
					if (component2 != null)
					{
						__instance.capacityKg = num2;
						component2.UserMaxCapacity = Mathf.Min(num2, component2.UserMaxCapacity);
						if (flag)
						{
							FuelTank component3 = ((Component)__instance).GetComponent<FuelTank>();
							if (component3)
							{
								component3.physicalFuelCapacity = num2;
							}
							OxidizerTank component4 = ((Component)__instance).GetComponent<OxidizerTank>();
							if (component4)
							{
								component4.maxFillMass = num2;
							}
							HEPFuelTank component5 = ((Component)__instance).GetComponent<HEPFuelTank>();
							if (component5)
							{
								component5.physicalFuelCapacity = num2;
							}
						}
					}
					else
					{
						__instance.capacityKg = num2;
					}
					Debug.Log((object)$"[BiggerCapacity] {prefabID} - multiplier {num}, new capacity {num2}");
				}
				if (num3 > 0f)
				{
					ConduitConsumer component6 = ((Component)__instance).GetComponent<ConduitConsumer>();
					if (component6)
					{
						component6.capacityKG = num3;
					}
				}
			}
			catch (Exception ex)
			{
				Debug.Log((object)("[BiggerCapacity] " + prefabID + " - " + ex.Message));
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(LogicPorts), "SendSignal")]
	private static class LogicPorts_SendSignal_Patch
	{
		private static bool Prefix(LogicPorts __instance)
		{
			if (__instance.outputPorts == null)
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Wire), "GetMaxWattageAsFloat")]
	private static class Wire_GetMaxWattageAsFloat_Patch
	{
		private static void Postfix(ref float __result)
		{
			try
			{
				__result *= config.WireMultiplier;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(Generator), "WattageRating", MethodType.Getter)]
	private static class Generator_WattageRating_Patch
	{
		private static void Postfix(Generator __instance, ref float __result)
		{
			// Transformers have their own rating multiplier at CreateBuildingDef.
			if (__instance is PowerTransformer)
			{
				return;
			}

			try
			{
				__result *= config.GeneratorMultiplier;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(SolidConduitDispenser), "ConduitUpdate")]
	private static class SolidConduitDispenser_ConduitUpdate_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Ldc_R4 && (float)list[i].operand == 20f)
				{
					list[i].operand = (float)config.Capacity.ConveyorRails * config.ConveyorMultiplier;
					Debug.Log((object)$"[BiggerCapacity] Conveyor - multiplier {config.ConveyorMultiplier}, new capacity {list[i].operand}");
				}
			}
			return list.AsEnumerable();
		}
	}

	[HarmonyPatch(typeof(ElementSplitterComponents), "CanFirstAbsorbSecond")]
	private static class ElementSplitterComponents_CanFirstAbsorbSecond_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> list = new List<CodeInstruction>(instructions);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].opcode == OpCodes.Ldc_R4 && (float)list[i].operand == 25000f)
				{
					list[i].operand = 99999f;
					Debug.Log((object)$"[BiggerCapacity] Elements stacking changed to {list[i].operand} kg");
				}
			}
			return list.AsEnumerable();
		}
	}

	[HarmonyPatch(typeof(TableSaltConfig), "CreatePrefab")]
	private static class TableSaltConfig_CreatePrefab_Patch
	{
		private static void Postfix(TableSaltConfig __instance, GameObject __result)
		{
			try
			{
				__result.GetComponent<EntitySplitter>().maxStackSize = 99999f;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	[HarmonyPatch(typeof(RotPileConfig), "CreatePrefab")]
	private static class RotPileConfig_CreatePrefab_Patch
	{
		private static void Postfix(RotPileConfig __instance, GameObject __result)
		{
			try
			{
				__result.GetComponent<EntitySplitter>().maxStackSize = 99999f;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	private const int CONFIG_VERSION = 9;

	public static Config config;

	public static string pathToMod;

	public static string pathToConfig;

	public static bool alreadyOpenedFileLog;

	public override void OnLoad(Harmony harmony)
	{
		PUtil.InitLibrary();
		ConfigStrings.RegisterDefaults();
		pathToMod = ((UserMod2)this).path;
		// Use the same path as PLib's options dialog to avoid divergent config files.
		pathToConfig = Path.GetFullPath(POptions.GetConfigFilePath(typeof(Config)));
		Directory.CreateDirectory(Path.GetDirectoryName(pathToConfig));
		LoadConfig();
		new POptions().RegisterOptions(this, typeof(Config));
		ROCKETRY.CARGO_CAPACITY_SCALE *= config.RocketCargoMultiplier;
		Debug.Log((object)$"[BiggerCapacity] RocketCargo - multiplier {config.RocketCargoMultiplier}");
		harmony.PatchAll();
		AnnouncePatches(harmony);
	}

	public static void AnnouncePatches(Harmony harmony)
	{
		Debug.Log((object)"[BiggerCapacity] Announce harmony patches");
		foreach (MethodBase patchedMethod in harmony.GetPatchedMethods())
		{
			Debug.Log((object)("[BiggerCapacity] -> " + patchedMethod.DeclaringType.FullName + "." + patchedMethod.Name));
		}
	}
}
