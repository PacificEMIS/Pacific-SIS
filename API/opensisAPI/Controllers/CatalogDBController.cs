using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using opensis.catalogdb.ViewModels;
using opensis.catelogdb.Interface;


namespace opensisAPI.Controllers
{
    [EnableCors("AllowOrigin")]
    [Route("api/CatalogDB")]
    [ApiController]
    public class CatalogDBController : ControllerBase
    {
        private ICatalogDBRepository _catalogDBRepository;
        public CatalogDBController(ICatalogDBRepository catalogDBRepository)
        {
            _catalogDBRepository = catalogDBRepository;
        }
        [HttpPost("CheckIfTenantIsAvailable")]
        public ActionResult<AvailableTenantViewModel> CheckIfTenantIsAvailable(AvailableTenantViewModel objModel)
        {
            var response = _catalogDBRepository.CheckIfTenantIsAvailable(objModel);

            return response;
        }
    }
}
