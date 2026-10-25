using Nebula.Core.Data.PackageReaders;
using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.MFAChunks
{
    public class MFAAppIcon : Chunk
    {
        public uint[] IconHandles = new uint[0];

        public MFAAppIcon()
        {
            ChunkName = "MFAAppIcon";
            ChunkID = 0x009A;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            IconHandles = new uint[reader.ReadInt()];
            for (int i = 0; i < IconHandles.Length; i++)
                IconHandles[i] = reader.ReadUInt();

            if (IconHandles.Length == 2)
            {
                var iconBank = ((MFAPackageData)NebulaCore.PackageData).IconBank;
                AddIcon(256, IconHandles[1]);
                AddIcon(128, IconHandles[0]);

                // Handles can be off by one, and can reference missing icons in older builds
                void AddIcon(int size, uint handle)
                {
                    if (iconBank.Images.ContainsKey(handle))
                        NebulaCore.CurrentReader.Icons[size] = iconBank[handle].GetBitmap();
                    else if (iconBank.Images.ContainsKey(handle + 1))
                        NebulaCore.CurrentReader.Icons[size] = iconBank[handle + 1].GetBitmap();
                }
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            ByteWriter chunkWriter = new ByteWriter(new MemoryStream());
            {
                chunkWriter.WriteInt(IconHandles.Length);
                foreach (uint handle in IconHandles)
                    chunkWriter.WriteUInt(handle);
            }

            writer.WriteByte((byte)ChunkID);
            writer.WriteInt((int)chunkWriter.Tell());
            writer.WriteWriter(chunkWriter);
            chunkWriter.Flush();
            chunkWriter.Close();
        }
    }
}
