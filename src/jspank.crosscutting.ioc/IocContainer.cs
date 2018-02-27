using jspank.core.services;
using jspank.crosscutting.googletranslate.services;
using SimpleInjector;

namespace jspank.crosscutting.ioc
{
    public class IocContainer
    {
        public static void RegisterAll(ref Container container)
        {
            if (container == null)
                container = new Container();

            //Crosscutting services
            container.Register<ITranslateService, TranslateService>(Lifestyle.Transient);
        }
    }
}
