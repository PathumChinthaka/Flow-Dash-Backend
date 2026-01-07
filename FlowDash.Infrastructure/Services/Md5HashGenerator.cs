using FlowDash.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace FlowDash.Infrastructure.Services
{
    internal class Md5HashGenerator : IMd5HashGenerator
    {
        public string Generate(string input)
        {
            MD5 md5 = MD5.Create();

            //compute hash from the bytes of text  
            md5.ComputeHash(Encoding.ASCII.GetBytes(input));

            //get hash result after compute it  
            byte[] result = md5.Hash!;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }
}
