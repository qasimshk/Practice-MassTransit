using MassTransit;
using MassTransit.Topology;
using System;
using System.Collections.Generic;

namespace practice.one.component.Abstractions
{
    public interface OrderFry : OrderLine
    {
        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }
    }

    public interface OrderFryCompleted : OrderLineCompleted
    {
        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }

        
    }

    public interface CookFry
    {
        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }
    }


    public class CookFryRequest : CookFry
    {
        public CookFryRequest(Guid orderRefrence, string productName, int quantity)
        {
            OrderRefrence = orderRefrence;
            ProductName = productName;
            Quantity = quantity;
        }

        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }
    }

    public interface FryReady : FutureCompleted
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }
    }

    #region NuGet

    [ExcludeFromTopology]
    public interface OrderLine
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
    }

    public interface OrderLineCompleted : FutureCompleted
    {
        Guid OrderId { get; }
        Guid OrderLineId { get; }
        string Description { get; }
    }

    public interface FutureCompleted
    {
        /// <summary>
        /// When the future was initially created
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// When the future was finally completed
        /// </summary>
        DateTime Completed { get; }
    }

    public interface OrderFaulted : FutureFaulted
    {
        Guid OrderId { get; }

        IDictionary<Guid, OrderLineCompleted> LinesCompleted { get; }

        IDictionary<Guid, Fault<OrderLine>> LinesFaulted { get; }
    }

    public interface FutureFaulted
    {
        /// <summary>
        /// When the future was initially created
        /// </summary>
        DateTime? Created { get; }

        /// <summary>
        /// When the future faulted
        /// </summary>
        DateTime? Faulted { get; }

        /// <summary>
        /// The exception related to the fault
        /// </summary>
        ExceptionInfo[] Exceptions { get; }
    }

    #endregion

    public class FryCompletedResult : OrderFryCompleted
    {
        public FryCompletedResult(
            DateTime created
            , DateTime completed
            , Guid orderId
            , Guid orderLineId

            , Guid orderReference
            , string productName
            , int quantity)
        {
            Created = created;
            Completed = completed;
            OrderId = orderId;
            OrderLineId = orderLineId;

            OrderRefrence = orderReference;
            ProductName = productName;
            Quantity = quantity;
        }

        public Guid OrderId { get; }

        public Guid OrderLineId { get; }

        public DateTime Created { get; }

        public DateTime Completed { get; }

        public string Description { get; }

        public Guid OrderRefrence { get; }

        public string ProductName { get; }

        public int Quantity { get; }        
    }
}


