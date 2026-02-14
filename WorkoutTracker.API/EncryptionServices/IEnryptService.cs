using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutTracker.API.EncryptionServices
{
    public interface IEnryptService
    {
        public string Encryption(string PlainText);
        public string Decryption(string EncText);

    }
}
