using ProtoBuf;
using StackExchange.Redis;
using System;
using System.IO;

namespace ConsoleApp1
{
	internal static class Program
	{
		private static ConnectionMultiplexer redis;

		private static void Main( string[] args )
		{
			Connect();
			Test();

			Console.ReadLine();
		}

		private static void Connect()
		{
			ConfigurationOptions config = new ConfigurationOptions
			{
				EndPoints =
				{
					{ "localhost", 7000 }
				},
				KeepAlive = 180,
				AbortOnConnectFail = false,
				Password = "159753"
			};
			redis = ConnectionMultiplexer.Connect( config );
			Console.WriteLine( "connected" );
		}

		private static async void Test()
		{
			IDatabase db = redis.GetDatabase();
			var person = new Person
			{
				Id = 12345,
				Name = "Fred",
				Address = new Address
				{
					Line1 = "Flat 1",
					Line2 = "The Meadows"
				}
			};
			MemoryStream ms = new MemoryStream();
			Serializer.Serialize( ms, person );
			bool t = await db.StringSetAsync( "xx", ms.ToArray() );
			Console.WriteLine( t );
			ms.Close();
			ms.Dispose();

			byte[] data = db.StringGet( "xx" );
			ms = new MemoryStream( data );
			person = Serializer.Deserialize<Person>( ms );
			Console.WriteLine( person );
		}
	}

	[ProtoContract]
	internal class Person
	{
		[ProtoMember( 1 )]
		public int Id { get; set; }
		[ProtoMember( 2 )]
		public string Name { get; set; }
		[ProtoMember( 3 )]
		public Address Address { get; set; }
	}
	[ProtoContract]
	internal class Address
	{
		[ProtoMember( 1 )]
		public string Line1 { get; set; }
		[ProtoMember( 2 )]
		public string Line2 { get; set; }
	}
}
