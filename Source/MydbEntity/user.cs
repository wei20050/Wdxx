﻿using System.ComponentModel.DataAnnotations;

namespace Tset.Entity
{
    public class user
    {
        [Key]
        public int? id { get; set; }
        public string name { get; set; }
    }
}
