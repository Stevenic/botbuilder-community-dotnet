using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Bot.Builder.Community.Components.SendActivities
{
    public abstract class SendActivityBase : Dialog
    {
        /// <summary>
        /// Gets or sets an optional expression which if is true will disable this action.
        /// </summary>
        /// <example>
        /// "user.age > 18".
        /// </example>
        /// <value>
        /// A boolean expression. 
        /// </value>
        [JsonProperty("disabled")]
        public BoolExpression Disabled { get; set; }

        [JsonConverter(typeof(StringEnumConverter), /*camelCase*/ true)]
        public enum SendOperationType
        {
            /// <summary>
            /// Sends a new activity.
            /// </summary>
            Send,

            /// <summary>
            /// Updates an existing activity in place or sends a new one.
            /// </summary>
            Update,

            /// <summary>
            /// Replaces and existing activity by first deleting the old one and then sending a new one.
            /// </summary>
            Replace,

            /// <summary>
            /// Deletes an existing activity.
            /// </summary>
            Delete,

            /// <summary>
            /// Sends a direct message to another user.
            /// </summary>
            DirectMessage
        }

        /// <summary>
        /// Gets or sets type of operation being performed.
        /// </summary>
        /// <value>
        /// Type of operation being performed.
        /// </value>
        [JsonProperty("operationType")]
        public EnumExpression<SendOperationType> OperationType { get; set; } = new EnumExpression<SendOperationType>(SendOperationType.Send);

        /// <summary>
        /// Gets or sets the property path containing address information for the recipient of a DirectMessage.
        /// </summary>
        /// <value>
        /// Property path containing address information for the recipient of a DirectMessage.
        /// </value>
        [JsonProperty("directMessageToProperty")]
        public StringExpression DirectMessageToProperty { get; set; } = "turn.activity.from";

        /// <summary>
        /// Gets or sets the property path for where the ID of the activity that was sent should be stored.
        /// </summary>
        /// <value>
        /// Optional property path for where the ID of the activity that was sent should be stored.
        /// </value>
        [JsonProperty("activityIdProperty")]
        public StringExpression ActivityIdProperty { get; set; } = "turn.activity.replyToId";

        /// <summary>
        /// Sends an activity using the configured operation mode.
        /// </summary>
        /// <param name="dc">Current DC</param>
        /// <param name="activity">Optional activity to send. Can be null for delete operations.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns></returns>
        protected async Task<ResourceResponse> SendActivityAsync(DialogContext dc, Activity activity = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            var operationType = OperationType.GetValue(dc.State);
            var idProperty = ActivityIdProperty != null ? ActivityIdProperty.GetValue(dc.State) : String.Empty;
            var lastId = !String.IsNullOrEmpty(idProperty) ? dc.State.GetValue<string>(idProperty) : String.Empty;

            // Ensure activity passed
            if (activity == null)
            {
                if (operationType != SendOperationType.Delete)
                {
                    throw new Exception($"{this.Id}: activity can only be empty for delete operations.");
                }
                else
                {
                    activity = MessageFactory.Text(String.Empty);
                }
            }

            //// Check for invoke activity
            //if (dc.Context.Activity.Type == ActivityTypes.Invoke)
            //{
            //    // Generate invoke response
            //    var invokeResponse = new InvokeResponse() { Status = (int)HttpStatusCode.OK, Body = activity.Value };
            //    activity = new Activity() { Type = ActivityTypesEx.InvokeResponse, Value = invokeResponse };
            //}

            // Perform operation
            ResourceResponse response = null;
            switch (operationType)
            {
                case SendOperationType.Send:
                    response = await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    break;
                case SendOperationType.Update:
                    if (!String.IsNullOrEmpty(lastId))
                    {
                        activity.Id = lastId;
                        activity.DeliveryMode = "update";
                        await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        response = await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case SendOperationType.Replace:
                    if (!String.IsNullOrEmpty(lastId))
                    {
                        activity.DeliveryMode = "replace";
                        activity.Id = lastId;
                    }
                    response = await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    break;
                case SendOperationType.Delete:
                    if (!String.IsNullOrEmpty(lastId))
                    {
                        activity.Id = lastId;
                        activity.DeliveryMode = "delete";
                        await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    }
                    break;
                case SendOperationType.DirectMessage:
                    var recipientProperty = DirectMessageToProperty.GetValue(dc.State);
                    var recipient = dc.State.GetValue<JObject>(recipientProperty).ToObject<ChannelAccount>();
                    var from = dc.Context.Activity.From;
                    dc.Context.Activity.From = recipient;   // Work around bug where replies are always sent to the From address
                    activity.DeliveryMode = "directMessage";
                    response = await dc.Context.SendActivityAsync(activity, cancellationToken).ConfigureAwait(false);
                    dc.Context.Activity.From = from;
                    break;
            }

            if (response != null && !String.IsNullOrEmpty(idProperty))
            {
                // Copy new ID to memory
                dc.State.SetValue(idProperty, response.Id);
            }

            return response;
        }
    }
}
