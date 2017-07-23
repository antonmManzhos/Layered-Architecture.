using NLayerApp.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLayerApp.BLL.DTO;
using NLayerApp.DAL.Interfaces;
using NLayerApp.DAL.Entities;
using NLayerApp.BLL.Infrastructure;
using NLayerApp.BLL.BusinessModels;
using AutoMapper;

namespace NLayerApp.BLL.Services
{
    public class OrderService : IOrderService
    {
        IUnitOfWork Database { get; set; }

        public OrderService(IUnitOfWork uow)
        {
            Database = uow;
        }
        public PhoneDTO GetPhone(int? id)
        {
            if (id == null)
            {
                throw new ValidationException("Не установлено id телефона", "");
            }
            var phone = Database.Phones.Get(id.Value);
            if(phone == null)
            {
                throw new ValidationException("Телефон не найден", "");
            }
            // применяем автомаппер для проекции Phone на PhoneDTO
            Mapper.Initialize(cfg => cfg.CreateMap<Phone, PhoneDTO>());
            return Mapper.Map<Phone, PhoneDTO>(phone);
        }
        public IEnumerable<PhoneDTO> GetPhones()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            Mapper.Initialize(cfg => cfg.CreateMap<Phone, PhoneDTO>());
            return Mapper.Map<IEnumerable<Phone>, List<PhoneDTO>>(Database.Phones.GetAll());
        }

        public void MakeOrder(OrderDTO orderDto)
        {
            Phone phone = Database.Phones.Get(orderDto.Id);
            //Валидация
            if(phone == null)
            {
                throw new ValidationException("Телефон не найден", "");
            }
            //применяем скидку
            decimal sum = new Discount(0.1m).GetDiscountedPrice(phone.Price);
            Order order = new Order
            {
                Date = DateTime.Now,
                Address = orderDto.Address,
                PhoneId = phone.Id,
                Sum = sum,
                PhoneNumber = orderDto.PhoneNumber
            };
            Database.Orders.Create(order);
            Database.Save();

        }
        public void Dispose()
        {
            Database.Dispose();
        }

    }
}
