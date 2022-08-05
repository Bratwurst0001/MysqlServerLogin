using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MysqlServer
{
    internal class Program
    {

		public enum Options
		{
			Register = 1,
			Login,
			License,
			GenerateLicense,
			Deletelicense,
			Acountsharing,
			Loginrequest,
			Block,
			BlockHWID,
			UnBlockHWID,
			Security,
			AntiCopy,
			ScreenShot
		}
		public static MySqlConnection conn;
		private static string myConnectionString = "server=127.0.0.1;uid=Anemo;pwd=sxjhvdfygcj,gcevrw3tvrbanzqfmvugbKJNS;database=users";

		static void Main(string[] args)
        {
			Console.WriteLine("Mysql server by Bratwurst001!");
			TcpListener tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), 6005);
			tcpListener.Start();
			init();
			while (true)
			{
				Console.Write("Waiting for a connection... ");
				TcpClient client = tcpListener.AcceptTcpClient();
				Task.Run(() => process(client));
			}

		}

		public static async Task process(TcpClient client)
		{
			StreamReader streamReader = new StreamReader(client.GetStream());
			StreamWriter writer = new StreamWriter(client.GetStream());
			Console.WriteLine("Connected!");
			try
			{
				Console.WriteLine("Waiting for Client");
				string text = await streamReader.ReadLineAsync();
				Console.WriteLine("Test: " + Options.License);
				Console.WriteLine("Received:" + text);
				if (text.StartsWith(Options.Register.ToString()))
				{
					string[] array = text.Replace(Options.Register.ToString(), "").Split(' ');
					Console.WriteLine("Try to regsiter!");
					await Register(array[1], array[2], array[3], array[4]);
				}
				else if (text.StartsWith(Options.Login.ToString()))
				{
					string[] array2 = text.Replace(Options.Login.ToString(), "").Split(' ');
					bool check = await Check(array2[1], array2[2], array2[3]);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("Login: {0}", check);
				}
				else if (text.StartsWith(Options.License.ToString()))
				{
					string text2 = text.Replace(Options.License.ToString() + " ", "");
					Console.WriteLine("DFATA:" + text2);
					bool check = await License(Options.License, text2);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("License: {0}", check);
				}
				else if (text.StartsWith(Options.GenerateLicense.ToString()))
				{
					string text3 = text.Replace(Options.GenerateLicense.ToString() + " ", "");
					Console.WriteLine("DATA:" + text3);
					bool check = await GenerateLicenseKey(Options.GenerateLicense, text3);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("Generated:: {0}", check);
				}
				else if (text.StartsWith(Options.Deletelicense.ToString()))
				{
					string text4 = text.Replace(Options.GenerateLicense.ToString() + " ", "");
					Console.WriteLine("DATA:" + text4);
					bool check = await DeleteLicense(Options.Deletelicense, text4);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("Generated:: {0}", check);
				}
				else if (text.StartsWith(Options.Block.ToString()))
				{
					string text5 = text.Replace(Options.Block.ToString() + " ", "");
					Console.WriteLine("HWID:" + text5);
					bool check = await Blocked(text5);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("Check HWID: {0}", check);
				}
				else if (text.StartsWith(Options.BlockHWID.ToString()))
				{
					string text6 = text.Replace(Options.BlockHWID.ToString() + " ", "");
					Console.WriteLine("HWID:" + text6);
					Console.WriteLine("Block HWID: {0}", await BlockHWID(text6));
				}
				else if (text.StartsWith(Options.UnBlockHWID.ToString()))
				{
					string text7 = text.Replace(Options.UnBlockHWID.ToString() + " ", "");
					Console.WriteLine("HWID:" + text7);
					Console.WriteLine("UnBlock HWID: {0}", await UnBlockHWID(text7));
				}
				else if (text.StartsWith(Options.Security.ToString()))
				{
					string[] array3 = text.Replace(Options.Security.ToString(), "").Split(' ');
					bool check = await WebHookManager.Security("https://discord.com/api/webhooks/1005221888261361834/tJO19gUxHI4kB62Z54O2qxjAXEAs-RK-qqVafYv8cCmnBZaPFkfJhbbwUid2HPhhEXcq", array3[1], array3[2]);
					await writer.WriteLineAsync(check.ToString() ?? "");
					Console.WriteLine("Login: {0}", check);
				}
				else if (text.StartsWith(Options.AntiCopy.ToString()))
				{
					string[] array4 = text.Replace(Options.AntiCopy.ToString(), "").Split(' ');
					await writer.WriteLineAsync((await WebHookManager.AntiCopy("https://discord.com/api/webhooks/1005222200569245787/MXfdN4_GQJAS3ESpkf6gGCyGgyQi05mn0y1KJe-rYQUgoR_n7ZsttGfELJ6UaKEWK9ia", array4[1], array4[2])).ToString() ?? "");
				}
				else if (text.StartsWith(Options.ScreenShot.ToString()))
				{
					string[] array5 = text.Replace(Options.ScreenShot.ToString(), "").Split(' ');
					await writer.WriteLineAsync((await WebHookManager.AntiScreenshot("https://discord.com/api/webhooks/1005222124442624042/BnAceND5ClflcyXDNWePlAN12mhJI1WOYeCB65OA-I9WgoA9JV0SIqEQcqX_2TGUSWW6", array5[1], array5[2])).ToString() ?? "");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
			await writer.FlushAsync();
			client.Close();
		}

		public static void init()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			//IL_0026: Expected O, but got Unknown
			try
			{
				conn = new MySqlConnection();
				((DbConnection)(object)conn).ConnectionString = myConnectionString;
				((DbConnection)(object)conn).Open();
			}
			catch (MySqlException val)
			{
				MySqlException val2 = val;
				Console.WriteLine("Failed Connections!\n\n\n\n" + ((object)val2).ToString());
			}
		}

		public static async Task Register(string Name, string Passwort, string Userhwid, string Userlicense)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlDataReader obj = new MySqlCommand("SELECT * FROM user WHERE name=\"" + Name + "\"", conn).ExecuteReader();
				((DbDataReader)(object)obj).Read();
				if (!((DbDataReader)(object)obj).HasRows)
				{
					Console.WriteLine("Checking License!");
					if (await License(Options.License, Userlicense))
					{
						await conn.CloseAsync();
						await ((DbConnection)(object)conn).OpenAsync();
						string text = "UPDATE `users`.`licensekey` SET `used`='1' WHERE  `key`='" + Userlicense + "';";
						MySqlCommand val = new MySqlCommand("INSERT INTO user(name, password,hwid,activeLicense) VALUES (@name, @password,@hwid,@activeLicense)", conn);
						MySqlCommand SetUsed = new MySqlCommand(text, conn);
						val.Parameters.AddWithValue("@name", (object)Name);
						val.Parameters.AddWithValue("@password", (object)Passwort);
						val.Parameters.AddWithValue("@hwid", (object)Userhwid);
						val.Parameters.AddWithValue("@activeLicense", (object)Userlicense);
						await ((DbCommand)val).ExecuteNonQueryAsync();
						await conn.CloseAsync();
						await ((DbConnection)(object)conn).OpenAsync();
						await ((DbCommand)(object)SetUsed).ExecuteNonQueryAsync();
						Console.WriteLine("Checking Done! ");
						Console.WriteLine("Entry in New User: NAME: " + Name + "  PASSWORD: " + Passwort + " HWID: " + Userhwid + " License: " + Userlicense);
					}
				}
				else
				{
					Console.WriteLine("Try to use same Name or Password!");
				}
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}
		public static async Task<bool> Check(string Name, string Password, string hwid)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlCommand val = new MySqlCommand("SELECT * FROM user WHERE name=\"" + Name + "\" AND password=\"" + Password + "\" AND hwid=\"" + hwid + "\"", conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (!((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Try to Find invalid user!");
					return false;
				}
				if (Data.GetString("name") == Name && Data.GetString("password") == Password)
				{
					if (Data.GetString("hwid") == hwid)
					{
						WebHookManager.LoginInfo("https://discord.com/api/webhooks/1005222076715647068/kjJr98OQn_r9FYK7ZhONZnyfYPTACgMiHSuYe_KiaOkNzBZ3-YqgKH0kdMC3eiZ399YZ", Name, Password, hwid);
						Console.WriteLine("a User are Login Sucessfully!");
						return true;
					}
					WebHookManager.AcountSharing("https://discord.com/api/webhooks/1005222261025931264/Drm90w4uZX2C4QAgFYuUGjafDTmYFyGtzwsLsoehwiSQWrEmVnp-UGY1rSk1oXFjh4RE", Name, hwid, Password);
					return false;
				}
				return false;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> License(Enum OPLicense, string license)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlCommand val = new MySqlCommand("USE users;SELECT `key`, `used` FROM `users`.`licensekey` WHERE  `key`='" + license + "' AND `used`='0';", conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (!((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Try to Find invalid license!");
					return false;
				}
				if (Data.GetString("key") == license && Data.GetString("used") == "0")
				{
					Console.WriteLine("license valid!");
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> Blocked(string HWID)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlCommand val = new MySqlCommand("USE users;SELECT `HWID` FROM `users`.`blocked` WHERE  `HWID`='" + HWID + "';", conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (!((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Checked HWID!");
					return false;
				}
				if (Data.GetString("HWID") == HWID)
				{
					Console.WriteLine("license valid!");
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> DeleteLicense(Enum OPLicense, string license)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				string getData = "USE users;SELECT `key`, `used` FROM `users`.`licensekey` WHERE  `key`='" + license + "';";
				MySqlCommand val = new MySqlCommand(getData, conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (!((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("License was not detected!");
					return false;
				}
				if (Data.GetString("key") == license)
				{
					await conn.CloseAsync();
					await ((DbConnection)(object)conn).OpenAsync();
					_ = "DELETE FROM `users`.`licensekey` WHERE  `key`='" + license + "'";
					await ((DbCommand)new MySqlCommand(getData, conn)).ExecuteNonQueryAsync();
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> GenerateLicenseKey(Enum LicenseKey, string license)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlCommand val = new MySqlCommand("USE users;INSERT INTO `users`.`licensekey` (`key`, `used`) VALUES ('" + license + "', '0');", conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Try to Generate Already used License!");
					return false;
				}
				return true;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> BlockHWID(string HWID)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				MySqlCommand val = new MySqlCommand("USE users;INSERT INTO `users`.`blocked` (`HWID`) VALUES ('" + HWID + "');", conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Try to Block a Already Blocked HWID!");
					return false;
				}
				return true;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}

		public static async Task<bool> UnBlockHWID(string HWID)
		{
			MySqlConnection conn = new MySqlConnection(myConnectionString);
			try
			{
				await ((DbConnection)(object)conn).OpenAsync();
				string getData = "USE users;SELECT `HWID` FROM `users`.`blocked` WHERE  `HWID`='" + HWID + "';";
				MySqlCommand val = new MySqlCommand(getData, conn);
				MySqlDataReader Data = val.ExecuteReader();
				await ((DbDataReader)(object)Data).ReadAsync();
				if (!((DbDataReader)(object)Data).HasRows)
				{
					Console.WriteLine("Try to unblock invalid HWID!");
					return false;
				}
				if (Data.GetString("HWID") == HWID)
				{
					await conn.CloseAsync();
					await ((DbConnection)(object)conn).OpenAsync();
					_ = "DELETE FROM `users`.`blocked` WHERE  `HWID`='" + HWID + "';";
					await ((DbCommand)new MySqlCommand(getData, conn)).ExecuteNonQueryAsync();
					return true;
				}
				return false;
			}
			finally
			{
				((IDisposable)conn)?.Dispose();
			}
		}
	}
}
