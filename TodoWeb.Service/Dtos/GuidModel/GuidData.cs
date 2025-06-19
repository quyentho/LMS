using TodoWeb.Service.Services;

namespace TodoWeb.Application.Dtos.GuidModel
{
    public class GuidData
    {
        public Guid GetGuid()
        {
            return Guid.NewGuid();
        }
    }
}
