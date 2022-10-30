using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qr_System.DTOs
{
    public class ResponseModel
    {
        public string message { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string address { get; set; }

        public string phone { get; set; }

        public string image { get; set; }

        public bool isAuthenticated { get; set; }

        public string token { get; set; }

        public string userName { get; set; }

        public string id { get; set; }

        public string email { get; set; }

        public List<string> roles { get; set; }

        public List<string> errors { get; set; }

        public DateTime expiresOn { get; set; }

        public string type { get; set; }

    }

}