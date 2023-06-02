using IvaETicaret.Data;
using IvaETicaret.Models;
using Microsoft.AspNetCore.SignalR;

namespace IvaETicaret.Hubs
{
    public class OrdersHub:Hub
    {
        public ApplicationDbContext _db;
        public OrdersHub(ApplicationDbContext db)
        {
        _db = db;
        }

        //public async Task SendOrders(OrderHeader orderHeader)
        //{
            
        //    await Clients.All.SendAsync("ReceiveOrders", orderHeader, _products.Count());
        //}

    }
}
