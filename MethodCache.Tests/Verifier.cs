namespace MethodCache.Tests
{
	using System.Diagnostics;

	public static class Verifier
	{
		public static string Verify(string assemblyPath2)
		{
			string exePath = GetPathToPEVerify();
			Process process = Process.Start(new ProcessStartInfo(exePath, "\"" + assemblyPath2 + "\"")
			{
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			process.WaitForExit(10000);

			return process.StandardOutput.ReadToEnd().Trim();
		}

		private static string GetPathToPEVerify()
		{
			return "C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v10.0A\\bin\\NETFX 4.6.1 Tools\\PEVerify.exe";
		}
	}
}