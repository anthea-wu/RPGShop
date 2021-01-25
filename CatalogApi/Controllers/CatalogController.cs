using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogApi.Data;
using CatalogApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using CatalogApi.ViewModels;

namespace CatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;

        // IOptionsSnapshot: Used to access the value of TOptions for the lifetime of a request.
        private readonly IOptionsSnapshot<CatalogSettings> _settings;
        private const string pictureUrlTemplate = "/api/picture/{0}";

        // DI 注入
        public CatalogController (CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> settings)
        {
            _catalogContext = catalogContext;
            _settings = settings;
        }

        // 回傳商品類型
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes ()
        {
            // IActionResult 會傳回型別是適當的 ActionResult ，代表各種 HTTP 狀態碼
            // 常見傳回型別為 BadRequestResult (400) 、 NotFoundResult (404) 和 OkObjectResult (200)
            // https://docs.microsoft.com/zh-tw/aspnet/core/web-api/action-return-types?view=aspnetcore-5.0

            // ToArrayAsync(): Creates an array from the query by enumerating it asynchronously.
            var items = await _catalogContext.CatalogTypes.ToArrayAsync();
            return Ok(items);
        }

        // 每頁呈現的資料
        // Get api/Catalog/items[?catalogTypeId=&pageSize=4&pageIndex=3]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items (int? catalogTypeId, [FromQuery] int pageSize=6, [FromQuery] int pageIndex = 0)
        {
            var root = _catalogContext.CatalogItems.AsQueryable();
            if (catalogTypeId.HasValue)
            {
                root = root.Where(c => c.CatalogTypeId == catalogTypeId);
            }

            var totalItems = await root
                                .LongCountAsync();
            var itemsOnPage = await root
                                .Select(x => new CatalogItemResponseVM
                                {
                                    Description = x.Description,
                                    Id = x.Id,
                                    Name = x.Name,
                                    Price = x.Price,
                                    PictureUrl = x.PictureFileName
                                })
                                .OrderBy(c => c.Name)
                                .Skip(pageSize * pageIndex)
                                .Take(pageSize)
                                .ToListAsync();

            ChangeItemPictureUrls(itemsOnPage);
            var model = new PaginatedItemsViewModel<CatalogItemResponseVM>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById (int id)
        {
            if (id <= 0)
            {
                return new BadRequestResult();
            }

            var item = await _catalogContext.CatalogItems
                .Select(x => new CatalogItemResponseVM
                {
                    Description = x.Description,
                    Id = x.Id,
                    Name = x.Name,
                    PictureUrl = x.PictureFileName,
                    Price = x.Price
                })
                .SingleOrDefaultAsync(x => x.Id == id);

            if (item != null)
            {
                item.PictureUrl = ChangeItemPictureUrl(item.PictureUrl);
                return Ok(item);
            }

            return NotFound();
        }



        private void ChangeItemPictureUrls(List<CatalogItemResponseVM> list)
        {
            list.ForEach(x => x.PictureUrl = ChangeItemPictureUrl(x.PictureUrl));
        }

        private string ChangeItemPictureUrl (string fileName)
        {
            // ExternalCatalogBaseUrl: From "appsettings.json"
            return _settings.Value.ExternalCatalogBaseUrl + string.Format(pictureUrlTemplate, fileName);
        }
    }
}
