namespace MethodCache.Tests
{
	using System.Diagnostics;
	using System.IO;
	using Microsoft.Build.Utilities;

	public static class Verifier
	{
		#region Public Methods and Operators

		public static string Verify(string assemblyPath2)
		{
			string exePath = GetPathToPEVerify();
			var process =
				Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
				{
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				});

			process.WaitForExit(10000);

			return process.StandardOutput.ReadToEnd().Trim();
		}

		#endregion

		#region Methods

		private static string GetPathToPEVerify()
		{
			return Path.Combine(ToolLocationHelper.GetPathToDotNetFrameworkSdk(TargetDotNetFrameworkVersion.Version40),
				@"bin\NETFX 4.0 Tools\peverify.exe");
		}

		#endregion
	}
}