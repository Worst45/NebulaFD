using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    // MFA frame chunk 0x31: frame included in this frame (frame property of recent Fusion builds).
    // 16 bytes seen in build 296: 1, 0, handle of the included frame (-1 = none), -1.
    // Only the third field is understood (checked against the frame handles of a project);
    // the others are kept as they are.
    public class FrameInclude : Chunk
    {
        public int Field0;
        public int Field1;
        public int IncludedFrameHandle = -1;
        public int Field3 = -1;

        public FrameInclude()
        {
            ChunkName = "FrameInclude";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            if (reader.Size() < 16)
                return;
            Field0 = reader.ReadInt();
            Field1 = reader.ReadInt();
            IncludedFrameHandle = reader.ReadInt();
            Field3 = reader.ReadInt();
            ((Frame)extraInfo[0]).FrameInclude = this;
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteByte(0x31);
            writer.WriteInt(16);
            writer.WriteInt(Field0);
            writer.WriteInt(Field1);
            writer.WriteInt(IncludedFrameHandle);
            writer.WriteInt(Field3);
        }
    }
}
