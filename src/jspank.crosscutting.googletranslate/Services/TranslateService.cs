using Google.Apis.Services;
using jspank.core.services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using TranslationsResource = Google.Apis.Translate.v2.Data.TranslationsResource;

namespace jspank.crosscutting.googletranslate.services
{
    public class TranslateService : ITranslateService
    {
        private const string TargetLanguage = "en";

        public string Key
        {
            get
            {
                var value = ConfigurationManager.AppSettings["GoogleApiKeyFilePath"] as string;

                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("GoogleApiKeyFilePath key not found on app.config");

                return value;
            }
        }

        public ICollection<string> Translate(string value, string targetLanguage = TargetLanguage)
        {
            var service = new Google.Apis.Translate.v2.TranslateService(new BaseClientService.Initializer()
            {
                ApiKey = this.Key,
                ApplicationName = this.GetType().FullName
            });

            if (string.IsNullOrEmpty(targetLanguage))
                targetLanguage = TargetLanguage;

            var response = service.Translations.List(new[] { value }, targetLanguage).Execute();
            var result = new Collection<string>();

            foreach (TranslationsResource translation in response.Translations)
                result.Add(translation.TranslatedText);

            return result;
        }
    }
}
