using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Blundergat.Common.Helpers
{
	// To avoid a memory leak the serializer must be cached.
	// https://stackoverflow.com/questions/23897145/memory-leak-using-streamreader-and-xmlserializer
	// This factory taken from 
	// https://stackoverflow.com/questions/34128757/wrap-properties-with-cdata-section-xml-serialization-c-sharp/34138648#34138648
	public static class XmlSerializerFactory
	{
		readonly static Dictionary<Tuple<Type, string, string>, XmlSerializer> _cache;
		readonly static object _padlock;

		static XmlSerializerFactory()
		{
			_padlock = new object();
			_cache = new Dictionary<Tuple<Type, string, string>, XmlSerializer>();
		}

		public static XmlSerializer Create(Type serializedType, string rootName, string rootNamespace)
		{
			if (serializedType == null)
				throw new ArgumentNullException();

			if (rootName == null && rootNamespace == null)
				return new XmlSerializer(serializedType);

			lock (_padlock)
			{
				var key = Tuple.Create(serializedType, rootName, rootNamespace);
				if (!_cache.TryGetValue(key, out XmlSerializer serializer))
				{
					_cache[key] = serializer = new XmlSerializer(
						serializedType,
						new XmlRootAttribute
						{
							ElementName = rootName,
							Namespace = rootNamespace
						});
				}
				return serializer;
			}
		}
	}
}