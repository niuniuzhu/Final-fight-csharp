using ProtoBuf;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using MySql.Data.MySqlClient;

namespace ConsoleApp1
{
	internal static class Program
	{
		private static ConnectionMultiplexer redis;

		private static void Main( string[] args )
		{
			TestRedis();
			TestDB();

			Console.ReadLine();
		}

		private static void TestDB()
		{
			string query = "select * from account_user";
			MySqlConnection myConnection = new MySqlConnection( "server=localhost;user id=root;password=ron159753;database=fball_accountdb" );
			MySqlCommand myCommand = new MySqlCommand( query, myConnection );
			myConnection.Open();
			myCommand.ExecuteNonQuery();
			MySqlDataReader myDataReader = myCommand.ExecuteReader();
			string bookres = "";
			while ( myDataReader.Read() )
			{
				bookres += myDataReader["id"];
				bookres += myDataReader["user_name"];
				bookres += myDataReader["cdkey"];
			}
			myDataReader.Close();
			myConnection.Close();
			Console.WriteLine( bookres );
		}

		private static void Connect()
		{
			ConfigurationOptions config = new ConfigurationOptions
			{
				EndPoints =
				{
					{ "61.140.77.233", 23680 }
				},
				KeepAlive = 180,
				AbortOnConnectFail = false,
				Password = "159753"
			};
			redis = ConnectionMultiplexer.Connect( config );
		}

		private static async void TestRedis()
		{
			Connect();
			IDatabase db = redis.GetDatabase();
			var person = new Person
			{
				Id = 12345,
				Name = "Fred",
				Address = new Address
				{
					Line1 = "Flat 1",
					Line2 = "The Meadows"
				},
				Addresses = new Dictionary<int, Address>
				{
					{
						0, new Address
						{
							Line1 = "Flat 2",
							Line2 = "The Meadows"
						}
					},
					{
						1, new Address
						{
							Line1 = "Flat 3",
							Line2 = "The Meadows"
						}
					}
				},
				eTest = ETest.T2
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
		[ProtoMember( 1, DataFormat = DataFormat.ZigZag )]
		public int Id { get; set; }
		[ProtoMember( 2, DataFormat = DataFormat.ZigZag )]
		public string Name { get; set; }
		[ProtoMember( 3, DataFormat = DataFormat.ZigZag )]
		public Address Address { get; set; }
		[ProtoMember( 4, DataFormat = DataFormat.ZigZag )]
		public Dictionary<int, Address> Addresses;
		[ProtoMember( 5 )]
		public ETest eTest;
	}
	[ProtoContract]
	internal class Address
	{
		[ProtoMember( 1, DataFormat = DataFormat.ZigZag )]
		public string Line1 { get; set; }
		[ProtoMember( 2, DataFormat = DataFormat.ZigZag )]
		public string Line2 { get; set; }
	}

	internal enum ETest
	{
		T1,
		T2
	}
}
