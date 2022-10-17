﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qr_System.DTOs
{
    public class ResponseModel
    {
        public string Message { get; set; }

        public bool IsAuthenticated { get; set; }

        public string Token { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public List<string> Errors { get; set; }

        public DateTime ExpiresOn { get; set; }

    }

}

