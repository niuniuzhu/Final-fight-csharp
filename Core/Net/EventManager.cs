using Core.Structure;
using System.Collections.Generic;

namespace Core.Net
{
	public class EventManager
	{
		private static EventManager _instance;
		public static EventManager instance => _instance ?? ( _instance = new EventManager() );

		private readonly SwitchQueue<NetEvent> _queue = new SwitchQueue<NetEvent>();

		private EventManager()
		{
		}

		public void Push( NetEvent netEvent )
		{
			this._queue.Push( netEvent );
		}

		public void PopEvents( Queue<NetEvent> container )
		{
			this._queue.Switch();
			while ( !this._queue.isEmpty )
				container.Enqueue( this._queue.Pop() );
		}
	}
}