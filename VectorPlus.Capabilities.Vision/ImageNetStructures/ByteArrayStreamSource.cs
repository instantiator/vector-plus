using System;
using System.IO;
using Microsoft.ML.Data;

namespace VectorPlus.Capabilities.Vision.ImageNetStructures
{
    public class ByteArrayStreamSource : IMultiStreamSource
    {
        private byte[] data;
        private string path;

        public ByteArrayStreamSource(byte[] data)
        {
            this.data = data;
            this.path = Path.GetTempFileName();
            File.WriteAllBytes(path, data);
        }

        public int Count => 1;

        public string GetPathOrNull(int index)
        {
            throw new NotImplementedException(); // shouldn't get called
            return index == 0 ? path : null;
        }

        public Stream Open(int index)
        {
            return new MemoryStream(data);
        }

        public TextReader OpenTextReader(int index)
        {
            throw new NotImplementedException(); // shouldn't get called
        }
    }
}
