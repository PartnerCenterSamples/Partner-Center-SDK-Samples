// -----------------------------------------------------------------------
// <copyright file="CustomerUserAssignMinecraftLicenses.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


using Microsoft.Store.PartnerCenter.Models.Licenses;

namespace Microsoft.Store.PartnerCenter.Samples.CustomerUser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    class CustomerUserAssignMinecraftLicenses : BasePartnerScenario
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerUserAssignMinecraftLicenses"/> class.
        /// </summary>
        /// <param name="context">The scenario context.</param>
        public CustomerUserAssignMinecraftLicenses(IScenarioContext context) : base("Assign customer user a minecraft license", context)
        {
        }

        protected override void RunScenario()
        {
            // Minecraft product id.
            string minecraftProductSkuId = "984df360-9a74-4647-8cf8-696749f6247a";

            // Subscribed Sku for minecraft;
            SubscribedSku minecraftSubscribedSku = null;

            // get customer Id of the entered customer user.
            string selectedCustomerId = this.ObtainCustomerId("Enter the ID of the customer");

            // get customer user Id.
            string selectedCustomerUserId = this.ObtainCustomerUserId("Enter the ID of the customer user to assign license");

            var partnerOperations = this.Context.UserPartnerOperations;

            this.Context.ConsoleHelper.StartProgress("Getting Minecraft Subscribed Skus");

            List<LicenseGroupId> groupIds = new List<LicenseGroupId>() { LicenseGroupId.Group2 };

            // get customer's subscribed skus information.
            var customerSubscribedSkus = partnerOperations.Customers.ById(selectedCustomerId).SubscribedSkus.Get(groupIds);
            // check if a minecraft exists  for a given user
            foreach (var customerSubscribedSku in customerSubscribedSkus.Items)
            {
                if (customerSubscribedSku.ProductSku.Id.ToString() == minecraftProductSkuId)
                {
                    minecraftSubscribedSku = customerSubscribedSku;
                }
            }
            if (minecraftSubscribedSku == null)
            {
                Console.WriteLine("Customer user doesnt have minecraft subscribed sku");
                this.Context.ConsoleHelper.StopProgress();
                return;
            }

            this.Context.ConsoleHelper.StopProgress();

            // Prepare license request.
            LicenseUpdate updateLicense = new LicenseUpdate();

            // Select the first subscribed sku.
            SubscribedSku sku = minecraftSubscribedSku;
            LicenseAssignment license = new LicenseAssignment();

            // assigning first subscribed sku as the license
            license.SkuId = sku.ProductSku.Id;
            license.ExcludedPlans = null;

            List<LicenseAssignment> licenseList = new List<LicenseAssignment>();
            licenseList.Add(license);
            updateLicense.LicensesToAssign = licenseList;

            this.Context.ConsoleHelper.StartProgress("Assigning Minecraft License");

            // Assign licenses to the user.
            var assignLicense = partnerOperations.Customers.ById(selectedCustomerId).Users.ById(selectedCustomerUserId).LicenseUpdates.Create(updateLicense);
            this.Context.ConsoleHelper.StopProgress();

            this.Context.ConsoleHelper.StartProgress("Getting Assigned Minecraft License");

            // get customer user assigned licenses information after assigning the license.
            var customerUserAssignedLicenses = partnerOperations.Customers.ById(selectedCustomerId).Users.ById(selectedCustomerUserId).Licenses.Get(groupIds);
            this.Context.ConsoleHelper.StopProgress();

            Console.WriteLine("Minecraft license was successfully assigned to the user.");
            License userLicense = customerUserAssignedLicenses.Items.First();
            this.Context.ConsoleHelper.WriteObject(userLicense, "Assigned License");
        }
    }
}
