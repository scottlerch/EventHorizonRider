using System.Dynamic;
using Newtonsoft.Json;
using PCLStorage;
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

        public async Task Save()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.CreateFolderAsync("EventHorizon", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync("player1.json", CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(JsonConvert.SerializeObject(this));
        }

        public async Task Load()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var folder = await rootFolder.GetFolderAsync("EventHorizon");

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

            if (HighestLevelNumber < 1) HighestLevelNumber = 1;
            if (DefaultLevelNumber < 1) DefaultLevelNumber = 1;
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
    }
}