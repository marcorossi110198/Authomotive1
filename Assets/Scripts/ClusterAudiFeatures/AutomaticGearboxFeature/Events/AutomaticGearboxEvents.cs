using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per AutomaticGearbox Feature
	/// IDENTICO al pattern eventi Mercedes
	/// </summary>

	/// <summary>
	/// Evento per aggiornamento RPM con dettagli estesi
	/// </summary>
	public class AutomaticGearboxRPMUpdateEvent
	{
		public float CurrentRPM { get; set; }
		public float MaxRPM { get; set; }
		public bool IsInRedZone { get; set; }
		public bool IsInWarningZone { get; set; }
		public Color RPMColor { get; set; }
		public DriveMode CurrentDriveMode { get; set; }
	}

	/// <summary>
	/// Evento per cambio marcia con animazione
	/// </summary>
	public class GearChangeEvent
	{
		public int PreviousGear { get; set; }
		public int NewGear { get; set; }
		public bool IsAutomaticShift { get; set; }
		public float TransitionSpeed { get; set; }
	}

	/// <summary>
	/// Evento per red zone warning
	/// </summary>
	public class RedZoneWarningEvent
	{
		public bool IsInRedZone { get; set; }
		public float CurrentRPM { get; set; }
		public float RedZoneThreshold { get; set; }
		public bool EnableFlashEffect { get; set; }
		public string WarningMessage { get; set; }

		public RedZoneWarningEvent(bool isInRedZone, float currentRPM, float threshold)
		{
			IsInRedZone = isInRedZone;
			CurrentRPM = currentRPM;
			RedZoneThreshold = threshold;
			EnableFlashEffect = isInRedZone;
			WarningMessage = isInRedZone ? "RED LINE!" : "";
		}
	}

	/// <summary>
	/// Evento per shift indicator (suggerimenti cambio marcia)
	/// </summary>
	public class ShiftIndicatorEvent
	{
		public bool ShowShiftUp { get; set; }
		public bool ShowShiftDown { get; set; }
		public string ShiftMessage { get; set; }
		public Color IndicatorColor { get; set; }

		public ShiftIndicatorEvent(bool shiftUp, bool shiftDown, string message = "")
		{
			ShowShiftUp = shiftUp;
			ShowShiftDown = shiftDown;
			ShiftMessage = message;
			IndicatorColor = shiftUp ? Color.green : (shiftDown ? Color.yellow : Color.white);
		}
	}

	/// <summary>
	/// Evento per configurazione AutomaticGearbox quando cambia modalità
	/// </summary>
	public class AutomaticGearboxConfigurationEvent
	{
		public AutomaticGearboxConfig Configuration { get; set; }
		public DriveMode DriveMode { get; set; }
		public string ConfigurationName { get; set; }

		public AutomaticGearboxConfigurationEvent(AutomaticGearboxConfig config, DriveMode mode)
		{
			Configuration = config;
			DriveMode = mode;
			ConfigurationName = $"{mode}Mode";
		}
	}

	/// <summary>
	/// Evento per performance metrics del motore
	/// </summary>
	public class EnginePerformanceEvent
	{
		public float PowerOutput { get; set; }    // kW
		public float TorqueOutput { get; set; }   // Nm
		public float Efficiency { get; set; }     // 0-1
		public float Temperature { get; set; }    // °C
		public bool IsOverheating { get; set; }

		public EnginePerformanceEvent(float power, float torque, float efficiency)
		{
			PowerOutput = power;
			TorqueOutput = torque;
			Efficiency = efficiency;
			Temperature = 90f; // Default engine temp
			IsOverheating = Temperature > 110f;
		}
	}
}