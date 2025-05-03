namespace TabloX2.Helpers
{
    public static class Base32
    {
        private static readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string ToBase32String(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            var result = new char[(data.Length * 8 + 4) / 5];
            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;
            int resultIndex = 0;

            while (bitsLeft > 0 || next < data.Length)
            {
                if (bitsLeft < 5)
                {
                    if (next < data.Length)
                    {
                        buffer <<= 8;
                        buffer |= data[next++] & 0xFF;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = 5 - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = 0x1F & (buffer >> (bitsLeft - 5));
                bitsLeft -= 5;
                result[resultIndex++] = _alphabet[index];
            }

            return new string(result);
        }
    }
} 