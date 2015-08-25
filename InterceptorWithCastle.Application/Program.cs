using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterceptorWithCastle.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Initialize();

            var processor = Bootstrapper.Container.Resolve<IOrderProcessor>();

            var order = new Order {ProductId = 1, Price = 12, Discount = 5, Quantity = 4, OrderDate = DateTime.Now};

            bool isSuccess = processor.ProcessOrder(order);

            Console.ReadKey();

        }
    }

    public class Order
    {

        public int ProductId { get; set; }
        public int Discount { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public interface IOrderProcessor
    {

        bool ProcessOrder(Order newOrder);
    }

    public class OrderProcessor : IOrderProcessor
    {

        public bool ProcessOrder(Order newOrder)
        {
            /***Fake Order Operations***/
            if (newOrder.Discount > 10)
            {
                return false;
            }
            else
                return true;
        }
    }

    /// <summary>
    /// Order processor interceptor.
    /// </summary>
    public class OrderProcessorInterceptor : IInterceptor
    {

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Interceptor was invoked.");
            if (invocation.Method.ReturnType == typeof(Boolean)
                && invocation.Arguments != null
                && invocation.Arguments.Count() > 0
                && invocation.Arguments[0] is Order)
            {
                Console.WriteLine("ProcessOrder method was invoked.");
                Console.WriteLine(DateTime.Now.ToLongTimeString());
                invocation.Proceed();
            }
        }
    }

    public class Bootstrapper
    {
        public static IWindsorContainer Container { get; set; }

        public static void Initialize()
        {
            Container = new WindsorContainer();
            Container.Register(Component.For<IInterceptor>()
                .ImplementedBy<OrderProcessorInterceptor>());
            Container.Register(Component.For<IOrderProcessor>()
                .ImplementedBy<OrderProcessor>()
                .Interceptors(InterceptorReference.ForType<OrderProcessorInterceptor>()).Anywhere);
        }
    }
}
