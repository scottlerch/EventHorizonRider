using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
#if !PSM
using Newtonsoft.Json;
using PCLStorage;
#endif
using System;
using System.Threading.Tasks;

namespace EventHorizonRider.Core.Engine
{
    internal class PlayerData : ComponentBase
    {
        public TimeSpan BestTime { get; set; }

        public int HighestLevelNumber { get; set; }

        public int DefaultLevelNumber { get; set; }

        public PlayerData()
        {
            HighestLevelNumber = 1;
            DefaultLevelNumber = 1;
        }
		
		#if !PSM
        public async Task Save()
        {
            try
            {
                var rootFolder = FileSystem.Current.LocalStorage;
                var folder = await rootFolder.CreateFolderAsync("EventHorizon", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync("player1.json", CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(JsonConvert.SerializeObject(this));
            }
            catch (Exception)
            {
                // TODO: add logging infrastructure
            }
        }

        public async Task Load()
        {
            try
            {
                var rootFolder = FileSystem.Current.LocalStorage;

                var folder = await rootFolder.CreateFolderAsync("EventHorizon", CreationCollisionOption.OpenIfExists);
                var fileExists = await folder.CheckExistsAsync("player1.json");

                if (fileExists == ExistenceCheckResult.FileExists)
                {
                    var file = await folder.GetFileAsync("player1.json");
                    var text = await file.ReadAllTextAsync();

                    if (!string.IsNullOrEmpty(text))
                    {
                        var data = JsonConvert.DeserializeObject<PlayerData>(text);

                        BestTime = data.BestTime;
                        HighestLevelNumber = data.HighestLevelNumber;
                        DefaultLevelNumber = data.DefaultLevelNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: add logging infrastructure
            }
            finally
            {
                if (HighestLevelNumber < 1) HighestLevelNumber = 1;
                if (DefaultLevelNumber < 1) DefaultLevelNumber = 1;
            }
        }

        internal async Task UpdateDefaultLevel(int defaultLevelNumber)
        {
            DefaultLevelNumber = defaultLevelNumber;
            await Save();
        }

        internal async Task UpdateBestTime(TimeSpan timeSpan, int levelNumber)
        {
            if (timeSpan > BestTime)
            {
                BestTime = timeSpan;
                HighestLevelNumber = levelNumber;
                await Save();
            }
        }

        internal async Task Reset()
        {
            BestTime = TimeSpan.Zero;
            HighestLevelNumber = 1;
            DefaultLevelNumber = 1;
            await Save();
        }
		#else
		public Task Save()
        {
            return Task.Factory.StartNew(() => {});
        }

        public Task Load()
        {
            if (HighestLevelNumber < 1) HighestLevelNumber = 1;
            if (DefaultLevelNumber < 1) DefaultLevelNumber = 1;
			
			return Task.Factory.StartNew(() => {});
        }

        internal Task UpdateDefaultLevel(int defaultLevelNumber)
        {
			return Task.Factory.StartNew(() => {});
        }

        internal Task UpdateBestTime(TimeSpan timeSpan, int levelNumber)
        {
            if (timeSpan > BestTime)
            {
                BestTime = timeSpan;
                HighestLevelNumber = levelNumber;
            }
			
			return Task.Factory.StartNew(() => {});
        }

        internal Task Reset()
        {
            BestTime = TimeSpan.Zero;
            HighestLevelNumber = 1;
            DefaultLevelNumber = 1;
			
			return Task.Factory.StartNew(() => {});
        }	
		#endif
    }
}