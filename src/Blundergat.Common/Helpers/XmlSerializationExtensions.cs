using System;
using System.Xml;

namespace Blundergat.Common.Helpers
{
	public static class XmlSerializationExtensions
	{
		// Adapted from this answer https://stackoverflow.com/a/60498500/3744182
		// To https://stackoverflow.com/questions/60449088/why-does-xmlserializer-throws-an-exception-and-raise-a-validationevent-when-a-sc
		// by handling comments.
		public static void ReadIXmlSerializable(XmlReader reader, Func<XmlReader, bool> handleXmlAttribute, Func<XmlReader, bool> handleXmlElement, Func<XmlReader, bool> handleXmlText, Func<XmlReader, bool> handleXmlComment)
		{
			//https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.ixmlserializable.readxml?view=netframework-4.8#remarks
			//When this method is called, the reader is positioned on the start tag that wraps the information for your type. 
			//That is, directly on the start tag that indicates the beginning of a serialized object. 
			//When this method returns, it must have read the entire element from beginning to end, including all of its contents. 
			//Unlike the WriteXml method, the framework does not handle the wrapper element automatically. Your implementation must do so. 
			//Failing to observe these positioning rules may cause code to generate unexpected runtime exceptions or corrupt data.
			reader.MoveToContent();
			if (reader.NodeType != XmlNodeType.Element)
				throw new XmlException(string.Format("Invalid NodeType {0}", reader.NodeType));

			if (reader.HasAttributes)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					handleXmlAttribute(reader);
				}
				reader.MoveToElement(); // Moves the reader back to the element node.
			}

			if (reader.IsEmptyElement)
			{
				reader.Read();
				return;
			}

			reader.ReadStartElement(); // Advance to the first sub element of the wrapper element.
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.NodeType == XmlNodeType.Element)
				{
					using (var subReader = reader.ReadSubtree())
					{
						subReader.MoveToContent();
						handleXmlElement(subReader);
					}
					// ReadSubtree() leaves the reader positioned ON the end of the element, so read that also.
					reader.Read();
				}
				else if (reader.NodeType == XmlNodeType.Text || reader.NodeType == XmlNodeType.CDATA)
				{
					var type = reader.NodeType;
					handleXmlText(reader);
					// Ensure that the reader was not advanced.
					if (reader.NodeType != type)
						throw new XmlException(string.Format("handleXmlText incorrectly advanced the reader to a new node {0}", reader.NodeType));

					reader.Read();
				}
				else if (reader.NodeType == XmlNodeType.Comment)
				{
					var type = reader.NodeType;
					handleXmlComment(reader);
					// Ensure that the reader was not advanced.
					if (reader.NodeType != type)
						throw new XmlException(string.Format("handleXmlComment incorrectly advanced the reader to a new node {0}", reader.NodeType));

					reader.Read();
				}
				else // Whitespace, etc.
				{
					// Skip() leaves the reader positioned AFTER the end of the node.
					reader.Skip();
				}
			}
			// Move past the end of the wrapper element
			reader.ReadEndElement();
		}

		public static void ReadIXmlSerializable(XmlReader reader, Func<XmlReader, bool> handleXmlAttribute, Func<XmlReader, bool> handleXmlElement, Func<XmlReader, bool> handleXmlText)
		{
			ReadIXmlSerializable(reader, handleXmlAttribute, handleXmlElement, handleXmlText, r => true);
		}

		public static void WriteIXmlSerializable(XmlWriter writer, Action<XmlWriter> writeAttributes, Action<XmlWriter> writeNodes)
		{
			// https://docs.microsoft.com/en-us/dotnet/api/system.xml.serialization.ixmlserializable.writexml?view=netframework-4.8#remarks
			// The WriteXml implementation you provide should write out the XML representation of the object. 
			// The framework writes a wrapper element and positions the XML writer after its start. Your implementation may write its contents, including child elements. 
			// The framework then closes the wrapper element.
			writeAttributes(writer);
			writeNodes(writer);
		}
	}
}
