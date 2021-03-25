using Blundergat.Common.Helpers;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XmlSerializerFactory = Blundergat.Common.Helpers.XmlSerializerFactory;

namespace Blundergat.Common.Settings.Types
{
	[Serializable]
	public sealed class Setting<T> : SettingBase, IXmlSerializable
	{
		public Setting() { }

		public Setting(T value, string description)
		{
			Value = value;
			Description = description;
		}

		public Setting(string command, T value, string description)
			: this(value, description)
		{
			Command = command;
		}

		public XmlSchema GetSchema() { return null; }

		public void ReadXml(XmlReader reader)
		{
			XmlSerializationExtensions.ReadIXmlSerializable(
				reader,
				r => true,
				r =>
				{
					switch (r.LocalName)
					{
						case "Command":
							Command = r.ReadElementContentAsString();
							break;
						case "Value":
							var serializer = XmlSerializerFactory.Create(typeof(T), "Value", reader.NamespaceURI);
							Value = (T)serializer.Deserialize(r);
							break;
					}
					return true;
				},
				r => true,
				r =>
				{
					Description += r.Value;
					return true;
				});
		}

		public void WriteXml(XmlWriter writer)
		{
			XmlSerializationExtensions.WriteIXmlSerializable(
				writer, w => { },
				w =>
				{
					if (Description != null)
						w.WriteComment(Description);

					if (Command != null)
						w.WriteElementString("Command", Command);

					if (Value != null)
					{
						var serializer = XmlSerializerFactory.Create(typeof(T), "Value", null);
						serializer.Serialize(w, Value);
					}
				});
		}

		public string Description { get; set; }

		public string Command { get; set; }

		public T Value { get; set; }

		public override object ValueUntyped { get { return Value; } }
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class XmlCommentAttribute : Attribute { }
}