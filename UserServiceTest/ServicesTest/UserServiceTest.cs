using AutoFixture;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using UserServiceTest.Client;
using Xunit;

namespace UserServiceTest.ServicesTest
{
    public class UserServiceTest : TestBase
    {
        public UserServiceTest() : base()
        {
        }

        [Fact(DisplayName = "Register valid creds")]
        public async Task WhenEmailIsValid_ThenRegister()
        {
            //Act
            var response = await authClient.RegisterAsync("test00@test.ru");

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Content.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Register invalid creds")]
        public async Task WhenEmailIsInvalid_ThenRegister()
        {
            //Act
            var response = await authClient.RegisterAsync("");

            //Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(403);
            response.Content.Should().BeNullOrEmpty();
        }

        [Fact(DisplayName = "RequestActivationCode valid id")]
        public async Task WhenIdIsValid_ThenRequestActivationCode()
        {
            //Arrange
            var registerResponse = await authClient.RegisterAsync("test0@test.ru");

            //Act
            var response = await authClient.RequestActivationCodeAsync(registerResponse.Content);

            //Assert
            response.Should().Be(200);
        }

        [Fact(DisplayName = "RequestActivationCode not found id")]
        public async Task WhenIdIsRandom_ThenRequestActivationCode()
        {
            //Act
            var response = await authClient.RequestActivationCodeAsync(Guid.NewGuid().ToString());

            //Assert
            response.Should().Be(400);
        }

        [Fact(DisplayName = "RequestActivationCode invalid id")]
        public async Task WhenIdIsInvalid_ThenRequestActivationCode()
        {
            //Act
            var response = await authClient.RequestActivationCodeAsync("");

            //Assert
            response.Should().Be(403);
        }

        [Fact(DisplayName = "ConfirmEmail valid creds")]
        public async Task WhenCredsIsValid_ThenConfirmEmail()
        {
            //Arrange
            var guestUserId = (await authClient.RegisterAsync("test1@test.ru")).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);

            //Act
            var response = await authClient.ConfirmEmailAsync(guestUserId, "passwoed!SKdma", activationCode);

            //Assert
            response.Should().Be(201);
        }

        [Fact(DisplayName = "ConfirmEmail invalid all creds")]
        public async Task WhenAllCredsIsInvalid_ThenConfirmEmail()
        {
            //Act
            var response = await authClient.ConfirmEmailAsync("random", "passwoed!SKdma", "random");

            //Assert
            response.Should().Be(400);
        }

        [Fact(DisplayName = "ConfirmEmail invalid activation code")]
        public async Task WhenActivationCodeIsInvalid_ThenConfirmEmail()
        {
            //Arrange
            var guestUserId = (await authClient.RegisterAsync("test2@test.ru")).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);

            //Act
            var response = await authClient.ConfirmEmailAsync(guestUserId, "passwoed!SKdma", "random");

            //Assert
            response.Should().Be(403);
        }

        [Fact(DisplayName = "SignIn valid creds")]
        public async Task WhenCreadsIsValid_ThenSignIn()
        {
            //Arrange
            var guestUserId = (await authClient.RegisterAsync("test3@test.ru")).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);
            var password = AutoFixture.Create<string>();
            await authClient.ConfirmEmailAsync(guestUserId, password, activationCode);

            //Act
            var response = await authClient.SignInAsync(guestUserId, password);

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Content.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "SignIn invalid creds")]
        public async Task WhenCreadsIsInvalid_ThenSignIn()
        {
            //Act
            var response = await authClient.SignInAsync("test3@test.ru", "test3@test.ru");

            //Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(404);
            response.Content.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "UpdateUser valid creds")]
        public async Task WhenCreadsIsValid_ThenUpdateUser()
        {
            //Arrange
            var email = "test33@test.ru";
            var guestUserId = (await authClient.RegisterAsync(email)).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);
            var password = AutoFixture.Create<string>();
            await authClient.ConfirmEmailAsync(guestUserId, password, activationCode);
            var authToken = (await authClient.SignInAsync(guestUserId, password)).Content;

            //Act
            var response = await authClient.UpdateUserAsync(authToken, "test_" + email, password);

            //Assert
            response.Should().Be(204);
        }

        [Fact(DisplayName = "UpdateUser invalid authToken")]
        public async Task WhenAuthTokenIsInvalid_ThenUpdateUser()
        {
            //Arrange
            var email = "test33@test.ru";
            var guestUserId = (await authClient.RegisterAsync(email)).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);
            var password = "passwoed!SKdma";
            await authClient.ConfirmEmailAsync(guestUserId, password, activationCode);
            var authToken = (await authClient.SignInAsync(guestUserId, password)).Content;

            //Act
            var response = await authClient.UpdateUserAsync(authToken+"123", email, password);

            //Assert
            response.Should().Be(401);
        }

        [Fact(DisplayName = "UpdateToken valid token")]
        public async Task WhenAuthTokenIsValid_ThenUpdateToken()
        {
            //Arrange
            var email = "test33@test.ru";
            var guestUserId = (await authClient.RegisterAsync(email)).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);
            var password = "passwoed!SKdma";
            await authClient.ConfirmEmailAsync(guestUserId, password, activationCode);
            var authToken = (await authClient.SignInAsync(guestUserId, password)).Content;

            //Act
            var response = await authClient.UpdateTokenAsync(authToken);

            //Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(204);
            response.Content.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "UpdateToken invalid token")]
        public async Task WhenAuthTokenIsInvalid_ThenUpdateToken()
        {
            //Act
            var response = await authClient.UpdateTokenAsync(AutoFixture.Create<string>());

            //Assert
            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be(404);
            response.Content.Should().BeNullOrEmpty();
        }

        [Fact(DisplayName = "Sign out IsAuthorized")]
        public async Task WhenIsAuthorized_ThenUpdateUser()
        {
            //Arrange
            var email = "test33@test.ru";
            var guestUserId = (await authClient.RegisterAsync(email)).Content;
            await authClient.RequestActivationCodeAsync(guestUserId);
            var activationCode = await authClient.ConfirmationCodeAsync(guestUserId);
            var password = "passwoed!SKdma";
            await authClient.ConfirmEmailAsync(guestUserId, password, activationCode);
            var authToken = (await authClient.SignInAsync(guestUserId, password)).Content;

            //Act
            var response = await authClient.SignOutAsync();

            //Assert
            response.Should().Be(200);
        }

        [Fact(DisplayName = "Sign out Is Not Authorized")]
        public async Task WhenIsNotAuthorized_ThenUpdateUser()
        {
            //Act
            var response = await authClient.SignOutAsync();

            //Assert
            response.Should().Be(400);
        }
    }
}
