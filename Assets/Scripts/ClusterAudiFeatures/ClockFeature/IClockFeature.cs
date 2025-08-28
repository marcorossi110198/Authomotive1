using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Clock Feature - SEMPLICE
	/// IDENTICA al pattern ISeatBeltFeature che hai già implementato
	/// </summary>
	public interface IClockFeature : IFeature
	{
		/// <summary>
		/// Istanzia la Clock Feature - IDENTICO pattern Mercedes
		/// </summary>
		Task InstantiateClockFeature();
	}
}