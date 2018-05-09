// -----------------------------------------------------------------------
// <copyright file="GetEntitlements.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Samples.Entitlements
{
    using System;
    using System.Linq;
    using Models.Entitlements;

    /// <summary>
    /// Get customer entitlements.
    /// </summary>
    public class GetEntitlements : BasePartnerScenario
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEntitlements"/> class.
        /// </summary>
        /// <param name="context">The scenario context.</param>
        public GetEntitlements(IScenarioContext context) : base("Get entitlements", context)
        {
        }

        /// <summary>
        /// Executes the get customer entitlements scenario.
        /// </summary>
        protected override void RunScenario()
        {
            string customerIdToRetrieve = this.ObtainCustomerId("Enter the ID of the customer to retrieve entitlements for");

            var partnerOperations = this.Context.UserPartnerOperations;
            this.Context.ConsoleHelper.StartProgress("Retrieving customer entitlements");

            var entitlements = partnerOperations.Customers.ById(customerIdToRetrieve).Entitlements.Get();
            this.Context.ConsoleHelper.StopProgress();

            foreach (var entitlement in entitlements.Items)
            {
                this.Context.ConsoleHelper.WriteObject(entitlement, "Entitlement details");

                try
                {
                    switch (entitlement.EntitlementType)
                    {
                        case EntitlementType.VirtualMachineReservedInstance:
                            var virtualMachineReservedInstanceArtifactDetailsLink =
                                ((VirtualMachineReservedInstanceArtifact)entitlement.EntitledArtifacts.FirstOrDefault(x => x.ArtifactType == ArtifactType.VirtualMachineReservedInstance))?.Link;

                            if (virtualMachineReservedInstanceArtifactDetailsLink != null)
                            {
                                var virtualMachineReservedInstanceArtifactDetails =
                                    virtualMachineReservedInstanceArtifactDetailsLink
                                        .InvokeAsync<VirtualMachineReservedInstanceArtifactDetails>(partnerOperations)
                                        .Result;
                                this.Context.ConsoleHelper.WriteObject(virtualMachineReservedInstanceArtifactDetails);
                            }

                            break;

                        case EntitlementType.Software:
                            var productKeyArtifactLink = ((ProductKeyArtifact)entitlement.EntitledArtifacts.FirstOrDefault(x => x.ArtifactType == ArtifactType.ProductKey))?.Link;

                            // ProductKeyLink could be null when there are no keys.
                            if (productKeyArtifactLink != null)
                            {
                                var productKeyArtifactDetails = productKeyArtifactLink.InvokeAsync<ProductKeyArtifactDetails>(partnerOperations).Result;
                                this.Context.ConsoleHelper.WriteObject(productKeyArtifactDetails);
                            }

                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.Context.ConsoleHelper.WriteObject(ex.Message, "Artifact Details");
                }
            }
        }
    }
}
