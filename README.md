# NKZIPLib

A small C# library to parse NKZIP archive files, which were used in few MMO games from the early 2000s.

## Format

The NKZIP archive format is really simple. There is no compression involved, so it's basically all files just in line one after another.

At the beginning of the files following header is present:
```
struct NKZIPHeader {
  string[16] _magic;
  string[16] _version;
  uint _rawDataBytes;
  uint _fileCount;
}
```

Now we read following structure for the amount we got from `_fileCount` above:
```
struct NKZIPFile {
  uint _fileSize;
  string[260] _fileName;
  byte[_fileSize] _fileBytes;
}
```

## Usage

A small example program using `NKZIPLib` to extract all files from an NKZIP archive:

```cs
using System.IO;
using NKZIPLib;

namespace NKZIPLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream file = File.Open(args[0], FileMode.Open);

            NKZIP nkZIP = new NKZIP(file);
            nkZIP.Extract();
        }
    }
}
```

## License

NKZIPLib is licensed under the MIT License