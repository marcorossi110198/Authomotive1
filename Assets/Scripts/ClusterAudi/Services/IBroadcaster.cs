using System;

namespace ClusterAudi
{
	public interface IBroadcaster : IService
	{
		public void Broadcast<T>(T arg);
		public void Add<T>(Action<T> arg);
		public void Remove<T>(Action<T> arg);
	}
}