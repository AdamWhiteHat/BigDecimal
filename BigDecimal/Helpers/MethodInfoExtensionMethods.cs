using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedNumerics.Helpers
{
	public static class MethodInfoExtensionMethods
	{
		public static string GetMethodSignature(this MethodInfo mi)
		{
			string[] param = mi.GetParameters()
							   .Select(p => String.Format("{0} {1}", p.ParameterType.Name, p.Name))
							   .ToArray();
			return string.Format("{0} {1}({2})", mi.ReturnType.Name, mi.Name, String.Join(",", param));
		}
	}
}
