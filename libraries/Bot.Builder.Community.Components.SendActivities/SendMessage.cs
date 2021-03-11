using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Community.Components.SendActivities
{
    /// <summary>
    /// Sends a message activity populated by an LG template.
    /// </summary>
    public class SendMessage : SendActivityBase
    {

        [JsonConstructor]
        public SendMessage([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "BotBuilderCommunity.SendMessage";

        /// <summary>
        /// Gets or sets template for the message.
        /// </summary>
        /// <value>
        /// Template for the message.
        /// </value>
        [JsonProperty("message")]
        public ITemplate<Activity> Message { get; set; }

        public async override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options is CancellationToken)
            {
                throw new ArgumentException($"{nameof(options)} cannot be a cancellation token");
            }

            if (this.Disabled != null && this.Disabled.GetValue(dc.State) == true)
            {
                return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            // Get activity
            Activity activity = null;
            if (Message != null)
            {
                activity = await Message.BindAsync(dc, dc.State).ConfigureAwait(false);
            }

            // Send activity
            var response = await this.SendActivityAsync(dc, activity, cancellationToken).ConfigureAwait(false);

            // End Dialog
            return await dc.EndDialogAsync(response, cancellationToken).ConfigureAwait(false); ;
        }

        protected override string OnComputeId()
        {
            if (this.OperationType.Value == SendOperationType.Delete)
            {
                return $"{this.GetType().Name}('delete', '{ActivityIdProperty?.ToString().Trim()}')";
            }
            
            if (Message is ActivityTemplate at)
            {
                return $"{this.GetType().Name}({StringUtils.Ellipsis(at.Template.Trim(), 30)})";
            }

            return $"{this.GetType().Name}('{StringUtils.Ellipsis(Message?.ToString().Trim(), 30)}')";
        }

    }
}
