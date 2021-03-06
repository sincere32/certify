﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Certify.Client;
using Certify.Models;
using Certify.Models.Config;
using Certify.UI.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Certify.UI.Tests.Integration
{
    [TestClass]
    public class ViewModelTest
    {
        [TestMethod, Ignore]
        public async Task TestViewModelSetup()
        {
            var mockClient = new Mock<ICertifyClient>();

            mockClient.Setup(c => c.GetPreferences()).Returns(
                Task.FromResult(new Models.Preferences { })
                );

            mockClient.Setup(c => c.GetManagedCertificates(It.IsAny<Models.ManagedCertificateFilter>()))
                .Returns(
                Task.FromResult(new List<ManagedCertificate> {
                    new ManagedCertificate{
                         Id= Guid.NewGuid().ToString(),
                         Name="Test  Managed Certificate"
                    }
                })
                );

            mockClient.Setup(c => c.GetAccounts())
                .Returns(
                Task.FromResult(
                    new List<AccountDetails> {
                        new AccountDetails {
                            Email = "test@example.com",
                            IsStagingAccount = true,
                            CertificateAuthorityId = StandardCertAuthorities.LETS_ENCRYPT
                        }
                    })
                );

            mockClient.Setup(c => c.GetCredentials())
              .Returns(
              Task.FromResult(new List<StoredCredential> { })
              );

            var appModel = new AppViewModel(mockClient.Object);

            await appModel.LoadSettingsAsync();

            Assert.IsTrue(appModel.ManagedCertificates.Count > 0, "Should have managed sites");

            Assert.IsTrue(appModel.HasRegisteredContacts, "Should have a registered contact");

            await appModel.RefreshStoredCredentialsList();

            appModel.RenewAll(new RenewalSettings { });
        }
    }
}
