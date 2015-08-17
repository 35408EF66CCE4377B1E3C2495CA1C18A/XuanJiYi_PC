using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tai_Shi_Xuan_Ji_Yi.Classes
{
    public class CPublicMethods
    {
        /// <summary>
        /// 讲对象序列化为二进制数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObjectToBinaryArray(object obj)
        {
            MemoryStream str = new MemoryStream();
            BinaryFormatter oBFormatter = new BinaryFormatter();
            oBFormatter.Serialize(str, obj);
            byte[] oSerializedObj = str.ToArray();
            return oSerializedObj;
        }

        /// <summary>
        /// 从二进制序列中恢复对象
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeObjectFromBinaryArray(byte[] data)
        {
            MemoryStream oStr = new MemoryStream(data);
            BinaryFormatter oBFormatter = new BinaryFormatter();
            return oBFormatter.Deserialize(oStr);
        }
    }
}
