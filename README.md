# RPGShop

## 建立商品服務的Api
### 01/24
**Can connect with SQL Server and insert info by code**  
1. ~~Can't login by sa and new account~~
2. ~~Error with *services.AddDbContext()*~~

### 01/25
**Can show the info from database by "api/catalog/item/{id}"**
1. ~~What can `_settings.Value.ExternalCatalogBaseUrl` return~~
2. What can `pictureUrlTemplate = "/api/picture/{0}"` get

### 01/26
**Can Serch info by choosing page number, catalogTypeId, and how much items show per page.**
1. Why `.Skip(pageSize * pageIndex).Take(pageSize)` ?
2. What is `IEnumerable<TEntity>` ?

---
##### 語法筆記: https://hackmd.io/@anthea-wu/RPGShop-ItemsApi
##### 參考網址: https://ithelp.ithome.com.tw/users/20128651/ironman/3738