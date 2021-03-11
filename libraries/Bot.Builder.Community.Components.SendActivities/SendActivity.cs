using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Community.Components.SendActivities
{
    /// <summary>
    /// Sends an activity that defined by a property in memory.
    /// </summary>
    public class SendActivity : SendActivityBase
    {

        [JsonConstructor]
        public SendActivity([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "BotBuilderCommunity.SendActivity";

        /// <summary>
        /// Gets or sets the property path for the activity to send.
        /// </summary>
        /// <value>
        /// The property path for the activity to send.
        /// </value>
        [JsonProperty("activityProperty")]
        public StringExpression ActivityProperty { get; set; }

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
            var activityProperty = this.ActivityProperty != null ? this.ActivityProperty.GetValue(dc.State) : null;
            if (!String.IsNullOrEmpty(activityProperty))
            {
                activity = dc.State.GetValue<Activity>(activityProperty);
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

            return $"{this.GetType().Name}('{StringUtils.Ellipsis(ActivityProperty?.ToString().Trim(), 30)}')";
        }

    }
}
