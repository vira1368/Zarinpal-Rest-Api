using System.Collections.Generic;
using ZarinpalRestApi.Models;

namespace ZarinpalRestApi.Helpers
{
    public static class TestDatabase
    {
        public static readonly List<ProductModel> Data;

        static TestDatabase()
        {
            Data = new List<ProductModel>
            {
                new ProductModel {Id = 1, Name = "iPhone 11 Pro Max", Image = "/images/1.jpg", Amount = 26000000},
                new ProductModel {Id = 2, Name = "Galaxy Note 10 Plus", Image = "/images/2.jpg", Amount = 13900000},
                new ProductModel {Id = 3, Name = "Smart Watch V9 Smart 2020", Image = "/images/3.jpg", Amount = 159000},
                new ProductModel {Id = 4, Name = "Smart Watch Galaxy Watch Active", Image = "/images/4.jpg", Amount = 2997000}
            };
        }

        public static ProductModel GetById(int id)
        {
            return Data.Find(model => model.Id == id);
        }
    }
}
