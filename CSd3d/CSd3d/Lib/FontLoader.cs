using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.IO;

namespace MelloRin.CSd3d.Lib
{
	/*class FontLoader :  FontCollectionLoader
	{
		private Factory factory;
		private FontCollection fontCollection;
		private string fontName;
		private float fontSize;
		private bool isExternal;

		DataStream keyStream;

		public FontLoader(Factory factory,string fontName,float fontSize, bool isExternal = false)
		{
			this.factory = factory;
			this.fontName = fontName;
			this.fontSize = fontSize;
			this.isExternal = isExternal;




			keyStream = new DataStream(sizeof(int) * _fontStreams.Count, true, true);
			for (int i = 0; i < _fontStreams.Count; i++)
				keyStream.Write((int)i);
			keyStream.Position = 0;

			// Register the 
			factory.RegisterFontFileLoader(this);
			factory.RegisterFontCollectionLoader(this);










		}

		public TextFormat getTextFormat()
		{



















			if(isExternal)
			{
				return new TextFormat(factory, fontName, fontCollection, FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, fontSize)
				{ TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
			}
			else
			{
				return new TextFormat(factory, fontName, fontSize)
				{ TextAlignment = TextAlignment.Leading, ParagraphAlignment = ParagraphAlignment.Near };
			}
			
		}

		public IDisposable Shadow { get; set; }

		public FontFileEnumerator CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public DataStream getDataStream()
		{
			return keyStream;
		}
	}*/

	public class ResourceFontFileStream : CallbackBase, FontFileStream
	{
		private readonly DataStream _stream;


		public ResourceFontFileStream(DataStream stream)
		{
			_stream = stream;
		}

		void FontFileStream.ReadFileFragment(out IntPtr fragmentStart, long fileOffset, long fragmentSize, out IntPtr fragmentContext)
		{
			lock (this)
			{
				fragmentContext = IntPtr.Zero;
				_stream.Position = fileOffset;
				fragmentStart = _stream.PositionPointer;
			}
		}

		void FontFileStream.ReleaseFileFragment(IntPtr fragmentContext)
		{
			// Nothing to release. No context are used
		}

		long FontFileStream.GetFileSize()
		{
			return _stream.Length;
		}

		long FontFileStream.GetLastWriteTime()
		{
			return 0;
		}
	}

	class ResourceFontFileEnumerator : CallbackBase, FontFileEnumerator
	{
		private Factory _factory;
		private FontFileLoader _loader;
		private DataStream keyStream;
		private FontFile _currentFontFile;


		public ResourceFontFileEnumerator(Factory factory, FontFileLoader loader, DataPointer key)
		{
			_factory = factory;
			_loader = loader;
			keyStream = new DataStream(key.Pointer, key.Size, true, false);
		}

		bool FontFileEnumerator.MoveNext()
		{
			bool moveNext = keyStream.RemainingLength != 0;
			if (moveNext)
			{
				if (_currentFontFile != null)
					_currentFontFile.Dispose();

				_currentFontFile = new FontFile(_factory, keyStream.PositionPointer, 4, _loader);
				keyStream.Position += 4;
			}
			return moveNext;
		}

		FontFile FontFileEnumerator.CurrentFontFile
		{
			get
			{
				((IUnknown)_currentFontFile).AddReference();
				return _currentFontFile;
			}
		}
	}

	public partial class ResourceFontLoader : CallbackBase, FontCollectionLoader, FontFileLoader
	{
		private readonly List<ResourceFontFileStream> _fontStreams = new List<ResourceFontFileStream>();
		private readonly List<ResourceFontFileEnumerator> _enumerators = new List<ResourceFontFileEnumerator>();
		private readonly DataStream _keyStream;
		private readonly Factory _factory;

		public bool resultAvailable;

		public ResourceFontLoader(Factory factory, string fontFile)
		{
			_factory = factory;

			if(File.Exists(fontFile))
			{
				var fontBytes = Utilities.ReadStream(typeof(ResourceFontLoader).Assembly.GetManifestResourceStream(fontFile));
				var stream = new DataStream(fontBytes.Length, true, true);
				stream.Write(fontBytes, 0, fontBytes.Length);
				stream.Position = 0;
				_fontStreams.Add(new ResourceFontFileStream(stream));

				// Build a Key storage that stores the index of the font
				_keyStream = new DataStream(sizeof(int) * _fontStreams.Count, true, true);
				for (int i = 0; i < _fontStreams.Count; i++)
					_keyStream.Write((int)i);
				_keyStream.Position = 0;

				// Register the 
				_factory.RegisterFontFileLoader(this);
				_factory.RegisterFontCollectionLoader(this);
				resultAvailable = true;
			}
			resultAvailable = false;
		}


		public DataStream Key
		{
			get
			{
				return _keyStream;
			}
		}

		FontFileEnumerator FontCollectionLoader.CreateEnumeratorFromKey(Factory factory, DataPointer collectionKey)
		{
			var enumerator = new ResourceFontFileEnumerator(factory, this, collectionKey);
			_enumerators.Add(enumerator);

			return enumerator;
		}

		FontFileStream FontFileLoader.CreateStreamFromKey(DataPointer fontFileReferenceKey)
		{
			var index = Utilities.Read<int>(fontFileReferenceKey.Pointer);
			return _fontStreams[index];
		}
	}
}
