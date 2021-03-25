using System.Collections.Generic;
using AdaptiveExpressions.Converters;
using Bot.Builder.Community.Components.AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Newtonsoft.Json;

namespace Bot.Builder.Community.Components.AdaptiveCards
{
    /// <summary>
    /// <see cref="AdaptiveCardsComponentRegistration"/> implementation for adaptive components.
    /// </summary>
    public class AdaptiveCardsComponentRegistration : ComponentRegistration, IComponentDeclarativeTypes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCardsComponentRegistration"/> class.
        /// </summary>
        public AdaptiveCardsComponentRegistration()
        {
        }

        /// <summary>
        /// Gets adaptive <see cref="DeclarativeType"/> resources.
        /// </summary>
        /// <param name="resourceExplorer"><see cref="ResourceExplorer"/> with expected path to get all schema resources.</param>
        /// <returns>Adaptive <see cref="DeclarativeType"/> resources.</returns>
        public virtual IEnumerable<DeclarativeType> GetDeclarativeTypes(ResourceExplorer resourceExplorer)
        {
            yield return new DeclarativeType<OnActionExecute>(OnActionExecute.Kind);
            yield return new DeclarativeType<OnActionSubmit>(OnActionSubmit.Kind);
            yield return new DeclarativeType<OnDataQuery>(OnDataQuery.Kind);
            yield return new DeclarativeType<GetAdaptiveCardTemplate>(GetAdaptiveCardTemplate.Kind);
            yield return new DeclarativeType<CreateAdaptiveCard>(CreateAdaptiveCard.Kind);
            yield return new DeclarativeType<SendAdaptiveCard>(SendAdaptiveCard.Kind);
            yield return new DeclarativeType<UpdateAdaptiveCard>(UpdateAdaptiveCard.Kind);
            yield return new DeclarativeType<SendActionExecuteResponse>(SendActionExecuteResponse.Kind);
            yield return new DeclarativeType<SendDataQueryResponse>(SendDataQueryResponse.Kind);
            yield break;
        }

        /// <summary>
        /// Gets adaptive <see cref="JsonConverter"/> resources.
        /// </summary>
        /// <param name="resourceExplorer">ResourceExplorer to use to resolve references.</param>
        /// <param name="sourceContext">SourceContext to build debugger source map.</param>
        /// <returns>Adaptive <see cref="JsonConverter"/> resources.</returns>
        public virtual IEnumerable<JsonConverter> GetConverters(ResourceExplorer resourceExplorer, SourceContext sourceContext)
        {
            yield return new ObjectExpressionConverter<object>();
            yield break;
        }
    }
}
