using Core;
using Core.Structure;
using Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace GateServer
{
	static class Progran
	{
		private const int HEART_BEAT_CD_TICK = 10;

		private static readonly SwitchQueue<string> INPUT_QUEUE = new SwitchQueue<string>();
		private static bool _disposed;

		static int Main( string[] args )
		{
			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			Thread tInputConsumer = new Thread( InputConsumer ) { IsBackground = true };
			tInputConsumer.Start();

			Logger.Init( File.ReadAllText( @".\Config\logger_config.xml" ), "GSKernel" );

			GSKernel kernel = GSKernel.instance;
			EResult eResult = kernel.Initialize();

			if ( EResult.Normal != eResult )
			{
				Logger.Error( $"Initialize GSKernel fail, error code is {eResult}" );
				return 0;
			}

			eResult = kernel.Start();
			if ( EResult.Normal != eResult )
			{
				Logger.Error( $"Start GSKernel fail, error code is {eResult}" );
				return 0;
			}

			MainLoop();
			tInputConsumer.Join();

			return 0;
		}

		private static void Dispose()
		{
			_disposed = true;
			GSKernel.instance.Dispose();
		}

		private static void MainLoop()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			long lastElapsed = 0;
			while ( !_disposed )
			{
				long elapsed = sw.ElapsedMilliseconds;
				GSKernel.instance.Update( elapsed, elapsed - lastElapsed );
				ProcessInput();
				lastElapsed = elapsed;
				Thread.Sleep( HEART_BEAT_CD_TICK );
			}
		}

		private static void InputConsumer()
		{
			while ( !_disposed )
			{
				string cmd = Console.ReadLine();
				INPUT_QUEUE.Push( cmd );
				Thread.Sleep( 10 );
			}
		}

		private static void ProcessInput()
		{
			INPUT_QUEUE.Switch();
			while ( !INPUT_QUEUE.isEmpty )
			{
				string cmd = INPUT_QUEUE.Pop();
				if ( cmd == "exit" )
				{
					Dispose();
				}
				else if ( cmd == "cls" )
				{
					Console.Clear();
				}
			}
		}
	}
}
