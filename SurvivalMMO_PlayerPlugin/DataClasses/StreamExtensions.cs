using DarkRift;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalMMO_PlayerPlugin
{
    public static class StreamExtensions
    {
        public static void WriteAs(this DarkRiftWriter writer, Type type, object input)
        {
            if (type == typeof(float))
            {
                writer.Write((float)input);
            }
            if (type == typeof(float[]))
            {
                writer.Write((float[])input);
            }
            else if (type == typeof(double))
            {
                writer.Write((double)input);
            }
            else if (type == typeof(double[]))
            {
                writer.Write((double[])input);
            }
            else if (type == typeof(bool))
            {
                writer.Write((bool)input);
            }
            else if (type == typeof(bool[]))
            {
                writer.Write((bool[])input);
            }
            else if (type == typeof(byte))
            {
                writer.Write((byte)input);
            }
            else if (type == typeof(byte[]))
            {
                writer.Write((byte[])input);
            }
            else if (type == typeof(char))
            {
                writer.Write((char)input);
            }
            else if (type == typeof(char[]))
            {
                writer.Write((char[])input);
            }
            else if (type == typeof(string))
            {
                writer.Write((string)input);
            }
            else if (type == typeof(string[]))
            {
                writer.Write((string[])input);
            }
            else if (type == typeof(Int16))
            {
                writer.Write((Int16)input);
            }
            else if (type == typeof(Int16[]))
            {
                writer.Write((Int16[])input);
            }
            else if (type == typeof(int))
            {
                writer.Write((Int64)input);
            }
            else if (type == typeof(int[]))
            {
                writer.Write((Int64[])input);
            }
            else if (type == typeof(UInt16))
            {
                writer.Write((UInt16)input);
            }
            else if (type == typeof(UInt16[]))
            {
                writer.Write((UInt16[])input);
            }
            else if (type == typeof(ushort))
            {
                writer.Write((ushort)input);
            }
            else if (type == typeof(vec3))
            {
                vec3 vector = (vec3)input;
                writer.Write(vector.X);
                writer.Write(vector.Y);
                writer.Write(vector.Z);
            }
            else if (type == typeof(object))
            {
                byte[] bytes;
                IFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, input);
                    bytes = stream.ToArray();
                }
                if (bytes != null)
                {
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                }
            }
        }


        public static object ReadAs(this DarkRiftReader reader, Type type)
        {
            if (type == typeof(float))
            {
                return reader.ReadSingle();
            }
            if (type == typeof(float[]))
            {
                return reader.ReadSingles();
            }
            else if (type == typeof(double))
            {
                return reader.ReadDouble();
            }
            else if (type == typeof(double[]))
            {
                return reader.ReadDoubles();
            }
            else if (type == typeof(bool))
            {
                return reader.ReadBoolean();
            }
            else if (type == typeof(bool[]))
            {
                return reader.ReadBooleans();
            }
            else if (type == typeof(byte))
            {
                return reader.ReadByte();
            }
            else if (type == typeof(byte[]))
            {
                return reader.ReadBytes();
            }
            else if (type == typeof(char))
            {
                return reader.ReadChar();
            }
            else if (type == typeof(char[]))
            {
                return reader.ReadChars();
            }
            else if (type == typeof(string))
            {
                return reader.ReadString();
            }
            else if (type == typeof(string[]))
            {
                return reader.ReadStrings();
            }
            else if (type == typeof(Int16))
            {
                return reader.ReadInt16();
            }
            else if (type == typeof(Int16[]))
            {
                return reader.ReadInt16();
            }
            else if (type == typeof(int))
            {
                return reader.ReadInt64();
            }
            else if (type == typeof(int[]))
            {
                return reader.ReadInt64s();
            }
            else if (type == typeof(UInt16))
            {
                return reader.ReadUInt16();
            }
            else if (type == typeof(UInt16[]))
            {
                return reader.ReadUInt16s();
            }
            else if (type == typeof(ushort))
            {
                return reader.ReadUInt16();
            }
            else if (type == typeof(vec3))
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                return new vec3(x, y, z);
            }
            else if (type == typeof(object))
            {
                ushort length = reader.ReadUInt16();
                byte[] bytes = reader.ReadBytes();
                IFormatter formatter = new BinaryFormatter();

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    return formatter.Deserialize(memoryStream);
                }
            }

            return null;
        }
    }

    /// <summary>
    ///     A primative 3 axis vector.
    /// </summary>
    [System.Serializable]
    public class vec3 : IDarkRiftSerializable, ISerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public vec3()
        {

        }

        public vec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public vec3(SerializationInfo info, StreamingContext ctxt)
        {
            X = (float)info.GetValue("x", typeof(float));
            Y = (float)info.GetValue("y", typeof(float));
            Z = (float)info.GetValue("z", typeof(float));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("X", X);
            info.AddValue("Y", Y);
            info.AddValue("Z", Z);
        }
        public void Deserialize(DeserializeEvent e)
        {
            this.X = e.Reader.ReadSingle();
            this.Y = e.Reader.ReadSingle();
            this.Z = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
            e.Writer.Write(Z);
        }
    }
}
