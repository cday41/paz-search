using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Exception used to identify bad xml load file
	/// </summary>
	internal sealed class CraptasticXmlException : System.ApplicationException
	{
		public CraptasticXmlException (string msg):base(msg)
		{
		}

	}
}
