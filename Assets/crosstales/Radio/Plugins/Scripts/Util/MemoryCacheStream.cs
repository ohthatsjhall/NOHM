using UnityEngine;

namespace Crosstales.Radio.Util
{
    /// <summary>Memory cache stream.</summary>
    public class MemoryCacheStream : System.IO.Stream
    {

        #region Variables

        /// <summary>
        /// The cache as byte[]
        /// </summary>
        private byte[] cache;

        //      /// <summary>
        //      /// The read/write position within the stream; note that this can be moved back to write over existing data.
        //      /// </summary>
        //      private long position;

        /// <summary>
        /// The write position within the stream.
        /// </summary>
        private long writePosition;

        /// <summary>
        /// The read position within the stream.
        /// </summary>
        private long readPosition;

        /// <summary>
        /// Stream length. Indicates where the end of the stream is.
        /// </summary>
        private long length;

        /// <summary>
        /// Cache size in bytes
        /// </summary>
        private int size;

        /// <summary>
        /// Maximum cache size in bytes
        /// </summary>
        private readonly int maxSize;

        #endregion


        #region Constructors
        /// <summary>
        /// Constructer with a specified cache size.
        /// </summary>
        /// <param name="cacheSize">Cache size of the stream in bytes.</param>
        /// <param name="maxCacheSize">Maximum cache size of the stream in bytes.</param>
        public MemoryCacheStream(int cacheSize = 64 * Constants.FACTOR_KB, int maxCacheSize = 64 * Constants.FACTOR_MB)
        {
            length = writePosition = readPosition = 0;
            size = cacheSize;
            maxSize = maxCacheSize;

            createCache();

            //Debug.Log("MemoryCacheStream: " + cacheSize + "-" + maxCacheSize);
        }

        #endregion


        #region Stream Overrides [Properties]

        /// <summary>
        /// Gets a flag flag that indicates if the stream is readable (always true).
        /// </summary>
        public override bool CanRead
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a flag flag that indicates if the stream is seekable (always true).
        /// </summary>
        public override bool CanSeek
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a flag flag that indicates if the stream is seekable (always true).
        /// </summary>
        public override bool CanWrite
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the current stream position.
        /// </summary>
        public override long Position
        {
            get
            {
                //return position;
                return readPosition;
            }
            set
            {
                if (value < 0L)
                {
                    throw new System.ArgumentOutOfRangeException("value", "Non-negative number required.");
                }

                //writePosition = readPosition = value; //TODO only readPosition?
                readPosition = value;
            }
        }

        /// <summary>
        /// Gets the current stream length.
        /// </summary>
        public override long Length
        {
            get
            {
                return length;
            }
        }

        #endregion


        #region Stream Overrides [Methods]

        public override void Flush()
        {
            // Memory based stream with nothing to flush; Do nothing.
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {

            //Debug.Log("Seek: " + offset + "-" + origin);

            //         if (offset < 0L)
            //            throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");

            switch (origin)
            {
                case System.IO.SeekOrigin.Begin:
                    {
                        //if(offset < 0) throw new IOException("An attempt was made to move the position before the beginning of the stream.");                  
                        Position = (int)offset;
                        break;
                    }
                case System.IO.SeekOrigin.Current:
                    {
                        long newPos = unchecked(Position + offset);
                        if (newPos < 0L)
                            throw new System.IO.IOException("An attempt was made to move the position before the beginning of the stream.");
                        Position = newPos;
                        break;
                    }
                case System.IO.SeekOrigin.End:
                    {
                        long newPos = unchecked(length + offset);
                        if (newPos < 0L)
                            throw new System.IO.IOException("An attempt was made to move the position before the beginning of the stream.");
                        Position = newPos;
                        break;
                    }
                default:
                    {
                        throw new System.ArgumentException("Invalid seek origin.");
                    }
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            //Debug.Log("SetLength: " + value);

            int size = (int)value;

            if (this.size != size)
            {
                this.size = size;
                length = Position = 0;

                createCache();
            }

            //length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (null == buffer)
                throw new System.ArgumentNullException("buffer", "Buffer cannot be null.");
            if (offset < 0)
                throw new System.ArgumentOutOfRangeException("offset", "Non-negative number required.");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Non-negative number required.");
            if ((buffer.Length - offset) < count)
            {
                throw new System.ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            // Test for end of stream (or beyond end)
            if (readPosition >= length)
            {
                return 0;
            }

            return read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (null == buffer)
                throw new System.ArgumentNullException("buffer", "Buffer cannot be null.");
            if (offset < 0)
                throw new System.ArgumentOutOfRangeException("offset", "Non-negative number required.");
            if (count < 0)
                throw new System.ArgumentOutOfRangeException("count", "Non-negative number required.");
            if (count > size)
                throw new System.ArgumentOutOfRangeException("count", "Value is larger than the cache size.");
            if ((buffer.Length - offset) < count)
            {
                throw new System.ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            if (0 == count)
            {
                // Nothing to do.
                return;
            }

            write(buffer, offset, count);
        }

        #endregion


        #region Private methods

        /// <summary>
        /// Read bytes from the memory stream into the provided buffer.
        /// </summary>
        private int read(byte[] buff, int offset, int count)
        {
            int arrayPosition = (int)(readPosition % size);

            if (arrayPosition + count > size)
            {
                int countEnd = size - arrayPosition;
                int countStart = count - countEnd;

                //Debug.Log("Read overflow: " + countEnd + "-" + countStart);

                System.Array.Copy(cache, arrayPosition, buff, offset, countEnd);

                System.Array.Copy(cache, 0, buff, offset + countEnd, countStart);
            }
            else
            {
                //Debug.Log("All good");
                System.Array.Copy(cache, arrayPosition, buff, offset, count);
            }

            readPosition += count;
            return count;
        }

        /// <summary>
        /// Write bytes into the memory stream.
        /// </summary>
        private void write(byte[] buff, int offset, int count)
        {
            int arrayPosition = (int)(writePosition % size);

            if (arrayPosition + count > size)
            {
                int countEnd = size - arrayPosition;
                int countStart = count - countEnd;

                //Debug.Log("Write overflow: " + countEnd + "-" + countStart);

                System.Array.Copy(buff, offset, cache, arrayPosition, countEnd);

                System.Array.Copy(buff, offset + countEnd, cache, 0, countStart);
            }
            else
            {
                //Debug.Log("All good");
                System.Array.Copy(buff, offset, cache, arrayPosition, count);
            }

            writePosition += count;

            //if (writePosition > length) {
            length = writePosition;
            //}
        }

        /// <summary>
        /// Create the cache
        /// </summary>
        private void createCache()
        {
            if (size > maxSize)
            {
                cache = new byte[maxSize];
                Debug.LogWarning("'size' is larger than 'maxSize'! Using 'maxSize' as cache!");
            }
            else
            {
                cache = new byte[size];
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)