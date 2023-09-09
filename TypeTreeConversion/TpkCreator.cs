using AssetRipper.Tpk;
using AssetRipper.Tpk.TypeTrees;
using AssetRipper.Tpk.TypeTrees.Json;
using UnityVersion = AssetRipper.Primitives.UnityVersion;

namespace TypeTreeConversion;

public static class TpkCreator
{
	public static byte[] ConvertJsonToTpk(string path)
	{
		UnityInfo info = UnityInfo.ReadFromJsonFile(path);

		TpkTypeTreeBlob blob = new();
		blob.Versions.Add(UnityVersion.MinVersion);

		blob.CommonString.Add(UnityVersion.MinVersion, (byte)info.Strings.Count);
		blob.CommonString.SetIndices(blob.StringBuffer, info.Strings.Select(s => s.String).ToList());

		blob.CreationTime = DateTime.UtcNow;

		foreach (UnityClass unityClass in info.Classes)
		{
			TpkUnityClass tpkUnityClass = ClassConversion.Convert(unityClass, blob.StringBuffer, blob.NodeBuffer);
			TpkClassInformation tpkClassInformation = new(unityClass.TypeID);
			tpkClassInformation.Classes.Add(new KeyValuePair<UnityVersion, TpkUnityClass?>(UnityVersion.MinVersion, tpkUnityClass));
			blob.ClassInformation.Add(tpkClassInformation);
		}

		return TpkFile.FromBlob(blob, TpkCompressionType.None).WriteToMemory();
	}
}