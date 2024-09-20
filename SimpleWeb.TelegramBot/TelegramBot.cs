using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace SimpleWeb.TelegramBot
{
    public class TelegramBot
    {
        private readonly ILogger<TelegramBot> _logger;
        private readonly string _telegramToken;
        private readonly string _telegramChartId;
        private readonly TelegramBotClient _bot;
        private readonly ChatId _chatId;
        private readonly IMemoryCache _memoryCache;

        public TelegramBot(ILogger<TelegramBot> logger, string telegramToken, string telegramChartId, HttpClient httpClient, IMemoryCache memoryCache)
        {
            _logger = logger;
            _telegramToken = telegramToken;
            _telegramChartId = telegramChartId;
            _bot = new TelegramBotClient(_telegramToken, httpClient);
            _chatId = new ChatId(Convert.ToInt64(_telegramChartId));
            _memoryCache = memoryCache;
        }

        public async Task<bool> SendMessage(string msg, Telegram.Bot.Types.Enums.ParseMode? parseMode = Telegram.Bot.Types.Enums.ParseMode.Html)
        {
            var throttleKey = $"{nameof(TelegramBot)}:throttle";
            try
            {
                if (_memoryCache.TryGetValue(throttleKey, out _))
                    return true;

                var debounceKey = $"{nameof(TelegramBot)}:{Md5SecurityHelper.Utf8Encrypt((JsonSerializer.Serialize(msg)))}";
                if (_memoryCache.TryGetValue(debounceKey, out _))
                    return true;

                var result = await _bot.SendTextMessageAsync(_chatId, msg, parseMode: parseMode);
                _memoryCache.Set(debounceKey, true, TimeSpan.FromSeconds(5));

                return true;
            }
            catch (Exception ex)
            {
                //too many request
                if (ex.Message.Contains("Too Many Requests: retry after"))
                    _memoryCache.Set(throttleKey, true, TimeSpan.FromSeconds(30));

                _logger.LogError(ex, $"Failed sending bot message. TelegramToken = {{TelegramToken}}. TelegramChartId = {{TelegramChartId}}. Message = {ex.Message}.", this._telegramToken, this._telegramChartId);
                return true;
            }
        }
        
        public async Task SendPhotoAsync(byte[] photoBytes, string caption)
        {
            using var stream = new MemoryStream(photoBytes);
            var inputOnlineFile = new InputOnlineFile(stream, "photo.jpg");
            await _bot.SendPhotoAsync(_chatId, inputOnlineFile, caption);
        }
    }
}