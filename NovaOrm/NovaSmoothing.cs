using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaOrm
{
    public class NovaSmoothing
    {
        protected int _chuckSize;
        protected int _sampleRate;
        protected string _name;
        protected string _column;
        protected SmoothingType _type;
        


        public NovaSmoothing(string column, int chunkSize, int sampleRate = 0, string name = null)
        {
            _column = column;
            _chuckSize = chunkSize; // how many to average together
            _sampleRate = sampleRate; // basically the modulus if you weren't averaging
            _name = name;
        }

        public string Calculation
        {
            get
            {
                return "(" + Column + " / " + ChunkSize + ")";
            }
        }
        public string Column
        {
            get
            {
                return _column;
            }
        }
        public int ChunkSize //we need chunks to fit inside a single sample, therefore take which one is smaller
        {
            get
            {
                return _chuckSize;
            }
        }
        public int Mod
        {
            get
            {
                return _sampleRate / _chuckSize;
            }
        }
        public bool Sample //Do we actually need to sample down the data?
        {
            get
            {
                return _sampleRate > _chuckSize; //we need chunks to fit inside a single sample
            }
        }
        public string Name
        {
            get
            {
                return _name ?? Column;
            }
        }
        public int Percent
        {
            get
            {
                if (_sampleRate < 1)
                {
                    return 100;
                }
                return (_chuckSize * 1000000) / _sampleRate;
            }
        }
    }
    public enum SmoothingType
    {
        Limiting = 1,
        Trending = 2
    }
}
