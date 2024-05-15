using Microsoft.Extensions.Caching.Memory;
using System;

namespace ResourceAllocationTool
{
    public class CacheAdapter
    {
        #region variables

        private readonly IMemoryCache _cache;


        #endregion

        #region Properties
        public string Key { get; set; }
        #endregion

        #region Constructors

        public CacheAdapter(IMemoryCache memoryCache)
        {
            _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }



        //public CacheAdapter()
        //{


        //    this.Key = typeof(T).FullName;
        //}
        #endregion

        #region Methods

        //public T GetSingle()
        //{
        //    return HttpContext.Current.Items[this.Key] as T;
        //}
        //public  ICollection<T> GetCollection(IEnumerable<T> lstItems)
        //{
        //    //return HttpContext.Current.Items[this.Key] as ICollection<T>;

        //    if (_cache.TryGetValue(this.Key, out lstItems))
        //    {
        //        return (ICollection<T>)lstItems;
        //    }


        //    var cacheExp = new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTime.Now.AddHours(Constants.CacheExpHrs),
        //        Priority = CacheItemPriority.Normal,
        //    };

        //   _cache.Set(this.Key, lstItems, cacheExp);

        //    return (ICollection<T>)lstItems;

        //}

        //public void PutSingle(T oItem)
        //{
        //    HttpContext.Current.Items[this.Key] = oItem;
        //}
        //public void PutCollection(ICollection<T> lstItems)
        //{
        //    HttpContext.Current.Items[this.Key] = lstItems;
        //}

        #endregion

    }

}
