using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Core;

namespace GateServer
{
	static class Progran
	{
		static int Main( string[] args )
		{
			AssemblyName[] assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies();
			foreach ( AssemblyName assembly in assemblies )
				Assembly.Load( assembly );

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

			Console.ReadLine();
			return 0;
		}
	}
}
