using System.Buffers.Text;
using System.Runtime.InteropServices;

using StronglyTypedIds;

namespace PostCreator.Domain.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct PostId
{
    private const char Slash = '/';
    private const byte SlashB = (byte)Slash;
    private const char EqualsChar = '=';
    private const char Hyphen = '-';
    private const char Undescore = '_';
    private const char Plus = '+';
    private const byte PlusB = (byte)'+';
    
    public static string ToSlug(PostId id)
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

        base64[22] = EqualsChar;
        base64[23] = EqualsChar;

        Span<byte> idB = stackalloc byte[16];
        Convert.TryFromBase64Chars(base64, idB, out _);
        return new PostId(new Guid(idB));
    }
}