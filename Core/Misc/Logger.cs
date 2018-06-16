using Core.Math;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Core.Misc
{
	public static class Logger
	{
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
			_log.Debug( obj + Environment.NewLine + GetStacks() );
		}

		public static void Log( object obj )
		{
			_log.Debug( obj );
		}

		public static void Warn( object obj )
		{
			_log.Warn( obj );
		}

		public static void Error( object obj )
		{
			_log.Error( obj );
		}

		public static void Info( object obj )
		{
			_log.Info( obj );
		}

		public static void Fatal( object obj )
		{
			_log.Fatal( obj );
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

		private static string GetStacks()
		{
			StackTrace st = new StackTrace( true );
			if ( st.FrameCount < 2 )
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			int count = MathUtils.Min( st.FrameCount, 5 );
			for ( int i = 2; i < count; i++ )
			{
				StackFrame sf = st.GetFrame( i );
				string fn = sf.GetFileName();
				int pos = fn.LastIndexOf( '\\' ) + 1;
				fn = fn.Substring( pos, fn.Length - pos );
				sb.Append( $" M:{sf.GetMethod()} in {fn}:{sf.GetFileLineNumber()},{sf.GetFileColumnNumber()}" );
				if ( i != count - 1 )
					sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}