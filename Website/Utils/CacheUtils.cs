using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADCOnline.Business.Implementation.ClientManager;
using ADCOnline.DA.Dapper;
using ADCOnline.DA.Dapper.SqlView;
using ADCOnline.Simple.Base;
using ADCOnline.Simple.Item;
using ADCOnline.Simple.Json;
using ADCOnline.Utils;
using System.IO;
using Dapper;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Website.Utils
{
    public class CacheUtils
    {
        private readonly IDistributedCache cache;
        private readonly DapperDA _dapperDa;
        private readonly WebsiteModuleManager _webModuleManager;
        private readonly WebsiteContentManager _webContentManager;
        private readonly ProductManager _productManager;
        private readonly WebsiteMenuManager _websiteMenuManager;
        private readonly ModulePositionManager _modulePositionManager;
        private readonly OtherContentManager _otherContentManager;

        public CacheUtils(IDistributedCache _distributedCache)
        {
            cache = _distributedCache;
            _dapperDa = new DapperDA(WebConfig.ConnectionString);
            _webModuleManager = new WebsiteModuleManager(WebConfig.ConnectionString);
            _webContentManager = new WebsiteContentManager(WebConfig.ConnectionString);
            _productManager = new ProductManager(WebConfig.ConnectionString);
            _websiteMenuManager = new WebsiteMenuManager(WebConfig.ConnectionString);
            _modulePositionManager = new ModulePositionManager(WebConfig.ConnectionString);
            _otherContentManager = new OtherContentManager(WebConfig.ConnectionString);
        }

        public async Task<IEnumerable<WebsiteMenuItem>> GetMainMenu(string Lang)
        {
            string key = WebConfig.CacheProject + "GetMainMenu_" + Lang;
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteMenuItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<WebsiteMenuItem>>(cache.GetString(key)) : await GetMenuJson(Lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await GetMenuJson(Lang);
        }

        public async Task<OtherContentItem> GetOtherItemByCode(string code, string lang)
        {
            var key = WebConfig.CacheProject + "GetOtherItemByCode_" + code + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    var result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<OtherContentItem>(cache.GetString(key)) : await _otherContentManager.GetByCodeLang(code, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        var data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        var expireTime = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _otherContentManager.GetByCodeLang(code, lang);
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetListBreadcrumb(int moduleId, string lang)
        {
            var key = WebConfig.CacheProject + "GetListBreadcrumb_" + moduleId + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    var result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : await _webModuleManager.GetListBreadcrumb(moduleId, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        var data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        var expireTime = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetListBreadcrumb(moduleId, lang);
        }

        public async Task<IEnumerable<WebsiteMenuItem>> GetMenuJson(string Lang)
        {
            try
            {
                string json = await ReadFileAsync("Menu" + Lang + ".json", "DataJson");
                return JsonConvert.DeserializeObject<IEnumerable<WebsiteMenuItem>>(json);
            }
            catch
            {
                IEnumerable<WebsiteMenuItem> listMenu = new List<WebsiteMenuItem>();
                return listMenu;
            }
        }

        public List<AreaJson> GetAreaIndex(string lang)
        {
            string key = WebConfig.CacheProject + "GetAreaIndex_" + lang;
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<AreaJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<AreaJson>>(cache.GetString(key)) : GetAreaJsons(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<AreaJson> result2 = GetAreaJsons(lang);
            return result2;
        }

        public List<AreaJson> GetAreaJsons(string lang)
        {
            List<AreaJson> listMenu = JsonConvert.DeserializeObject<List<AreaJson>>(ReadFile("Area" + lang + ".json", "DataJson"));
            if (listMenu == null)
            {
                listMenu = new List<AreaJson>();
            }
            return listMenu.ToList();
        }

        public List<AgencyJson> GetAllAgencyJson(string lang)
        {
            string key = WebConfig.CacheProject + "GetAllAgencyJson_" + lang;
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<AgencyJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<AgencyJson>>(cache.GetString(key)) : GetAgencyJsons(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<AgencyJson> result2 = GetAgencyJsons(lang);
            return result2;
        }

        public List<AgencyJson> GetAgencyJsons(string lang)
        {
            List<AgencyJson> listMenu = new();
            Task t = Task.Run(() =>
            {
                listMenu = JsonConvert.DeserializeObject<List<AgencyJson>>(ReadFile("Agencies" + lang + ".json", "DataJson"));
            });
            t.Wait();
            return listMenu.ToList();
        }

        public ConfigPopupJson GetCacheConfigPopup()
        {
            string key = WebConfig.CacheProject + "GetCacheConfigPopup";
            try
            {
                if (WebConfig.EnableCache)
                {
                    ConfigPopupJson result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<ConfigPopupJson>(cache.GetString(key)) : GetConfigPopup();
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            ConfigPopupJson result2 = GetConfigPopup();
            return result2;
        }

        public ConfigPopupJson GetConfigPopup()
        {
            ConfigPopupJson listMenu = new ConfigPopupJson();
            Task t = Task.Run(() =>
            {
                listMenu = JsonConvert.DeserializeObject<ConfigPopupJson>(ReadFile("ConfigPopup.json", "DataJson"));
            });
            t.Wait();
            return listMenu;
        }

        public Dictionary<string, string> GetCacheResource(string lang)
        {
            string key = WebConfig.CacheProject + "GetCacheResource";
            try
            {
                if (WebConfig.EnableCache)
                {
                    Dictionary<string, string> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(cache.GetString(key)) : GetResource(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            Dictionary<string, string> result2 = GetResource(lang);
            return result2;
        }

        public Dictionary<string, string> GetResource(string lang)
        {
            Dictionary<string, string> listMenu = new();
            Task t = Task.Run(() =>
            {
                listMenu = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadFile("Resources_" + lang + ".json", "DataJson/Resource"));
            });
            t.Wait();
            return listMenu;
        }

        public List<ModulePositionItem> GetListPositionView(string view)
        {
            string key = WebConfig.CacheProject + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<ModulePositionItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<ModulePositionItem>>(cache.GetString(key)) : _modulePositionManager.GetListView(view);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<ModulePositionItem> result2 = _modulePositionManager.GetListView(view);
            return result2;
        }

        public List<ModulePositionItem> GetListPositionByJsonView(string view, string lang)
        {
            string key = WebConfig.CacheProject + "GetListPositionByJsonView" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<ModulePositionItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<ModulePositionItem>>(cache.GetString(key)) : PositionByJsonView(view, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<ModulePositionItem> result2 = PositionByJsonView(view, lang);
            return result2;
        }

        public List<ModulePositionItem> PositionByJsonView(string view, string lang)
        {
            List<ModulePositionItem> list = JsonConvert.DeserializeObject<List<ModulePositionItem>>(ReadFile("Position" + view + lang + ".json", "DataJson"));
            if (list == null)
            {
                list = new List<ModulePositionItem>();
            }
            return list;
        }

        public List<ModulePositionItem> GetListPositionByCode(string view)
        {
            string key = WebConfig.CacheProject + "GetListPositionByCode_" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<ModulePositionItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<ModulePositionItem>>(cache.GetString(key)) : _modulePositionManager.GetListByCode(view);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<ModulePositionItem> result2 = _modulePositionManager.GetListByCode(view);
            return result2;
        }

        public List<WebsiteModulesJson> GetListModuleInPositionCode(string view, string Lang)
        {
            string key = WebConfig.CacheProject + "GetListModuleInPositionCode" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesJson>>(cache.GetString(key)) : _modulePositionManager.GetListModuleInPositionCode(view, Lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _modulePositionManager.GetListModuleInPositionCode(view, Lang);
        }

        public List<WebsiteModulesJson> GetListModuleInPositionIds(string id, string Lang, int take)
        {
            string key = WebConfig.CacheProject + "GetListModuleInPositionIds" + id + take;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesJson>>(cache.GetString(key)) : _modulePositionManager.GetListModuleInPositionIds(id, Lang, take);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _modulePositionManager.GetListModuleInPositionIds(id, Lang, take);
        }

        public List<AdvertisingItem> GetListAdvertisingItemByCode(string view, string Lang)
        {
            string key = WebConfig.CacheProject + "GetListAdvertisingItemByCode" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<AdvertisingItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<AdvertisingItem>>(cache.GetString(key)) : _modulePositionManager.GetListAdvertisingByCode(view, Lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _modulePositionManager.GetListAdvertisingByCode(view, Lang);
        }

        public List<AdvertisingItem> GetListAdvertisingItemInPositionIds(string id, string Lang, int take)
        {
            string key = WebConfig.CacheProject + "GetListAdvertisingItemInPositionIds" + id + take;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<AdvertisingItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<AdvertisingItem>>(cache.GetString(key)) : _modulePositionManager.GetListAdvertisingInPositionIds(id, Lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _modulePositionManager.GetListAdvertisingInPositionIds(id, Lang, take);
        }

        public async Task<IEnumerable<AdvertisingItem>> GetListAdvertisingItemByCodeAsync(string view, string Lang)
        {
            string key = WebConfig.CacheProject + "GetListAdvertisingItemByCode" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<AdvertisingItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<AdvertisingItem>>(cache.GetString(key)) : await _modulePositionManager.GetListAdvertisingByCodeAsync(view, Lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _modulePositionManager.GetListAdvertisingByCodeAsync(view, Lang);
        }

        public List<ModulePositionItem> GetListPositionViewIndex(string view)
        {
            string key = WebConfig.CacheProject + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<ModulePositionItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<ModulePositionItem>>(cache.GetString(key)) : _modulePositionManager.GetListView(view);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<ModulePositionItem> result2 = _modulePositionManager.GetListViewIndex(view);
            return result2;
        }

        public ModulePositionItem GetPositionViewID(int id)
        {
            string key = WebConfig.CacheProject + "GetPositionViewID" + id;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    ModulePositionItem result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<ModulePositionItem>(cache.GetString(key)) : _modulePositionManager.GetPositionViewID(id);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            ModulePositionItem result2 = _modulePositionManager.GetPositionViewID(id);
            return result2;
        }

        public List<WebsiteModulesItem> GetListModuleView(string view, List<ModulePositionItem> listPosition, string lang)
        {
            string key = WebConfig.CacheProject + "WebsiteModulesItem" + view;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache && cache != null && cache.GetString(key) != null)
                {
                    return JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key));
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }

            #region vi tri module

            int i = 1;
            StringBuilder sqlModule = new();
            foreach (ModulePositionItem item in listPosition)
            {
                if (item.TypeView == StaticEnum.Module)
                {
                    sqlModule.Append(string.Format(SqlModule.SqlHome, item.NumberCount ?? 20, item.TypeView, lang, item.Code, item.SqlModule));// " ORDER BY OrderDisplay asc, CreatedDate desc"
                    if (i < listPosition.Count)
                    {
                        sqlModule.Append(SqlConst.UnionAll);
                    }
                }
                else if (item.TypeView == StaticEnum.Advertising)
                {
                    sqlModule.Append(string.Format(SqlAdvertising.SqlHome, item.NumberCount ?? 2, item.TypeView, lang, item.Code, item.SqlModule));//" ORDER BY OrderDisplay asc, CreatedDate desc"
                    if (i < listPosition.Count)
                    {
                        sqlModule.Append(SqlConst.UnionAll);
                    }
                }
                else if (item.TypeView == StaticEnum.Content || item.TypeView == StaticEnum.Product)
                {
                    sqlModule.Append(string.Format(SqlModule.SqlHome, item.NumberCount ?? 3000, item.TypeView, lang, item.Code, item.SqlModule));
                    if (i < listPosition.Count)
                    {
                        sqlModule.Append(SqlConst.UnionAll);
                    }
                }
                i++;
            }

            string sql = string.Format("SELECT DISTINCT * FROM({0}) c ", sqlModule.ToString());
            List<WebsiteModulesItem> result = _dapperDa.Select<WebsiteModulesItem>(sql).ToList();

            #endregion vi tri module

            try
            {
                if (WebConfig.EnableCache && cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                {
                    string data = JsonConvert.SerializeObject(result);
                    DistributedCacheEntryOptions expireTime = new()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                    };
                    cache.SetStringAsync(key, data, expireTime);
                }
                return result;
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<WebsiteModulesItem> result2 = _dapperDa.Select<WebsiteModulesItem>(sql).ToList();
            return result2;
        }

        public async Task<SystemConfig> SystemConfig(string lang)
        {
            string key = WebConfig.CacheProject + "SystemConfig_" + lang;
            try
            {
                if (WebConfig.EnableCache)
                {
                    SystemConfig result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<SystemConfig>(cache.GetString(key)) : await GetSystemConfigJson(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await GetSystemConfigJson(lang);
        }

        public async Task<SystemConfig> GetSystemConfigJson(string Lang)
        {
            string json = await ReadFileAsync("SystemConfig" + Lang + ".json", "DataJson");
            SystemConfig listMenu = JsonConvert.DeserializeObject<SystemConfig>(json);
            if (listMenu == null)
            {
                listMenu = new SystemConfig();
            }
            return listMenu;
        }

        public SystemConfigJson SystemConfigItem(string lang)
        {
            string key = WebConfig.CacheProject + "SystemConfigItem_" + lang;
            try
            {
                if (WebConfig.EnableCache)
                {
                    SystemConfigJson result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<SystemConfigJson>(cache.GetString(key)) : GetSystemConfigBase(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        string data = JsonConvert.SerializeObject(result);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return GetSystemConfigBase(lang);
        }

        public SystemConfigJson GetSystemItemConfigJson(string Lang)
        {
            string json = ReadFile("SystemConfig" + Lang + ".json", "DataJson");
            SystemConfigJson listMenu = JsonConvert.DeserializeObject<SystemConfigJson>(json);
            if (listMenu == null)
            {
                listMenu = new SystemConfigJson();
            }
            return listMenu;
        }

        public SystemConfigJson GetSystemConfigBase(string lang)
        {
            using SqlConnection connect = _dapperDa.GetOpenConnection();
            IEnumerable<SystemConfigJson> result = connect.Query<SystemConfigJson>("select * from SystemConfig where lang = @lang", new { lang });
            connect.Close();
            return result.FirstOrDefault();
        }

        #region Module

        public async Task<WebsiteModulesItem> GetModuleByNameAscii(string nameAscii)
        {
            string key = WebConfig.CacheProject + "Module_" + nameAscii;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    WebsiteModulesItem result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<WebsiteModulesItem>(cache.GetString(key)) : await _webModuleManager.GetByNameAscii(nameAscii);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetByNameAscii(nameAscii);
        }

        public WebsiteModulesItem GetModuleById(int id)
        {
            string key = WebConfig.CacheProject + "Module_" + id;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    WebsiteModulesItem result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<WebsiteModulesItem>(cache.GetString(key)) : _webModuleManager.GetById(id);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            WebsiteModulesItem result2 = _webModuleManager.GetById(id);
            return result2;
        }

        public ProductDetail GetProductById(int id)
        {
            string key = WebConfig.CacheProject + "GetProductById_" + id;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    ProductDetail result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<ProductDetail>(cache.GetString(key)) : _productManager.GetId(id);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            ProductDetail result2 = _productManager.GetId(id);
            return result2;
        }

        //public List<ProductItem> GetListProductMenu(string moduleIds, int take = 2)
        //{
        //    string key = WebConfig.CacheProject + "GetListProductMenu_" + moduleIds;
        //    try
        //    {
        //        if (WebConfig.EnableCache)
        //        {
        //            List<ProductItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<ProductItem>>(cache.GetString(key)) :
        //                _productManager.GetListProductBestSelling(take, moduleIds);
        //            if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
        //            {
        //                JsonSerializerSettings settings = new()
        //                {
        //                    StringEscapeHandling = StringEscapeHandling.EscapeHtml,
        //                };
        //                string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
        //                DistributedCacheEntryOptions expireTime = new() { SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute) };
        //                cache.SetStringAsync(key, data, expireTime);
        //            }
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.AddLogError(ex);
        //    }
        //    List<ProductItem> result2 = _productManager.GetListProductBestSelling(take, moduleIds);
        //    return result2;
        //}

        public async Task<IEnumerable<WebsiteModulesItem>> GetListModuleChidrentAsync(int moduleId)
        {
            string key = WebConfig.CacheProject + "GetListModuleChidrentAsync_" + moduleId;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : await _webModuleManager.GetListChidrentAsync(moduleId);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetListChidrentAsync(moduleId);
        }

        public List<WebsiteModulesItem> GetListModuleChidrentNotAsync(int moduleId)
        {
            string key = WebConfig.CacheProject + "GetListModuleChidrentNotAsync_" + moduleId;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : _webModuleManager.GetListChidrentNotAsync(moduleId);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _webModuleManager.GetListChidrentNotAsync(moduleId);
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetListModuleChildID(int moduleId, string lang)
        {
            string key = WebConfig.CacheProject + "GetListModuleChildID" + moduleId + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : await _webModuleManager.GetListByParentIDAsync(moduleId, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetListByParentIDAsync(moduleId, lang);
        }

        public List<WebsiteModulesItem> GetListModuleChildIDNotAsync(int moduleId, string lang)
        {
            string key = WebConfig.CacheProject + "GetListModuleChildIDNotAsync" + moduleId + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : _webModuleManager.GetListByParentID(moduleId, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _webModuleManager.GetListByParentID(moduleId, lang);
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetListModuleChildIDSimple(int moduleId, string lang)
        {
            string key = WebConfig.CacheProject + "GetListModuleChildIDSimple" + moduleId + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : await _webModuleManager.GetListByParentSimple(moduleId, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetListByParentSimple(moduleId, lang);
        }

        public List<WebsiteModulesItem> GetByModuleTypeCode(string code, string lang)
        {
            string key = WebConfig.CacheProject + "ModuleGetByModuleTypeCode_" + code;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : _webModuleManager.GetByModuleTypeCode(code, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<WebsiteModulesItem> result2 = _webModuleManager.GetByModuleTypeCode(code, lang);
            return result2;
        }

        public async Task<IEnumerable<WebsiteProductItemJson>> GetProductHomeJsonAsync(string lang)
        {
            string key = WebConfig.CacheProject + "GetProductHomeJson" + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteProductItemJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<WebsiteProductItemJson>>(cache.GetString(key)) : await GetProductJsonAsync(lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await GetProductJsonAsync(lang);
        }

        public IEnumerable<WebsiteProductItemJson> GetProductNewAsync(SearchModel search, string lang, int size = 6)
        {
            string key = WebConfig.CacheProject + "GetProductNew" + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteProductItemJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<WebsiteProductItemJson>>(cache.GetString(key)) : _productManager.GetListProductNew(search, 0, "", size);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _productManager.GetListProductNew(search, 0, "", size);
        }

        public IEnumerable<WebsiteProductItemJson> GetProductHotAsync(SearchModel search, string lang, int size = 6)
        {
            string key = WebConfig.CacheProject + "GetProductHot" + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteProductItemJson> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<WebsiteProductItemJson>>(cache.GetString(key)) : _productManager.GetListProductHotAsync(search, 0, "", size);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _productManager.GetListProductHotAsync(search, 0, "", size);
        }

        public async Task<IEnumerable<WebsiteProductItemJson>> GetProductJsonAsync(string Lang)
        {
            string json = await ReadFileAsync("ProductHome" + Lang + ".json", "DataJson");
            IEnumerable<WebsiteProductItemJson> listMenu = JsonConvert.DeserializeObject<IEnumerable<WebsiteProductItemJson>>(json);
            if (listMenu == null)
            {
                listMenu = new List<WebsiteProductItemJson>();
            }
            return listMenu;
        }

        public WebsiteModulesItem GetByModuleTypeCodeSimple(string code, string lang)
        {
            string key = WebConfig.CacheProject + "GetByModuleTypeCodeSimple_" + code;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    WebsiteModulesItem result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<WebsiteModulesItem>(cache.GetString(key)) : _webModuleManager.GetByTypeCode(code, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            WebsiteModulesItem result2 = _webModuleManager.GetByTypeCode(code, lang);
            return result2;
        }

        public List<WebsiteModulesItem> GetByModuleTypeCodeID(string code, int id, string lang)
        {
            string key = WebConfig.CacheProject + "ModuleGetByModuleTypeCodeID_" + code + id + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteModulesItem>>(cache.GetString(key)) : _webModuleManager.GetByModuleTypeCodeID(code, id, lang);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<WebsiteModulesItem> result2 = _webModuleManager.GetByModuleTypeCodeID(code, id, lang);
            return result2;
        }

        public async Task<IEnumerable<WebsiteModulesItem>> GetByAllTradeMark(string ids, string lang, string code)
        {
            string key = WebConfig.CacheProject + "GetByAllTradeMark_" + ids + lang;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<WebsiteModulesItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<IEnumerable<WebsiteModulesItem>>(cache.GetString(key)) : await _webModuleManager.GetByAllTradeMark(ids, lang, code);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetByAllTradeMark(ids, lang, code);
        }

        #endregion Module

        #region Content

        public WebsiteContentItem GetContentByNameAscii(string nameAscii)
        {
            string key = WebConfig.CacheProject + "ContentGetContentByNameAscii" + nameAscii;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    WebsiteContentItem result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<WebsiteContentItem>(cache.GetString(key)) : _webContentManager.GetByNameAscii(nameAscii);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            WebsiteContentItem result2 = _webContentManager.GetByNameAscii(nameAscii);
            return result2;
        }

        public ProductDetail GetProductByNameAscii(string nameAscii)
        {
            string key = WebConfig.CacheProject + "GetProductByNameAscii" + nameAscii;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    ProductDetail result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<ProductDetail>(cache.GetString(key)) : _productManager.GetByNameAscii(nameAscii);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return _productManager.GetByNameAscii(nameAscii);
        }

        public List<WebsiteContentItem> GetListcontentByIds(string ids)
        {
            string key = WebConfig.CacheProject + "GetListcontentByIds_" + ids;
            try
            {
                if (WebConfig.EnableCache)
                {
                    List<WebsiteContentItem> result = cache != null && cache.GetString(key) != null ? JsonConvert.DeserializeObject<List<WebsiteContentItem>>(cache.GetString(key)) : _webContentManager.GetListByArrId(ids);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            List<WebsiteContentItem> result2 = _webContentManager.GetListByArrId(ids);
            return result2;
        }

        #endregion Content

        #region Tag

        public async Task<TagItem> GetTagByNameAscii(string nameAscii)
        {
            string key = WebConfig.CacheProject + "GetTagByNameAscii_" + nameAscii;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    TagItem result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<TagItem>(cache.GetString(key)) : await _webModuleManager.GetTagByNameAscii(nameAscii);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _webModuleManager.GetTagByNameAscii(nameAscii);
        }

        #endregion Tag

        #region Attribute

        public async Task<IEnumerable<AttributeItem>> GetListAttributeItems(string ids)
        {
            string key = WebConfig.CacheProject + "GetListAttributeItems_" + ids;
            //Nếu tồn tại thì lấy cache
            try
            {
                if (WebConfig.EnableCache)
                {
                    IEnumerable<AttributeItem> result = (cache != null && cache.GetString(key) != null) ? JsonConvert.DeserializeObject<IEnumerable<AttributeItem>>(cache.GetString(key)) : await _productManager.GetAttributeByListIds(ids);
                    if (cache != null && cache.GetString(key) == null && WebConfig.CachingExpireMinute > 0)
                    {
                        JsonSerializerSettings settings = new()
                        {
                            StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                        };
                        string data = JsonConvert.SerializeObject(result, Formatting.None, settings);
                        DistributedCacheEntryOptions expireTime = new()
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(WebConfig.CachingExpireMinute)
                        };
                        await cache.SetStringAsync(key, data, expireTime);
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Common.AddLogError(ex);
            }
            return await _productManager.GetAttributeByListIds(ids);
        }

        #endregion Attribute

        private static string ReadFile(string fileName, string path)

        {
            string fileContent = string.Empty;
            try
            {
                fileContent = File.ReadAllText(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {
            }
            return fileContent;
        }

        private static async Task<string> ReadFileAsync(string fileName, string path)
        {
            string fileContent = string.Empty;
            try
            {
                fileContent = await File.ReadAllTextAsync(WebConfig.PathServer + path + "/" + fileName);
            }
            catch
            {
            }
            return fileContent;
        }
    }
}