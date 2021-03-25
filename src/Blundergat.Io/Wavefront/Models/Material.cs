using Blundergat.Common.Model.Io;
using System;
using System.Text;

namespace Blundergat.Io.Wavefront.Models
{
	public class Material : MaterialBase
	{
		public Material() : base() { }

		public Material(string name, string fileName) : base(name, fileName) { }

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("newmtl " + Name);

			b.AppendLine(String.Format("Ka {0}", AmbientColor));
			b.AppendLine(String.Format("Kd {0}", DiffuseColor));
			b.AppendLine(String.Format("Ks {0}", SpecularColor));
			b.AppendLine(String.Format("Tf {0}", TransmissionFilter));
			b.AppendLine(String.Format("Ke {0}", EmissiveCoefficient));
			b.AppendLine(String.Format("Ns {0}", SpecularExponent));
			b.AppendLine(String.Format("Ni {0}", OpticalDensity));
			b.AppendLine(String.Format("d {0}", Dissolve));
			b.AppendLine(String.Format("illum {0}", IlluminationModel));

			return b.ToString();
		}
	}
}