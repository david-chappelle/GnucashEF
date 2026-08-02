using System;
using System.Collections.Generic;
using System.Text;

namespace GnucashLib
{
	public static class GuidHelper
	{
		public static string Random()
		{
			return Guid.NewGuid().ToString().ToLower().Replace("-", "");
		}
	}
}
