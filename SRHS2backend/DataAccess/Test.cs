﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAccess
{
    public class Test
    {
        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }
    }
}
