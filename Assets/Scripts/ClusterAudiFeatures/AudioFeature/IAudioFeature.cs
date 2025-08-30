using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface IAudioFeature : IFeature
	{
		Task InstantiateAudioFeature();

		void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1);

		void StopCurrentAudio();

		void SetMasterVolume(float volume);
	}
}