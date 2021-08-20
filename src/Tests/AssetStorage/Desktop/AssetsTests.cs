using Fusee.Base.Core;
using System.Collections.Generic;
using Xunit;

namespace Fusee.Tests.Assets.Desktop
{

    public static class AssetsTest
    {
        static AssetsTest()
        {
            var fap = new Fusee.Base.Imp.Desktop.FileAssetProvider("TestAssets");
            AssetStorage.RegisterProvider(fap);
        }

        [Fact]
        static void SimpleReadOfTxtFile()
        {
            Assert.Equal("Test123", AssetStorage.Get<string>("test.txt"));
        }

        [Fact]
        static async void AsyncReadOfTxtFile()
        {
            Assert.Equal("Test123", await AssetStorage.GetAsync<string>("test.txt").ConfigureAwait(false));
        }

        [Fact]
        static void SimpleMultipleReadOfTxtFile()
        {
            // To check the cache dictionary
            for (var i = 0; i < 10000; i++)
                Assert.Equal("Test123", AssetStorage.Get<string>("test.txt"));
        }

        [Fact]
        static async void MultipleAsyncReadOfTxtFile()
        {
            var list = new List<string>();

            for (var i = 0; i < 10000; i++)
                list.Add("test.txt");

            var actual = await AssetStorage.GetAssetsAsync<string>(list).ConfigureAwait(false);

            for (var i = 0; i < 10000; i++)
                Assert.Equal("Test123", actual[i]);
        }
    }
}