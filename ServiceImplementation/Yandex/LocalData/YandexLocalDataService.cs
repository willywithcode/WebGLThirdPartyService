#if YANDEX_GAMES
namespace WebGLThirdPartyService.ServiceImplementation.Yandex.LocalData
{
    using System;
    using GameFoundation.Scripts.Features.Logger.Services;
    using GameFoundation.Scripts.LocalData.Interfaces;
    using GameFoundation.Scripts.LocalData.Service;

    public abstract class YandexLocalDataService<T> : ILocalDataService<T> where T : ILocalData, new()
    {
        private readonly ILogger logger;

        public T Data { get; protected set; }

        protected YandexLocalDataService(ILogger logger = null)
        {
            this.logger = logger ?? new LoggerService();
            this.Data = new T();
            this.Load();
        }

        public virtual void Save()
        {
            try
            {
                string key = this.Data.GetKey();
                LocalDataUtils.SaveData(key, this.Data);
                this.logger.Info($"[YandexLocalData] Saved: {key}");
            }
            catch (Exception e)
            {
                this.logger.Error($"[YandexLocalData] Error saving: {e.Message}");
            }
        }

        public virtual void Load()
        {
            try
            {
                string key = this.Data.GetKey();
                this.Data = LocalDataUtils.LoadData<T>(key);
                this.logger.Info($"[YandexLocalData] Loaded: {key}");
            }
            catch (Exception e)
            {
                this.logger.Error($"[YandexLocalData] Error loading: {e.Message}");
                this.Data ??= new T();
                this.Data.Reset();
            }
        }

        public virtual void DeleteData()
        {
            try
            {
                string key = this.Data.GetKey();
                LocalDataUtils.DeleteData(key);
                this.Data.Reset();
                this.logger.Info($"[YandexLocalData] Deleted: {key}");
            }
            catch (Exception e)
            {
                this.logger.Error($"[YandexLocalData] Error deleting: {e.Message}");
            }
        }
    }
}
#endif
