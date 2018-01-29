// -----------------------------------------------------------------------
// <copyright file="DeleteCustomerFromTipAccount.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Store.PartnerCenter.Samples.Customers
{
    using System;

    /// <summary>
    /// Deletes a customer from a testing in production account.
    /// </summary>
    public class DeleteAllCustomers : BasePartnerScenario
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAllCustomers"/> class.
        /// </summary>
        /// <param name="context">The scenario context.</param>
        public DeleteAllCustomers(IScenarioContext context) : base("Delete all customers", context)
        {
        }

        /// <summary>
        /// Executes the scenario.
        /// </summary>
        protected override void RunScenario()
        {
            var partnerOperations = this.Context.UserPartnerOperations;
            var allCustomers = partnerOperations.Customers.Get();

            foreach (var customer in allCustomers.Items)
            {
                this.Context.ConsoleHelper.StartProgress($"Deleting customer {customer.Id}");
                partnerOperations.Customers.ById(customer.Id).DeleteAsync();
            }
            
            this.Context.ConsoleHelper.StopProgress();
            this.Context.ConsoleHelper.Success("Customers successfully deleted");
        }
    }
}
