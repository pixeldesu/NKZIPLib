using System;
using System.IO;

namespace NKZIPLib
{
    public class NKZIP
    {
        /// <summary>
        /// Files that were parsed from the NKZIP file
        /// </summary>
        private NKZIPFile[] _files;

        /// <summary>
        /// Version of the NKZIP file
        /// </summary>
        private int _version;

        /// <summary>
        /// Size of all raw file bytes contained in the NKZIP file (sum of all FileBytes in _files)
        /// </summary>
        private uint _rawDataBytes;

        /// <summary>
        /// Amount of files present in the NKZIP file
        /// </summary>
        private uint _fileCount;
        
        /// <summary>
        /// NKZIP constructor
        /// </summary>
        /// <param name="fileStream">File stream of the NKZIP file to be parsed</param>
        public NKZIP(Stream fileStream)
        {
            Parse(fileStream);
        }

        /// <summary>
        /// Method to parse the NKZIP file content
        /// </summary>
        /// <param name="fileStream">File stream of the NKZIP file to be parsed</param>
        private void Parse(Stream fileStream) {
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                string magic = (new String(reader.ReadChars(16))).Replace("\0", string.Empty);

                if (magic != "NKZIP")
                {
                    throw new Exception("The file to be parsed is not a valid NKZIP file");
                }

                _version = (int) Char.GetNumericValue(reader.ReadChar());

                reader.BaseStream.Seek(15, SeekOrigin.Current);

                _rawDataBytes = reader.ReadUInt32();
                _fileCount = reader.ReadUInt32();

                _files = new NKZIPFile[_fileCount];

                for (int i = 0; i < _fileCount; i++)
                {
                    NKZIPFile nkZipFile = new NKZIPFile();

                    nkZipFile.FileSize = reader.ReadUInt32();
                    nkZipFile.FileName = (new String(reader.ReadChars(260))).Replace("\0", string.Empty);
                    nkZipFile.FileBytes = reader.ReadBytes((int) nkZipFile.FileSize);

                    _files[i] = nkZipFile;
                }
            }
        }

        /// <summary>
        /// Method to extract the NKZIP file contents
        /// </summary>
        public void Extract()
        {
            for (int i = 0; i < _files.Length; i++)
            {
                FileInfo file = new FileInfo(_files[i].FileName);
                file.Directory.Create();
                File.WriteAllBytes(file.FullName, _files[i].FileBytes);
            }
        }
    }

    /// <summary>
    /// Structure of a single file that's part of an NKZIP
    /// </summary>
    public struct NKZIPFile
    {
        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public uint FileSize;

        /// <summary>
        /// File name/path of the file
        /// </summary>
        public string FileName;

        /// <summary>
        /// Contents of the file as a byte array
        /// </summary>
        public byte[] FileBytes;
    }
}
