﻿using JS.AMS.Data.Core.Entity;

namespace JS.AMS.Data.Entity.TicketModule
{
    public class TicketCategory : Auditable
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Remark { get; set; }
    }
}