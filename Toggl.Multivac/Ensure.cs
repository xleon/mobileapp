using System;
namespace Toggl.Multivac
{
	public static class Ensure
	{
		public static void ArgumentIsNotNull<T>(T value, string argumentName)
			where T : class
		{
			if (value != null) return;

			throw new ArgumentNullException(argumentName);
		}
	}
}
