using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Coffee.OpenSesameCompilers
{
    internal class Unzip : IDisposable
    {
        class Entry
        {
            public string Name { get; set; }
            public int OriginalSize { get; set; }
            public bool Deflated { get; set; }
            public DateTime Timestamp { get; set; }
            public int HeaderOffset { get; set; }
            public int DataOffset { get; set; }
            public bool IsDirectory { get { return Name.EndsWith("/"); } }
        }

        private const int EntrySignature = 0x02014B50;
        private const int FileSignature = 0x04034b50;
        private const int DirectorySignature = 0x06054B50;
        private const int BufferSize = 16 * 1024;

        public Unzip(string fileName)
        {
            Stream = File.OpenRead(fileName);
            Reader = new BinaryReader(Stream);
            Entries = ReadZipEntries().ToArray();
        }

        private Stream Stream { get; set; }
        private BinaryReader Reader { get; set; }
        private Entry[] Entries { get; set; }

        public void Dispose()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = null;
            }

            if (Reader != null)
            {
                Reader.Close();
                Reader = null;
            }
        }

        public void ExtractToDirectory(string directoryName)
        {
            //var entries = ReadZipEntries().ToArray();
            foreach (var entry in Entries)
            {
                // create target directory for the file
                var fileName = Path.Combine(directoryName, entry.Name);
                var dirName = Path.GetDirectoryName(fileName);
                Directory.CreateDirectory(dirName);

                if (entry.IsDirectory)
                    continue;

                using (var outStream = File.Create(fileName))
                {
                    Extract(entry, outStream);
                }

                File.SetLastWriteTime(fileName, entry.Timestamp);
            }
        }

        void Extract(Entry entry, Stream outputStream)
        {
            // check file signature
            Stream.Seek(entry.HeaderOffset, SeekOrigin.Begin);
            if (Reader.ReadInt32() != FileSignature)
            {
                throw new InvalidDataException("File signature doesn't match.");
            }

            // move to file data
            Stream.Seek(entry.DataOffset, SeekOrigin.Begin);
            var inputStream = Stream;
            if (entry.Deflated)
            {
                inputStream = new DeflateStream(Stream, CompressionMode.Decompress, true);
            }

            // allocate buffer, prepare for CRC32 calculation
            var count = entry.OriginalSize;
            var bufferSize = Math.Min(BufferSize, entry.OriginalSize);
            var buffer = new byte[bufferSize];

            while (count > 0)
            {
                // decompress data
                var read = inputStream.Read(buffer, 0, bufferSize);
                if (read == 0)
                {
                    break;
                }

                // copy to the output stream
                outputStream.Write(buffer, 0, read);
                count -= read;
            }
        }

        private IEnumerable<Entry> ReadZipEntries()
        {
            if (Stream.Length < 22)
                yield break;

            Stream.Seek(-22, SeekOrigin.End);

            // find directory signature
            while (Reader.ReadInt32() != DirectorySignature)
            {
                if (Stream.Position <= 5)
                    yield break;

                // move 1 byte back
                Stream.Seek(-5, SeekOrigin.Current);
            }

            // read directory properties
            Stream.Seek(6, SeekOrigin.Current);
            var entries = Reader.ReadUInt16();
            Reader.ReadInt32(); //difSize
            var dirOffset = Reader.ReadUInt32();
            Stream.Seek(dirOffset, SeekOrigin.Begin);

            // read directory entries
            for (int i = 0; i < entries; i++)
            {
                if (Reader.ReadInt32() != EntrySignature)
                {
                    continue;
                }

                // read file properties
                Reader.ReadInt32();
                bool utf8 = (Reader.ReadInt16() & 0x0800) != 0;
                short method = Reader.ReadInt16();
                int timestamp = Reader.ReadInt32();
                Reader.ReadUInt32(); // crc32
                Reader.ReadInt32(); //compressedSize
                int fileSize = Reader.ReadInt32();
                short fileNameSize = Reader.ReadInt16();
                short extraSize = Reader.ReadInt16();
                short commentSize = Reader.ReadInt16();
                Reader.ReadInt32(); //headerOffset
                Reader.ReadInt32();
                int fileHeaderOffset = Reader.ReadInt32();
                var fileNameBytes = Reader.ReadBytes(fileNameSize);
                Stream.Seek(extraSize, SeekOrigin.Current);
                Reader.ReadBytes(commentSize); // fileCommentBytes
                var fileDataOffset = CalculateFileDataOffset(fileHeaderOffset);

                // zip file entry
                var encoder = utf8 ? Encoding.UTF8 : Encoding.Default;
                yield return new Entry
                {
                    Name = encoder.GetString(fileNameBytes),
                    OriginalSize = fileSize,
                    HeaderOffset = fileHeaderOffset,
                    DataOffset = fileDataOffset,
                    Deflated = method == 8,
                    Timestamp = ConvertToDateTime(timestamp)
                };
            }
        }

        private int CalculateFileDataOffset(int fileHeaderOffset)
        {
            var position = Stream.Position;
            Stream.Seek(fileHeaderOffset + 26, SeekOrigin.Begin);
            var fileNameSize = Reader.ReadInt16();
            var extraSize = Reader.ReadInt16();

            var fileOffset = (int)Stream.Position + fileNameSize + extraSize;
            Stream.Seek(position, SeekOrigin.Begin);
            return fileOffset;
        }

        static DateTime ConvertToDateTime(int t)
        {
            return new DateTime((t >> 25) + 1980, (t >> 21) & 15, (t >> 16) & 31,
                (t >> 11) & 31, (t >> 5) & 63, (t & 31) * 2);
        }
    }
}
