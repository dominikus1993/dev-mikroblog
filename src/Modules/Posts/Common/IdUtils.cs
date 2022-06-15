using System.Buffers.Text;
using System.Runtime.InteropServices;

using PostId = DevMikroblog.Modules.Posts.Domain.Model.PostId;

namespace DevMikroblog.Modules.Posts.Common;

public static class Id
{
    private const char Slash = '/';
    private const byte SlashB = (byte)Slash;
    private const char Equals = '=';
    private const char Hyphen = '-';
    private const char Undescore = '_';
    private const char Plus = '+';
    private const byte PlusB = (byte)'+';
    
    public static string ToSlug(this PostId id)
    {
        Span<byte> idB = stackalloc byte[16];
        Span<byte> base64 = stackalloc byte[24];

        var guid = id.Value;
        MemoryMarshal.TryWrite(idB, ref guid);
        Base64.EncodeToUtf8(idB, base64, out _, out _);

        Span<char> result = stackalloc char[24];

        for (int i = 0; i < 22; i++)
        {
            result[i] = base64[i] switch
            {
                SlashB => Hyphen,
                PlusB => Undescore,
                var b => (char)b
            };
        }

        return new string(result);
    }
    
    public static PostId FromSlug(ReadOnlySpan<char> slug)
    {
        Span<char> base64 = stackalloc char[24];

        for (int i = 0; i < 22; i++)
        {
            base64[i] = slug[i] switch
            {
                Hyphen => Slash,
                Undescore => Plus,
                var ch => ch,
            };
        }

        base64[22] = Equals;
        base64[23] = Equals;

        Span<byte> idB = stackalloc byte[16];
        Convert.TryFromBase64Chars(base64, idB, out _);
        return new PostId(new Guid(idB));
    }
}