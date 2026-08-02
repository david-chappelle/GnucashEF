using System;
namespace GncEF
{
	public static class GuidHelper
	{
		public static string Random()
		{
			return Guid.NewGuid().ToString().ToLower().Replace("-", "");
		}
	}
}
