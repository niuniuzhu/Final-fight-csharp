using System;
using System.IO;
using System.Threading;
using Core;

namespace GateServer
{
	static class Progran
	{
		static int Main( string[] args )
		{
			Logger.Init( File.ReadAllText( @".\Config\logger_config.xml" ), "GSKernel" );

			GSKernel kernel = GSKernel.instance;
			EResult eResult = kernel.Initialize();

			if ( EResult.Normal != eResult )
			{
				Logger.Error( $"Initialize GSKernel fail, error code is {eResult}" );
				Thread.Sleep( 5000 );
				return 0;
			}

			Console.ReadLine();
			return 0;
		}
	}
}
