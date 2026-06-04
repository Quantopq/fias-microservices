

namespace ClientApiService.Services;

public class DeepSeekLeadGenerator : ILeadGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<DeepSeekLeadGenerator> _logger;

    public DeepSeekLeadGenerator(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<DeepSeekLeadGenerator> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["DeepSeek:ApiKey"]!;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri("https://api.deepseek.com/v1");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<GeneratedLeadDto> GenerateRandomLeadAsync()
    {
        try
        {
            var prompt = @"Генерируй случайные данные для теста системы адресации:
1. ФИО клиента (полностью, русское)
2. Кривой адрес (с ошибками, сокращениями, без формата)

Примеры кривых адресов:
- 'москва тверская 1'
- 'спб невский пр 100 кв 5'
- 'екатеринбург ленина 50 оф 305'

Верни ответ в формате JSON:
{
  ""clientName"": ""Иванов Иван Иванович"",
  ""badAddress"": ""москва тверская 1""
}";

            var requestBody = new
            {
                model = "deepseek-chat",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 200
            };

            var response = await _httpClient.PostAsJsonAsync("/chat/completions", requestBody);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<DeepSeekResponse>();
            var content = result?.Choices.FirstOrDefault()?.Message.Content;

            if (string.IsNullOrEmpty(content))
                return GenerateFallbackLead();

            // Парсим JSON из ответа
            var jsonStart = content.IndexOf('{');
            var jsonEnd = content.LastIndexOf('}');
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var json = content.Substring(jsonStart, jsonEnd - jsonStart + 1);
                var lead = JsonSerializer.Deserialize<GeneratedLeadDto>(json);
                if (lead != null)
                    return lead;
            }

            return GenerateFallbackLead();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating lead with DeepSeek");
            return GenerateFallbackLead();
        }
    }

    private GeneratedLeadDto GenerateFallbackLead()
    {
        var random = new Random();

        // БОЛЬШЕ ФАМИЛИЙ 
        var lastNames = new[] {
        "Иванов", "Петров", "Сидоров", "Козлов", "Новиков", "Попов", "Лебедев",
        "Кузнецов", "Смирнов", "Васильев", "Павлов", "Семенов", "Голубев", "Виноградов",
        "Богданов", "Воробьев", "Федоров", "Михайлов", "Беляев", "Тарасов", "Белов",
        "Комаров", "Орлов", "Киселев", "Макаров", "Андреев", "Ковалев", "Ильин",
        "Гусев", "Титов", "Кузьмин", "Кудрявцев", "Баранов", "Куликов", "Алексеев",
        "Степанов", "Яковлев", "Сорокин", "Сергеев", "Романов", "Захаров", "Борисов",
        "Королев", "Герасимов", "Пономарев", "Григорьев", "Лазарев", "Медведев",
        "Ершов", "Никитин", "Соболев", "Рябов", "Поляков", "Цветков", "Данилов"
    };

        // БОЛЬШЕ ИМЁН 
        var firstNames = new[] {
        "Иван", "Петр", "Сергей", "Александр", "Дмитрий", "Алексей", "Михаил",
        "Владимир", "Николай", "Андрей", "Юрий", "Анатолий", "Евгений", "Виктор",
        "Борис", "Василий", "Артем", "Павел", "Константин", "Олег", "Роман",
        "Игорь", "Валерий", "Денис", "Станислав", "Максим", "Григорий", "Федор",
        "Кирилл", "Вячеслав", "Леонид", "Эдуард", "Владислав", "Руслан", "Марат"
    };

        // БОЛЬШЕ ОТЧЕСТВ 
        var middleNames = new[] {
        "Иванович", "Петрович", "Сергеевич", "Александрович", "Дмитриевич",
        "Алексеевич", "Михайлович", "Владимирович", "Николаевич", "Андреевич",
        "Юрьевич", "Анатольевич", "Евгеньевич", "Викторович", "Борисович",
        "Васильевич", "Артемович", "Павлович", "Константинович", "Олегович",
        "Романович", "Игоревич", "Валерьевич", "Денисович", "Станиславович",
        "Максимович", "Григорьевич", "Федорович", "Кириллович", "Вячеславович",
        "Леонидович", "Эдуардович", "Владиславович", "Русланович", "Маратович"
    };

        // БОЛЬШЕ ГОРОДОВ 
        var cities = new[] {
        "москва", "санкт-петербург", "екатеринбург", "казань", "новосибирск",
        "самара", "омск", "челябинск", "нижний новгород", "уфа", "ростов-на-дону",
        "красноярск", "воронеж", "пермь", "волгоград", "краснодар", "саратов",
        "тюмень", "тольятти", "ижерск", "барнаул", "ульяновск", "иркутск",
        "владивосток", "ярославль", "хабаровск", "магнитогорск", "новороссийск",
        "пенза", "рязань", "тула", "липецк", "киров", "чебоксары", "брянск"
    };

        //  БОЛЬШЕ УЛИЦ 
        var streets = new[] {
        "ленина", "тверская", "невский проспект", "кирова", "гагарина", "пушкина",
        "советская", "октябрьская", "мировая", "карла маркса", "интернациональная",
        "комсомольская", "пионерская", "парковая", "садовая", "цветочная",
        "зеленая", "речная", "озерная", "лесная", "полевая", "луговая",
        "центральный переулок", "новая", "молодежная", "строителей", "дорожная",
        "заводская", "промышленная", "рабочая", "колхозная", "заречная",
        "набережная", "горького", "чуковского", "маяковского", "некрасова",
        "толстого", "достоевского", "чехова", "гоголя", "грибоедова"
    };

        // БОЛЬШЕ ТИПОВ ЗДАНИЙ
        var buildingTypes = new[] { "д", "дом", "д.", "зд", "корпус", "к", "кв", "оф" };

        var randomLastName = lastNames[random.Next(lastNames.Length)];
        var randomFirstName = firstNames[random.Next(firstNames.Length)];
        var randomMiddleName = middleNames[random.Next(middleNames.Length)];

        var randomCity = cities[random.Next(cities.Length)];
        var randomStreet = streets[random.Next(streets.Length)];
        var buildingNumber = random.Next(1, 250);
        var buildingType = buildingTypes[random.Next(buildingTypes.Length)];

        // РАЗНЫЕ ФОРМАТЫ АДРЕСОВ (для разнообразия)
        var addressFormats = new[]
        {
        $"{randomCity} {randomStreet} {buildingType} {buildingNumber}",
        $"{randomCity}, {randomStreet} {buildingType}.{buildingNumber}",
        $"{randomCity} {randomStreet} {buildingNumber}",
        $"г {randomCity}, ул {randomStreet}, {buildingType} {buildingNumber}",
        $"{randomCity} {randomStreet} {buildingNumber} кв {random.Next(1, 300)}",
        $"{randomCity} {randomStreet} {buildingType}.{buildingNumber} оф {random.Next(1, 100)}"
    };

        return new GeneratedLeadDto
        {
            ClientName = $"{randomLastName} {randomFirstName} {randomMiddleName}",
            BadAddress = addressFormats[random.Next(addressFormats.Length)]
        };
    }

    private class DeepSeekResponse
    {
        public Choice[] Choices { get; set; } = Array.Empty<Choice>();
    }

    private class Choice
    {
        public Message Message { get; set; } = new();
    }

    private class Message
    {
        public string Content { get; set; } = string.Empty;
    }
}