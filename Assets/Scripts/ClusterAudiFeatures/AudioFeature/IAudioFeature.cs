using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia pubblica per Audio Feature
	/// IDENTICA al pattern ISeatBeltFeature che hai già implementato
	/// </summary>
	public interface IAudioFeature : IFeature
	{
		/// <summary>
		/// Istanzia l'Audio Feature - IDENTICO pattern Mercedes
		/// </summary>
		Task InstantiateAudioFeature();

		/// <summary>
		/// Riproduce clip audio con priorità
		/// </summary>
		void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1);

		/// <summary>
		/// Ferma riproduzione audio corrente
		/// </summary>
		void StopCurrentAudio();

		/// <summary>
		/// Imposta volume master
		/// </summary>
		void SetMasterVolume(float volume);
	}
}