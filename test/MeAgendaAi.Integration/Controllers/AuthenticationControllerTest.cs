using FluentAssertions;
using FluentAssertions.Extensions;
using MeAgendaAi.Common.Builder;
using MeAgendaAi.Common.Builder.Common;
using MeAgendaAi.Common.Builder.RequestAndResponse;
using MeAgendaAi.Domains.RequestAndResponse;
using MeAgendaAí.Infra.Notification;
using MeAgendaAi.Integration.SetUp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MeAgendaAi.Integration.Controllers
{
    public class AddClientAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn201Created()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithNameInvalid().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldAddAPhysicalPersonTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddPhysicalPersonRequestBuilder().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

            var physicalPersonInDatabase = await DbContext.PhysicalPersons.FirstAsync(f => f.Email.Address == request.Email);
            var responseExpected = new SuccessMessage<Guid>(physicalPersonInDatabase.Id, "Cadastrado com sucesso");
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorMessageWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<string>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryingToAddAnIndividualWithAlreadyRegisteredEmail()
        {
            var physicalPerson = new PhysicalPersonBuilder().Generate();
            await DbContext.PhysicalPersons.AddAsync(physicalPerson);
            await DbContext.SaveChangesAsync();
            var request = new AddPhysicalPersonRequestBuilder().WithEmail(physicalPerson.Email.Address).Generate();
            var listErrorsExpected = new List<Notification>
            {
                new("Email", "Email já cadastrado")
            };
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddAPhysicalPersonWithPasswordAndConfirmPasswordNotEqual()
        {
            var request = new AddPhysicalPersonRequestBuilder().WithConfirmPassword("different-password").Generate();
            var listErrorsExpected = new List<Notification>
            {
                new("ConfirmPassword", "Senha de confirmação não é igual a senha")
            };
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }
    }

    public class AddCompanyAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task AuthenticaitonAddCompany_ShouldAddCompanyTypeUserAndReturn201Created()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);

            response.Should().Be201Created();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturn400BadRequest()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), requestInvalid);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task AuthenticationAddCompany_ShouldAddCompanyTypeUserAndReturnBaseResponseWithGuid()
        {
            var request = new AddCompanyRequestBuilder().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<Guid>>();

            var companyInDatabase = await DbContext.Companies.FirstAsync(f => f.Email.Address == request.Email);
            var responseExpected = new SuccessMessage<Guid>(companyInDatabase.Id, "Cadastrado com sucesso");
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorMessageWhenTryToAddAPhysicalPersonWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();
            var messageError = "Request: Email: Can't be empty";
            var responseExpected = new ErrorMessage<string>(messageError, "Invalid requisition");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<string>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddACompanyWithLongLengthName()
        {
            var requestInvalid = new AddCompanyRequestBuilder().WithNameInvalid(100).Generate();
            var companyInvalid = new CompanyBuilder().ByRequest(requestInvalid).Generate();
            var noticationContext = new NotificationContext();
            noticationContext.AddNotifications(companyInvalid.ValidationResult);
            var listErrorsExpected = new List<Notification>();
            listErrorsExpected.AddRange(noticationContext.Notifications.ToList());

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), requestInvalid);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnAnErrorWhenTryToAddACompanyWithAlreadyRegisteredEmail()
        {
            var company = new CompanyBuilder().Generate();
            await DbContext.Companies.AddAsync(company);
            await DbContext.SaveChangesAsync();
            var request = new AddCompanyRequestBuilder().WithEmail(company.Email.Address).Generate();
            var listErrorsExpected = new List<Notification>
            {
                new("Email", "E-mail already registered.")
            };
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturnErrorWhenTryToAddACompanyWithPasswordAndConfirmPasswordNotEqual()
        {
            var request = new AddCompanyRequestBuilder().WithConfirmPassword("different-password").Generate();
            var listErrorsExpected = new List<Notification>
            {
                new("ConfirmPassword", "Confirmation password is not the same as password.")
            };
            var responseExpected = new ErrorMessage<List<Notification>>(listErrorsExpected, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddCompany"), request);
            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();

            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task AuthenticationAddClient_ShouldReturn400BadRequestWhenTryToAddACompanyWithAnInvalidRequest()
        {
            var requestInvalid = new AddPhysicalPersonRequestBuilder().WithEmailInvalid().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "AddPhysicalPerson"), requestInvalid);

            response.Should().Be400BadRequest();
        }
    }

    public class AuthenticateAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task Authenticate_ShouldReturn200Ok()
        {
            var id = Guid.NewGuid();
            var request = new AuthenticateRequestBuilder().Generate();
            var password = PasswordBuilder.Encrypt(request.Password, id);
            var user = new UserBuilder().WithId(id).WithEmail(request.Email).WithPassword(password).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            response.Should().Be200Ok();
        }

        [Test]
        public async Task Authenticate_ShouldReturnBadRequest400WhenNotFindUser()
        {
            var request = new AuthenticateRequestBuilder().Generate();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task Authenticate_ShouldReturnErrorWhenNotFindUser()
        {
            var request = new AuthenticateRequestBuilder().Generate();
            var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>()
            {
                new Notification("User", "User not found!")
            }, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task Authenticate_ShouldReturnAnErrorWhenThePasswordIsNotTheSameRegistered()
        {
            var id = Guid.NewGuid();
            var request = new AuthenticateRequestBuilder().Generate();
            var user = new UserBuilder().WithId(id).WithEmail(request.Email).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>()
            {
                new Notification("User", "Wrong password.")
            }, "Errors");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task Authenticate_ShouldReturnBadRequest400WhenThePasswordIsNotTheSameRegistered()
        {
            var id = Guid.NewGuid();
            var request = new AuthenticateRequestBuilder().Generate();
            var user = new UserBuilder().WithId(id).WithEmail(request.Email).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task Authenticate_ShouldReturnAuthenticateResponseCorrectly()
        {
            var id = Guid.NewGuid();
            var request = new AuthenticateRequestBuilder().Generate();
            var password = PasswordBuilder.Encrypt(request.Password, id);
            var user = new UserBuilder().WithId(id).WithEmail(request.Email).WithPassword(password).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            var authenticateResponse = new AuthenticateResponseBuilder().FromUser(user).Generate();
            var responseExpected = new SuccessMessage<AuthenticateResponse>(authenticateResponse, "Successfully authenticated");

            var response = await Client.PostAsJsonAsync(RequisitionAssemblyFor("Authentication", "Authenticate"), request);

            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<AuthenticateResponse>>();
            content?.Should()
                .BeEquivalentTo(responseExpected, option => option
                    .Excluding(prop => prop.Result.Token)
                    .Excluding(prop => prop.Result.RefreshToken)
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
            content?.Result.Token.Should().NotBeNullOrEmpty();
            content?.Result.RefreshToken.Should().NotBeNullOrEmpty();
        }
    }

    public class RefreshTokenAuthenticationControllerTest : TestBase
    {
        [Test]
        public async Task RefreshToken_ShouldReturn200Ok()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            response.Should().Be200Ok();
        }

        [Test]
        public async Task RefreshToken_ShouldReturnBadRequest400WhenNotFindRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString("N");

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task RefreshToken_ShouldReturnBadRequest400WhenNotFindUser()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();
            await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            response.Should().Be400BadRequest();
        }

        [Test]
        public async Task RefreshToken_ShouldReturnErrorWhenNotFindRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>
            {
                new Notification("Resfresh Token", "Refresh token found.")
            }, "Errors");

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task RefreshToken_ShouldReturnErrorWhenNotFindUser()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();
            await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));
            var responseExpected = new ErrorMessage<List<Notification>>(new List<Notification>
            {
                new Notification("User", "User not found.")
            }, "Errors");

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            var content = await response.Content.ReadFromJsonAsync<ErrorMessage<List<Notification>>>();
            content.Should().BeEquivalentTo(responseExpected);
        }

        [Test]
        public async Task RefreshToken_ShouldReturnAuthenticateResponseCorrectly()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var id = Guid.NewGuid();
            var user = new UserBuilder().WithId(id).Generate();
            await DbContext.Users.AddAsync(user);
            await DbContext.SaveChangesAsync();
            await DbRedis.SetStringAsync(refreshToken, JsonConvert.SerializeObject(user.Id));
            var authenticateResponse = new AuthenticateResponseBuilder()
               .FromUser(user)
               .Generate();
            var responseExpected = new SuccessMessage<AuthenticateResponse>(authenticateResponse, "Successfully authenticated");

            var response = await Client.PostAsync(RequisitionAssemblyFor("Authentication", "RefreshToken", new Dictionary<string, string>()
            {
                {
                    "RefreshToken",
                    refreshToken
                }
            }), default!);

            var content = await response.Content.ReadFromJsonAsync<SuccessMessage<AuthenticateResponse>>();
            content?.Should()
                .BeEquivalentTo(responseExpected, option => option
                    .Excluding(prop => prop.Result.Token)
                    .Excluding(prop => prop.Result.RefreshToken)
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
            content?.Result.Token.Should().NotBeNullOrEmpty();
            content?.Result.RefreshToken.Should().NotBeNullOrEmpty();
        }
    }
}