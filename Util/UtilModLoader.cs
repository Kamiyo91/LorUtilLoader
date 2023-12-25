using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Mod;
using UtilLoader21341.Models;
using Debug = UnityEngine.Debug;

namespace UtilLoader21341.Util
{
    public static class UtilModLoader
    {
        private static readonly List<string> IgnoreDll = new List<string>
        {
            "0Harmony", "Mono.Cecil", "MonoMod.RuntimeDetour", "MonoMod.Utils", "1BigDLL4221", "1SMotion-Loader",
            ModParameters.DLLName
        };

        public static void LoadMods()
        {
            foreach (var modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods().Where(modContentInfo =>
                         modContentInfo.activated &&
                         modContentInfo.invInfo.workshopInfo.uniqueId != ModParameters.UtilDLLPackageId))
                try
                {
                    var modId = modContentInfo.invInfo?.workshopInfo?.uniqueId;
                    var path = modContentInfo.dirInfo.FullName + "/Assemblies";
                    if (string.IsNullOrEmpty(modId) || !Directory.Exists(path)) continue;
                    var loadBy =
                        Directory.Exists(modContentInfo.dirInfo.FullName + "/Assemblies" + ModParameters.BaseFolderUri)
                            ? "XML"
                            : "";
                    var directoryInfo = new DirectoryInfo(path);
                    var assemblies = (from fileInfo in directoryInfo.GetFiles()
                        where fileInfo.Extension.ToLower() == ".dll" && !IgnoreDll.Contains(fileInfo.FullName)
                        select Assembly.LoadFile(fileInfo.FullName)).ToList();
                    if (string.IsNullOrEmpty(loadBy) &&
                        assemblies.Any(x => x.GetType($"{x.GetName().Name}.UtilLoader21341") != null))
                        loadBy = "DLL";
                    if (string.IsNullOrEmpty(loadBy)) continue;
                    ModParameters.PackageIds.Add(modId);
                    ModParameters.Path.Add(modId, path);
                    var stopwatch = new Stopwatch();
                    Debug.Log($"Util Loader Tool : Start loading mod files {modId} at path {path}");
                    stopwatch.Start();
                    var loadByLog = "Parameters loaded by XML";
                    if (loadBy.Equals("DLL"))
                    {
                        loadByLog = "Parameters loaded by DLL";
                        LoadModParametersFromDLL(modId, assemblies);
                    }
                    else
                    {
                        LoadModParameters(path, modId);
                    }

                    CardUtil.ChangeCardItem(ItemXmlDataList.instance, modId);
                    ArtUtil.GetArtWorks(new DirectoryInfo(path + "/ArtWork"), modId);
                    LocalizationUtil.AddGlobalLocalize(modId);
                    ArtUtil.MakeCustomBook(modId);
                    ArtUtil.InitCustomEffects(assemblies);
                    CardUtil.InitKeywordsList(assemblies);
                    ModParameters.Assemblies.AddRange(assemblies);
                    stopwatch.Stop();
                    Debug.Log(
                        $"Util Loader Tool : Loading mod files {modId} at path {path} finished in {stopwatch.ElapsedMilliseconds} ms - {loadByLog}");
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"Error while loading the mod {modContentInfo.invInfo?.workshopInfo?.uniqueId} - {ex.Message}");
                }
        }

        public static void LoadModsAfter()
        {
            foreach (var modContentInfo in Singleton<ModContentManager>.Instance.GetAllMods().Where(modContentInfo =>
                         modContentInfo.activated &&
                         modContentInfo.invInfo.workshopInfo.uniqueId != ModParameters.UtilDLLPackageId &&
                         Directory.Exists(modContentInfo.dirInfo.FullName + "/Assemblies" +
                                          ModParameters.BaseFolderUri)))
                try
                {
                    var modId = modContentInfo.invInfo.workshopInfo.uniqueId;
                    PassiveUtil.ChangePassiveItem(PassiveXmlList.Instance, modId);
                    CardUtil.ChangeCardItem(ItemXmlDataList.instance, modId);
                    ArtUtil.PreLoadBufIcons();
                }
                catch (Exception ex)
                {
                    Debug.LogError(
                        $"Error while loading the mod (after) {modContentInfo.invInfo.workshopInfo.uniqueId} - {ex.Message}");
                }
        }

        private static void LoadModParametersFromDLL(string modId, List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                LoadModParametersDLLInternal(modId, assembly);
        }

