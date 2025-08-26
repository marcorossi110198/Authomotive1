using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Speedometer Feature
	/// IDENTICA al pattern IClusterDriveModeFeature seguendo modello Mercedes
	/// Definisce COSA può fare la feature (contratto pubblico)
	/// </summary>
	
	public interface ISpeedometerFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del Mercedes
		/// </summary>
		Client GetClient();

		/// <summary>
		/// Ottiene configurazione corrente speedometer
		/// </summary>
		SpeedometerConfig GetCurrentConfiguration();

		/// <summary>
		/// Ottiene unità di misura corrente
		/// </summary>
		SpeedometerData.SpeedUnit GetCurrentSpeedUnit();
	}
}