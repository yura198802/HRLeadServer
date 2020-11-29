using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Monica.Core.Service.Extension
{
    /// <summary>
    /// Класс для работы с XML 
    /// </summary>
    public static class XmlFactory
    {
        public static string XmlSerializer(this object obj)
        {
            string xml;
            using (var sww = new Utf8StringWriter())
            {
                var ser = new XmlSerializer(obj.GetType());
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    ser.Serialize(writer, obj);
                    xml = sww.ToString();
                }
            }

            return xml;
        }

        /// <summary>
        /// Десериализует Xml объект
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="sww">Объект, который нужно десириализовать</param>
        /// <returns></returns>
        public static TModel XmlDeSerializer<TModel>(this MemoryStream sww)
        {
            var ser = new XmlSerializer(typeof(TModel));
            using (XmlReader reader = XmlReader.Create(sww))
            {
                var model = (TModel)ser.Deserialize(reader);
                return model;
            }
        }


        /// <summary>
        /// Десериализует Xml объект
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="xml">Xml в строке</param>
        /// <returns></returns>
        public static TModel XmlDeSerializer<TModel>(this string xml)
        {
            var ser = new XmlSerializer(typeof(TModel));
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                var model = (TModel)ser.Deserialize(reader);
                return model;
            }
        }
    }

    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}