        private static void LoadModParametersDLLInternal(string modId, Assembly assembly)
        {
            var loaderType = assembly.GetType($"{assembly.GetName().Name}.UtilLoader21341");
            if (loaderType == null) return;
            ModParameters.DefaultKeyWordOptions.Add(
                LoadModParametersDLLMethod<DefaultKeywordOption>(loaderType.GetMethod(nameof(DefaultKeywordOption))));
            ModParameters.CardOptions.AddRange(
                LoadModParametersDLLMethod<List<CardOptionRoot>>(loaderType.GetMethod(nameof(CardOptionRoot))));
            ModParameters.CategoryOptions.AddRange(
                LoadModParametersDLLMethod<List<CategoryOptionRoot>>(loaderType.GetMethod(nameof(CategoryOptionRoot))));
            ModParameters.RewardOptions.AddRange(
                LoadModParametersDLLMethod<List<RewardOptionRoot>>(loaderType.GetMethod(nameof(RewardOptionRoot))));
            ModParameters.KeypageOptions.AddRange(
                LoadModParametersDLLMethod<List<KeypageOptionRoot>>(loaderType.GetMethod(nameof(KeypageOptionRoot))));
            ModParameters.PassiveOptions.AddRange(
                LoadModParametersDLLMethod<List<PassiveOptionRoot>>(loaderType.GetMethod(nameof(PassiveOptionRoot))));
            ModParameters.SkinOptions.AddRange(
                LoadModParametersDLLMethod<List<SkinOptionRoot>>(loaderType.GetMethod(nameof(SkinOptionRoot))));
            ModParameters.CustomSkinOptions.AddRange(
                LoadModParametersDLLMethod<List<CustomSkinOptionRoot>>(
                    loaderType.GetMethod(nameof(CustomSkinOptionRoot))));
            ModParameters.SpriteOptions.AddRange(
                LoadModParametersDLLMethod<List<SpriteOptionRoot>>(loaderType.GetMethod(nameof(SpriteOptionRoot))));
            ModParameters.StageOptions.AddRange(
                LoadModParametersDLLMethod<List<StageOptionRoot>>(loaderType.GetMethod(nameof(StageOptionRoot))));
            ModParameters.BuffOptions.AddRange(
                LoadModParametersDLLMethod<List<BuffOptionRoot>>(loaderType.GetMethod(nameof(BuffOptionRoot))));
            ModParameters.MapModels.AddRange(
                LoadModParametersDLLMethod<List<MapModelRoot>>(loaderType.GetMethod(nameof(MapModelRoot))));
            ModParameters.UnitModels.AddRange(
                LoadModParametersDLLMethod<List<UnitModelRoot>>(loaderType.GetMethod(nameof(UnitModelRoot))));
            ModParameters.RushBattleModels.AddRange(
                LoadModParametersDLLMethod<List<RushBattleModelMainRoot>>(
                    loaderType.GetMethod(nameof(RushBattleModelMainRoot))));
            var assets =
                LoadModParametersDLLMethod<AssetsBundleOptionsRoot>(
                    loaderType.GetMethod(nameof(AssetsBundleOptionsRoot)));
            if (assets?.AssetsBundleOption == null || !assets.AssetsBundleOption.Any()) return;
            foreach (var item in assets.AssetsBundleOption)
                ModParameters.AssetBundle.Add(modId, new Assets(modId, item.Name));
        }

        public static T LoadModParametersDLLMethod<T>(MethodInfo method) where T : new()
        {
            try
            {
                if (method == null) return new T();
                return (T)method.Invoke(null, null);
            }
            catch (Exception)
            {
                Debug.LogError(
                    $"Error Invoking the method - {method?.Name ?? "Not Found"}");
                return new T();
            }
        }

