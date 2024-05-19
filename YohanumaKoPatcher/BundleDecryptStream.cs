using System.Security.Cryptography;
using System.Text;

class BundleDecryptStream : Stream
{
    private Stream input;
    private ICryptoTransform cipher;
    public BundleDecryptStream(Stream stream, string key, string salt)
    {
        input = stream;
        var keyDerive = new PasswordDeriveBytes(key, Encoding.UTF8.GetBytes(salt));
        var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;
        cipher = aes.CreateEncryptor(keyDerive.GetBytes(16), null);

        var magic = new byte[8];
        Read(magic, 0, 8);
        if (Encoding.Default.GetString(magic) != "UnityFS\0")
        {
            throw new CryptographicException("Invalid cipher key");
        }
        Seek(0, SeekOrigin.Begin);
    }

    public override bool CanRead => true;

    public override bool CanSeek => input.CanSeek;

    public override bool CanWrite => false;

    public override long Length => input.Length;

    public override long Position { get => input.Position; set => input.Position = value; }

    public override void Flush() => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var pos = (int)input.Position;
        var posOffset = pos % 16;
        var blockCount = (count + posOffset + 15) / 16;
        var keyStreamBuffer = new byte[blockCount * 16];
        var cipherCounter = 1 + pos / 16;
        for (int i = 0; i < blockCount; i++)
        {
            BitConverter.GetBytes(cipherCounter + i).CopyTo(keyStreamBuffer, i * 16);
        }
        cipher.TransformBlock(keyStreamBuffer, 0, blockCount * 16, keyStreamBuffer, 0);
        var readCount = input.Read(buffer, offset, count);
        for (int i = offset; i < readCount; i++)
        {
            buffer[i] ^= keyStreamBuffer[posOffset + i];
        }
        return readCount;
    }

    public override long Seek(long offset, SeekOrigin origin) => input.Seek(offset, origin);

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
}
