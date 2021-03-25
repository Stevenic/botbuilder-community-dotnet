using System;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bot.Builder.Community.Components.AdaptiveCards
{
    public class CreateAdaptiveCard : BaseAdaptiveCard
    {

        [JsonProperty("$kind")]
        public const string Kind = "BotBuilderCommunity.CreateAdaptiveCard";

        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        protected override Task<object> OnProcessCardAsync(DialogContext dc, JObject card, CancellationToken cancellationToken = default)
        {
            // Write card to memory
            var resultProperty = this.ResultProperty?.GetValue(dc.State);
            if (!String.IsNullOrEmpty(resultProperty))
            {
                dc.State.SetValue(resultProperty, card);
            }

            return Task.FromResult(card as object);
        }
    }
}
