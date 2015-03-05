using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telerik.JustMock.Autofac.Tests
{
	[TestClass]
	public class ContainerTests
	{
		[TestMethod]
		public void Container_ResolveWithMocks_ExplicitlyResolveMocks()
		{
			var container = new ContainerBuilder().Build();
			container.Arrange<ICounter>(x => x.Next).Returns(5);
			container.Arrange<IMessage>(x => x.Message).Returns("yay");

			var greeter = container.ResolveWithMocks<Greeter>();
			greeter.Greet();

			container.Assert<ILogger>(x => x.Log("5: yay"));
		}

		[TestMethod]
		public void Container_EnableMocking_ImplicitlyResolveMocks()
		{
			var builder = new ContainerBuilder();
			builder.RegisterType<Greeter>().AsSelf();
			var container = builder.Build().EnableMocking();

			var greeter = container.Resolve<Greeter>();
			greeter.Greet();

			container.Assert<ILogger>(x => x.Log("0: "));
		}

		[TestMethod]
		public void Container_Assert()
		{
			var container = new ContainerBuilder().Build();
			container.Arrange<ICounter>(x => x.Next).Returns(5).OccursOnce();
			container.Arrange<IMessage>(x => x.Message).Returns("yay").OccursOnce();
			container.Arrange<ILogger>(x => x.Log("5: yay")).OccursOnce();

			AssertEx.Throws<AssertFailedException>(() => container.Assert());

			var greeter = container.ResolveWithMocks<Greeter>();
			greeter.Greet();

			container.Assert();
		}

		[TestMethod]
		public void Container_AssertAll()
		{
			var container = new ContainerBuilder().Build();
			container.Arrange<ICounter>(x => x.Next).Returns(5);
			container.Arrange<IMessage>(x => x.Message).Returns("yay");
			container.Arrange<ILogger>(x => x.Log("5: yay"));

			AssertEx.Throws<AssertFailedException>(() => container.AssertAll());

			var greeter = container.ResolveWithMocks<Greeter>();
			greeter.Greet();

			container.AssertAll();
		}

		public interface ILogger
		{
			void Log(string message);
		}
		
		public interface IMessage
		{
			string Message { get; }
		}
		
		public interface ICounter
		{
			int Next { get; }
		}
		
		public class Greeter
		{
			private ILogger logger;
			private IMessage message;
			private ICounter counter;

			public Greeter(ILogger logger, IMessage message, ICounter counter)
			{
				this.logger = logger;
				this.message = message;
				this.counter = counter;
			}

			public void Greet()
			{
				this.logger.Log(string.Format("{0}: {1}", this.counter.Next, this.message.Message));
			}
		}
	}
}
