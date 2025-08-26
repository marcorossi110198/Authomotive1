using UnityEngine;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Eventi per Speedometer Feature
	/// IDENTICO al pattern ClusterDriveModeEvents seguendo modello Mercedes
	/// </summary>

	/// <summary>
	/// Evento cambio unità velocità
	/// </summary>
	public class SpeedUnitChangedEvent
	{
		public SpeedometerData.SpeedUnit NewUnit { get; }
		public SpeedometerData.SpeedUnit PreviousUnit { get; }

		public SpeedUnitChangedEvent(SpeedometerData.SpeedUnit newUnit, SpeedometerData.SpeedUnit previousUnit)
		{
			NewUnit = newUnit;
			PreviousUnit = previousUnit;
		}
	}

	/// <summary>
	/// Evento configurazione speedometer aggiornata
	/// </summary>
	public class SpeedometerConfigUpdatedEvent
	{
		public SpeedometerConfig NewConfiguration { get; }
		public ClusterAudi.DriveMode DriveMode { get; }

		public SpeedometerConfigUpdatedEvent(SpeedometerConfig config, ClusterAudi.DriveMode mode)
		{
			NewConfiguration = config;
			DriveMode = mode;
		}
	}

	/// <summary>
	/// Evento velocità pericolosa raggiunta
	/// </summary>
	public class DangerousSpeedReachedEvent
	{
		public float CurrentSpeed { get; }
		public float DangerThreshold { get; }
		public SpeedometerData.SpeedUnit Unit { get; }

		public DangerousSpeedReachedEvent(float speed, float threshold, SpeedometerData.SpeedUnit unit)
		{
			CurrentSpeed = speed;
			DangerThreshold = threshold;
			Unit = unit;
		}
	}

	/// <summary>
	/// Evento update metriche speedometer (per debug/analytics)
	/// </summary>
	public class SpeedometerMetricsUpdateEvent
	{
		public float CurrentSpeed { get; set; }
		public float DisplayedSpeed { get; set; }  // Smoothed speed
		public float MaxSpeedReached { get; set; }
		public Color CurrentSpeedColor { get; set; }
		public SpeedometerData.SpeedUnit CurrentUnit { get; set; }
		public float ResponseLatency { get; set; }  // Tempo di risposta smoothing
	}

	/// <summary>
	/// Richiesta cambio configurazione speedometer dall'UI
	/// </summary>
	public class SpeedometerConfigChangeRequest
	{
		public string ConfigurationType { get; }
		public object ConfigurationValue { get; }

		public SpeedometerConfigChangeRequest(string type, object value)
		{
			ConfigurationType = type;
			ConfigurationValue = value;
		}
	}
}