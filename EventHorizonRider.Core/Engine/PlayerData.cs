using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Newtonsoft.Json;

namespace EventHorizonRider.Core.Engine
{
    internal class PlayerData : ComponentBase
    {
        public TimeSpan Highscore { get; set; }

        public void Save()
        {
            var result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            result.AsyncWaitHandle.WaitOne();
            var storageDevice = StorageDevice.EndShowSelector(result);

            result = storageDevice.BeginOpenContainer("EventHorizon", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (var container = storageDevice.EndOpenContainer(result))
            using (var writer = new StreamWriter(container.CreateFile("user.dat")))
            {
                writer.WriteLine(JsonConvert.SerializeObject(this));
            }
        }

        public void Load()
        {
            var result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            result.AsyncWaitHandle.WaitOne();
            var storageDevice = StorageDevice.EndShowSelector(result);
            result.AsyncWaitHandle.Dispose();

            result = storageDevice.BeginOpenContainer("EventHorizon", null, null);
            result.AsyncWaitHandle.WaitOne();

            using (var container = storageDevice.EndOpenContainer(result))
            {
                if (container.FileExists("user.dat"))
                {
                    // TODO: open file API not implemented yet in monogame
                }
            }

            result.AsyncWaitHandle.Dispose();
        }

        internal void UpdateBestTime(TimeSpan timeSpan)
        {
            if (timeSpan > Highscore)
            {
                Highscore = timeSpan;
                //Save();
            }
        }
    }
}