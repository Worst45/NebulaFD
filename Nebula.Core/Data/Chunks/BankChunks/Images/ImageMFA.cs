using Nebula.Core.Memory;
using Nebula.Core.Utilities;
using System.Diagnostics;

namespace Nebula.Core.Data.Chunks.BankChunks.Images
{
    public class ImageMFA : Image
    {
        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt();
            if (NebulaCore.Build < 284)
                Handle++;

            Checksum = reader.ReadInt();
            References = reader.ReadInt();
            var dataSize = reader.ReadInt();
            if (Parameters.DontIncludeImages)
            {
                // Header only: the pixel data is skipped, not decompressed (ImageData stays empty)
                Width = reader.ReadShort();
                Height = reader.ReadShort();
                GraphicMode = reader.ReadByte();
                Flags.Value = reader.ReadByte();
                reader.ReadShort();
                HotspotX = reader.ReadShort();
                HotspotY = reader.ReadShort();
                ActionPointX = reader.ReadShort();
                ActionPointY = reader.ReadShort();
                TransparentColor = reader.ReadColor();
                reader.Skip(dataSize);
                ImageBank.LoadedImageCount++;
                return;
            }
            var newReader = new ByteReader(reader.ReadBytes(dataSize + 20));
            Width = newReader.ReadShort();
            Height = newReader.ReadShort();
            GraphicMode = newReader.ReadByte();
            Flags.Value = newReader.ReadByte();
            newReader.ReadShort();
            HotspotX = newReader.ReadShort();
            HotspotY = newReader.ReadShort();
            ActionPointX = newReader.ReadShort();
            ActionPointY = newReader.ReadShort();
            TransparentColor = newReader.ReadColor();
            if (Flags["LZX"])
            {
                var decompressedSize = newReader.ReadInt();
                ImageData = Decompressor.DecompressBlock(newReader, (int)(newReader.Size() - newReader.Tell()));
            }
            else ImageData = newReader.ReadBytes(dataSize);
            newReader.Dispose();

            ImageBank.LoadedImageCount++;
        }
    }
}
