using Core.Misc;
using Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace LoginServer
{
	static class LSBootstrap
	{
		private const int HEART_BEAT_CD_TICK = 10;

		private static bool _disposed;
		private static InputHandler _inputHandler;

		static int Main( string[] args )
		{
			Console.Title = "LS";

			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

			Logger.Init( File.ReadAllText( @".\Config\LSLogCfg.xml" ), "BS" );

			_inputHandler = new InputHandler();
			_inputHandler.cmdHandler = HandleInput;
			_inputHandler.Start();

			LS ls = LS.instance;
			ErrorCode eResult = ls.Initialize();

			if ( ErrorCode.Success != eResult )
			{
				Logger.Error( $"Initialize BS fail, error code is {eResult}" );
				return 0;
			}

			eResult = ls.Start();
			if ( ErrorCode.Success != eResult )
			{
				Logger.Error( $"Start BS fail, error code is {eResult}" );
				return 0;
			}

			MainLoop();
			_inputHandler.Stop();

			return 0;
		}

		private static void Dispose()
		{
			_disposed = true;
			LS.instance.Dispose();
		}

		private static void MainLoop()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			long lastElapsed = 0;
			while ( !_disposed )
			{
				long elapsed = sw.ElapsedMilliseconds;
				LS.instance.Update( elapsed, elapsed - lastElapsed );
				_inputHandler.ProcessInput();
				lastElapsed = elapsed;
				Thread.Sleep( HEART_BEAT_CD_TICK );
			}
		}

		private static void HandleInput( string cmd )
		{
			switch ( cmd )
			{
				case "exit":
					Dispose();

					break;
				case "cls":
					Console.Clear();
					break;
			}
		}
	}
}
