namespace TodoWeb.Application.Services
{
    public interface ISingletonGenerator
    {
        Guid Generate();
    }

    public class SingltonGenerator : ISingletonGenerator
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly IGuidGenerator guidGenerator;

        public SingltonGenerator(IServiceProvider serviceProvider, IGuidGenerator guidGenerator)
        {
            _serviceProvider = serviceProvider;
            this.guidGenerator = guidGenerator;
        }

        public Guid Generate()
        {
            var guidGenerator = _serviceProvider.GetService<IGuidGenerator>();
            return guidGenerator.Generate();
        } 
    } 
}
