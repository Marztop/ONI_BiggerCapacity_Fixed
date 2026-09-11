using System;
using System.IO;
using KMod;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace BiggerCapacity;

public partial class BiggerCapacity
{
	private static Config LoadConfig()
	{
		JObject ownJson = ReadConfigObject(pathToConfig);
		if (ownJson != null && ownJson.HasValues)
		{
			// Once our file has settings, never import from the original mod again.
			config = ownJson.ToObject<Config>();
			bool needsSave = config.version != CONFIG_VERSION || config.Capacity == null;
			config.Capacity = config.Capacity ?? new Config.Capacities();
			if (!ownJson.TryGetValue(nameof(Config.TransformerMultiplier),
				StringComparison.OrdinalIgnoreCase, out _))
			{
				config.TransformerMultiplier = config.WireMultiplier;
				needsSave = true;
			}
			config.version = CONFIG_VERSION;
			if (needsSave)
				SaveConfig();
			return config;
		}

		// The property initializers are the template, also used by PLib's defaults.
		// Overlay only recognized, usable numbers; keep defaults for missing fields.
		JObject template = JObject.FromObject(new Config());
		string legacyPath = Path.Combine(Manager.GetDirectory(), "settings", "BiggerCapacity", "config.json");
		try
		{
			JObject legacyJson = ReadConfigObject(legacyPath);
			if (legacyJson != null && legacyJson.HasValues)
			{
				InheritNumbers(template, legacyJson);
				if (!legacyJson.TryGetValue(nameof(Config.TransformerMultiplier),
					StringComparison.OrdinalIgnoreCase, out JToken transformer)
					|| !TryReadNumber(transformer, false, out _))
				{
					template[nameof(Config.TransformerMultiplier)] =
						template[nameof(Config.WireMultiplier)].DeepClone();
				}
				Debug.Log("[BiggerCapacity] Inherited original mod settings into the new config.");
			}
		}
		catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is JsonException)
		{
			// A damaged/unavailable legacy file must not prevent creating our config.
			Debug.LogWarning("[BiggerCapacity] Could not import original config; using defaults: " + error.Message);
		}
		config = template.ToObject<Config>();
		config.version = CONFIG_VERSION;
		SaveConfig();
		return config;
	}

	private static JObject ReadConfigObject(string path)
	{
		if (!File.Exists(path))
			return null;
		string content = File.ReadAllText(path);
		if (string.IsNullOrWhiteSpace(content))
			return null;
		JToken token = JToken.Parse(content);
		if (token.Type == JTokenType.Null)
			return null;
		if (token is JObject json)
			return json;
		// Invalid non-empty *own* files are reported, not overwritten or reimported.
		throw new JsonSerializationException("Config must be a JSON object: " + path);
	}

	private static void InheritNumbers(JObject template, JObject source)
	{
		foreach (JProperty property in template.Properties())
		{
			if (property.Name == nameof(Config.version)
				|| !source.TryGetValue(property.Name, StringComparison.OrdinalIgnoreCase, out JToken value))
				continue;
			if (property.Value is JObject child)
			{
				if (value is JObject sourceChild)
					InheritNumbers(child, sourceChild);
				continue;
			}
			bool integer = property.Value.Type == JTokenType.Integer;
			if (TryReadNumber(value, integer, out double number))
				property.Value = integer ? new JValue((int)number) : new JValue((float)number);
			else
				Debug.LogWarning("[BiggerCapacity] Ignoring invalid original config value: " + property.Name);
		}
	}

	private static bool TryReadNumber(JToken value, bool integer, out double number)
	{
		number = 0;
		if (value == null || (value.Type != JTokenType.Integer && value.Type != JTokenType.Float))
			return false;
		try
		{
			number = value.Value<double>();
			if (double.IsNaN(number) || double.IsInfinity(number) || number <= 0)
				return false;
			return integer
				? number <= int.MaxValue && number == Math.Truncate(number)
				: number <= float.MaxValue && (float)number > 0f;
		}
		catch (Exception error) when (error is OverflowException || error is InvalidCastException || error is FormatException)
		{
			return false;
		}
	}

	private static void SaveConfig()
	{
		Directory.CreateDirectory(Path.GetDirectoryName(pathToConfig));
		File.WriteAllText(pathToConfig, JsonConvert.SerializeObject(config, Formatting.Indented));
	}
}
