using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.ResolveAnything;

namespace Telerik.JustMock.Autofac
{
	internal sealed class MocksSource : IRegistrationSource
	{
		private readonly List<object> mocks = new List<object>();
		private readonly AnyConcreteTypeNotAlreadyRegisteredSource concretesSource = new AnyConcreteTypeNotAlreadyRegisteredSource();

		public Type ResolvedType { get; set; }

		public bool IsAdapterForIndividualComponents
		{
			get { return false; }
		}

		public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
		{
			var typedService = service as TypedService;
			if (typedService == null
				|| typedService.ServiceType.IsGenericType && typedService.ServiceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
				|| typedService.ServiceType.IsArray
				|| typeof(IStartable).IsAssignableFrom(typedService.ServiceType))
				yield break;

			if (typedService.ServiceType == this.ResolvedType)
			{
				foreach (var reg in concretesSource.RegistrationsFor(service, registrationAccessor))
					yield return reg;
			}
			else
			{
				var rb = RegistrationBuilder.ForDelegate((c, p) => CreateMock(c, typedService))
					.As(service)
					.InstancePerLifetimeScope();

				yield return rb.CreateRegistration();
			}
		}

		public void Assert()
		{
			foreach (var mock in mocks)
				Mock.Assert(mock);
		}

		public void AssertAll()
		{
			foreach (var mock in mocks)
				Mock.AssertAll(mock);
		}

		private object CreateMock(IComponentContext context, TypedService typedService)
		{
			var mock = Mock.Create(typedService.ServiceType);
			this.mocks.Add(mock);
			return mock;
		}
	}
}
