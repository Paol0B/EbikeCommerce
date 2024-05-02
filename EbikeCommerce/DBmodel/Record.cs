namespace EbikeCommerce.DBmodel
{
    public record ProductRecord
    {
        short _modelYear;

        public int product_id { get; set; }
        public required string product_name { get; set; }
        public int brand_id { get; set; }
        public int category_id { get; set; }
        public short model_year
        {
            get => _modelYear;
            set
            {
                if (value >= 0 && value <= 9999)
                    _modelYear = value;
                else
                    throw new ArgumentOutOfRangeException("model_year", "Il valore deve essere compreso tra 0 e 9999.");
            }
        }
        public decimal list_price { get; set; }

        public string brand
        {
            get => Dictionaries.Brands.TryGetValue(brand_id, out var brand) ? brand : string.Empty;
        }
    }

    public record CustomerRecord
    {
        public int customer_id { get; init; }
        public required string first_name { get; init; }
        public required string last_name { get; init; }
        public string? phone { get; init; }
        public required string username { get; init; }
        public required string email { get; init; }
        public required string passwd { get; init; }
        public string? street { get; init; }
        public string? city { get; init; }
        public string? state { get; init; }
        public string? zip_code { get; init; }
    }

    public record Cart
    {
        public int ID { get; set; }
        public int Qta { get; set; }
    }

    public static class Dictionaries
    {
        public static readonly Dictionary<int, string> Categories = DBservice.FillCategories();
        public static readonly Dictionary<int, string> Brands = DBservice.FillBrands();

        private static Dictionary<int, Tuple<int, int>> priceRanges = new()
        {
            { 1, Tuple.Create(0, 500) },
            { 2, Tuple.Create(500, 1000) },
            { 3, Tuple.Create(1000, 2000) },
            { 4, Tuple.Create(2000, 5000) },
            { 5, Tuple.Create(5000, 100000) }
        };

        public static Dictionary<int, Tuple<int, int>> PriceRanges { get => priceRanges; set => priceRanges = value; }
    }

    public record Orders
    {
        public int order_id { get; set; }
        public int customer_id { get; set; }
        public DateTime order_date { get; set; }
        public decimal total { get; set; }
    }

    public record CreditCard
    {
        public required string card_number { get; set; }
        public required string Name { get; set; }
        public int expiration_month { get; set; }
        public int expiration_year { get; set; }
        public required string cvv { get; set; }
    }
}