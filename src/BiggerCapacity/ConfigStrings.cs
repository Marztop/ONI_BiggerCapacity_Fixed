using System;
using System.IO;
using HarmonyLib;
using UnityEngine;

namespace BiggerCapacity;

internal static class ConfigStrings
{
	[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
	private static class LocalizationInitializePatch
	{
		private static void Postfix()
		{
			try
			{
				string code = Localization.GetLocale()?.Code;
				if (string.IsNullOrEmpty(code)) code = Localization.GetCurrentLanguageCode();
				LoadTranslations(code, BiggerCapacity.pathToMod);
			}
			catch (Exception error)
			{
				Debug.LogException(error);
			}
		}
	}

	internal static void LoadTranslations(string languageCode, string modPath)
	{
		RegisterDefaults();
		if (string.IsNullOrEmpty(languageCode)) return;
		string filename = Path.Combine(modPath, "translations", languageCode + ".po");
		if (!File.Exists(filename)) return;
		// These keys are stored directly in Strings, not in a LocString type tree.
		// Localization.OverloadStrings only updates LocString fields, so apply them here.
		foreach (var entry in Localization.LoadStringsFile(filename, false))
			if (entry.Key.StartsWith("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.", StringComparison.Ordinal))
				Strings.Add(entry.Key, entry.Value);
	}

	// English fallback is restored before each locale load.
	internal static void RegisterDefaults()
	{
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CATEGORIES.POWER", "Power");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CATEGORIES.STORAGE", "Storage and Transport");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CATEGORIES.OTHER", "Rockets and Other");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CATEGORIES.CAPACITY", "Base Capacities");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.WIREMULTIPLIER.NAME", "Wire Load Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.WIREMULTIPLIER.TOOLTIP", "Multiplies the maximum power load of wires. 1 uses the game's original rating.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.BATTERYMULTIPLIER.NAME", "Battery Capacity Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.BATTERYMULTIPLIER.TOOLTIP", "Multiplies regular battery storage capacity. Does not affect batteries inside transformers.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GENERATORMULTIPLIER.NAME", "Generator Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GENERATORMULTIPLIER.TOOLTIP", "Multiplies both generator output and internal energy buffer capacity. Does not affect transformers.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.TRANSFORMERMULTIPLIER.NAME", "Transformer Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.TRANSFORMERMULTIPLIER.TOOLTIP", "Multiplies output power, charging power, and internal capacity for both transformer sizes.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.STORAGEMULTIPLIER.NAME", "Storage Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.STORAGEMULTIPLIER.TOOLTIP", "Applies to storage bins, reservoirs, ration boxes, refrigerators, and wood storage. Final capacity = base capacity × this multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.PIPEMULTIPLIER.NAME", "Pipe Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.PIPEMULTIPLIER.TOOLTIP", "Multiplies capacity per pipe segment and maximum valve flow for liquids and gases.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORMULTIPLIER.NAME", "Conveyor Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORMULTIPLIER.TOOLTIP", "Multiplies conveyor loader, receptacle, and rail packet capacities.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.ROCKETFUELMULTIPLIER.NAME", "Rocket Fuel Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.ROCKETFUELMULTIPLIER.TOOLTIP", "Multiplies fuel and oxidizer storage capacities supported by the original mod.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.ROCKETCARGOMULTIPLIER.NAME", "Rocket Cargo Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.ROCKETCARGOMULTIPLIER.TOOLTIP", "Multiplies rocket cargo bay capacities.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.OTHERMULTIPLIER.NAME", "Other Storage Multiplier");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.OTHERMULTIPLIER.TOOLTIP", "Multiplies internal storage capacities classified as other by the original mod.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CAPACITY.NAME", "Base Capacities");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CAPACITY.TOOLTIP", "The values below are multiplied by their respective category multipliers.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LOCKER.NAME", "Storage Bin (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LOCKER.TOOLTIP", "Base capacity for storage bins, smart storage bins, and automatic dispensers. Multiplied by the storage multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.WOODSTORAGE.NAME", "Wood Storage (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.WOODSTORAGE.TOOLTIP", "Base capacity, multiplied by the storage multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LIQUIDRESERVOIR.NAME", "Liquid Reservoir (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LIQUIDRESERVOIR.TOOLTIP", "Base capacity, multiplied by the storage multiplier. The original mod's 99,999 kg final capacity limit still applies.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GASRESERVOIR.NAME", "Gas Reservoir (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GASRESERVOIR.TOOLTIP", "Base capacity, multiplied by the storage multiplier. The original mod's 99,999 kg final capacity limit still applies.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.RATIONBOX.NAME", "Ration Box (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.RATIONBOX.TOOLTIP", "Base capacity, multiplied by the storage multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.REFRIGERATOR.NAME", "Refrigerator (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.REFRIGERATOR.TOOLTIP", "Base capacity, multiplied by the storage multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.MINIFRIDGE.NAME", "Mini Fridge (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.MINIFRIDGE.TOOLTIP", "Base capacity, multiplied by the same storage multiplier as regular refrigerators.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.STORAGETILE.NAME", "Storage Tile (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.STORAGETILE.TOOLTIP", "Base capacity, multiplied by the storage multiplier. Also sets the capacity slider's upper bound; a lower player-selected limit is preserved.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONDUCTIONPANEL.NAME", "Conduction Panel (kg/s)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONDUCTIONPANEL.TOOLTIP", "Base liquid transfer rate, multiplied by the pipe multiplier. This controls flow, not stored mass or thermal conductivity.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CARGOBAY.NAME", "Cargo Bay (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CARGOBAY.TOOLTIP", "Base capacity, multiplied by the rocket cargo multiplier. Liquid and gas cargo bays retain the original mod's 99,999 kg limit.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORINBOX.NAME", "Conveyor Loader (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORINBOX.TOOLTIP", "Base capacity, multiplied by the conveyor multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYOROUTBOX.NAME", "Conveyor Receptacle (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYOROUTBOX.TOOLTIP", "Base capacity, multiplied by the conveyor multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORRAILS.NAME", "Conveyor Rail Packet (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.CONVEYORRAILS.TOOLTIP", "Base capacity per packet, multiplied by the conveyor multiplier.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LIQUIDPIPE.NAME", "Liquid Pipe (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.LIQUIDPIPE.TOOLTIP", "Base capacity per segment, multiplied by the pipe multiplier. Also controls maximum liquid valve flow.");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GASPIPE.NAME", "Gas Pipe (kg)");
		Strings.Add("STRINGS.BIGGERCAPACITYFIXED.OPTIONS.GASPIPE.TOOLTIP", "Base capacity per segment, multiplied by the pipe multiplier. Also controls maximum gas valve flow.");
	}
}
