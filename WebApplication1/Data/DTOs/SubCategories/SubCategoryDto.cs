﻿namespace Candle_API.Data.DTOs.SubCategories
{
    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
