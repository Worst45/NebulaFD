using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    // Comment (remark) parameter, only found in MFAs.
    // The text itself is stored in FrameEvents.Comments and referenced by TextID.
    public class ParameterComment : ParameterChunk
    {
        public byte[] Font = new byte[0]; // 16-bit LOGFONT
        public Color FontColor;
        public Color BackColor;
        public short Align;
        public ushort TextID;
        public byte[] Style = new byte[0];

        public int Size; // Set by Parameter before reading

        public ParameterComment()
        {
            ChunkName = "ParameterComment";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            long start = reader.Tell();
            Font = reader.ReadBytes(18 + (NebulaCore.Yunicode ? 64 : 32));
            FontColor = reader.ReadColor();
            BackColor = reader.ReadColor();
            Align = reader.ReadShort();
            TextID = reader.ReadUShort();
            Style = reader.ReadBytes(Math.Max(0, Size - (int)(reader.Tell() - start)));
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteBytes(Font);
            writer.WriteColor(FontColor);
            writer.WriteColor(BackColor);
            writer.WriteShort(Align);
            writer.WriteUShort(TextID);
            writer.WriteBytes(Style);
        }

        public override string ToString()
        {
            Comment? comment = Parent?.FrameEvents?.Comments.FirstOrDefault(x => x.Handle == TextID);
            return comment == null ? string.Empty : comment.Value;
        }
    }
}
