using Core.Math;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Core.Misc
{
	/// <summary>
	/// 日志输出类
	/// 此类不保证线程安全
	/// </summary>
	public static class Logger
	{
		private static readonly StringBuilder SB = new StringBuilder();
		private static ILog _log;

		public static void Init( string config, string domain )
		{
			ILoggerRepository repository = LogManager.CreateRepository( "NETCoreRepository" );
			using ( var stream = GenerateStreamFromString( config ) )
			{
				XmlConfigurator.Configure( repository, stream );
			}
			_log = LogManager.GetLogger( repository.Name, domain );
		}

		public static void Dispose()
		{
			LogManager.Shutdown();
		}

		public static void Debug( object obj )
		{
			_log.Debug( $"{obj}{Environment.NewLine}{ GetStacks( 2, 100 )}" );
		}

		public static void Log( object obj )
		{
			_log.Debug( $"{ GetStacks( 2, 1 )}: {obj}" );
		}

		public static void Warn( object obj )
		{
			_log.Warn( $"{ GetStacks( 2, 1 )}: {obj}" );
		}

		public static void Error( object obj )
		{
			_log.Error( $"{ GetStacks( 2, 1 )}: {obj}" );
		}

		public static void Info( object obj )
		{
			_log.Info( $"{ GetStacks( 2, 1 )}: {obj}" );
		}

		public static void Fatal( object obj )
		{
			_log.Fatal( $"{obj}{Environment.NewLine}{ GetStacks( 2, 100 )}" );
		}

		private static Stream GenerateStreamFromString( string s )
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter( stream );
			writer.Write( s );
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		private static string GetStacks( int startFrame, int count )
		{
			StackTrace st = new StackTrace( true );
			int endFrame = startFrame + count - 1;
			endFrame = MathUtils.Min( st.FrameCount, endFrame );
			if ( startFrame > endFrame )
				return string.Empty;

			SB.Clear();
			for ( int i = startFrame; i <= endFrame; i++ )
			{
				StackFrame sf = st.GetFrame( i );
				MethodBase method = sf.GetMethod();
				SB.Append( $"{method.DeclaringType.FullName}::{method.Name}:{sf.GetFileLineNumber()}" );
				if ( i != endFrame )
					SB.AppendLine();
			}
			return SB.ToString();
		}
	}
}