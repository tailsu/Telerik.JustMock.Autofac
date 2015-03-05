using System;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Telerik.JustMock.Expectations;
using Telerik.JustMock.Helpers;

namespace Telerik.JustMock.Autofac
{
	/// <summary>
	/// Extension methods enabling auto-mocking with Autofac containers.
	/// </summary>
	public static class AutomockContainerExtensions
	{
		/// <summary>
		/// Turn this container into an auto-mocking container. After this call,
		/// any requested unregistered service will be implicitly resolved as a mock.
		/// </summary>
		/// <param name="container">The container to configure.</param>
		/// <returns>The same container, enabling fluent configuration.</returns>
		public static IContainer EnableMocking(this IContainer container)
		{
			container.GetMocksSource();
			return container;
		}

		/// <summary>
		/// Resolve T, replacing any missing dependencies with mocks.
		/// The type T doesn't require prior registration.
		/// </summary>
		/// <typeparam name="T">The type to resolve.</typeparam>
		/// <param name="container">An Autofac container.</param>
		/// <returns>An instance with all missing dependencies filled in with mocks.</returns>
		public static T ResolveWithMocks<T>(this IContainer container)
		{
			var mocks = container.GetMocksSource();
			mocks.ResolvedType = typeof(T);
			try
			{
				return container.Resolve<T>();
			}
			finally
			{
				mocks.ResolvedType = null;
			}
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static FuncExpectation<object> Arrange<TDependency>(this IContainer container, Expression<Func<TDependency, object>> expression)
		{
			return container.EnableMocking().Resolve<TDependency>().Arrange(expression);
		}

		/// <summary>
		/// Register a mocked type and arrange a method on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation Arrange<TDependency>(this IContainer container, Expression<Action<TDependency>> expression)
		{
			return container.EnableMocking().Resolve<TDependency>().Arrange(expression);
		}
	
		/// <summary>
		/// Register a mocked type and arrange a setter or event on it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="action">The setter or event to arrange.</param>
		/// <returns>Fluent interface to further configure the behavior of this arrangement.</returns>
		public static ActionExpectation ArrangeSet<TDependency>(this IContainer container, Action<TDependency> action)
		{
			return container.EnableMocking().Resolve<TDependency>().ArrangeSet(action);
		}

		/// <summary>
		/// Register a mocked type and apply a functional specification to it.
		/// </summary>
		/// <typeparam name="TDependency">The type of mock to register.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="functionalSpecification">The method to arrange.</param>
		public static void ArrangeLike<TDependency>(this IContainer container, Expression<Func<TDependency, bool>> functionalSpecification)
		{
			container.EnableMocking().Resolve<TDependency>().ArrangeLike(functionalSpecification);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IContainer container, Expression<Action<TDependency>> expression)
		{
			container.EnableMocking().Resolve<TDependency>().Assert(expression);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		public static void Assert<TDependency>(this IContainer container, Expression<Func<TDependency, object>> expression)
		{
			container.EnableMocking().Resolve<TDependency>().Assert(expression);
		}

		/// <summary>
		/// Assert an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		public static void Assert<TDependency>(this IContainer container)
		{
			container.EnableMocking().Resolve<TDependency>().Assert();
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IContainer container, Expression<Func<TDependency, object>> expression, Occurs occurs)
		{
			container.EnableMocking().Resolve<TDependency>().Assert(expression, occurs);
		}

		/// <summary>
		/// Assert a method on an registered mock.
		/// </summary>
		/// <typeparam name="TDependency">The type of the registered mock.</typeparam>
		/// <param name="container">The mocking container.</param>
		/// <param name="expression">The method to assert.</param>
		/// <param name="occurs">Occurrence expectation.</param>
		public static void Assert<TDependency>(this IContainer container, Expression<Action<TDependency>> expression, Occurs occurs)
		{
			container.EnableMocking().Resolve<TDependency>().Assert(expression, occurs);
		}
	
		/// <summary>
		/// Asserts all explicit expectations on all registered mocks.
		/// </summary>
		public static void Assert(this IContainer container)
		{
			container.GetMocksSource().Assert();
		}

		/// <summary>
		/// Asserts all explicit and implicit expectations on all registered mocks.
		/// </summary>
		public static void AssertAll(this IContainer container)
		{
			container.GetMocksSource().AssertAll();
		}

		private static MocksSource GetMocksSource(this IContainer container)
		{
			var mocks = container.ComponentRegistry.Sources.OfType<MocksSource>().FirstOrDefault();
			if (mocks == null)
			{
				mocks = new MocksSource();
				container.ComponentRegistry.AddRegistrationSource(mocks);
			}
			return mocks;
		}
	}
}
