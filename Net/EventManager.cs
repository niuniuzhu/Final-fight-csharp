using Core.Structure;

namespace Net
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

		public void Update()
		{
			this._queue.Switch();
			while ( !this._queue.isEmpty )
			{
				NetEvent netEvent = this._queue.Pop();
				netEvent.session.ProcessEvent( netEvent );
			}
		}
	}
}