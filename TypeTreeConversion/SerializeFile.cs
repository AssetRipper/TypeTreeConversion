using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Collections;
using System.Runtime.CompilerServices;

namespace TypeTreeConversion;

public readonly record struct SerializeFile
{
	public AssetsFileInstance FileInstance { get; }
	public AssetsManager Manager { get; }

	public SerializeFile(AssetsFileInstance file, AssetsManager manager)
	{
		FileInstance = file;
		Manager = manager;
	}

	public AssetList Assets => new(this);

	public static SerializeFile Load(string path)
	{
		AssetsManager manager = new();
		AssetsFileInstance file = manager.LoadAssetsFile(path, false);
		return new SerializeFile(file, manager);
	}

	public readonly struct AssetList : IList<UnityAsset>
	{
		private readonly SerializeFile _file;

		public AssetList(SerializeFile file)
		{
			_file = file;
		}

		private IList<AssetFileInfo> InternalList => _file.FileInstance.file.AssetInfos;

		private UnityAsset GetAsset(AssetFileInfo info)
		{
			return new UnityAsset(_file.Manager, _file.FileInstance, info);
		}

		public UnityAsset this[int index] { get => GetAsset(InternalList[index]); set => InternalList[index] = value.FileInfo; }

		public int Count => InternalList.Count;

		public bool IsReadOnly => InternalList.IsReadOnly;

		public void Add(UnityAsset item)
		{
			ThrowIfFromDifferentFile(item);
			InternalList.Add(item.FileInfo);
		}

		public void Clear()
		{
			InternalList.Clear();
		}

		public bool Contains(UnityAsset item)
		{
			return item.File == _file && InternalList.Contains(item.FileInfo);
		}

		public void CopyTo(UnityAsset[] array, int arrayIndex)
		{
			for (int i = 0; i < InternalList.Count; i++)
			{
				array[arrayIndex + i] = GetAsset(InternalList[i]);
			}
		}

		public IEnumerator<UnityAsset> GetEnumerator()
		{
			foreach (AssetFileInfo info in InternalList)
			{
				yield return GetAsset(info);
			}
		}

		public int IndexOf(UnityAsset item)
		{
			if (item.File != _file)
			{
				return -1;
			}
			return InternalList.IndexOf(item.FileInfo);
		}

		public void Insert(int index, UnityAsset item)
		{
			ThrowIfFromDifferentFile(item);
			InternalList.Insert(index, item.FileInfo);
		}

		public bool Remove(UnityAsset item)
		{
			return InternalList.Remove(item.FileInfo);
		}

		public void RemoveAt(int index)
		{
			InternalList.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		private void ThrowIfFromDifferentFile(UnityAsset asset, [CallerArgumentExpression(nameof(asset))] string? paramName = null)
		{
			if (asset.File != _file)
			{
				throw new ArgumentException("Asset is from a different file.", paramName);
			}
		}
	}
}
