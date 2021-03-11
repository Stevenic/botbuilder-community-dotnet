using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Builder.Community.Components.SendActivities
{
    public static class DeliveryModesPlus
    {
        public const string Update = "update";
        public const string Replace = "replace";
        public const string Delete = "delete";
        public const string DirectMessage = "directMessage";
    }
}