        private static void LoadModParameters(string path, string modId)
        {
            var defaultKeywordOption =
                LoadParameters<DefaultKeywordRoot>(path + ModParameters.BaseFolderUri + "DefaultKeyword", modId);
            if (defaultKeywordOption.Key)
                ModParameters.DefaultKeyWordOptions.Add(defaultKeywordOption.Value.DefaultKeywordOption);
            var cardOptions =
                LoadParameters<CardOptionsRoot>(path + ModParameters.BaseFolderUri + "CardOptions", modId);
            ModParameters.CardOptions.AddRange(cardOptions.Key
                ? cardOptions.Value.CardOption
                : new List<CardOptionRoot>());
            var categoryOptions =
                LoadParameters<CategoryOptionsRoot>(path + ModParameters.BaseFolderUri + "CategoryOptions", modId);
            ModParameters.CategoryOptions.AddRange(categoryOptions.Key
                ? categoryOptions.Value.CategoryOption
                : new List<CategoryOptionRoot>());
            var rewardOptions =
                LoadParameters<RewardOptionsRoot>(path + ModParameters.BaseFolderUri + "RewardOptions", modId);
            ModParameters.RewardOptions.AddRange(rewardOptions.Key
                ? rewardOptions.Value.RewardOption
                : new List<RewardOptionRoot>());
            var keypageOptions =
                LoadParameters<KeypageOptionsRoot>(path + ModParameters.BaseFolderUri + "KeypageOptions", modId);
            ModParameters.KeypageOptions.AddRange(keypageOptions.Key
                ? keypageOptions.Value.KeypageOption
                : new List<KeypageOptionRoot>());
            var passiveOptions =
                LoadParameters<PassiveOptionsRoot>(path + ModParameters.BaseFolderUri + "PassiveOptions", modId);
            ModParameters.PassiveOptions.AddRange(passiveOptions.Key
                ? passiveOptions.Value.PassiveOptions
                : new List<PassiveOptionRoot>());
            var skinOptions =
                LoadParameters<SkinOptionsRoot>(path + ModParameters.BaseFolderUri + "SkinOptions", modId);
            ModParameters.SkinOptions.AddRange(skinOptions.Key
                ? skinOptions.Value.SkinOption
                : new List<SkinOptionRoot>());
            var customSkinOptions =
                LoadParameters<CustomSkinOptionsRoot>(path + ModParameters.BaseFolderUri + "ProjectionSkinOptions",
                    modId);
            ModParameters.CustomSkinOptions.AddRange(customSkinOptions.Key
                ? customSkinOptions.Value.CustomSkinOption
                : new List<CustomSkinOptionRoot>());
            var spriteOptions =
                LoadParameters<SpriteOptionsRoot>(path + ModParameters.BaseFolderUri + "SpriteOptions", modId);
            ModParameters.SpriteOptions.AddRange(spriteOptions.Key
                ? spriteOptions.Value.SpriteOption
                : new List<SpriteOptionRoot>());
            var stageOptions =
                LoadParameters<StageOptionsRoot>(path + ModParameters.BaseFolderUri + "StageOptions", modId);
            ModParameters.StageOptions.AddRange(stageOptions.Key
                ? stageOptions.Value.StageOption
                : new List<StageOptionRoot>());
            var buffOptions =
                LoadParameters<BuffOptionsRoot>(path + ModParameters.BaseFolderUri + "BuffOptions", modId);
            ModParameters.BuffOptions.AddRange(buffOptions.Key
                ? buffOptions.Value.BuffOption
                : new List<BuffOptionRoot>());
            var mapModels =
                LoadParameters<MapModelsRoot>(path + ModParameters.BaseFolderUri + "MapModels", modId);
            ModParameters.MapModels.AddRange(mapModels.Key
                ? mapModels.Value.MapModels
                : new List<MapModelRoot>());
            var unitModels =
                LoadParameters<UnitModelsRoot>(path + ModParameters.BaseFolderUri + "UnitModels", modId);
            ModParameters.UnitModels.AddRange(unitModels.Key
                ? unitModels.Value.UnitModels
                : new List<UnitModelRoot>());
            var rushBattleModels =
                LoadParameters<RushBattleModelsRoot>(path + ModParameters.BaseFolderUri + "RushBattleOptions", modId);
            ModParameters.RushBattleModels.AddRange(rushBattleModels.Key
                ? rushBattleModels.Value.RushBattleModels
                : new List<RushBattleModelMainRoot>());
            var assetsOptions =
                LoadParameters<AssetsBundleOptionsRoot>(path + ModParameters.BaseFolderUri + "AssetsBundleOptions",
                    modId);
            if (!assetsOptions.Key) return;
            foreach (var item in assetsOptions.Value.AssetsBundleOption)
                ModParameters.AssetBundle.Add(modId, new Assets(modId, item.Name));
        }

        private static KeyValuePair<bool, T> LoadParameters<T>(string path, string packageId)
        {
            var error = false;
            try
            {
                var file = new DirectoryInfo(path).GetFiles().FirstOrDefault();
                error = true;
                if (file == null) return new KeyValuePair<bool, T>(false, default);
                using (var stringReader = new StringReader(File.ReadAllText(file.FullName)))
                {
                    var root =
                        (T)new XmlSerializer(typeof(T))
                            .Deserialize(stringReader);
                    return new KeyValuePair<bool, T>(true, root);
                }
            }
            catch (Exception ex)
            {
                if (error)
                    Debug.LogError($"Error loading {typeof(T)} packageId : " + packageId + " Error : " + ex.Message);
                return new KeyValuePair<bool, T>(false, default);
            }
        }
    }
}