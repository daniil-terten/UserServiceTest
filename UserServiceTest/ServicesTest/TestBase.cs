using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserServiceTest.Client;

namespace UserServiceTest.ServicesTest
{
    public abstract class TestBase
    {
        protected readonly IFixture AutoFixture = new Fixture().Customize(new AutoMoqCustomization());
        protected AunthentificateClient authClient;

        public TestBase()
        {
            authClient = new AunthentificateClient();
        }
    }
}
