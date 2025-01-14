using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glTFInterface
{
    public class BinChunkStore
    {
        public BinChunkStore()
        {

        }
        // the key for the chunk is the value mesh index property in the Node structure
        // a chunk can be deactivated by setting the key to a negative value but the data remains
        // until some later time when the byte offsets and byte lengths can be stuffed into the
        // node's bufferviews at the end of the 
        private Dictionary<int, Byte[]> ChunkStoreN { get; set; } = new Dictionary<int, byte[]>();
        private List<byte[]> ChunkStore { get; set; } = new List<byte[]> ();
        public int AddChunk(ref byte[] chunk)
        {
            ChunkStore.Add(chunk);
            m_nextByte += chunk.Length;
            return ByteOffset;
        }
        public int AddChunk(float[] fchunk)
        {
            int nFloat = fchunk.Length;
            int nBytes = nFloat * sizeof(float);
            byte[] tbuffer = new byte[nBytes];
            System.Buffer.BlockCopy(fchunk, 0, tbuffer, 0, nBytes);
            return AddChunk(ref tbuffer);
        }
        public int AddChunk(ushort[] schunk)
        {
            int nShort = schunk.Length;
            int nBytes = nShort * sizeof(ushort);
            byte[] tbuffer = new byte[nBytes];
            System.Buffer.BlockCopy(schunk, 0, tbuffer, 0, nBytes);
            return AddChunk(ref tbuffer);
        }
        public int AddChunk(short[] schunk)
        {
            int nShort = schunk.Length;
            int nBytes = nShort * sizeof(short);
            byte[] tbuffer = new byte[nBytes];
            System.Buffer.BlockCopy(schunk, 0, tbuffer, 0, nBytes);
            return AddChunk(ref tbuffer);
        }
        private int m_nextByte = 0;
        public int ByteOffset
        {
            get
            {
                return m_nextByte;
            }
        }
        public int NumberOfChunks
        {
            get
            {
                return ChunkStore.Count;
            }
        }
        public void WriteChunks(string uri, string worldName, string instanceID)
        {
            string fileName = uri + worldName + "." + instanceID + ".bin";
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                for (int nChunk = 0; nChunk < ChunkStore.Count; nChunk++)
                {
                    stream.Write(ChunkStore[nChunk], 0, ChunkStore[nChunk].Length);
                }
            }
        }
        public void Clear()
        {
            ChunkStore.Clear();
            m_nextByte = 0;
        }
    }
}
