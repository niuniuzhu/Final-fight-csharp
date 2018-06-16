using Core.Structure;
using System;
using System.Threading;

namespace Shared
{
	public class InputHandler
	{
		public delegate void CMDHandler( string cmd );

		public CMDHandler cmdHandler;

		private static readonly SwitchQueue<string> INPUT_QUEUE = new SwitchQueue<string>();
		private Thread _tInputConsumer;
		private bool _isRunning;

		public void Start()
		{
			this._isRunning = true;
			this._tInputConsumer = new Thread( this.InputConsumer ) { IsBackground = true };
			this._tInputConsumer.Start();
		}

		public void Stop()
		{
			this._isRunning = false;
			this._tInputConsumer.Join();
		}

		private void InputConsumer()
		{
			while ( this._isRunning )
			{
				string cmd = Console.ReadLine();
				INPUT_QUEUE.Push( cmd );
				if ( cmd == "exit" )
					break;
				Thread.Sleep( 10 );
			}
		}

		public void ProcessInput()
		{
			INPUT_QUEUE.Switch();
			while ( !INPUT_QUEUE.isEmpty )
			{
				string cmd = INPUT_QUEUE.Pop();
				this.cmdHandler( cmd );
			}
		}
	}
}