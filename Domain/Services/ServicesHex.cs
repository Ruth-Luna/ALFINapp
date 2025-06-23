using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFINapp.Domain.Services
{
    public class ServicesHex
    {
        public static byte[] HexStringToBytes(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return Array.Empty<byte>();

            if (hex.StartsWith("0x"))
                hex = hex.Substring(2); // Remueve el '0x'

            if (hex.Length % 2 != 0)
                throw new ArgumentException("La cadena hexadecimal no tiene longitud par.");

            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }
    }
}