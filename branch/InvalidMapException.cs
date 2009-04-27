using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// thrown when a map doesn't validate correctly
	/// </summary>
	internal sealed class InvalidMapException : System.ApplicationException
	{
		internal InvalidMapException(string msg):base(msg)
		{
		}
	}
}
